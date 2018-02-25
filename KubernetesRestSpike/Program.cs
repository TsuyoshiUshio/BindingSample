using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace KubernetesRestSpike
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        static Program()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
            Configuration = builder.Build();

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPlicyErrors) => true;
            client = new HttpClient(httpClientHandler);            
        }

        static HttpClient client;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("E:" + System.Environment.GetEnvironmentVariable("serverUrl")); // it doesn't work.
            Console.WriteLine("C:" + Configuration["serverUrl"]);

            RunAsync().GetAwaiter().GetResult();

            Console.ReadLine();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri(Configuration["serverUrl"]);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Configuration["kubernetesToken"]);
            var response = await client.GetAsync("/api/v1/namespaces/default/pods");
            var result = await response.Content.ReadAsStringAsync();
            var resultOject = JsonConvert.DeserializeObject<Rootobject>(result);
            
            foreach(var item in resultOject.items)
            {

                Console.WriteLine($"Pod: {item.metadata.name}");
                Console.WriteLine($"Status: {item.status.phase}");
                var ts = DateTime.UtcNow - item.status.startTime;
                Console.WriteLine($"Started {ts.TotalMinutes} min before");
            }

          //  Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}

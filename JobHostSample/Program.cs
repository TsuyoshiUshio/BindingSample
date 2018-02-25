using Microsoft.Azure.WebJobs;
using System;
using KubernetesBindings;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.Files;
using System.IO;

namespace JobHostSample
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();
            FilesConfiguration filesConfig = new FilesConfiguration();
            if(config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
                filesConfig.RootPath = @"C:\temp\files";
            }

            config.UseFiles(filesConfig);
            config.KubernetesSample();
            //config.UseCore();

            EnsureSampleDirectoriesExist(filesConfig.RootPath);

            JobHost host = new JobHost(config);
            config.TypeLocator = new SamplesTypeLocator(
                typeof(KubernetesSamples));
            host.RunAndBlock();

        }

        private static void EnsureSampleDirectoriesExist(string rootFilesPath)
        {
            // Ensure all the directories referenced by the file sample bindings
            // exist
            Directory.CreateDirectory(rootFilesPath);
            Directory.CreateDirectory(Path.Combine(rootFilesPath, "import"));
            Directory.CreateDirectory(Path.Combine(rootFilesPath, "cache"));
            Directory.CreateDirectory(Path.Combine(rootFilesPath, "convert"));
            Directory.CreateDirectory(Path.Combine(rootFilesPath, "converted"));

            File.WriteAllText(Path.Combine(rootFilesPath, "input.txt"), "WebJobs SDK Extensions!");
        }

    }
    internal class SamplesTypeLocator : ITypeLocator
    {
        private Type[] _types;
        public SamplesTypeLocator(params Type[] types)
        {
            _types = types;
        }
        public IReadOnlyList<Type> GetTypes()
        {
            return _types;
        }
    }

    public static class KubernetesSamples
    {
        public static void KubernetesTrigger([KubernetesTrigger("KubernetesToken", 5)] KubernetesTriggerValue value)
        {
            Console.WriteLine("**** Something Wrong with your Pods. *** ");
            // Console.WriteLine(value.Result);
        }
    }
}


using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using KubernetesBindings;

namespace KubernetesBindingsSample
{
    public static class KubernetesTriggerSample
    {
        [FunctionName("KubernetesTriggerSample")]
        public static void Run([KubernetesTrigger] KubernetesTriggerValue value, TraceWriter log)
        {
            log.Info("**** Something Wrong with your Pods. *** ");
            // Console.WriteLine(value.Result);
        }
    }
}

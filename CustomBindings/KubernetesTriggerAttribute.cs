using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using System;

namespace KubernetesBindings
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class KubernetesTriggerAttribute : Attribute
    {
        
        public string Token { get; set; }

        public int PendingTimeLimit { get; set; }

        public KubernetesTriggerAttribute()
        {
            Token = System.Environment.GetEnvironmentVariable("kubernetesToken");
            PendingTimeLimit = 5;
        }
    }
}

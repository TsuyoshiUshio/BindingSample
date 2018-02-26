using Microsoft.Azure.WebJobs.Description;
using System;

namespace KubernetesBindings
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class KubernetesTriggerAttribute : Attribute
    {
        public string Token { get; private set; }
        public int PendingTimeLimit { get; private set; }
        public KubernetesTriggerAttribute(string token, int pendingTimeLimit)
        {
            Token = token;
            PendingTimeLimit = pendingTimeLimit;
        }
    }
}

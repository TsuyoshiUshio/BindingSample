using System;

namespace KubernetesBindings
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class KubernetesTriggerAttribute : Attribute
    {
        public string Token { get; private set; }
        public KubernetesTriggerAttribute(string token)
        {
            Token = token;
        }
    }
}

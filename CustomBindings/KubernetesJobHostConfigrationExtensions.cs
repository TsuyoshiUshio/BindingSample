using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using System;
using System.Collections.Generic;
using System.Text;
using KubernetesBindings;
using Microsoft.Azure.WebJobs.Host;

namespace Microsoft.Azure.WebJobs
{
    public static class KubernetesJobHostConfigrationExtensions
    {
        public static void KubernetesSample(this JobHostConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            // Register our extension configuration provider
            config.RegisterExtensionConfigProvider(new KubernetesExtensionConfig());
        }

        public class KubernetesExtensionConfig : IExtensionConfigProvider
        {
            private TraceWriter _tracer;
            public void Initialize(ExtensionConfigContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                if (context.Trace == null)
                    throw new ArgumentNullException("context.Trace");

                _tracer = context.Trace;
                // Register our extension bindings providers
                context.Config.RegisterBindingExtensions(
                    new KubernetesTriggerAttributeBindingProvider());
            }
        }

    }
}

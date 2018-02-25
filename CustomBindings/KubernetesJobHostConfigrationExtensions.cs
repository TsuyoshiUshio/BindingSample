using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using System;
using System.Collections.Generic;
using System.Text;
using KubernetesBindings;

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

        private class KubernetesExtensionConfig : IExtensionConfigProvider
        {
            public void Initialize(ExtensionConfigContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                // Register our extension bindings providers
                context.Config.RegisterBindingExtensions(
                    new KubernetesTriggerAttributeBindingProvider());
            }
        }

    }
}

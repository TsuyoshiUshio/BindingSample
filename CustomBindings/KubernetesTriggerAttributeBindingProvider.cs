using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KubernetesBindings
{
    internal class KubernetesTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var parameter = context.Parameter;
            KubernetesTriggerAttribute attribute = parameter.GetCustomAttribute<KubernetesTriggerAttribute>(inherit: false);
            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            if (parameter.ParameterType != typeof(KubernetesTriggerValue) &&
                parameter.ParameterType != typeof(string))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind KubernetesTriggerAttribute to type '{0}'", parameter.ParameterType));
                
            }
            return Task.FromResult<ITriggerBinding>(new KubernetesTriggerBinding(context.Parameter));

        }

        private class KubernetesTriggerBinding : ITriggerBinding
        {
            private readonly ParameterInfo _parameter;
            private readonly IReadOnlyDictionary<string, Type> _bindingContract;

            public KubernetesTriggerBinding(ParameterInfo parameter)
            {
                _parameter = parameter;
                _bindingContract = CreateBindingDataContract();
            }

            public IReadOnlyDictionary<string, Type> BindingDataContract
            {
                get { return _bindingContract; }
            }

            public Type TriggerValueType
            {
                get { return typeof(KubernetesTriggerValue); }
            }


            public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
            {
                // TODO: Perfrom any required conversions on the value 
                KubernetesTriggerValue triggerValue = value as KubernetesTriggerValue;
                IValueBinder valueBinder = new KubernetesValueBinder(_parameter, triggerValue);
                return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, GetBindingData(triggerValue)));
            }

            public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
            {
                return Task.FromResult<IListener>(new Listener(context.Executor));
            }

            public ParameterDescriptor ToParameterDescriptor()
            {
                return new KubernetesTriggerParameterDescriptor
                {
                    Name = _parameter.Name,
                    DisplayHints = new ParameterDisplayHints
                    {
                        // TODO: Customize your Dashboard display strings
                        Prompt = "Kubernetes",
                        Description = "Kubernetes Trigger fired",
                        DefaultValue = "Kubernets"
                    }
                };
            }

            private IReadOnlyDictionary<string, Type> CreateBindingDataContract()
            {
                Dictionary<string, Type> contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
                contract.Add("KubernetesTrigger", typeof(KubernetesTriggerValue));
                return contract;
            }

            private IReadOnlyDictionary<string, object> GetBindingData(KubernetesTriggerValue value)
            {
                Dictionary<string, object> bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                bindingData.Add("KubernetesTrigger", value);
                // TOD: Add any additional binding data
                return bindingData;
            }
        }

        private class KubernetesTriggerParameterDescriptor : TriggerParameterDescriptor
        {
            public override string GetTriggerReason(IDictionary<string, string> arguments)
            {
                // TODO: Customize your Dashboard display string
                return string.Format("Kubernetes trigger fired at {0}", DateTime.Now.ToString("o"));
            }
        }

        private class Listener : IListener
        {
            private ITriggeredFunctionExecutor _executor;

            public Listener(ITriggeredFunctionExecutor executor)
            {
                _executor = executor;
                
            }

            public void Cancel()
            {
                // TODO: cancel the task
            }

            public void Dispose()
            {
                // Do some clean up
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                // TODO: Start monitoring your event source. 

                return Task.FromResult(true);
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                // TODO: Stop monitoring your event source
                return Task.FromResult(true);
            }
        }

        private class KubernetesValueBinder : ValueBinder
        {
            private readonly object _value;

            public KubernetesValueBinder(ParameterInfo parameter, KubernetesTriggerValue value) : base(parameter.ParameterType)
            {
                _value = value;
            }
            public override Task<object> GetValueAsync()
            {
                if (Type == typeof(string))
                {
                    return Task.FromResult<object>(_value.ToString());
                }
                return Task.FromResult(_value);
            }
            public override string ToInvokeString()
            {
                return "Kubernetes Trigger Invoked";
            }
        }
    }

}

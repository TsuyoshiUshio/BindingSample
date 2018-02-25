using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
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
                return Task.FromResult<IListener>(new Listener(context.Executor, _parameter.GetCustomAttribute<KubernetesTriggerAttribute>(false)));
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
            private KubernetesTriggerAttribute _attribute;
            private System.Timers.Timer _timer;

            public Listener(ITriggeredFunctionExecutor executor, KubernetesTriggerAttribute attribute)
            {
                _executor = executor;
                _attribute = attribute;
                _timer = new System.Timers.Timer(5 * 1000)
                {
                    AutoReset = true
                };
                _timer.Elapsed += OnTimer;
            }

            public void Cancel()
            {
                // TODO: cancel the task
            }

            public void Dispose()
            {
                // Do some clean up
                _timer.Dispose();
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                // TODO: Start monitoring your event source. 
                _timer.Start();

                
                return Task.FromResult(true);
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                // TODO: Stop monitoring your event source
                _timer.Stop();
                return Task.FromResult(true);
            }

            private static HttpClient client;

            static Listener()
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPlicyErrors) => true;
                client = new HttpClient(httpClientHandler);
                client.BaseAddress = new Uri(System.Environment.GetEnvironmentVariable("serverUrl"));
            }

            private async void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
            {
                // Call Kubernetes REST API
                
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", System.Environment.GetEnvironmentVariable("kubernetesToken"));
                var response = await client.GetAsync("/api/v1/namespaces/default/pods");
                var result = await response.Content.ReadAsStringAsync();
                var resultOject = JsonConvert.DeserializeObject<Rootobject>(result);

                bool hasWrongPod = false;
                foreach (var item in resultOject.items)
                {
                    var ts = DateTime.UtcNow - item.status.startTime;
                    if ("Pending" == item.status.phase || ts.TotalMinutes > 5)
                    {
                        Console.WriteLine("**** Wrong Pod Detected ****");
                        hasWrongPod = true;
                    }
                    Console.WriteLine($"Pod: {item.metadata.name}");
                    Console.WriteLine($"Status: {item.status.phase}");
                    Console.WriteLine($"Started {ts.TotalMinutes} min before");
                }
                // Get to know if it is wrong Pod
                if (hasWrongPod) {
                    // Trigger the function.
                    var triggerValue = new KubernetesTriggerValue();
                    triggerValue.Result = result;
                    TriggeredFunctionData input = new TriggeredFunctionData
                    {

                        TriggerValue = triggerValue
                    };
                    await _executor.TryExecuteAsync(input, CancellationToken.None);
                }

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

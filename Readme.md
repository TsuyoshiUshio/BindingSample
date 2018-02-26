# Kubernetes bindings sample 

For my learning, I implement the Kubernetes bindings for Azure Functions. 

This trigger find the wrong state of the pods. The condition is the pod is Pending for 5 min.
Then trigged this function.  

# Configuration

## JobHostSample (WebJobs)

JobHostSample is WebJobs local environment.

Set the Environemnt Variables on your Visual Studio. Then start Storage Emulator on your Windows PC. 

| Name | Value |
|:-----|:------|
|AzureWebJobsEnv|Development|
|AzureWebJobsStorage|UseDevelopmentStorage=true
|AzureWebJobsDashboard|UseDevelopmentStorage=true|
|kubernetesToken| YOUR_KUBERNETES_TOKEN |
|serverUrl| YOUR_SERVER_URL |

NOTE: Please refer the following section to get the kubernetesToken and serverUrl

Hit the JobSample then it start WebJobs. 

## KubernetesBindingsSample (Azure Functions)

Set the same Configuration to the `local.settings.json` as the JobHostSample


## KubernetesRestSpike

Spike solution for Kubernetes Trigger. You need to create appsettings.json. Copy the appsettings.json_example to the appsetting.json. Then put it on your secret information from your Kubernetes cluster.

_SERVER_URL_

```
kubectl config view | grep server | cut -f 2- -d ":" | tr -d " "
```

_KUBERNETES_TOKEN_

```
kubectl describe secret $(kubectl get secrets | \
  grep default | cut -f1 -d ' ') | \
  grep -E '^token' | cut -f2 -d':' | tr -d '\t')
```


# Note

When I tried the first time, the Program.cs cause error `the specified container/blob does not exist` I couldn't fix that. The next day, I just add break points, it solved. I don't know why however, after that I can't reproduce the issue. Even if I clear the Storage Emulator. 


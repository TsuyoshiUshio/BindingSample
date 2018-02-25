# Kubernetes bindings sample 

For my learning, I implement the Kubernetes bindings for Azure Functions. 

# Configuration

Set the Environemnt Variables on your Visual Studio. Then start Storage Emulator on your Windows PC. 

| Name | Value |
|:-----|:------|
|AzureWebJobsEnv|Development|
|AzureWebJobsStorage|UseDevelopmentStorage=true
|AzureWebJobsDashboard|UseDevelopmentStorage=true|

Hit the JobSample then it start WebJobs. 

# Note

When I tried the first time, the Program.cs cause error `the specified container/blob does not exist` I couldn't fix that. The next day, I just add break points, it solved. I don't know why however, after that I can't reproduce the issue. Even if I clear the Storage Emulator. 


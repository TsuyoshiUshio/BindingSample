using System;
using System.Collections.Generic;
using System.Text;

namespace KubernetesRestSpike
{

    public class Rootobject
    {
        public string kind { get; set; }
        public string apiVersion { get; set; }
        public Metadata metadata { get; set; }
        public Item[] items { get; set; }
    }

    public class Metadata
    {
        public string selfLink { get; set; }
        public string resourceVersion { get; set; }
    }

    public class Item
    {
        public Metadata1 metadata { get; set; }
        public Spec spec { get; set; }
        public Status status { get; set; }
    }

    public class Metadata1
    {
        public string name { get; set; }
        public string generateName { get; set; }
        public string _namespace { get; set; }
        public string selfLink { get; set; }
        public string uid { get; set; }
        public string resourceVersion { get; set; }
        public DateTime creationTimestamp { get; set; }
        public Labels labels { get; set; }
        public Annotations annotations { get; set; }
        public Ownerreference[] ownerReferences { get; set; }
    }

    public class Labels
    {
        public string app { get; set; }
        public string podtemplatehash { get; set; }
        public string controllerrevisionhash { get; set; }
        public string dockerProviderVersion { get; set; }
        public string podtemplategeneration { get; set; }
        public string agentVersion { get; set; }
    }

    public class Annotations
    {
        public string kubernetesiocreatedby { get; set; }
    }

    public class Ownerreference
    {
        public string apiVersion { get; set; }
        public string kind { get; set; }
        public string name { get; set; }
        public string uid { get; set; }
        public bool controller { get; set; }
        public bool blockOwnerDeletion { get; set; }
    }

    public class Spec
    {
        public Volume[] volumes { get; set; }
        public Container[] containers { get; set; }
        public string restartPolicy { get; set; }
        public int terminationGracePeriodSeconds { get; set; }
        public string dnsPolicy { get; set; }
        public string serviceAccountName { get; set; }
        public string serviceAccount { get; set; }
        public string nodeName { get; set; }
        public Securitycontext securityContext { get; set; }
        public string schedulerName { get; set; }
        public Toleration[] tolerations { get; set; }
        public Nodeselector nodeSelector { get; set; }
    }

    public class Securitycontext
    {
    }

    public class Nodeselector
    {
        public string betakubernetesioos { get; set; }
    }

    public class Volume
    {
        public string name { get; set; }
        public Persistentvolumeclaim persistentVolumeClaim { get; set; }
        public Secret secret { get; set; }
        public Hostpath hostPath { get; set; }
    }

    public class Persistentvolumeclaim
    {
        public string claimName { get; set; }
    }

    public class Secret
    {
        public string secretName { get; set; }
        public int defaultMode { get; set; }
    }

    public class Hostpath
    {
        public string path { get; set; }
        public string type { get; set; }
    }

    public class Container
    {
        public string name { get; set; }
        public string image { get; set; }
        public Port[] ports { get; set; }
        public Env[] env { get; set; }
        public Resources resources { get; set; }
        public Volumemount[] volumeMounts { get; set; }
        public string terminationMessagePath { get; set; }
        public string terminationMessagePolicy { get; set; }
        public string imagePullPolicy { get; set; }
        public Livenessprobe livenessProbe { get; set; }
        public Securitycontext1 securityContext { get; set; }
    }

    public class Resources
    {
    }

    public class Livenessprobe
    {
        public Exec exec { get; set; }
        public int initialDelaySeconds { get; set; }
        public int timeoutSeconds { get; set; }
        public int periodSeconds { get; set; }
        public int successThreshold { get; set; }
        public int failureThreshold { get; set; }
    }

    public class Exec
    {
        public string[] command { get; set; }
    }

    public class Securitycontext1
    {
        public bool privileged { get; set; }
    }

    public class Port
    {
        public int containerPort { get; set; }
        public string protocol { get; set; }
    }

    public class Env
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Volumemount
    {
        public string name { get; set; }
        public string mountPath { get; set; }
        public bool readOnly { get; set; }
    }

    public class Toleration
    {
        public string key { get; set; }
        public string _operator { get; set; }
        public string effect { get; set; }
        public int tolerationSeconds { get; set; }
        public string value { get; set; }
    }

    public class Status
    {
        public string phase { get; set; }
        public Condition[] conditions { get; set; }
        public string hostIP { get; set; }
        public string podIP { get; set; }
        public DateTime startTime { get; set; }
        public Containerstatus[] containerStatuses { get; set; }
        public string qosClass { get; set; }
    }

    public class Condition
    {
        public string type { get; set; }
        public string status { get; set; }
        public object lastProbeTime { get; set; }
        public DateTime lastTransitionTime { get; set; }
    }

    public class Containerstatus
    {
        public string name { get; set; }
        public State state { get; set; }
        public Laststate lastState { get; set; }
        public bool ready { get; set; }
        public int restartCount { get; set; }
        public string image { get; set; }
        public string imageID { get; set; }
        public string containerID { get; set; }
    }

    public class State
    {
        public Running running { get; set; }
        public Waiting waiting { get; set; }
        
    }

    public class Running
    {
        public DateTime startedAt { get; set; }
    }
    public class Waiting
    {
        public string resason { get; set; }
        public string message { get; set; }
    }

    public class Laststate
    {
    }

}

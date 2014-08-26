<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Worker" generation="1" functional="0" release="0" Id="4ae216c6-d05d-4133-907c-adc666f42a0c" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="WorkerGroup" generation="1" functional="0" release="0">
      <settings>
        <aCS name="JobWorker:ServiceBusConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Worker/WorkerGroup/MapJobWorker:ServiceBusConnectionString" />
          </maps>
        </aCS>
        <aCS name="JobWorker:StorageConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Worker/WorkerGroup/MapJobWorker:StorageConnectionString" />
          </maps>
        </aCS>
        <aCS name="JobWorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Worker/WorkerGroup/MapJobWorkerInstances" />
          </maps>
        </aCS>
      </settings>
      <maps>
        <map name="MapJobWorker:ServiceBusConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Worker/WorkerGroup/JobWorker/ServiceBusConnectionString" />
          </setting>
        </map>
        <map name="MapJobWorker:StorageConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Worker/WorkerGroup/JobWorker/StorageConnectionString" />
          </setting>
        </map>
        <map name="MapJobWorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Worker/WorkerGroup/JobWorkerInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="JobWorker" generation="1" functional="0" release="0" software="C:\Projects\bbvAcademyAzure\source\TheFlatterist\Worker\csx\Release\roles\JobWorker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="ServiceBusConnectionString" defaultValue="" />
              <aCS name="StorageConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;JobWorker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;JobWorker&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Worker/WorkerGroup/JobWorkerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Worker/WorkerGroup/JobWorkerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Worker/WorkerGroup/JobWorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="JobWorkerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="JobWorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="JobWorkerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
</serviceModel>
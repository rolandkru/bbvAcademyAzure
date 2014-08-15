// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lab3Controller.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Lab3Controller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Web.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web.Http;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using Web.Common;
    using Web.DataAccess;

    public class Lab3Controller : ApiController
    {
        private const string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=theflatterist;AccountKey=y9yG05qIryQmqBTgVS8k1YL3ag93iQY5pfJEYape9IVQIC7wsftb2Tn3OBwiW8dEArcYrzovIafNRBu/FS+L6Q==;";

        private const string serviceBusNamespace = "flatterist";
        private const string sasKey = "RT9H5z3VQIzPn533rsJpOEN2t+mAH9kTS0bzE32cMJo=";

        readonly string serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");


        // GET api/lab3/[id]
        public SasDto Get(string clientId)
        {
            Client client;
            using (var dbcontext = new Flatterist())
            {
                client = dbcontext.Clients.FirstOrDefault(c => c.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase));
                if (client == null)
                {
                    client = new Client { ClientId = clientId, QueueName = "jobqueue" };
                    dbcontext.Clients.Add(client);
                }
            }

            this.SetupQueue(client.QueueName, "Send", AccessRights.Send);
            this.SetupQueue(client.ClientId, "Listen", AccessRights.Listen, TimeSpan.FromHours(24));

            var sasDto = new SasDto();
            sasDto.ExpirationTime = DateTimeOffset.Now.AddHours(24);
            sasDto.BlobContainerUrl = this.GetBlobContainerSas(sasDto.ExpirationTime, client.ClientId);
            
            sasDto.ServiceBusNamespace = serviceBusNamespace;
            
            sasDto.JobQueue = client.QueueName;
            sasDto.JobQueueSasUrl = this.GetQueueSas(client.QueueName, "Send", TimeSpan.FromHours(24));

            sasDto.ResultQueue = client.ClientId;
            sasDto.ResultQueueUrl = this.GetQueueSas(client.ClientId, "Listen", TimeSpan.FromHours(24));

            return sasDto;
        }

        private string GetBlobContainerSas(DateTimeOffset expirationTime, string clientId)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(clientId);
            container.CreateIfNotExists();

            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy();
            policy.SharedAccessExpiryTime = expirationTime;
            policy.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Delete;
            return container.Uri + container.GetSharedAccessSignature(policy);
        }

        private string GetQueueSas(string queueName, string policyName, TimeSpan expirationTime)
        {
            var serviceUri1 = ServiceBusEnvironment.CreateServiceUri("sb", serviceBusNamespace, queueName)
               .ToString()
               .Trim('/');


            var sas = SharedAccessSignatureTokenProvider.GetSharedAccessSignature(
                policyName,
                sasKey,
                serviceUri1,
                expirationTime);

            return sas;
        }

        private void SetupQueue(string queueName, string sasPolicyName, AccessRights accessRight)
        {
            this.SetupQueue(queueName, sasPolicyName, accessRight, new TimeSpan(0));
        }

        private void SetupQueue(string queueName, string sasPolicyName, AccessRights accessRight, TimeSpan autoDeleteOnIdle)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(this.serviceBusConnectionString);
            if (!namespaceManager.QueueExists(queueName))
            {
                namespaceManager.CreateQueue(queueName);
            }

            var queue = namespaceManager.GetQueue(queueName);
            if (queue.Authorization.All(a => a.KeyName != sasPolicyName))
            {
                queue.Authorization.Add(new SharedAccessAuthorizationRule(sasPolicyName, sasKey, new[] { accessRight }));
            }

            if (autoDeleteOnIdle.TotalSeconds > 0)
            {
                queue.AutoDeleteOnIdle = autoDeleteOnIdle;
            }

            namespaceManager.UpdateQueue(queue);
        }
    }
}

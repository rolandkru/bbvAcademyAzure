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
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Common;
    using Common.Dtos;
    using Common.Helper;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage.Blob;

    using Web.DataAccess;

    public class Lab3Controller : ApiController
    {
        private readonly string serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
        private readonly string serviceBusNamespace = CloudConfigurationManager.GetSetting("ServiceBusNamespace");
        private readonly string sasKey = CloudConfigurationManager.GetSetting("SasPolicyKey");

        // GET api/lab3/[id]
        public async Task<SasDto> Get(string clientId)
        {
            var client = await GetClientInfoFromDB(clientId);

            await this.SetupQueue(client.QueueName, "Send", AccessRights.Send);
            await this.SetupQueue(client.ClientId, "Listen", AccessRights.Listen, TimeSpan.FromHours(24));

            var sasDto = new SasDto();
            sasDto.ExpirationTime = DateTimeOffset.Now.AddHours(24);
            sasDto.BlobContainerUrl = await this.GetBlobContainerSas(sasDto.ExpirationTime, client.ClientId);
            
            sasDto.ServiceBusNamespace = this.serviceBusNamespace;
            
            sasDto.JobQueue = client.QueueName;
            sasDto.JobQueueSasUrl = this.GetQueueSas(client.QueueName, "Send", TimeSpan.FromHours(24));

            sasDto.ResultQueue = client.ClientId;
            sasDto.ResultQueueUrl = this.GetQueueSas(client.ClientId, "Listen", TimeSpan.FromHours(24));

            return sasDto;
        }

        private static async Task<Client> GetClientInfoFromDB(string clientId)
        {
            Client client;
            using (var dbcontext = new Flatterist())
            {
                client = await dbcontext.Clients.FirstOrDefaultAsync(c => c.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase));
                if (client == null)
                {
                    client = new Client { ClientId = clientId, QueueName = Constants.Lab3JobQueueName };
                    dbcontext.Clients.Add(client);
                    await dbcontext.SaveChangesAsync();
                }
            }

            return client;
        }

        private async Task<string> GetBlobContainerSas(DateTimeOffset expirationTime, string clientId)
        {
            CloudBlobContainer container = await HelperMethods.GetJobContainer(clientId);

            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy();
            policy.SharedAccessExpiryTime = expirationTime;
            policy.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Delete;
            return container.Uri + container.GetSharedAccessSignature(policy);
        }

        private string GetQueueSas(string queueName, string policyName, TimeSpan expirationTime)
        {
            var serviceUri1 = ServiceBusEnvironment.CreateServiceUri("sb", serviceBusNamespace, queueName).ToString().Trim('/');

            var sas = SharedAccessSignatureTokenProvider.GetSharedAccessSignature(
                policyName,
                sasKey,
                serviceUri1,
                expirationTime);

            return sas;
        }

        private async Task SetupQueue(string queueName, string sasPolicyName, AccessRights accessRight)
        {
            await this.SetupQueue(queueName, sasPolicyName, accessRight, new TimeSpan(0));
        }

        private async Task SetupQueue(string queueName, string sasPolicyName, AccessRights accessRight, TimeSpan autoDeleteOnIdle)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(this.serviceBusConnectionString);
            if (!await namespaceManager.QueueExistsAsync(queueName))
            {
                await namespaceManager.CreateQueueAsync(queueName);
            }

            var queue = await namespaceManager.GetQueueAsync(queueName);
            if (queue.Authorization.All(a => a.KeyName != sasPolicyName))
            {
                queue.Authorization.Add(new SharedAccessAuthorizationRule(sasPolicyName, this.sasKey, new[] { accessRight }));
            }

            if (autoDeleteOnIdle.TotalSeconds > 0)
            {
                queue.AutoDeleteOnIdle = autoDeleteOnIdle;
            }

            await namespaceManager.UpdateQueueAsync(queue);
        }
    }
}

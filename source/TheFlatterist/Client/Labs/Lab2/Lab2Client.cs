namespace Client.Labs.Lab2
{
    using System;
    using System.Threading.Tasks;

    using Common;
    using Common.Helper;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage.Blob;

    internal class Lab2Client
    {
        private string serviceBusConnectionString;
        private CloudBlobContainer blobContainer;

        public async Task RunAsync(string name)
        {
            string clientId = Guid.NewGuid().ToString("n");
            
            this.serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            this.blobContainer = await HelperMethods.GetJobContainer(clientId);
            
            await this.InitializeServiceBus(clientId);

            UploadJob uploadJob = new UploadJob(this.blobContainer, this.serviceBusConnectionString, Constants.Lab2JobQueueName);
            uploadJob.StartAsync(name, clientId);

            DownloadJob downloadJob = new DownloadJob(this.blobContainer, this.serviceBusConnectionString, Constants.Lab2ResponseTopicName, clientId);
            downloadJob.StartAsync();

            Console.ReadLine();
        }

        private async Task InitializeServiceBus(string clientId)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(this.serviceBusConnectionString);

            if (!await namespaceManager.QueueExistsAsync(Constants.Lab2JobQueueName))
            {
                await namespaceManager.CreateQueueAsync(Constants.Lab2JobQueueName);
            }

            if (!await namespaceManager.TopicExistsAsync(Constants.Lab2ResponseTopicName))
            {
                await namespaceManager.CreateTopicAsync(Constants.Lab2ResponseTopicName);
            }

            if (!await namespaceManager.SubscriptionExistsAsync(Constants.Lab2ResponseTopicName, clientId))
            {
                await namespaceManager.CreateSubscriptionAsync(Constants.Lab2ResponseTopicName, clientId, new SqlFilter(string.Format("ClientId = '{0}'", clientId)));
            }
        }
    }
}
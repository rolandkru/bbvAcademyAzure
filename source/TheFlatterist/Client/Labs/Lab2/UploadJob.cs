namespace Client.Labs.Lab2
{
    using System;
    using System.Threading;

    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure.Storage.Blob;

    internal class UploadJob
    {
        private readonly CloudBlobContainer blobContainer;

        private readonly QueueClient queue;

        internal UploadJob(CloudBlobContainer container, string serviceBusConnectionString, string jobQueueName)
        {
            this.blobContainer = container;
            this.queue = QueueClient.CreateFromConnectionString(serviceBusConnectionString, jobQueueName);
        }

        internal async void StartAsync(string name, string clientId)
        {
            while (true)
            {
                string jobId = Guid.NewGuid().ToString("n");

                CloudBlockBlob jobBlob = this.blobContainer.GetBlockBlobReference("job/" + jobId);
                await jobBlob.UploadTextAsync(name);

                var message = new BrokeredMessage(jobId);
                message.Properties.Add("ClientId", clientId);
                await this.queue.SendAsync(message);

                Thread.Sleep(5000);
            }
        }
    }
}

namespace Client.Labs.Lab3
{
    using System;
    using System.Threading;

    using Common.Dtos;

    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure.Storage.Blob;

    internal class UploadJob
    {
        private readonly CloudBlobContainer blobContainer;

        private readonly QueueClient queue;

        internal UploadJob(SasDto sasDto)
        {
            this.blobContainer = new CloudBlobContainer(new Uri(sasDto.BlobContainerUrl));
            this.queue = Lab3Helper.GetQueueClient(sasDto.ServiceBusNamespace, sasDto.JobQueueSasUrl, sasDto.JobQueue);
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

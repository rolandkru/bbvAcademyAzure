namespace Client.Labs.Lab2
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure.Storage.Blob;

    internal class DownloadJob
    {
        private readonly CloudBlobContainer blobContainer;

        private readonly SubscriptionClient subscription;

        internal DownloadJob(CloudBlobContainer container, string serviceBusConnectionString, string responseTopicName, string clientId)
        {
            this.blobContainer = container;
            this.subscription = SubscriptionClient.CreateFromConnectionString(serviceBusConnectionString, responseTopicName, clientId);
        }

        internal async void StartAsync()
        {
            while (true)
            {
                var jobId = await this.ListenToResponseSubscription();
                await this.DownloadResult(jobId);
            }
        }

        private async Task<string> ListenToResponseSubscription()
        {
            string jobid;
            while (true)
            {
                var message = await this.subscription.ReceiveAsync(TimeSpan.FromSeconds(5));
                if (message != null)
                {
                    jobid = message.GetBody<string>();
                    await message.CompleteAsync();
                    break;
                }
            }

            return jobid;
        }

        private async Task DownloadResult(string jobId)
        {
            CloudBlockBlob resultBlob = this.blobContainer.GetBlockBlobReference("result/" + jobId);
            if (await resultBlob.ExistsAsync())
            {
                var result = await resultBlob.DownloadTextAsync();
                Console.WriteLine(result);
                await resultBlob.DeleteAsync();

                CloudBlockBlob jobBlob = this.blobContainer.GetBlockBlobReference("job/" + jobId);
                await jobBlob.DeleteAsync();
            }
        }
    }
}

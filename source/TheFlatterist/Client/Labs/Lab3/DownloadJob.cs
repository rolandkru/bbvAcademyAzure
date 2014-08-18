namespace Client.Labs.Lab3
{
    using System;
    using System.Threading.Tasks;

    using Common.Dtos;

    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure.Storage.Blob;

    internal class DownloadJob
    {
        private readonly CloudBlobContainer blobContainer;

        private readonly QueueClient queue;

         internal DownloadJob(SasDto sasDto)
        {
            this.blobContainer = new CloudBlobContainer(new Uri(sasDto.BlobContainerUrl));
            this.queue = Lab3Helper.GetQueueClient(sasDto.ServiceBusNamespace, sasDto.ResultQueueUrl, sasDto.ResultQueue);
        }

        internal async void StartAsync()
        {
            while (true)
            {
                var jobId = await this.ListenToResponseQueue();
                await this.DownloadResult(jobId);
            }
        }

        private async Task<string> ListenToResponseQueue()
        {
            string jobid;
            while (true)
            {
                var message = await this.queue.ReceiveAsync(TimeSpan.FromSeconds(5));
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

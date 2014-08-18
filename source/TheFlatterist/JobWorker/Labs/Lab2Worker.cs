namespace JobWorker.Labs
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Common;
    using Common.Dtos;
    using Common.Helper;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage.Blob;

    internal class Lab2Worker
    {
        private QueueClient jobQueue;
        private TopicClient responseTopic;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await this.InitializeServiceBus();

            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                var job = await this.ListenToJobQueue();
                await this.ProcessJob(job);
                await this.AddMessageToResponseTopic(job);
                
                Thread.Sleep(500);
            }
        }

        private async Task<JobDto> ListenToJobQueue()
        {
            while (true)
            {
                // Long Polling
                var message = await this.jobQueue.ReceiveAsync(TimeSpan.FromSeconds(5));
                if (message != null)
                {
                    JobDto job = new JobDto();
                    job.ClientId = message.Properties["ClientId"].ToString();
                    job.JobId = message.GetBody<string>();

                    message.Complete();

                    return job;
                }
            }
        }

        private async Task ProcessJob(JobDto job)
        {
            var clientContainer = await HelperMethods.GetJobContainer(job.ClientId);

            CloudBlockBlob jobBlob = clientContainer.GetBlockBlobReference("job/" + job.JobId);
            var name = jobBlob.DownloadText();
            var result = Compliments.GetRandomCompliment(name);

            CloudBlockBlob resultBlob = clientContainer.GetBlockBlobReference("result/" + job.JobId);
            resultBlob.UploadText(result);
        }

        private async Task AddMessageToResponseTopic(JobDto job)
        {
            var message = new BrokeredMessage(job.JobId);
            message.Properties["ClientId"] = job.ClientId;
            await this.responseTopic.SendAsync(message);
        }

        private async Task InitializeServiceBus()
        {
            var serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(serviceBusConnectionString);

            if (!await namespaceManager.QueueExistsAsync(Constants.Lab2JobQueueName))
            {
                await namespaceManager.CreateQueueAsync(Constants.Lab2JobQueueName);
            }

            if (!await namespaceManager.TopicExistsAsync(Constants.Lab2ResponseTopicName))
            {
                await namespaceManager.CreateTopicAsync(Constants.Lab2ResponseTopicName);
            }

            this.jobQueue = QueueClient.CreateFromConnectionString(serviceBusConnectionString, Constants.Lab2JobQueueName);
            this.responseTopic = TopicClient.CreateFromConnectionString(serviceBusConnectionString, Constants.Lab2ResponseTopicName);
        }
    }
}

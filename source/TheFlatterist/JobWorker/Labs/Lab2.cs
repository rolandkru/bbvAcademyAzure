// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lab1.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Lab1 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JobWorker.Labs
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    class Lab2
    {
        private const string JobQueueName = "JobQueue";
        private const string ResponseTopicName = "ResponseTopic";

        readonly string serviceBusConnectionString = CloudConfigurationManager.GetSetting("ServiceBusConnectionString");
        readonly string storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                var jobId = this.ListenToJobQueue();
                var clientId = this.ProcessJob(jobId);
                this.AddMessageToResponseTopic(clientId, jobId);
                

                Thread.Sleep(5000);
            }
        }

        private string ListenToJobQueue()
        {
            var client = QueueClient.CreateFromConnectionString(this.serviceBusConnectionString, JobQueueName);
            string jobid;
            while (true)
            {
                var message = client.Receive(TimeSpan.FromSeconds(5));
                if (message != null)
                {
                    jobid = message.GetBody<string>();
                    message.Complete();
                    break;
                }
            }

            return jobid;
        }

        private string ProcessJob(string jobId)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(jobId);

            // Retrieve reference to a blob named "jobfile".
            CloudBlockBlob jobBlob = container.GetBlockBlobReference("jobfile");
            var name = jobBlob.DownloadText();
            var clientId = jobBlob.Metadata["ClientId"];
            
            var compliment = Compliments.GetRandomCompliment();
            var result = string.Format("{0}, {1}", name, compliment);

            CloudBlockBlob resultBlob = container.GetBlockBlobReference("resultfile");
            resultBlob.UploadText(result);

            // Create or overwrite the "jobfile" blob.
            jobBlob.UploadText(name);

            return clientId;
        }

        private void AddMessageToResponseTopic(string clientId, string jobId)
        {
            var client = TopicClient.CreateFromConnectionString(this.serviceBusConnectionString, ResponseTopicName);
            var message = new BrokeredMessage(jobId);
            message.Properties["ClientId"] = clientId;
            client.Send(message);
        }
    }
}

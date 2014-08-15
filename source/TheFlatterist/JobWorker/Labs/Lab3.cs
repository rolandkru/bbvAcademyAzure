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

    class Lab3
    {
        private const string JobQueueName = "JobQueue";

        readonly string serviceBusConnectionString = CloudConfigurationManager.GetSetting("ServiceBusConnectionString");
        readonly string storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                string clientId = null;
                var jobId = this.ListenToJobQueue(out clientId);
                this.ProcessJob(clientId, jobId);
                this.AddMessageToResponseTopic(clientId, jobId);
                

                Thread.Sleep(500);
            }
        }

        private string ListenToJobQueue(out string clientId)
        {
            var client = QueueClient.CreateFromConnectionString(this.serviceBusConnectionString, JobQueueName);
            string jobid;
            while (true)
            {
                var message = client.Receive(TimeSpan.FromSeconds(5));
                if (message != null)
                {
                    jobid = message.GetBody<string>();
                    clientId = message.Properties["ClientId"].ToString();
                    message.Complete();
                    break;
                }
            }

            return jobid;
        }

        private void ProcessJob(string clientId, string jobId)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(clientId);

            // Retrieve reference to a blob named "jobfile".
            CloudBlockBlob jobBlob = container.GetBlockBlobReference(jobId + "_job");
            var name = jobBlob.DownloadText();
            
            var compliment = Compliments.GetRandomCompliment();
            var result = string.Format("{0}, {1}", name, compliment);

            CloudBlockBlob resultBlob = container.GetBlockBlobReference(jobId + "_result");
            resultBlob.UploadText(result);

            // Create or overwrite the "jobfile" blob.
            jobBlob.UploadText(name);
        }

        private void AddMessageToResponseTopic(string clientId, string jobId)
        {
            var client = QueueClient.CreateFromConnectionString(this.serviceBusConnectionString, clientId);
            var message = new BrokeredMessage(jobId);
            client.Send(message);
        }
    }
}

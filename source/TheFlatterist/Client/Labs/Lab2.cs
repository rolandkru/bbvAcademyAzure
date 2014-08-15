// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lab1.cs" company="">
//   
// </copyright>
// <summary>
//   The lab 1.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Client.Labs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// The lab 1.
    /// </summary>
    internal class Lab2
    {
        /// <summary>
        /// The storage connection string.
        /// </summary>
        private const string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=theflatterist;AccountKey=y9yG05qIryQmqBTgVS8k1YL3ag93iQY5pfJEYape9IVQIC7wsftb2Tn3OBwiW8dEArcYrzovIafNRBu/FS+L6Q==;";

        private const string JobQueueName = "JobQueue";
        private const string ResponseTopicName = "ResponseTopic";

        readonly string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void Run(string name)
        {
            string clientId = Guid.NewGuid().ToString("n");
            this.InitializeResponseSubscription(clientId);

            var producerThread = new Thread(() => this.SetupJob(name, clientId));
            producerThread.Start();

            var consumerThread = new Thread(() => this.GetResults(clientId));
            consumerThread.Start();

            Console.ReadLine();
        }

        private void SetupJob(string name, string clientId)
        {
            while (true)
            {
                string jobId = Guid.NewGuid().ToString("n");

                this.UploadFile(name, clientId, jobId);
                this.AddJobToQueue(jobId);

                Thread.Sleep(5000);
            }
        }

        private void GetResults(string clientId)
        {
            while (true)
            {
                var jobId = this.ListenToResponseSubscription(clientId);
                this.DownloadResult(jobId);
            }
        }

        private void InitializeResponseSubscription(string clientId)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.TopicExists(ResponseTopicName))
            {
                namespaceManager.CreateTopic(ResponseTopicName);
            }

            if (!namespaceManager.SubscriptionExists(ResponseTopicName, clientId))
            {
                namespaceManager.CreateSubscription(ResponseTopicName, clientId, new SqlFilter(string.Format("ClientId = '{0}'", clientId)));
            }
        }

        private void UploadFile(string name, string clientId,string jobId)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(jobId);
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "jobfile".
            CloudBlockBlob jobBlob = container.GetBlockBlobReference("jobfile");
            

            // Create or overwrite the "jobfile" blob.
            jobBlob.UploadText(name);
            
            jobBlob.Metadata.Add("ClientId", clientId);
            jobBlob.SetMetadata();
        }

        private void AddJobToQueue(string jobId)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists(JobQueueName))
            {
                namespaceManager.CreateQueue(JobQueueName);
            }

            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, JobQueueName);

            var message = new BrokeredMessage(jobId);

            // Send message to the queue
            client.Send(message);
        }

        private string ListenToResponseSubscription(string clientId)
        {
            var client = SubscriptionClient.CreateFromConnectionString(this.connectionString, ResponseTopicName, clientId);

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

        private void DownloadResult(string jobId)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(jobId);

            CloudBlockBlob resultBlob = container.GetBlockBlobReference("resultfile");
            if (resultBlob.Exists())
            {
                var result = resultBlob.DownloadText();
                Console.WriteLine(result);
                container.Delete();
            }
        }
    }
}
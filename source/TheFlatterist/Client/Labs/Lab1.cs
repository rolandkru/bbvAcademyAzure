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

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// The lab 1.
    /// </summary>
    internal class Lab1
    {
        /// <summary>
        /// The storage connection string.
        /// </summary>
        private const string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=theflatterist;AccountKey=y9yG05qIryQmqBTgVS8k1YL3ag93iQY5pfJEYape9IVQIC7wsftb2Tn3OBwiW8dEArcYrzovIafNRBu/FS+L6Q==;";

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void Run(string name)
        {
            while (true)
            {
                string jobId = Guid.NewGuid().ToString("n");

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

                while (true)
                {
                    CloudBlockBlob resultBlob = container.GetBlockBlobReference("resultfile");
                    if (resultBlob.Exists())
                    {
                        var result = resultBlob.DownloadText();
                        Console.WriteLine(result);
                        break;
                    }

                    Thread.Sleep(5000);
                }
            }
        }
    }
}
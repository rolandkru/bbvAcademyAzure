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

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    class Lab1
    {
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                var containers = blobClient.ListContainers(detailsIncluded: ContainerListingDetails.Metadata);

                foreach (var container in containers)
                {
                    Guid temp;
                    if (!Guid.TryParse(container.Name, out temp))
                    {
                        // not a job container
                        continue;
                    }

                    if (container.Metadata.ContainsKey("Status") && container.Metadata["Status"] == "finished")
                    {
                        // already finished
                        continue;
                    }


                    this.ProcessJob(container);
                }

                Thread.Sleep(5000);
            }
        }

        private void ProcessJob(CloudBlobContainer container)
        {
            // Retrieve reference to a blob named "jobfile".
            CloudBlockBlob jobBlob = container.GetBlockBlobReference("jobfile");
            var name = jobBlob.DownloadText();

            var compliment = Compliments.GetRandomCompliment();
            var result = string.Format("{0}, {1}", name, compliment);

            CloudBlockBlob resultBlob = container.GetBlockBlobReference("resultfile");
            resultBlob.UploadText(result);

            // Create or overwrite the "jobfile" blob.
            jobBlob.UploadText(name);

            container.Metadata["Status"] = "finished";
            container.SetMetadata();
        }
    }
}

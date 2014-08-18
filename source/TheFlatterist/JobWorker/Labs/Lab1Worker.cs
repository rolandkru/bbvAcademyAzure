namespace JobWorker.Labs
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Helper;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    internal class Lab1Worker
    {
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var storageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                var containers = await blobClient.ListContainersAsync(ContainerListingDetails.Metadata);

                foreach (var container in containers.Where(c => c.Metadata.ContainsKey("JobContainer")))
                {
                    var jobBlobs = await container.ListBlobsAsync("job/", BlobListingDetails.Metadata);
                    foreach (var blob in jobBlobs)
                    {
                        var jobBlob = blob as CloudBlockBlob;
                        if (jobBlob != null && !(jobBlob.Metadata.ContainsKey("Status") && jobBlob.Metadata["Status"] == "finished"))
                        {
                            await this.ProcessJob(jobBlob, container);
                        }
                    }
                }

                Thread.Sleep(500);
            }
        }

        private async Task ProcessJob(CloudBlockBlob jobBlob, CloudBlobContainer container)
        {
            var name = await jobBlob.DownloadTextAsync();
            var result = Compliments.GetRandomCompliment(name);

            string resultBlobName = jobBlob.Name.Replace("job/", "result/");

            CloudBlockBlob resultBlob = container.GetBlockBlobReference(resultBlobName);
            await resultBlob.UploadTextAsync(result);

            jobBlob.Metadata["Status"] = "finished";
            await jobBlob.SetMetadataAsync();
        }
    }
}

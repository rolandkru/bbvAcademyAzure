namespace Client.Labs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Helper;

    using Microsoft.WindowsAzure.Storage.Blob;

    internal class Lab1Client
    {
        public async Task RunAsync(string name)
        {
            // Generate a unique identifier for the client instance.
            string clientId = Guid.NewGuid().ToString("n");
            
            var blobContainer = await HelperMethods.GetJobContainer(clientId);

            while (true)
            {
                // Create a new job
                string jobId = Guid.NewGuid().ToString("n");

                // Retrieve reference to a blob named "jobfile".
                CloudBlockBlob jobBlob = blobContainer.GetBlockBlobReference("job/" + jobId);

                // Create or overwrite the "jobfile" blob.
                await jobBlob.UploadTextAsync(name);

                // Wait for the result (polling...)
                while (true)
                {
                    CloudBlockBlob resultBlob = blobContainer.GetBlockBlobReference("result/" + jobId);
                    if (await resultBlob.ExistsAsync())
                    {
                        var result = await resultBlob.DownloadTextAsync();
                        Console.WriteLine(result);
                        await jobBlob.DeleteAsync();
                        await resultBlob.DeleteAsync();
                        break;
                    }

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
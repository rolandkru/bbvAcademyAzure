namespace Common.Helper
{
    using System.Configuration;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public static class HelperMethods
    {
        public static async Task<CloudBlobContainer> GetJobContainer(string clientId)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a blob container for this client.
            CloudBlobContainer container = blobClient.GetContainerReference(clientId);
            
            if (!await container.ExistsAsync())
            {
                await container.CreateIfNotExistsAsync();

                container.Metadata.Add("JobContainer", "true");
                await container.SetMetadataAsync();
            }

            return container;
        }
    }
}
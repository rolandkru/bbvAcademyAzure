namespace Common.Helper
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public static class StorageExtensions
    {
        public static async Task<List<CloudBlobContainer>> ListContainersAsync(this CloudBlobClient client, ContainerListingDetails details)
        {
            BlobContinuationToken continuationToken = null;
            BlobRequestOptions blobRequestOptions = new BlobRequestOptions();
            OperationContext operationContext = new OperationContext();

            List<CloudBlobContainer> results = new List<CloudBlobContainer>();
            do
            {
                var response = await client.ListContainersSegmentedAsync(string.Empty, details, null, continuationToken, blobRequestOptions, operationContext);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results;
        }

        public static async Task<List<IListBlobItem>> ListBlobsAsync(this CloudBlobContainer container, string prefix, BlobListingDetails details)
        {
            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = await container.ListBlobsSegmentedAsync(prefix, true, details, null, continuationToken, new BlobRequestOptions(), new OperationContext());
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results;
        }
    }
}

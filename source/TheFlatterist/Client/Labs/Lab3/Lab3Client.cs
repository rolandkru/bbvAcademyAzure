// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lab3Client.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Lab3Client type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Client.Labs.Lab3
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Common.Dtos;

    using Microsoft.WindowsAzure;

    internal class Lab3Client
    {
        public async Task RunAsync(string name)
        {
            string clientId = Guid.NewGuid().ToString("n");

            var sasDto = await GetSharedAccessSignatures(clientId);

            UploadJob uploadJob = new UploadJob(sasDto);
            uploadJob.StartAsync(name, clientId);

            DownloadJob downloadJob = new DownloadJob(sasDto);
            downloadJob.StartAsync();

            Console.ReadLine();
        }

        private static async Task<SasDto> GetSharedAccessSignatures(string clientId)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(CloudConfigurationManager.GetSetting("BackendUrl"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await httpClient.GetAsync(string.Format("api/Lab3?clientId={0}", clientId));

            SasDto sasDto;
            if (response.IsSuccessStatusCode)
            {
                sasDto = await response.Content.ReadAsAsync<SasDto>();
            }
            else
            {
                throw new Exception("Backend could not be reached.");
            }

            return sasDto;
        }
    }
}

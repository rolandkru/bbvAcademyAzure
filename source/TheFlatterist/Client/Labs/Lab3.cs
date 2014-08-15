// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lab3.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Lab3 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Client.Labs
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure.Storage.Blob;

    using Web.Common;

    class Lab3
    {
        public void Run(string name)
        {
            string clientId = Guid.NewGuid().ToString("n");

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://flatterist.azurewebsites.net/");

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = httpClient.GetAsync(string.Format("api/Lab3?clientId={0}", clientId)).Result;
            SasDto sasDto;
            if (response.IsSuccessStatusCode)
            {
                sasDto = response.Content.ReadAsAsync<SasDto>().Result;
            }
            else
            {
                throw new Exception("Backend could not be reached.");
            }

             var producerThread = new Thread(() => this.SetupJob(sasDto, name, clientId));
            producerThread.Start();

            var consumerThread = new Thread(() => this.GetResults(sasDto, clientId));
            consumerThread.Start();

            Console.ReadLine();
        }

        private void SetupJob(SasDto sasDto, string name, string clientId)
        {
            while (true)
            {
                string jobId = Guid.NewGuid().ToString("n");

                this.UploadFile(sasDto, name, clientId, jobId);
                this.AddJobToQueue(sasDto, clientId, jobId);

                Thread.Sleep(5000);
            }
        }

        private void GetResults(SasDto sasDto, string clientId)
        {
            while (true)
            {
                var jobId = this.ListenToResponseSubscription(sasDto, clientId);
                this.DownloadResult(sasDto, jobId);
            }
        }

        private void UploadFile(SasDto sasDto, string name, string clientId, string jobId)
        {
            // Retrieve reference to a container.
            var container = new CloudBlobContainer(new Uri(sasDto.BlobContainerUrl));

            // Retrieve reference to a blob named "jobfile".
            var jobBlob = container.GetBlockBlobReference(jobId + "_job");
            
            // Create or overwrite the "jobfile" blob.
            jobBlob.UploadText(name);
            
            jobBlob.Metadata.Add("ClientId", clientId);
            jobBlob.SetMetadata();
        }

        private void AddJobToQueue(SasDto sasDto, string clientId, string jobId)
        {
            var serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", sasDto.ServiceBusNamespace, string.Empty)
               .ToString()
               .Trim('/');

            var factory = MessagingFactory.Create(serviceUri, new MessagingFactorySettings
            {
                TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(sasDto.JobQueueSasUrl)
            });

            var queue = factory.CreateQueueClient(sasDto.JobQueue);
            var message = new BrokeredMessage(jobId);
            message.Properties.Add("ClientId", clientId);
            queue.Send(message);
        }

        private string ListenToResponseSubscription(SasDto sasDto, string clientId)
        {
            var serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", sasDto.ServiceBusNamespace, string.Empty)
             .ToString()
             .Trim('/');

            var factory = MessagingFactory.Create(serviceUri, new MessagingFactorySettings
            {
                TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(sasDto.ResultQueueUrl)
            });

            var queue = factory.CreateQueueClient(sasDto.ResultQueue);

            string jobid;
            while (true)
            {
                var message = queue.Receive(TimeSpan.FromSeconds(5));
                if (message != null)
                {
                    jobid = message.GetBody<string>();
                    message.Complete();
                    break;
                }
            }

            return jobid;
        }

        private void DownloadResult(SasDto sasDto, string jobId)
        {
            var container = new CloudBlobContainer(new Uri(sasDto.BlobContainerUrl));

            CloudBlockBlob jobBlob = container.GetBlockBlobReference(jobId + "_job");
            CloudBlockBlob resultBlob = container.GetBlockBlobReference(jobId + "_result");
            if (resultBlob.Exists())
            {
                var result = resultBlob.DownloadText();
                Console.WriteLine(result);
                resultBlob.Delete();
                jobBlob.Delete();
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SasDto.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the SasDto type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Web.Common 
{
    using System;

    public class SasDto
    {
        public DateTimeOffset ExpirationTime { get; set; }

        public string BlobContainerUrl { get; set; }

        public string ServiceBusNamespace { get; set; }

        public string JobQueue { get; set; }

        public string JobQueueSasUrl { get; set; }

        public string ResultQueue { get; set; }

        public string ResultQueueUrl { get; set; }
    }
}

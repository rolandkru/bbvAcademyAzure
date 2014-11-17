// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Client type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Web.DataAccess
{
    using System.ComponentModel.DataAnnotations;

    public class Client
    {
        [Key]
        public string ClientId { get; set; }

        public string QueueName { get; set; }
    }
}
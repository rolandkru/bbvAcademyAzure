// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ConfigurationManager.cs" company="Selectron Systems AG">
// //   Copyright (c) Selectron Selectron Systems AG. All rights reserved.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

namespace Client.Labs.Lab3
{
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    internal static class Lab3Helper
    {
        internal static QueueClient GetQueueClient(string serviceBusNamespace, string queueSas, string queueName)
        {
            var serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceBusNamespace, string.Empty).ToString().Trim('/');

            var factory = MessagingFactory.Create(serviceUri, TokenProvider.CreateSharedAccessSignatureTokenProvider(queueSas));

            var queue = factory.CreateQueueClient(queueName);
            return queue;
        }
    }
}
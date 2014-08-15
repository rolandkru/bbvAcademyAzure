// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Configuration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using Web.DataAccess;

    internal sealed class Configuration : DbMigrationsConfiguration<Web.DataAccess.Flatterist>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Flatterist context)
        {
            context.Clients.AddOrUpdate(
                c => c.ClientId,
                new Client
                {
                    ClientId = "3d60d48ca1df4e43a6e86eeb752486db",
                    QueueName = "jobqueue"
                });
        }
    }
}

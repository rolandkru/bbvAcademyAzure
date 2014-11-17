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
    using System.Data.Entity.Migrations;

    using Web.DataAccess;

    internal sealed class Configuration : DbMigrationsConfiguration<Flatterist>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Flatterist context)
        {
        }
    }
}

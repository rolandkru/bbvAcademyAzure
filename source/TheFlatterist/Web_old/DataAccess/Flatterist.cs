// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Flatterist.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Flatterist type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Web.DataAccess
{
    using System;
    using System.Data.Entity;

    using Microsoft.AspNet.Identity.EntityFramework;

    using Web.Models;

    public class Flatterist : IdentityDbContext<ApplicationUser>
    {
        // Your context has been configured to use a 'Flatterist' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Web.DataAccess.Flatterist' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Flatterist' 
        // connection string in the application configuration file.
        public Flatterist() : base("name=Flatterist")
        {
        }

        public static Flatterist Create()
        {
            return new Flatterist();
        }

        public virtual DbSet<Client> Clients { get; set; }
    }
}
// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ConfigurationManager.cs" company="Selectron Systems AG">
// //   Copyright (c) Selectron Selectron Systems AG. All rights reserved.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace Web.App_Start
{
    using System.Web.Http;

    public class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
        }
    }
}
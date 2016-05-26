using System;
using System.IO;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Web
{
    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

           
            var fileserverOptions = new FileServerOptions()
            {
                EnableDefaultFiles = true
            };
            fileserverOptions.StaticFileOptions.FileSystem = new PhysicalFileSystem(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot"));
            fileserverOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            fileserverOptions.StaticFileOptions.DefaultContentType = "text/plain";

            appBuilder.UseFileServer(fileserverOptions);

        }
    }
}

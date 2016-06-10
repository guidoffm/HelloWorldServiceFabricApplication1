using System.Web.Http;
using Microsoft.ServiceFabric.Data;
using Owin;

namespace QueueService
{
    public class Startup : IOwinAppBuilder
    {
        private readonly IReliableStateManager _stateManager;

        public Startup(IReliableStateManager stateManager)
        {
            this._stateManager = stateManager;
        }

        /// <summary>
        /// Configures the app builder using Web API.
        /// </summary>
        /// <param name="appBuilder"></param>
        public void Configuration(IAppBuilder appBuilder)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            // Configure Web API for self-host. 
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //appBuilder.UseWebApi(config);

            //FormatterConfig.ConfigureFormatters(config.Formatters);
            //UnityConfig.RegisterComponents(config, this.stateManager);

            //config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);
        }
    }
}
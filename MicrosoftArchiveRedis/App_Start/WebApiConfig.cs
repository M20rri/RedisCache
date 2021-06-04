using System.Web.Http;
using System.Web.Http.Cors;

namespace MicrosoftArchiveRedis
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                        "DefaultApi",
                        "api/{action}/{id}",
                        new { id = RouteParameter.Optional }
                    );

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            //Install-Package Microsoft.AspNet.WebApi.Cors
            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
        }
    }
}

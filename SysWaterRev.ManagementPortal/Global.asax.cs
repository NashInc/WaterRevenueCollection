using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SysWaterRev.ManagementPortal
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());

            //DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RemoteRangeAndPriceValidation), typeof(RemoteAttributeAdapter));

            AutoMapperWebConfiguration.Configure();
        }
    }
}
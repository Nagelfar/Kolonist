using Castle.MicroKernel.Registration;
using Kolonist.Web.Infrastructure.Mvc;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Kolonist.Web
{
    public class FilterConfig : IWindsorInstaller
    {
        public static void RegisterGlobalMvcFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterGlobalWebApiFilters(System.Web.Http.Filters.HttpFilterCollection filters)
        {
            filters.Add(new ValidationActionFilter());
        }

        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            RegisterGlobalMvcFilters(GlobalFilters.Filters);
            RegisterGlobalWebApiFilters(GlobalConfiguration.Configuration.Filters);
        }
    }
}
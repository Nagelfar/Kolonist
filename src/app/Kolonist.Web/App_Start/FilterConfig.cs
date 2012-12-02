using Castle.MicroKernel.Registration;
using System.Web;
using System.Web.Mvc;

namespace Kolonist.Web
{
    public class FilterConfig :IWindsorInstaller
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
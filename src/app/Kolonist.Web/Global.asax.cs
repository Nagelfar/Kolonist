using Castle.Windsor;
using Kolonist.Web.App_Start;
using Kolonist.Web.Infrastructure.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Kolonist.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        static IWindsorContainer _container;

        public static IWindsorContainer Container
        {
            get { return _container; }
        }

        protected void Application_Start()
        {
            _container = Bootstrapper.BootUp();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(_container.Kernel));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new ReleasingHttpControllerFactory(_container));
        }

        protected void Application_End()
        {
            if (_container != null)
                _container.Dispose();
        }
    }
}
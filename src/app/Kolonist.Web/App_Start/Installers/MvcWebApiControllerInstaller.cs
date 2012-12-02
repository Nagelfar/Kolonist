using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Kolonist.Web.App_Start.Installers
{
    public class MvcWebApiControllerInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient());
            container.Register(Classes.FromThisAssembly().BasedOn<IHttpController>().LifestyleTransient());
        }
    }
}
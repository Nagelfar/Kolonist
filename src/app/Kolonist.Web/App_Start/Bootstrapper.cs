using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.App_Start
{
    public class Bootstrapper
    {
        public static IWindsorContainer BootUp()
        {
            var container = new WindsorContainer();

            container.Install(Castle.Windsor.Installer.FromAssembly.This());

            return container;
        }
    }
}
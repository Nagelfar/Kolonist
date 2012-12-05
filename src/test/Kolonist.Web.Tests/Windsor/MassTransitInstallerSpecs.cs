using Kolonist.Web.App_Start;
using Machine.Specifications;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Web.Tests.Windsor
{
    [Subject(typeof(MassTransistInstaller))]
    public class MassTransitInstallerSpecs : WindsorSpecs
    {
        Establish context = () => windsorContainer.Install(new MassTransistInstaller());
    }

    public class when_installing_mass_transit : MassTransitInstallerSpecs
    {
        It should_be_registered_as_a_single_instance = () => GetHandlersFor<IServiceBus>().Count().ShouldEqual(1);

        It should_be_registered_as_singleton = () => GetHandlersFor<IServiceBus>().Single().ComponentModel.LifestyleType.ShouldEqual(Castle.Core.LifestyleType.Singleton);
    }
}

using Kolonist.Contracts.Commands;
using Kolonist.Web.App_Start.Installers;
using Machine.Specifications;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Web.Tests.Windsor
{
    [Subject(typeof(MassTransitConsumerInstaller))]
    public abstract class MassTransitConsumeInstallerSpecs : WindsorSpecs
    {
        Establish context = () => windsorContainer.Install(new MassTransitConsumerInstaller());
    }

    public class when_installing_masstransit_consumers : MassTransitConsumeInstallerSpecs
    {
        private static Castle.MicroKernel.IHandler[] handlers;

        Because of_getting_handlers = () => handlers = GetHandlersFor<IConsumer>();
        It should_register_classes = () => handlers.ShouldNotBeEmpty();
        It should_register_classes_as_transient = () => handlers.ShouldEachConformTo(x => x.ComponentModel.LifestyleType == Castle.Core.LifestyleType.Transient);

        It should_be_resolvable = () => windsorContainer.Resolve<Consumes<ConstructANewBuilding>.All>().ShouldNotBeNull();
    }
}

using Castle.MicroKernel.Registration;
using Kolonist.Domain;
using Kolonist.Projections;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.App_Start.Installers
{
    public class MassTransitConsumerInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                AllTypes.FromAssemblyContaining<Buildings>()
                    .BasedOn<IConsumer>()
                    .LifestyleTransient(),

                AllTypes.FromAssemblyContaining<ViewBase>()
                    .BasedOn<IConsumer>()
                    .LifestyleTransient()
                );
        }
    }
}
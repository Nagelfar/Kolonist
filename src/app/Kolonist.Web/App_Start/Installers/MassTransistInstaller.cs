using Castle.MicroKernel.Registration;
using Kolonist.Domain;
using MassTransit;
using MassTransit.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.App_Start
{
    public class MassTransistInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            container.Register(
                Component.For<IServiceBus>()
                    .UsingFactoryMethod(() =>
                        ServiceBusFactory.New(sbc =>
                        {
                            sbc.UseMsmq();
                            sbc.VerifyMsmqConfiguration();

                            sbc.UseMulticastSubscriptionClient();

                            sbc.ReceiveFrom("msmq://localhost/web");
                            sbc.SetCreateMissingQueues(true);
                            sbc.UseJsonSerializer();

                            sbc.Subscribe(subs => subs.LoadFrom(container));
                        })
                    )
                    .LifestyleSingleton()
                    );
        }

    }
}
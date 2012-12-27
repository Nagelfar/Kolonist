using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit.Testing;
using Kolonist.Contracts.Commands;
using Kolonist.Contracts.Identities;
using Kolonist.Contracts.Events;
using MassTransit;
using Kolonist.Contracts;

namespace Kolonist.Domain.Tests.Scenarios.Buildings
{
    [Subject(typeof(Kolonist.Domain.Buildings))]
    public class WhenConstructingABuilding
    {
        static BuildingTypeId TypeId = new BuildingTypeId(Guid.NewGuid());
        static BuildingId BuildingId = new BuildingId(Guid.NewGuid());

        private static ConsumerTest<BusTestScenario, Domain.Buildings> Scenario;

        Establish context = () => Scenario = MassTransit.Testing.TestFactory.ForConsumer<Kolonist.Domain.Buildings>()
            .InSingleBusScenario()
            .New(x =>
            {

                x.ConstructUsing(() => new Kolonist.Domain.Buildings());

                x.Send(new ConstructANewBuilding()
                {
                    BuildingType = TypeId,
                    Id = BuildingId,
                    Position = new MapPosition(10, 12)
                });
                x.Validate();
            });

        Because of = () =>
        {
            Scenario.Execute();
            //Scenario.Dispose();
        };

        //It should_publish_an_event = () => Scenario.Published.Any().ShouldBeTrue();
        //It should_be_of_type_BuildingCreated = () => Scenario.Published.ShouldContain(x => x is BuildingConstructed);
        //It should_have_the_right_properties = () => Scenario.Published.OfType<BuildingConstructed>().ShouldEachConformTo(x => x.BuildingType == TypeId && x.Id == BuildingId);

        //Cleanup cleanup = () => Scenario.Dispose();
    }

}

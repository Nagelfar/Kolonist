using Kolonist.Contracts.Commands;
using MassTransit;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kolonist.Contracts.Events;
using Kolonist.Contracts;
using MassTransit.Saga;
using Kolonist.Contracts.Events.Buildings;

namespace Kolonist.Domain
{
    public class Buildings : //SagaStateMachine<Buildings>, ISaga
        Consumes<ConstructANewBuilding>.All
    {

        //static Buildings()
        //{
        //    Define(() =>
        //    {
        //        Initially(When(
        //    });
        //}

        public IServiceBus Bus { get; set; }

        public void Consume(ConstructANewBuilding message)
        {

            Bus.Publish(new BuildingConstructedImpl
            {
                Id = message.Id,
                BuildingType = message.BuildingType,
                Position = message.Position
            });
        
        }

        private class BuildingConstructedImpl : BuildingConstructed
        {
            public Contracts.Identities.BuildingTypeId BuildingType { get; set; }

            public Contracts.Identities.BuildingId Id { get; set; }
            public MapPosition Position { get; set; }
        }

    }
}
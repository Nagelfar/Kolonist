using Kolonist.Contracts.Commands;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kolonist.Contracts.Events;

namespace Kolonist.Domain
{
    public class Buildings : Consumes<ConstructANewBuilding>.All
    {

        public IServiceBus Bus { get; set; }

        public void Consume(ConstructANewBuilding message)
        {
            //var proxy = Magnum.Reflection.InterfaceImplementationExtensions.InitializeProxy<BuildingConstructed>(new { });
            //Bus.Publish<BuildingConstructed>(proxy
            //, ctx =>
            //{
            //    //ctx.Message.Position = message.Position;
            //    ctx.Message.BuildingType = message.BuildingTypeId;
            //    ctx.Message.Id = message.BuildingId;
            //});
        }
    }
}
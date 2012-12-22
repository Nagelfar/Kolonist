using Kolonist.Contracts.Events.Buildings;
using Kolonist.Contracts.Identities;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Projections.Buildings
{
    public class BuildingTypes : DtoBase<BuildingTypeId>
    {
        public BuildingTypes(BuildingTypeId id)
        {
            Id = id;
        }

        public string Name { get; set; }
    }

    public class BuildingTypeHandlers : ViewBaseWithId<BuildingTypes, BuildingTypeId>,
        Consumes<BuildingTypeCreated>.All
    {

        public void Consume(BuildingTypeCreated message)
        {
            AddOrThrow(new BuildingTypes(message.Id)
            {
                Name = message.Name
            });
        }
    }
}

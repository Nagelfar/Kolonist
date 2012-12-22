using Kolonist.Contracts.Commands;
using Kolonist.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Models
{
    public class ConstructBuilding : ICommandConverter<ConstructANewBuilding>
    {
        public class PositionOnMap
        {
            [Required]
            public double? X { get; set; }
            [Required]
            public double? Y { get; set; }
        }

        [Required]
        public Guid? BuildingTypeId { get; set; }

        [Required]
        public PositionOnMap Position { get; set; }


        public ConstructANewBuilding ToCommand()
        {
            return new ConstructANewBuilding
            {
                BuildingType = new Contracts.Identities.BuildingTypeId(BuildingTypeId.Value),
                Id = new Contracts.Identities.BuildingId(Guid.NewGuid()),
                Position = new Contracts.MapPosition(Position.X.Value, Position.Y.Value)
            };
        }
    }
}
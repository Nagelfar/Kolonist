using Kolonist.Contracts.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Contracts.Commands
{
    public class ConstructANewBuilding : ICommand<BuildingId>
    {
        public BuildingTypeId BuildingType { get; set; }
        public MapPosition Position { get; set; }
        public BuildingId Id { get; set; }
    }
}

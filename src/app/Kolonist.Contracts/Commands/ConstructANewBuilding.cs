using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Contracts.Commands
{
    public class ConstructANewBuilding : ICommand
    {
        public Guid BuildingTypeId { get; set; }
        public MapPosition Position { get; set; }
    }
}

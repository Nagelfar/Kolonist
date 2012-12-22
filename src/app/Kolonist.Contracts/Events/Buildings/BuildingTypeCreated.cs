using Kolonist.Contracts.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Contracts.Events.Buildings
{
    public class BuildingTypeCreated : IEvent<BuildingTypeId>
    {
        public string Name { get; set; }

        public BuildingTypeId Id
        {
            get;
            set;
        }
    }
}

using Kolonist.Contracts.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Contracts.Events.Buildings
{
    public interface BuildingTypeCreated : IEvent<BuildingTypeId>
    {
        string Name { get; set; }
    }
}

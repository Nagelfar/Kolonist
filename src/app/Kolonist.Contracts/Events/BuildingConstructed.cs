using Kolonist.Contracts.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kolonist.Contracts.Events
{
    public interface BuildingConstructed : IEvent<BuildingId>
    {
        MapPosition Position { get; }

        BuildingTypeId BuildingType { get; }

    }
}

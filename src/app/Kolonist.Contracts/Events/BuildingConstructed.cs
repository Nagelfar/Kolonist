using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kolonist.Contracts.Events
{
    public interface BuildingConstructed : IEvent
    {
        //MapPosition Position { get; set; }

        Guid BuildingType { get; set; }

        Guid BuildingId { get; set; }
    }
}

using Kolonist.Contracts.Commands;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Domain
{
    public class Buildings : Consumes<ConstructANewBuilding>.All
    {
        public void Consume(ConstructANewBuilding message)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Contracts.Identities
{
    public class BuildingId : AbstractIdentity<Guid>
    {
        public BuildingId(Guid id)
        {
            Contract.Requires(id != Guid.Empty);

            Id = id;
        }

        public override Guid Id { get; protected set; }
    }
}

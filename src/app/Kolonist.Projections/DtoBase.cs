using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Projections
{
    public abstract class DtoBase
    {
    }
    public abstract class DtoBase<TId> : DtoBase
    {
        public TId Id { get; protected set; }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Projections
{
    public abstract class ViewBase
    {
    }
    public abstract class ViewBaseWithId<TDto, TId> : ViewBase<TDto, TId> where TDto : DtoBase<TId>
    {
    }

    public abstract class ViewBase<TDto, TId> : ViewBase where TDto : DtoBase
    {


        protected virtual void AddOrThrow(TDto dto)
        {
            Contract.Requires(dto != null);
        }

        protected virtual void Update(TId id, Action<TDto> change)
        {
            Contract.Requires(change != null);
            Contract.Requires(id != null);

            var item = Get(id);

            change(item);

            Update(item);
        }

        protected virtual void Update(TDto dto)
        {

        }

        protected virtual TDto Get(TId id)
        {
            Contract.Requires(id != null);

            return default(TDto);
        }

        protected virtual void Delete(TDto dto)
        {
        }

        protected virtual void Delete(TId id)
        {
        }

    }
}

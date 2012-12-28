using Kolonist.Contracts.Identities;
using Raven.Client;
using Raven.Client.Converters;
using Raven.Client.Document;
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
        public IDocumentSession Session { get; set; }
    }
    public abstract class ViewBaseWithId<TDto, TId> : ViewBase<TDto, TId> where TDto : DtoBase<TId>
    {
    }

    public abstract class ViewBase<TDto, TId> : ViewBase where TDto : DtoBase
    {


        protected virtual void AddOrThrow(TDto dto)
        {
            Contract.Requires(dto != null);

            Session.Store(dto);
            Session.SaveChanges();
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
            Contract.Requires(dto != null);

            Session.Store(dto);
            Session.SaveChanges();
        }

        protected virtual TDto Get(IIdentity id)
        {
            return Get(IdentityConvert.ToTransportable(id));
        }
        protected virtual TDto Get(string id)
        {
            return Session.Load<TDto>(id);
        }
        protected virtual TDto Get(TId id)
        {
            Contract.Requires(id != null);

            var identity = id as IIdentity;
            if (identity != null)
                return Get(identity);
            else
                return Get(id.ToString());
        }

        protected virtual void Delete(TDto dto)
        {
            Contract.Requires(dto != null);

            Session.Delete(dto);
        }

        protected virtual void Delete(TId id)
        {
            Session.Delete(Get(id));
        }

    }
}

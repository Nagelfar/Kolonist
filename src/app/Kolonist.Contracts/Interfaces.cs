using Kolonist.Contracts.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Contracts
{
    public interface IMessage { }

    public interface ICommand : IMessage { }
    public interface IEvent : IMessage { }

    public interface ICommand<out TIdentity> : ICommand
        where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }
    public interface IEvent<out TIdentity> : IEvent
        where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }
}

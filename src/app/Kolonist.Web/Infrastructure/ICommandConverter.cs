using Kolonist.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kolonist.Web.Infrastructure
{
    
    public interface ICommandConverter<TCommand> where TCommand : ICommand
    {
        TCommand ToCommand();
    }
}
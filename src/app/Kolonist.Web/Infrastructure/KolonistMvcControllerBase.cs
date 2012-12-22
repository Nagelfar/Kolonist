using Kolonist.Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kolonist.Web.Infrastructure
{
    public abstract class KolonistMvcControllerBase : Controller
    {
        public IServiceBus Bus { get; set; }

        protected internal virtual bool IsValid(object potentialCommand)
        {
            // Model-State validation check first to prevent duplicate validation of the command!
            return potentialCommand != null && ModelState.IsValid && TryValidateModel(potentialCommand);
        }

        protected internal virtual void ExecuteCommand(ICommand command)
        {
            Bus.Endpoint.Send(command);
        }

        protected internal TCommand ExecuteCommand<TCommand>(ICommandConverter<TCommand> potentialCommand, Action<TCommand> callback = null)
            where TCommand : ICommand
        {            
            if (IsValid(potentialCommand))
            {
                var command = potentialCommand.ToCommand();
                if (callback != null)
                    callback(command);

                ExecuteCommand(command);
                return command;
            }

            return default(TCommand);
        }
    }
}
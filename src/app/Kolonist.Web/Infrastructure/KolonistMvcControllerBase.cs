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

        protected void ExecuteCommand(ICommand command)
        {
            Bus.Publish(command);
        }

        protected TCommand ExecuteCommand<TCommand>(ICommandConverter<TCommand> potentialCommand)
            where TCommand : ICommand
        {
            // Model-State validation check first to prevent duplicate validation of the command!
            if (ModelState.IsValid && TryValidateModel(potentialCommand))
            {
                var command = potentialCommand.ToCommand();
                ExecuteCommand(command);
                return command;
            }

            return default(TCommand);
        }
    }
}
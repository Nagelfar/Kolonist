using Kolonist.Contracts;
using Kolonist.Web.Infrastructure;
using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Web.Tests.Infrastructure
{
    [Subject(typeof(KolonistMvcControllerBase))]
    public abstract class Can_call_ExecuteCommands
    {
        internal static MyKolonistMvcControllerBase controller;

        Establish context = () => controller = new MyKolonistMvcControllerBase();

        internal class MyKolonistMvcControllerBase : KolonistMvcControllerBase
        {
            protected internal override bool IsValid(object potentialCommand)
            {
                return Valid;
            }
            protected internal override void ExecuteCommand(ICommand command)
            {
                ExecuteCalled = true;
                PassedCommand = command;
            }
            public bool Valid { get; set; }

            public bool ExecuteCalled { get; private set; }

            public ICommand PassedCommand { get; set; }
        }
        internal class MyModel : ICommandConverter<MyCommand>
        {
            public MyCommand Command = new MyCommand();

            public MyCommand ToCommand()
            {
                WasCalled = true;
                return Command;
            }

            public bool WasCalled { get; set; }
        }
        internal class MyCommand : ICommand
        {
        }
    }

    public class Can_call_with_null_as_argument : Can_call_ExecuteCommands
    {
        Because of = () => controller.ExecuteCommand(default(ICommandConverter<ICommand>));

        It should_not_call_execute = () => controller.ExecuteCalled.ShouldBeFalse();

    }
    public class With_invalid_properties : Can_call_ExecuteCommands
    {
        static MyModel model;

        Establish modelContext = () => model = new MyModel();

        Because of = () => controller.ExecuteCommand(model);

        It should_not_call_ToCommand = () => model.WasCalled.ShouldBeFalse();
        It should_not_call_execute = () => controller.ExecuteCalled.ShouldBeFalse();
    }

    [Subject("Valid properties")]
    public class With_valid_properties : Can_call_ExecuteCommands
    {
        static MyModel model;

        Establish modelContext = () => model = new MyModel { };
        Establish validController = () => controller.Valid = true;

        Because of = () => controller.ExecuteCommand(model);

        It should_call_ToCommand = () => model.WasCalled.ShouldBeTrue();
        It should_call_Execute = () => controller.ExecuteCalled.ShouldBeTrue();
        It should_pass_the_command_to_the_ExecuteCommand_method = () => controller.PassedCommand.ShouldBeOfType<MyCommand>();
    }

    public class With_callback : Can_call_ExecuteCommands
    {
        static MyModel model;
        private static bool WasCalled;

        Establish modelContext = () => model = new MyModel { };
        Establish validController = () => controller.Valid = true;

        static void Callback(MyCommand command)
        {
            WasCalled = true;
        }

        Because of = () => controller.ExecuteCommand(model, Callback);


        It should_invoke_the_callback = () => WasCalled.ShouldBeTrue();
    }
}

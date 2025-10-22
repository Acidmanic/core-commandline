using Acidmanic.Utilities.Reflection;
using CoreCommandLine.Attributes;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    [CommandName("--help", "-h")]
    public class Help<T> : ICommand
    {
        private ILogger _logger = new ConsoleOutput();
        private Action<string> _output = Console.WriteLine;

        private NameBundle NameBundle { get; } = new NameBundle
        {
            Name = "--help",
            ShortName = "-h"
        };

        public int Execute(Context context, string[] args)
        {
            var ownerType = typeof(T);

            var childrenTypes = context.Factory.GetChildrenTypes(ownerType, false);

            var message = CommandUtilities.GetHelpMessage(context.Factory, childrenTypes);

            _output(message);

            context.ApplicationExit = true;

            return 0;
        }


        // private bool CanHelp(Type ownerType, string[] args)
        // {
        //     if (!TypeCheck.Implements<ICommand>(ownerType))
        //     {
        //         return CommandUtilities.IndexOf(NameBundle, args) > -1;
        //     }
        //
        //     var ownerName = ownerType.GetCommandName();
        //
        //     if (ownerName)
        //     {
        //         var ownerIndex = CommandUtilities.IndexOf(ownerName.Value, args);
        //
        //         if (ownerIndex > -1)
        //         {
        //             if (args.Length > ownerIndex + 1)
        //             {
        //                 var expectedMyName = args[ownerIndex + 1].ToLower();
        //
        //                 if (expectedMyName == "--help" || expectedMyName == "-h")
        //                 {
        //                     return true;
        //                 }
        //             }
        //         }
        //     }
        //
        //     return false;
        // }

        public string Description => "Displays this message";

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void SetOutput(Action<string> output)
        {
            _output = output;
        }
    }
}
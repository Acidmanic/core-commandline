using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Results;
using CoreCommandLine.Attributes;
using CoreCommandLine.Di;
using CoreCommandLine.Dtos;
using CoreCommandLine.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace CoreCommandLine
{
    public class CommandLineApplication
    {
        public ILogger Logger { get; internal set; } = new ConsoleLogger();

        public string ApplicationTitle { get; set; } = "Command line Application";

        public string ApplicationDescription { get; set; } = "";

        private readonly List<Type> _applicationSubCommands;

        internal Action<ExecutionActionAssets> BeforeExecute { get; set; } = _ => { };

        internal Action<ExecutionActionAssets> AfterExecute { get; set; } = _ => { };

        internal Action<Context> InitializeContext { get; set; } = _ => { };

        private readonly CommandFactory factory;

        public CommandLineApplication(List<Assembly> assemblies, IResolver resolver)
        {
            _applicationSubCommands = extractRootCommands(assemblies);

            factory = new CommandFactory(resolver, _applicationSubCommands);
        }


        private List<Type> extractRootCommands(List<Assembly> assemblies)
        {
            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }

            var baseType = typeof(ICommand);

            var rootCommands = types
                .Where(t => t.IsAssignableTo(baseType))
                .Where(t => t.GetCustomAttributes().Any(a => a is RootCommandAttribute))
                .Distinct()
                .ToList();

            return rootCommands;
        }

        public void Execute(string[] args)
        {
            var context = new Context(factory);

            InitializeContext(context);

            var type = GetType();

            WrapExecute(type, context, args, false);
        }

        public void ExecuteInteractive()
        {
            Logger.LogInformation(ApplicationTitle);

            Logger.LogInformation(Line(ApplicationTitle, 15));

            Logger.LogInformation(ApplicationDescription);

            Logger.LogInformation(Line(ApplicationDescription, 0));

            var stay = true;

            while (stay)
            {
                var context = new Context(factory);

                InitializeContext(context);

                var type = this.GetType();

                string line = Console.ReadLine();

                var args = line.SplitToArgs();

                WrapExecute(type, context, args, true);

                stay = !context.InteractiveExit;
            }
        }

        private string Line(string text, int nullLength)
        {
            int length = nullLength;

            if (!string.IsNullOrEmpty(text))
            {
                length = text.Length;
            }

            var line = "";

            for (int i = 0; i < length; i++)
            {
                line += "-";
            }

            return line;
        }

        private Result<ICommand> FindRootCommand(string[] args)
        {
            if (args is { Length: > 0 })
            {
                var command = factory.Make(args[0], this.GetType(), false);

                if (command.GetType() != typeof(NullCommand))
                {
                    return new Result<ICommand>().Succeed(command);
                }
            }

            return new Result<ICommand>().FailAndDefaultValue();
        }

        private void WrapExecute(Type type, Context context, string[] args, bool useExitCommand)
        {
            var foundCommand = FindRootCommand(args);

            if (foundCommand)
            {
                BeforeExecute(new ExecutionActionAssets(context, args, foundCommand.Value, this));
            }

            Execute(type, context, args, useExitCommand);

            if (foundCommand)
            {
                AfterExecute(new ExecutionActionAssets(context, args, foundCommand.Value, this));
            }
        }

        private void Execute(Type parentType, Context context, string[] args, bool useExitCommand)
        {
            if (context.ApplicationExit)
            {
                return;
            }

            var childrenTypes = factory.GetChildrenTypes(parentType, useExitCommand);

            foreach (var childType in childrenTypes)
            {
                if (context.ApplicationExit)
                {
                    return;
                }

                Execute(childType, context, args, useExitCommand);
            }

            var instance = factory.Instantiate(parentType);

            if (instance && !context.ApplicationExit)
            {
                instance.Value.SetLogger(Logger);

                instance.Value.Execute(context, args);
            }
        }
    }
}
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

        internal Action<ExecutionActionAssets> BeforeExecute { get; set; } = _ => { };

        internal Action<ExecutionActionAssets> AfterExecute { get; set; } = _ => { };

        internal Action<Context> InitializeContext { get; set; } = _ => { };

        private readonly CommandFactory factory;

        public CommandLineApplication(List<Assembly> assemblies, IResolver resolver)
        {
            var applicationSubCommands = extractRootCommands(assemblies);

            factory = new CommandFactory(resolver, applicationSubCommands);
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

        public async Task Execute(string[] args, CancellationToken cancellationToken)
        {
            var context = new Context(factory, false);

            InitializeContext(context);

            var type = GetType();

            await WrapExecute(type, context, args, false, cancellationToken);
        }

        public async Task ExecuteInteractive(CancellationToken cancellationToken)
        {
            Logger.LogInformation(ApplicationTitle);

            Logger.LogInformation(Line(ApplicationTitle, 15));

            Logger.LogInformation(ApplicationDescription);

            Logger.LogInformation(Line(ApplicationDescription, 0));

            var stay = true;

            while (stay)
            {
                var context = new Context(factory, true);

                InitializeContext(context);

                var type = GetType();

                string line = Console.ReadLine() ?? string.Empty;

                var args = line.SplitToArgs();

                await WrapExecute(type, context, args, true, cancellationToken);

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

        private Result<ICommand> FindRootCommand(string[] args, bool includeExitCommand)
        {
            if (args is { Length: > 0 })
            {
                var command = factory.Make(args[0], this.GetType(), includeExitCommand);

                if (command.GetType() != typeof(NullCommand))
                {
                    return new Result<ICommand>().Succeed(command);
                }
            }

            return new Result<ICommand>().FailAndDefaultValue();
        }

        private async Task WrapExecute(Type type, Context context, string[] args, bool useExitCommand,
            CancellationToken cancellationToken)
        {
            var foundCommand = FindRootCommand(args, useExitCommand);

            if (foundCommand)
            {
                BeforeExecute(new ExecutionActionAssets(context, args, foundCommand.Value, this));
            }

            await Execute(type, context, args, useExitCommand, cancellationToken);

            if (foundCommand)
            {
                AfterExecute(new ExecutionActionAssets(context, args, foundCommand.Value, this));
            }
        }

        private async Task<int> Execute(Type parentType, Context context, string[] args, bool useExitCommand,
            CancellationToken cancellationToken)
        {
            if (context.ApplicationExit)
            {
                return 0;
            }

            var childrenTypes = factory.GetChildrenTypes(parentType, useExitCommand);

            var childrenTypeNameBundles = childrenTypes.Select(ct => new
            {
                NameBUndle = ct.GetCommandName(),
                Type = ct
            }).ToList();

            int argIndex = 0;

            while (argIndex < args.Length)
            {
                var currentCommand = args[argIndex];

                var childType = childrenTypeNameBundles.FirstOrDefault(
                    tb => CommandUtilities.AreNamesEqual(tb.NameBUndle, currentCommand));

                if (childType is { } ctb)
                {
                    var shiftArgs = args.Skip(argIndex + 1).ToArray();

                    argIndex += await Execute(ctb.Type, context, shiftArgs, useExitCommand, cancellationToken);
                }

                argIndex++;

                if (context.ApplicationExit)
                {
                    return 0;
                }
            }

            var instance = factory.Instantiate(parentType);

            if (instance && !context.ApplicationExit)
            {
                instance.Value.SetLogger(Logger);

                return await instance.Value.Execute(context, args, cancellationToken);
            }

            return 0;
        }
    }
}
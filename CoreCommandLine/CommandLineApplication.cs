using System.Reflection;
using System.Runtime.CompilerServices;
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
        private sealed record TypedNameBundle(NameBundle NameBundle, Type Type);

        private sealed record NotFound(string Name, Type CallerType);

        public ILogger Logger { get; internal set; } = new ConsoleLogger();

        public string ApplicationTitle { get; set; } = "Command line Application";

        public string ApplicationDescription { get; set; } = "";

        internal Action<ExecutionActionAssets> BeforeExecute { get; set; } = _ => { };

        internal Action<ExecutionActionAssets> AfterExecute { get; set; } = _ => { };

        internal Action<Context> InitializeContext { get; set; } = _ => { };

        private readonly CommandUtilities _commandUtilities;

        public CommandLineApplication(List<Assembly> assemblies, IResolver resolver)
        {
            var applicationSubCommands = ExtractRootCommands(assemblies);

            _commandUtilities = new CommandUtilities(resolver, applicationSubCommands);
        }


        private List<Type> ExtractRootCommands(List<Assembly> assemblies)
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
            var context = new Context(_commandUtilities, false);

            InitializeContext(context);

            var type = GetType();

            await Execute(type, context, args, false, true, cancellationToken);
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
                var context = new Context(_commandUtilities, true);

                InitializeContext(context);

                var type = GetType();

                string line = Console.ReadLine() ?? string.Empty;

                var args = line.SplitToArgs();

                await Execute(type, context, args, true, true, cancellationToken);

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

        private async Task Execute(Type parentType, Context context, string[] args, bool useExitCommand,
            bool wrapExecution,
            CancellationToken cancellationToken)
        {
            var notFoundCommands = new List<NotFound>();

            await Execute(parentType, context, args, useExitCommand, wrapExecution, notFoundCommands,
                cancellationToken);

            foreach (var notFound in notFoundCommands)
            {
                var helpBundle = typeof(Help<object>).GetCommandName();

                var callerPhrase = notFound.CallerType.IsAssignableTo(typeof(ICommand))
                    ? " as a sub-command of "+notFound.CallerType.GetCommandName().Value.Name + " "
                    : string.Empty;
                
                Logger.LogError("No command named '{CommandName}' has been found{CallerPhrase}, use {Name} Or {ShortName} to display help message.",
                    notFound.Name,callerPhrase, helpBundle.Value.Name, helpBundle.Value.ShortName);
            }
        }


        private async Task<int> Execute(
            Type parentType, Context context, string[] args, bool useExitCommand,
            bool wrapExecution,
            List<NotFound> notFoundCommands,
            CancellationToken cancellationToken)
        {
            if (context.ApplicationExit)
            {
                return 0;
            }

            var childrenTypes = _commandUtilities.GetChildrenTypes(parentType, useExitCommand);

            var childrenTypeNameBundles = childrenTypes.Select(ct => new
                TypedNameBundle(ct.GetCommandName(), ct)).ToList();

            var notFounds = new Dictionary<int, string>();

            int argIndex = 0;

            while (argIndex < args.Length)
            {
                var currentCommand = args[argIndex];

                var childType = childrenTypeNameBundles.FirstOrDefault(
                    tb => ArgumentProcess.AreNamesEqual(tb.NameBundle, currentCommand));

                if (childType is { } ct)
                {
                    var shiftArgs = args.Skip(argIndex + 1).ToArray();

                    argIndex += await Execute(childType.Type, context, shiftArgs, useExitCommand, false, notFoundCommands, cancellationToken);
                }
                else
                {
                    notFounds.Add(argIndex, currentCommand);
                }

                argIndex++;

                if (context.ApplicationExit)
                {
                    return 0;
                }
            }


            var consumedArguments = 0;

            if (!context.ApplicationExit && parentType != typeof(CommandLineApplication))
            {
                var parentNameBundle = parentType.GetCommandName();

                var parentName = parentNameBundle.Success ? parentNameBundle.Value.Name : parentType.Name;

                var instance = _commandUtilities.Instantiate(parentType) ?? new UnableToResolveCommand(parentName);

                consumedArguments = await ExecuteWrapped(context, args, instance, wrapExecution, cancellationToken);
            }

            foreach (var kv in notFounds)
            {
                if (kv.Key >= consumedArguments &&
                    notFoundCommands.All(nf => nf.Name != kv.Value && nf.CallerType != parentType))
                    notFoundCommands.Add(new NotFound(kv.Value, parentType));
            }

            return consumedArguments;
        }

        private async Task<int> ExecuteWrapped(Context context, string[] args, ICommand command, bool wrap,
            CancellationToken cancellationToken)
        {
            if (wrap) BeforeExecute(new ExecutionActionAssets(context, args, command, this));

            var numberOfArguments = await command.Execute(context, args, cancellationToken);

            if (wrap) AfterExecute(new ExecutionActionAssets(context, args, command, this));

            return numberOfArguments;
        }
    }
}
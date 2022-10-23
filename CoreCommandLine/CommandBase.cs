using System;
using Acidmanic.Utilities.Results;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    public abstract class CommandBase : ICommand
    {
        public string Name { get; }

        public string ShortName { get; }


        public NameBundle NameBundle { get; }

        public CommandBase()
        {
            var nameBundle = this.GetType().GetCommandName();

            if (nameBundle)
            {
                this.Name = nameBundle.Value.Name;

                this.ShortName = nameBundle.Value.ShortName;

                this.NameBundle = nameBundle.Value;
            }
        }

        public abstract bool Execute(Context context, string[] args);

        public virtual string Description => GetType().Name;

        protected ILogger Logger { get; private set; } = new ConsoleOutput();

        protected Action<String> Output { get; private set; } = Console.WriteLine;

        
        public void SetLogger(ILogger logger)
        {
            Logger = logger;
        }

        public void SetOutput(Action<string> output)
        {
            Output = output;
        }

        protected Result<CommandArguments> GetCommandArguments(string[] args)
        {
            if (args.Length == 0)
            {
                return new Result<CommandArguments>().FailAndDefaultValue();
            }

            var arguments = Array.Empty<string>();

            if (args.Length > 1)
            {
                arguments = new string[args.Length - 1];

                Array.Copy(args, 1, arguments, 0, arguments.Length);
            }

            var commandName = args[0];

            return new Result<CommandArguments>(true, new CommandArguments
            {
                Arguments = arguments,
                Command = commandName
            });
        }

        protected int IndexOf(string name, string[] args)
        {
            return CommandUtilities.IndexOf(name, args);
        }

        protected int IndexOf(NameBundle nameBundle, string[] args)
        {
            return CommandUtilities.IndexOf(nameBundle, args);
        }

        protected bool AreNamesEqual(NameBundle commandName, string name)
        {
            return CommandUtilities.AreNamesEqual(commandName, name);
        }

        protected bool AreNamesEqual(string name1, string name2)
        {
            return CommandUtilities.AreNamesEqual(name1, name2);
        }

        protected bool IsThisSetMyCommand(Context context, string[] args)
        {
            var arguments = GetCommandArguments(args);

            if (arguments)
            {
                if (AreNamesEqual(NameBundle, arguments.Value.Command))
                {
                    return true;
                }
            }

            return false;
        }

        protected bool AmIPresent(string[] args)
        {
            return IndexOf(NameBundle, args) > -1;
        }

        protected Result<string> FindMyValue(string[] args)
        {
            var index = IndexOf(NameBundle, args);

            if (index > -1)
            {
                if (args.Length > index + 1)
                {
                    return new Result<string>(true, args[1]);
                }
            }

            return new Result<string>().FailAndDefaultValue();
        }
    }
}
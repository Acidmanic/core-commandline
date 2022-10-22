using System;
using Acidmanic.Utilities.Results;
using CoreCommandLine.Attributes;
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

        protected ILogger Logger { get; private set; }

        public void SetLogger(ILogger logger)
        {
            Logger = logger;
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
            for (int i = 0; i < args.Length; i++)
            {
                if (name.ToLower() == args[i].ToLower())
                {
                    return i;
                }
            }

            return -1;
        }

        protected int IndexOf(NameBundle nameBundle, string[] args)
        {
            int index = IndexOf(nameBundle.Name, args);

            if (index < 0)
            {
                index = IndexOf(nameBundle.ShortName, args);
            }

            return index;
        }

        protected bool AreNamesEqual(NameBundle commandName, string name)
        {
            return AreNamesEqual(commandName.Name, name) ||
                   AreNamesEqual(commandName.ShortName, name);
        }

        protected bool AreNamesEqual(string name1, string name2)
        {
            if (name1 == null && name2 == null)
            {
                return true;
            }

            if (name1 == null || name2 == null)
            {
                return false;
            }

            return name1.ToLower() == name2.ToLower();
        }

        protected bool IsThisSetMyCommand(Context context,string[] args)
        {
            var arguments = GetCommandArguments(args);

            if (arguments)
            {
                if (AreNamesEqual(NameBundle,arguments.Value.Command))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
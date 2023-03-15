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

        /// <summary>
        /// Given a set of inputs (string[] args) this method grabs the first one as command name,
        /// and the reset as arguments of the command.
        /// </summary>
        /// <param name="args">input arguments to be processed</param>
        /// <returns>
        /// an instance of CommandArguments structure containing the command name and it's arguments,
        /// wrapped in a Result class.
        /// </returns>
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

        /// <summary>
        /// Determines if the given string[]args starts with current commands .
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args">input arguments to be processed</param>
        /// <returns>
        /// True if this set of inputs should result in execution of current command. Otherwise returns False.
        /// </returns>
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

        /// <summary>
        /// Searches the args[] array to see if current command is appeared. 
        /// </summary>
        /// <param name="args">input arguments to be processed</param>
        /// <returns>True if current command name is mentioned in inputs. Otherwise returns False.</returns>
        protected bool AmIPresent(string[] args)
        {
            return IndexOf(NameBundle, args) > -1;
        }

        /// <summary>
        /// For Argument Commands, Searches the given set of input arguments for any occurrence of current command.
        /// If found any, the next input would be considered as the value user entered for this command. 
        /// </summary>
        /// <param name="args">input arguments to be processed</param>
        /// <returns>Successful Result containing the found value. Otherwise returns a Failure Result.</returns>
        private Result<string> FindMyValue(string[] args)
        {
            var index = IndexOf(NameBundle, args);

            if (index > 0 && index < args.Length - 1)
            {
                return new Result<string>(true, args[index + 1]);
            }

            return new Result<string>().FailAndDefaultValue();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Results;
using CoreCommandLine.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace CoreCommandLine
{
    public abstract class CommandLineApplication
    {
        public ILogger Logger { get; set; } = new ConsoleLogger();

        public Action<string> Output { get; set; } = Console.WriteLine;

        public string ApplicationTitle { get; set; } = "Command line Application";

        public string ApplicationDescription { get; set; } = "";


        private readonly List<Type> _applicationSubCommands;


        public CommandLineApplication()
        {
            _applicationSubCommands = GetChildrenTypes(this.GetType(), true);
        }


        protected bool IsRootCommand(Type commandType)
        {
            foreach (var type in _applicationSubCommands)
            {
                if (type == commandType)
                {
                    return true;
                }
            }

            return false;
        }

        protected bool IsRootCommand(ICommand command)
        {
            if (command == null)
            {
                return false;
            }

            return IsRootCommand(command.GetType());
        }

        protected List<Type> GetChildrenTypes(Type type, bool addExit)
        {
            var childrenTypes = new List<Type>();

            var childrenAttribute = type.GetCustomAttributes<SubcommandsAttribute>(true).FirstOrDefault();

            if (childrenAttribute != null)
            {
                childrenTypes.AddRange(childrenAttribute.Children);
            }

            var genericHelpType = typeof(Help<>);

            if (!type.IsGenericType || type.GetGenericTypeDefinition() != genericHelpType)
            {
                var helpType = genericHelpType.MakeGenericType(type);

                childrenTypes.Add(helpType);
            }

            if (addExit && type.IsSubclassOf(typeof(CommandLineApplication)))
            {
                childrenTypes.Add(typeof(Exit));
            }

            return childrenTypes;
        }


        public void Execute(string[] args)
        {
            var context = new Context();

            InitializeContext(context);

            var type = this.GetType();

            WrapExecute(type, context, args, false);
        }

        public void ExecuteInteractive()
        {
            Output(ApplicationTitle);

            Output(Line(ApplicationTitle, 15));

            Output(ApplicationDescription);

            Output(Line(ApplicationDescription, 0));

            var stay = true;

            while (stay)
            {
                var context = new Context();

                InitializeContext(context);

                var type = this.GetType();

                string line = Console.ReadLine();

                var args = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

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
            if (args != null && args.Length > 0)
            {
                var command = CommandFactory.Instance.Make(args[0], this.GetType());

                if (command != null && command.GetType() != typeof(NullCommand))
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
                OnBeforeExecution(context,args,foundCommand.Value);
            }
            
            Execute(type, context, args, useExitCommand);
            
            if (foundCommand)
            {
                OnAfterExecution(context,args,foundCommand.Value);
            }
        }

        private void Execute(Type parentType, Context context, string[] args, bool useExitCommand)
        {
            if (context.ApplicationExit)
            {
                return;
            }

            var childrenTypes = GetChildrenTypes(parentType, useExitCommand);

            foreach (var childType in childrenTypes)
            {
                if (context.ApplicationExit)
                {
                    return;
                }

                Execute(childType, context, args, useExitCommand);
            }

            var instance = new ObjectInstantiator().BlindInstantiate(parentType) as ICommand;

            if (instance != null && !context.ApplicationExit)
            {
                instance.SetLogger(Logger);

                instance.SetOutput(Output);

                instance.Execute(context, args);
            }
        }

        protected virtual void InitializeContext(Context context)
        {
        }

        protected virtual void OnBeforeExecution(Context context, string[] args, ICommand command)
        {
        }

        protected virtual void OnAfterExecution(Context context, string[] args, ICommand command)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Acidmanic.Utilities.Reflection;
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

            Execute(type, context, args, false);
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

                Execute(type, context, args, true);

                stay = !context.InteractiveExit;
            }
        }

        private string Line(string applicationTitle, int nullLength)
        {
            int length = nullLength;

            if (!string.IsNullOrEmpty(applicationTitle))
            {
                length = applicationTitle.Length;
            }

            var line = "";

            for (int i = 0; i < length; i++)
            {
                line += "-";
            }

            return line;
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

                OnBeforeExecution(context, args, instance);

                instance.Execute(context, args);

                OnAfterExecution(context, args, instance);
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
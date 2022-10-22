using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using CoreCommandLine.Attributes;

namespace CoreCommandLine
{
    public abstract class CommandLineApplication
    {
        protected List<Type> GetChildrenTypes(Type type, bool addExit)
        {
            var childrenTypes = new List<Type>();

            var childrenAttribute = type.GetCustomAttributes<SubcommandsAttribute>(true).FirstOrDefault();

            if (childrenAttribute != null)
            {
                childrenTypes.AddRange(childrenAttribute.Children);
            }
            
            var genericHelpType = typeof(Help<>);

            if (!type.IsGenericType || type.GetGenericTypeDefinition()!= genericHelpType)
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

            var type = this.GetType();

            Execute(type, context, args,false);
        }
        
        public void ExecuteInteractive()
        {
            var stay = true;

            while (stay)
            {
                var context = new Context();

                var type = this.GetType();

                string line = Console.ReadLine();

                var args = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                
                Execute(type, context, args,true);

                stay = !context.InteractiveExit;
            }
        }

        private void Execute(Type parentType, Context context, string[] args,bool useExitCommand)
        {
            if (context.ApplicationExit)
            {
                return;
            }
            var childrenTypes = GetChildrenTypes(parentType,useExitCommand);

            foreach (var childType in childrenTypes)
            {
                if (context.ApplicationExit)
                {
                    return;
                }
                
                Execute(childType, context, args,useExitCommand);
            }
            var instance = new ObjectInstantiator().BlindInstantiate(parentType) as ICommand;

            if (instance != null && !context.ApplicationExit)
            {
                instance.Execute(context, args);
            }
        }
    }
}
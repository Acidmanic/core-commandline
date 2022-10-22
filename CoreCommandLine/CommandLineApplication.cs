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
        protected List<Type> GetChildrenTypes(Type type)
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
            
            return childrenTypes;
        }

        protected List<ICommand> GetChildren(Type type)
        {
            var childrenTypes = GetChildrenTypes(type);

            var children = new List<ICommand>();

            foreach (var childType in childrenTypes)
            {
                var child = new ObjectInstantiator().BlindInstantiate(childType) as ICommand;

                if (child != null)
                {
                    children.Add(child);
                }
            }

            return children;
        }

        public void Execute(string[] args)
        {
            var context = new Context();

            var type = this.GetType();

            Execute(type, context, args);
        }

        private void Execute(Type parentType, Context context, string[] args)
        {
            if (context.ApplicationExit)
            {
                return;
            }
            var childrenTypes = GetChildrenTypes(parentType);

            foreach (var childType in childrenTypes)
            {
                if (context.ApplicationExit)
                {
                    return;
                }
                
                Execute(childType, context, args);
            }
            var instance = new ObjectInstantiator().BlindInstantiate(parentType) as ICommand;

            if (instance != null && !context.ApplicationExit)
            {
                instance.Execute(context, args);
            }
        }
    }
}
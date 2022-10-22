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

            var childrenAttribute = type.GetCustomAttributes<SubcommandsAttribute>().FirstOrDefault();

            if (childrenAttribute != null)
            {
                childrenTypes.AddRange(childrenAttribute.Children);
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
            var childrenTypes = GetChildrenTypes(parentType);

            foreach (var childType in childrenTypes)
            {
                Execute(childType, context, args);
            }
            var child = new ObjectInstantiator().BlindInstantiate(parentType) as ICommand;

            if (child != null)
            {
                child.Execute(context, args);
            }
        }
    }
}
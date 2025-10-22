using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Results;
using CoreCommandLine.Attributes;
using CoreCommandLine.Di;

namespace CoreCommandLine
{
    internal class CommandFactory
    {
        private readonly List<Type> applicationSubCommands;
        private readonly IResolver resolver;

        internal CommandFactory(IResolver resolver, List<Type> applicationSubCommands)
        {
            this.resolver = resolver;
            this.applicationSubCommands = applicationSubCommands;
        }

        public ICommand Make(string name, Type caller, bool includeExitCommand)
        {
            List<Type> children;

            var applicationType = typeof(CommandLineApplication);

            if (applicationType == caller || caller.IsSubclassOf(applicationType))
            {
                children = new List<Type>(applicationSubCommands);

                if (includeExitCommand) children.Add(typeof(Exit));
            }
            else
            {
                children = GetChildrenTypes(caller, includeExitCommand);
            }

            return Make(name, children);
        }


        private ICommand Make(string name, List<Type> children)
        {
            if (!string.IsNullOrEmpty(name))
            {
                foreach (var child in children)
                {
                    var foundName = child.GetCommandName();

                    if (foundName)
                    {
                        if (AreNamesEqual(foundName.Value, name))
                        {
                            var instance = Instantiate(child);

                            if (instance)
                            {
                                return instance.Value;
                            }
                        }
                    }
                }
            }

            return new CommandNotFoundCommand();
        }

        public List<Type> GetChildrenTypes(Type type, bool addExit)
        {
            var childrenTypes = new List<Type>();

            if (makeHelpType(type) is { } help)
            {
                childrenTypes.Add(help);
            }

            var commandLineApplicationType = typeof(CommandLineApplication);

            if (commandLineApplicationType == type || type.IsSubclassOf(commandLineApplicationType))
            {
                childrenTypes.AddRange(applicationSubCommands);
            }

            var childrenAttribute = type.GetCustomAttributes<SubcommandsAttribute>(true).FirstOrDefault();

            if (childrenAttribute != null)
            {
                childrenTypes.AddRange(childrenAttribute.Children);
            }

            var commandlineApplicationType = typeof(CommandLineApplication);

            if (addExit && (type == commandlineApplicationType || type.IsSubclassOf(commandlineApplicationType)))
            {
                childrenTypes.Add(typeof(Exit));
            }

            return childrenTypes;
        }

        private Type? makeHelpType(Type type)
        {
            var genericHelpType = typeof(Help<>);

            if (!type.IsGenericType || type.GetGenericTypeDefinition() != genericHelpType)
            {
                var helpType = genericHelpType.MakeGenericType(type);

                return helpType;
            }

            return null;
        }

        private bool AreNamesEqual(NameBundle commandName, string name)
        {
            return AreNamesEqual(commandName.Name, name) ||
                   AreNamesEqual(commandName.ShortName, name);
        }

        private bool AreNamesEqual(string name1, string name2)
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

        public Result<ICommand> Instantiate(Type type)
        {
            var command = resolver.Resolve(type) as ICommand ?? new ObjectInstantiator().BlindInstantiate(type) as ICommand;

            return new Result<ICommand>(command != null, command);
        }
    }
}
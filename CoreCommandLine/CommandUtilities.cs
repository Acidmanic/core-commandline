using System.Reflection;
using Acidmanic.Utilities.Reflection;
using CoreCommandLine.Attributes;
using CoreCommandLine.Di;

namespace CoreCommandLine
{
    internal class CommandUtilities
    {
        private readonly List<Type> _applicationSubCommands;
        private readonly IResolver _resolver;

        internal CommandUtilities(IResolver resolver, List<Type> applicationSubCommands)
        {
            _resolver = resolver;
            _applicationSubCommands = applicationSubCommands;
        }

        public List<Type> GetChildrenTypes(Type type, bool addExit)
        {
            var childrenTypes = new List<Type>();

            if (MakeHelpType(type) is { } help)
            {
                childrenTypes.Add(help);
            }

            var commandLineApplicationType = typeof(CommandLineApplication);

            if (commandLineApplicationType == type || type.IsSubclassOf(commandLineApplicationType))
            {
                childrenTypes.AddRange(_applicationSubCommands);
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

        private Type? MakeHelpType(Type type)
        {
            var genericHelpType = typeof(Help<>);

            if (!type.IsGenericType || type.GetGenericTypeDefinition() != genericHelpType)
            {
                var helpType = genericHelpType.MakeGenericType(type);

                return helpType;
            }

            return null;
        }

        public ICommand? Instantiate(Type type)
        {
            var command = _resolver.Resolve(type) as ICommand ??
                          new ObjectInstantiator().BlindInstantiate(type) as ICommand;

            return command;
        }
        
        public string GetHelpMessage(Type ownerType, bool addExit)
        {
            var message = "";

            var childrenTypes = GetChildrenTypes(ownerType, addExit);

            foreach (var childType in childrenTypes)
            {
                var child = Instantiate(childType);
                
                var nameBundle = childType.GetCommandName();

                if (nameBundle && child is {} )
                {
                    var line = "\t" + nameBundle.Value.Name;

                    if (nameBundle.Value.ShortName != nameBundle.Value.Name)
                    {
                        line += " | " + nameBundle.Value.ShortName;
                    }

                    line += "\t" + child.Description;

                    message += line + "\n";
                }
            }

            return message;
        }
    }
}
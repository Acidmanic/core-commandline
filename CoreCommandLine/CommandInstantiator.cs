using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Results;
using CoreCommandLine.Di;

namespace CoreCommandLine
{
    internal class CommandInstantiator
    {
        public IResolver Resolver { get; private set; } = new NullResolver();

        private static object _locker = new object();
        private static CommandInstantiator _instantiator = null;


        private CommandInstantiator()
        {
        }

        public static CommandInstantiator Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instantiator == null)
                    {
                        _instantiator = new CommandInstantiator();
                    }
                }

                return _instantiator;
            }
        }

        public CommandInstantiator UseResolver(IResolver resolver)
        {
            Resolver = resolver;

            return this;
        }


        public Result<ICommand> Instantiate(Type type)
        {
            ICommand command = null;

            if (Resolver != null)
            {
                command = Resolver.Resolve(type) as ICommand;
            }

            if (command == null)
            {
                command = new ObjectInstantiator().BlindInstantiate(type) as ICommand;
            }

            return new Result<ICommand>(command != null, command);
        }
    }
}
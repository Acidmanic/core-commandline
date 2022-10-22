using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.TypeCenter;

namespace CoreCommandLine
{
    public class CommandFactory
    {
        private static object Lock { get; } = new object();

        private static CommandFactory _instance = null;

        public static CommandFactory Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CommandFactory();
                    }
                }

                return _instance;
            }
        }

        private Dictionary<string, Type> _typesByName;
        private Dictionary<string, Type> _typesByShortName;
        public List<ICommand> CommandInstances { get; }

        private CommandFactory()
        {
            _typesByName = new Dictionary<string, Type>();
            _typesByShortName = new Dictionary<string, Type>();
            CommandInstances = new List<ICommand>();

            var assembly = Assembly.GetCallingAssembly();

            var allTypes = assembly.GetAvailableTypes();

            foreach (var type in allTypes)
            {
                
                
                
                var instance = new ObjectInstantiator().CreateObject(type, true);

                if (instance != null && instance is ICommand command)
                {
                    _typesByName.Add(command.Name, type);
                    _typesByShortName.Add(command.ShortName, type);
                    CommandInstances.Add(command);
                }
            }
        }

        public ICommand Make(string name)
        {
            Type type = typeof(NullCommand);

            if (_typesByName.ContainsKey(name))
            {
                type = _typesByName[name];
            }
            if (_typesByShortName.ContainsKey(name))
            {
                type = _typesByShortName[name];
            }
            
            return new ObjectInstantiator().CreateObject(type, true) as ICommand;
        }
    }
}
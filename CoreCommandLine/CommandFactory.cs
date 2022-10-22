using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;

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

        
        private CommandFactory()
        {
        
        }

        public ICommand Make(string name,Type caller)
        {
            var children = caller.GetChildren();

            return Make(name, children);
        }
        
        
        private ICommand Make(string name,List<Type> children)
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
                            var instance = 
                                new ObjectInstantiator()
                                .BlindInstantiate(child);
                            if (instance is ICommand command)
                            {
                                return command;
                            }
                        }
                    }
                }      
            }

            return new NullCommand();
        }

        private bool AreNamesEqual(NameBundle commandName, string name)
        {
            return AreNamesEqual(commandName.Name,name) ||
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
    }
}
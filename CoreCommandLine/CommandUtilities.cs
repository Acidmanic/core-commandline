using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection;
using CoreCommandLine.Attributes;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    public class CommandUtilities
    {
        public static string GetHelpMessage(List<Type> childrenTypes)
        {
            var message = "";

            foreach (var childType in childrenTypes)
            {
                var child = CommandInstantiator.Instance.Instantiate(childType);

                if (child)
                {
                    var nameBundle = childType.GetCommandName();

                    if (nameBundle)
                    {
                        var line = "\t" + nameBundle.Value.Name;

                        if (nameBundle.Value.ShortName != nameBundle.Value.Name)
                        {
                            line += " | " + nameBundle.Value.ShortName;
                        }

                        line += "\t" + child.Value.Description;

                        message += line + "\n";
                    }
                }
            }

            return message;
        }
        
        public static int IndexOf(string name, string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (name.ToLower() == args[i].ToLower())
                {
                    return i;
                }
            }

            return -1;
        }

        public static int IndexOf(NameBundle nameBundle, string[] args)
        {
            int index = IndexOf(nameBundle.Name, args);

            if (index < 0)
            {
                index = IndexOf(nameBundle.ShortName, args);
            }

            return index;
        }
        
        
        public static bool AreNamesEqual(NameBundle commandName, string name)
        {
            return AreNamesEqual(commandName.Name, name) ||
                   AreNamesEqual(commandName.ShortName, name);
        }

        public static bool AreNamesEqual(string name1, string name2)
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
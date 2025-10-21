using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Results;
using CoreCommandLine.Attributes;

namespace CoreCommandLine
{
    public static class CommandExtensions
    {
        public static Result<NameBundle> GetCommandName(this ICommand command)
        {
            if (command != null)
            {
                return GetCommandName(command.GetType());
            }

            return new Result<NameBundle>().FailAndDefaultValue();
        }

        public static Result<NameBundle> GetCommandName(this Type commandType)
        {
            if (TypeCheck.Implements<ICommand>(commandType))
            {
                var nameAttribute =
                    commandType.GetCustomAttributes<CommandNameAttribute>()
                        .FirstOrDefault();

                if (nameAttribute != null)
                {
                    return new Result<NameBundle>(true, new NameBundle
                    {
                        Name = nameAttribute.Name,
                        ShortName = nameAttribute.ShortName
                    });
                }
            }

            return new Result<NameBundle>().FailAndDefaultValue();
        }
    }
}
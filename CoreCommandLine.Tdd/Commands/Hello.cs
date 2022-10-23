using System;
using CoreCommandLine.Attributes;
using CoreCommandLine.Tdd.Commands.Arguments;
using CoreCommandLine.Tdd.Commands.Models;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine.Tdd.Commands
{
    [CommandName("hello")]
    [Subcommands(typeof(Upper),typeof(Lower))]
    public class Hello:CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            if (IsThisSetMyCommand(context, args))
            {
                var message = "Hello";

                var textStyle = context.Get(nameof(TextStyle),TextStyle.Regular);

                if (textStyle == TextStyle.Upper)
                {
                    message = message.ToUpper();
                    
                }else if (textStyle == TextStyle.Lower)
                {
                    message = message.ToLower();
                }

                Output(message);

                return true;
            }
            return false;
        }

        public override string Description => "Prints the phrase Hello.";
    }
}
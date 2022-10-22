using System;
using CoreCommandLine.Attributes;
using CoreCommandLine.Tdd.Commands.Arguments;
using CoreCommandLine.Tdd.Commands.Models;

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

                Console.WriteLine(message);

                return true;
            }
            return false;
        }
    }
}
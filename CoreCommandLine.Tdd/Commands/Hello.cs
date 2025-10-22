using System;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using CoreCommandLine.Tdd.Commands.Arguments;
using CoreCommandLine.Tdd.Commands.Models;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine.Tdd.Commands
{
    [CommandName("hello")]
    [RootCommand]
    [Subcommands(typeof(Upper), typeof(Lower))]
    public class Hello : NodeCommandBase
    {
        protected override void Execute(Context context)
        {
            var message = "Hello";

            var textStyle = context.Get(nameof(TextStyle), TextStyle.Regular);

            if (textStyle == TextStyle.Upper)
            {
                message = message.ToUpper();
            }
            else if (textStyle == TextStyle.Lower)
            {
                message = message.ToLower();
            }

            Output(message);
        }

        public override string Description => "Prints the phrase Hello.";
    }
}
using System;
using CoreCommandLine.Attributes;
using CoreCommandLine.Tdd.Commands.Arguments;
using CoreCommandLine.Tdd.Commands.Models;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine.Tdd.Commands
{
    [CommandName("world")]
    [RootCommand]
    [Subcommands(typeof(Upper), typeof(Lower))]
    public class World : CommandBase
    {
        public override int Execute(Context context, string[] args)
        {
            var message = "World";

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

            return 0;
        }

        public override string Description => "Prints the phrase World.";
    }
}
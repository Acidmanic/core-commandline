using CoreCommandLine.Attributes;
using CoreCommandLine.Tdd.Commands.Models;

namespace CoreCommandLine.Tdd.Commands.Arguments
{
    [CommandName("--lower","-l")]
    public class Lower:CommandBase
    {
        public override int Execute(Context context, string[] args)
        {
            context.Set(nameof(TextStyle),TextStyle.Lower);

            return 0;
        }

        public override string Description => "This Argument Sets output to lower case.";
    }
}
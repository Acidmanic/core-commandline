using CoreCommandLine.Attributes;
using CoreCommandLine.Tdd.Commands.Models;

namespace CoreCommandLine.Tdd.Commands.Arguments
{
    [CommandName("--upper", "-u")]
    public class Upper : CommandBase
    {
        public override int Execute(Context context, string[] args)
        {
            context.Set(nameof(TextStyle), TextStyle.Upper);

            return 0;
        }

        public override string Description => "This Argument Sets output to UPPER case.";
    }
}
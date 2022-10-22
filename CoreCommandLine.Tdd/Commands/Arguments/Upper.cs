using CoreCommandLine.Attributes;
using CoreCommandLine.Tdd.Commands.Models;

namespace CoreCommandLine.Tdd.Commands.Arguments
{
    [CommandName("--upper","-u")]
    public class Upper:CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            var name = this.GetCommandName();

            if (name)
            {
                var index = IndexOf(name, args);

                if (index >= 0)
                {
                    context.Set(nameof(TextStyle),TextStyle.Upper);

                    return true;
                }
            }

            return false;
        }
        
        public override string Description => "This Argument Sets output to UPPER case.";
    }
}
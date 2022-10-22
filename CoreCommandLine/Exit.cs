using CoreCommandLine.Attributes;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    [CommandName("exit")]
    public class Exit:CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            if (IsThisSetMyCommand(context, args))
            {
                context.InteractiveExit = true;

                return true;
            }

            return false;
        }

        public override string Description => "Exits the interactive mode.";
        
        
    }
}
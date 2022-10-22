using CoreCommandLine.Attributes;

namespace CoreCommandLine
{
    
    [CommandName("--help","-h")]
    public class Help:CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            return false;
        }
    }
}
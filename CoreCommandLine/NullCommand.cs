using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    public class NullCommand:ICommand
    {
        public bool Execute(Context context, string[] args)
        {
            return true;
        }

        public string Name => "Null";
        public string ShortName => "";
        public string Description => "";
        public void SetLogger(ILogger logger)
        {
        }
    }
}
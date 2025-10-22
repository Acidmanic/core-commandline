using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    public class NullCommand:ICommand
    {
        public int Execute(Context context, string[] args)
        {
            return 0;
        }

        public string Name => "Null";
        public string ShortName => "";
        public string Description => "";
        public void SetLogger(ILogger logger)
        {
        }

    }
}
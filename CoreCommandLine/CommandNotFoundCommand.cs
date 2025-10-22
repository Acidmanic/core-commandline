using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace CoreCommandLine
{
    public class CommandNotFoundCommand:ICommand
    {
        private  ILogger _logger = new ConsoleLogger();
        
        public int Execute(Context context, string[] args)
        {
            var helpBundle = typeof(Help<object>).GetCommandName();
            
            _logger.LogError("Command Not Found, use {Name} Or {ShortName} to display help message.",helpBundle.Value.Name,helpBundle.Value.ShortName);
            
            return 0;
        }

        public Task<int> Execute(Context context, string[] args, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(context,args));
        }

        public string Name => "Null";
        public string ShortName => "";
        public string Description => "";
        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

    }
}
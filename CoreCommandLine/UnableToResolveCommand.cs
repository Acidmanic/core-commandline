using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

namespace CoreCommandLine
{
    public class UnableToResolveCommand : ICommand
    {
        private ILogger _logger = new ConsoleLogger();

        public UnableToResolveCommand(string name)
        {
            Name = name;
        }

        public int Execute(Context context, string[] args)
        {
            _logger.LogDebug("Command Not Instantiate command {Name}.", Name);

            return 0;
        }

        public Task<int> Execute(Context context, string[] args, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(context, args));
        }

        public string Name { get; }
        public string ShortName => "";
        public string Description => "";

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }
    }
}
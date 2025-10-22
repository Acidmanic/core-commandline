using CoreCommandLine.Attributes;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    [CommandName("--help", "-h")]
    public class Help<T> : ICommand
    {
        private ILogger _logger = new ConsoleOutput();
        private Action<string> _output = Console.WriteLine;

        private NameBundle NameBundle { get; } = new NameBundle
        {
            Name = "--help",
            ShortName = "-h"
        };

        public int Execute(Context context, string[] args)
        {
            var ownerType = typeof(T);

            var childrenTypes = context.Factory.GetChildrenTypes(ownerType, false);

            var message = CommandUtilities.GetHelpMessage(context.Factory, childrenTypes);

            _output(message);

            context.ApplicationExit = true;

            return 0;
        }

        public Task<int> Execute(Context context, string[] args, CancellationToken cancellationToken) => Task.FromResult(Execute(context, args));

        public string Description => "Displays this message";

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void SetOutput(Action<string> output)
        {
            _output = output;
        }
    }
}
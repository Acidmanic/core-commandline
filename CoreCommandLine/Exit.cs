using CoreCommandLine.Attributes;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    [CommandName("exit")]
    public class Exit : CommandBase
    {
        public override int Execute(Context context, string[] args)
        {
            context.InteractiveExit = true;

            Logger.LogInformation("Exiting Interactive application...");

            return args.Length;
        }

        public override Task<int> Execute(Context context, string[] args, CancellationToken cancellationToken) => Task.FromResult(Execute(context, args));

        public override string Description => "Exits the interactive mode.";
    }
}
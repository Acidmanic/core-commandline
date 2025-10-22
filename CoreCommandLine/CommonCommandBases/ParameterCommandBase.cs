using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine.CommonCommandBases
{
    public abstract class ParameterCommandBase : CommandBase
    {
        public override int Execute(Context context, string[] args)
        {
            if (args.Length > 0)
            {
                var value = FindMyValue(args);

                RetrieveData(context, value.Value);

                return 1;
            }

            Logger.LogWarning("No value has been given for {Name}", Name);

            return 0;
        }

        public override Task<int> Execute(Context context, string[] args, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(context, args));
        }

        protected abstract void RetrieveData(Context context, string parameterStringValue);
    }
}
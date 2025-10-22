using Microsoft.Extensions.Logging;

namespace CoreCommandLine.CommonCommandBases;

public abstract class ArgumentRangeAsyncCommandBase : CommandBase
{
    public override int Execute(Context context, string[] args)
    {
        return Execute(context, args, CancellationToken.None).Result;
    }

    public override async Task<int> Execute(Context context, string[] args, CancellationToken cancellationToken)
    {
        var expectedCount = NumberOfExpectedArguments ?? args.Length;

        var actualArgumentsCount = Math.Min(expectedCount, args.Length);

        if (actualArgumentsCount != expectedCount && WarnWhenUnsatisfied)
        {
            Logger.LogWarning("Expected to read {Expected} arguments for {Command} but only found {Actual}",expectedCount,Name,actualArgumentsCount);
        }
        
        var a = args.Take(actualArgumentsCount).ToArray();
        
        await DoExecute(context,a,cancellationToken);

        return expectedCount;
    }
    
    protected abstract Task DoExecute(Context context,string[] args, CancellationToken cancellationToken);

    protected virtual int? NumberOfExpectedArguments => null;
    
    protected virtual bool WarnWhenUnsatisfied => true;
}
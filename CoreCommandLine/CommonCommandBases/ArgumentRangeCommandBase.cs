using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine.CommonCommandBases;

public abstract class ArgumentRangeCommandBase : CommandBase
{
       
    public override int Execute(Context context, string[] args)
    {
        var expectedCount = NumberOfExpectedArguments ?? args.Length;

        var actualArgumentsCount = Math.Min(expectedCount, args.Length);

        if (actualArgumentsCount != expectedCount && WarnWhenUnsatisfied)
        {
            Logger.LogWarning("Expected to read {Expected} arguments for {Command} but only found {Actual}",expectedCount,Name,actualArgumentsCount);
        }
        
        var a = args.Take(actualArgumentsCount).ToArray();
        
        DoExecute(context,a);

        return expectedCount;
    }

    public override Task<int> Execute(Context context, string[] args, CancellationToken cancellationToken)
    {
        return Task.FromResult(Execute(context, args));
    }
    
    protected abstract void DoExecute(Context context,string[] args);

    protected virtual int? NumberOfExpectedArguments => null;
 
    protected virtual bool WarnWhenUnsatisfied => true;
}
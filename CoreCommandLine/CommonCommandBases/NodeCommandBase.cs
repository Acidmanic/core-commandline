namespace CoreCommandLine.CommonCommandBases;

public abstract class NodeCommandBase : CommandBase
{
    public override int Execute(Context context, string[] args)
    {
        Execute(context);

        return 0;
    }

    public override Task<int> Execute(Context context, string[] args, CancellationToken cancellationToken)
    {
        return Task.FromResult(Execute(context, args));
    }
    
    protected abstract void Execute(Context context);
}
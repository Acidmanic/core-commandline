namespace CoreCommandLine.CommonCommandBases;

public abstract class NodeAsyncCommandBase : CommandBase
{
    public override int Execute(Context context, string[] args)
    {
        Execute(context, CancellationToken.None).Wait();

        return 0;
    }

    public override async Task<int> Execute(Context context, string[] args, CancellationToken cancellationToken)
    {
        await Execute(context, cancellationToken);

        return 0;
    }

    protected abstract Task Execute(Context context, CancellationToken cancellationToken);
}
namespace CoreCommandLine.CommonCommandBases;

public abstract class NodeCommandBase : CommandBase
{
    public override int Execute(Context context, string[] args)
    {
        Execute(context);

        return 0;
    }

    protected abstract void Execute(Context context);
}
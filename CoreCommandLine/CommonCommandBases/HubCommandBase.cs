namespace CoreCommandLine.CommonCommandBases
{
    public abstract class HubCommandBase : CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            return IsThisSetMyCommand(context, args,
                commandSet => Execute(context, commandSet));
        }

        protected abstract void Execute(Context context, CommandArguments arguments);
    }
}
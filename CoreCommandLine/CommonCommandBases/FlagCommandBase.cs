namespace CoreCommandLine.CommonCommandBases
{
    public abstract class FlagCommandBase : CommandBase
    {
        public override int Execute(Context context, string[] args)
        {
            SetFlagOnPresence(context);

            return 0;
        }

        public override Task<int> Execute(Context context, string[] args, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(context, args));
        }

        protected abstract void SetFlagOnPresence(Context context);
    }
}
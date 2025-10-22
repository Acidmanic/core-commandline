namespace CoreCommandLine.CommonCommandBases
{
    public abstract class FlagCommandBase : CommandBase
    {
        public override int Execute(Context context, string[] args)
        {
            SetFlagOnPresence(context);

            return 0;
        }

        protected abstract void SetFlagOnPresence(Context context);
    }
}
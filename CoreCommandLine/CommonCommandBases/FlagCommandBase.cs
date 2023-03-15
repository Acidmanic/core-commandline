namespace CoreCommandLine.CommonCommandBases
{
    public abstract class FlagCommandBase : CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            if (AmIPresent(args))
            {
                SetFlagOnPresence(context);

                return true;
            }

            return false;
        }

        protected abstract void SetFlagOnPresence(Context context);
    }
}
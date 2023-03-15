namespace CoreCommandLine.CommonCommandBases
{
    public  abstract class ParameterCommandBase:CommandBase
    {
        public override bool Execute(Context context, string[] args)
        {
            if (AmIPresent(args))
            {
                var value = FindMyValue(args);

                RetrieveData(context,value.Value);
                
                return true;
            }

            return false;
        }

        protected abstract void RetrieveData(Context context, string parameterStringValue);
    }
}
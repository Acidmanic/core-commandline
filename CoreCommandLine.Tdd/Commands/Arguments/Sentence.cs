using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;

namespace CoreCommandLine.Tdd.Commands.Arguments
{
    [CommandName(Key, "s")]
    public class Sentence : ParameterCommandBase
    {
        public const string Key = "sentence";

        protected override void RetrieveData(Context context, string parameterStringValue)
        {
            context.Set(Name, parameterStringValue);
        }
    }
}
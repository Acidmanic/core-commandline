using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using CoreCommandLine.Tdd.Commands.Models;

namespace CoreCommandLine.Tdd.Commands.Arguments
{
    [CommandName("--lower","-l")]
    public class Lower:ParameterCommandBase
    {
        protected override void RetrieveData(Context context, string parameterStringValue)
        {
            context.Set(nameof(TextStyle),TextStyle.Lower);
        }

        public override string Description => "This Argument Sets output to lower case.";
    }
}
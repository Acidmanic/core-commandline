using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using CoreCommandLine.Tdd.Commands.Models;

namespace CoreCommandLine.Tdd.Commands.Arguments
{
    [CommandName("--upper", "-u")]
    public class Upper : ParameterCommandBase
    {
        protected override void RetrieveData(Context context, string parameterStringValue)
        {
            context.Set(nameof(TextStyle), TextStyle.Upper);
        }

        public override string Description => "This Argument Sets output to UPPER case.";
    }
}
using CoreCommandLine;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;

namespace Example.CommonCommandBases
{
    [CommandName("example-parameter", "ep")]
    public class ExampleParameterCommand : ParameterCommandBase
    {
        public static string Key => nameof(ExampleParameterCommand);

        protected override void RetrieveData(Context context, string parameterStringValue)
        {
            context.Set(Key, parameterStringValue);
        }
    }
}
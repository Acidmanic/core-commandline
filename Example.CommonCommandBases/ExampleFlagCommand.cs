using CoreCommandLine;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;

namespace Example.CommonCommandBases
{
    [CommandName("example-flag", "ef")]
    public class ExampleFlagCommand : FlagCommandBase
    {
        public static string Key => nameof(ExampleFlagCommand);

        protected override void SetFlagOnPresence(Context context)
        {
            context.Set(Key, true);
        }
    }
}
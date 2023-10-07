using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using CoreCommandLine.Tdd.Commands.Arguments;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine.Tdd.Commands
{
    [CommandName("echo", "e")]
    [Subcommands(typeof(Sentence))]
    public class Echo : HubCommandBase
    {
        protected override void Execute(Context context, CommandArguments arguments)
        {
            var sentence = context.Get(Sentence.Key, "");

            Logger.LogInformation("[ECHO] {Sentence}", sentence);
        }
    }
}
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using CoreCommandLine.Tdd.Commands.Arguments;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine.Tdd.Commands
{
    [RootCommand]
    [CommandName("echo", "e")]
    [Subcommands(typeof(Sentence))]
    public class Echo : NodeCommandBase
    {
        protected override void Execute(Context context)
        {
            var sentence = context.Get(Sentence.Key, "");

            Logger.LogInformation("[ECHO] {Sentence}", sentence);
        }
    }
}
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using CoreCommandLine.Tdd.Commands.Arguments;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine.Tdd.Commands
{
    [RootCommand]
    [CommandName("echo", "e")]
    [Subcommands(typeof(Sentence))]
    public class Echo : CommandBase
    {
        public override int Execute(Context context, string[] args)
        {
            var sentence = context.Get(Sentence.Key, "");

            Logger.LogInformation("[ECHO] {Sentence}", sentence);

            return 0;
        }
    }
}
using CoreCommandLine;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using Microsoft.Extensions.Logging;

namespace Example.DotnetDi.Commands;

[RootCommand]
[CommandName("hello","-hl")]
public class Hello:NodeCommandBase
{
    protected override void Execute(Context context)
    {
        Logger.LogInformation("Hello World");
    }
}
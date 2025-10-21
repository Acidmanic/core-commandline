using System;
using CoreCommandLine;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;

namespace Example.CommonCommandBases
{
    [RootCommand]
    [CommandName("example-hub","eh")]
    [Subcommands(typeof(ExampleParameterCommand),typeof(ExampleFlagCommand))]
    public class ExampleHubCommand:HubCommandBase
    {
        protected override void Execute(Context context, CommandArguments arguments)
        {
            Console.WriteLine("This is a hub command. It Performs some task.");
            Console.WriteLine($"The flag argument is {(context.Get(ExampleFlagCommand.Key,false)?"":"NOT")} set.");
            Console.WriteLine("The parameter argument is: " + context.Get(ExampleParameterCommand.Key,"[None Given]"));
        }
    }
}
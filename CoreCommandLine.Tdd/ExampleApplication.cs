using System;
using CoreCommandLine.Attributes;
using CoreCommandLine.Tdd.Commands;

namespace CoreCommandLine.Tdd
{
    
    [Subcommands(typeof(Hello),typeof(World))]
    public class ExampleApplication:CommandLineApplication
    {
        protected override void OnBeforeExecution(Context context, string[] args, ICommand command)
        {
            Output($"-------------- <{command.GetCommandName().Value.Name}> -----------------");
        }

        protected override void OnAfterExecution(Context context, string[] args, ICommand command)
        {
            Output($"-------------- </{command.GetCommandName().Value.Name}> -----------------");
        }
    }
}
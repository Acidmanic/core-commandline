using CoreCommandLine.Attributes;
using CoreCommandLine.Tdd.Commands;

namespace CoreCommandLine.Tdd
{
    
    [Subcommands(typeof(Hello),typeof(World))]
    public class ExampleApplication:CommandLineApplication
    {
        
    }
}
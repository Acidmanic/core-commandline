using CoreCommandLine;
using CoreCommandLine.Attributes;

namespace Example.CommonCommandBases
{
    [Subcommands(typeof(ExampleHubCommand))]
    public class ExampleApplication:CommandLineApplication
    {
        
    }
}
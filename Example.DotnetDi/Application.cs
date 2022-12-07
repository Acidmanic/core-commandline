using CoreCommandLine;
using CoreCommandLine.Attributes;
using Example.DotnetDi.Commands;

namespace Example.DotnetDi
{
    [Subcommands(typeof(Echo))]
    public class Application : CommandLineApplication
    {
        public Application()
        {
            ApplicationDescription = "This application represents di usage.";
            ApplicationTitle = "Di Example";
        }
    }
}
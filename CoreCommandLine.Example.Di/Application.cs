using CoreCommandLine.Attributes;
using CoreCommandLine.Example.Di.Commands;

namespace CoreCommandLine.Example.Di
{
    [Subcommands(typeof(Echo))]
    public class Application:CommandLineApplication
    {


        public Application()
        {
            ApplicationDescription = "This application represents di usage.";
            ApplicationTitle = "Di Example";
        }
    }
}
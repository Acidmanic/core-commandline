using CoreCommandLine;
using CoreCommandLine.Attributes;
using Example.Di.Commands;

namespace Example.Di
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
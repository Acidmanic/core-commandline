using System;
using CoreCommandLine.DotnetDi;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.DotnetDi
{
    class Program
    {
        static void Main(string[] args)
        {

            var application = new DotnetCommandlineApplicationBuilder<Application>()
                .UseLogger(new ConsoleLogger())
                .UseStartup<Startup>()
                .Build();
            
            application.ExecuteInteractive();
        }
    }
}

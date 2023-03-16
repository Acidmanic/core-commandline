using System;
using CoreCommandLine.DotnetDi;
using Microsoft.Extensions.Logging.LightWeight;

namespace Example.CommonCommandBases
{
    class Program
    {
        static void Main(string[] args)
        {

            var app = new DotnetCommandlineApplicationBuilder<ExampleApplication>()
                .Describe("Example", "3 Common Commands")
                .UseLogger(new ConsoleLogger().Shorten())
                .Build();
            
            app.ExecuteInteractive();
        }
    }
}

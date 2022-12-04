using System;
using CoreCommandLine.Example.Di.Commands;
using CoreCommandLine.Example.Di.Contracts;
using CoreCommandLine.Example.Di.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.LightWeight;

namespace CoreCommandLine.Example.Di
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddTransient<IUpperCaseService, UpperCaseService>();
            services.AddTransient<IAddDateService, AddDateService>();
            services.AddTransient<Echo>();
            
            
            var provider = services.BuildServiceProvider();

            var app = new Application();

            app.UseDotnetResolver(provider);

            app.Logger = new ConsoleLogger();

            app.ExecuteInteractive();
                        
        }
    }
}

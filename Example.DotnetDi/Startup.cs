using System;
using CoreCommandLine.Di;
using Example.DotnetDi.Commands;
using Example.DotnetDi.Contracts;
using Example.DotnetDi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Example.DotnetDi
{
    public class Startup
    {



        protected void ConfigureServices(IServiceCollection services,ILogger logger)
        {
            services.AddTransient<IUpperCaseService, UpperCaseService>();
            services.AddTransient<IAddDateService, AddDateService>();
            services.AddTransient<Echo>();
        }
        
        
        protected void ConfigureServices(IServiceProvider provider,ILogger logger)
        {
            logger.LogInformation("I Have a provider: {Provider} " +
                                  " but nothing configure with it!",provider);

        }
    }
}
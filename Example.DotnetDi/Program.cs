
using CoreCommandLine.DotnetDi;
using Example.DotnetDi;
using Example.DotnetDi.Commands;
using Example.DotnetDi.Contracts;
using Example.DotnetDi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

var builder = new DotnetCommandlineApplicationBuilder<Application>()
    .UseLogger(new ConsoleLogger());


builder.Services.AddTransient<IUpperCaseService, UpperCaseService>();
builder.Services.AddTransient<IAddDateService, AddDateService>();
builder.Services.AddTransient<Echo>();

var app = builder.Build();
    
app.Logger.LogInformation("I Have a provider: {Provider} " +
                                 " but nothing configure with it!",app.Resolver.Services);    
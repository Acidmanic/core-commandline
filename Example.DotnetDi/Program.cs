using CoreCommandLine;
using Example.DotnetDi.Commands;
using Example.DotnetDi.Contracts;
using Example.DotnetDi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;

var builder = new ConsoleApplicationBuilder()
    .UseDotnetResolver()
    .Describe("Di Example", "This application represents di usage.")
    .UseLogger(new ConsoleLogger());


builder.Services.AddTransient<IUpperCaseService, UpperCaseService>();
builder.Services.AddTransient<IAddDateService, AddDateService>();
builder.Services.AddTransient<Echo>();

var app = builder.Build();

app.Logger.LogInformation("I Have a provider: {Provider} " +
                          " but nothing configure with it!", app.Resolver.Services);

app.ExecuteInteractive();
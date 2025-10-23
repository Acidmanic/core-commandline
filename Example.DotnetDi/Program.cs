using CoreCommandLine;
using Example.DotnetDi.Commands;
using Example.DotnetDi.Contracts;
using Example.DotnetDi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.LightWeight;

var builder = new ConsoleApplicationBuilder()
    .UseDotnetResolver()
    .Describe("Di Example", "This application represents di usage.")
    .UseLogger(new ConsoleLogger());


builder.Services.AddTransient<IUpperCaseService, UpperCaseService>();
builder.Services.AddTransient<IAddDateService, AddDateService>();
builder.Services.AddTransient<Echo>();
builder.Services.AddTransient<Hello>();

var app = builder.Build();

await app.ExecuteInteractive(CancellationToken.None);
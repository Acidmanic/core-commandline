using CoreCommandLine;
using Example.Di;
using Example.Di.Commands;
using Example.Di.Contracts;
using Example.Di.Services;
using Example.Di.SimpleThirdpartyDiExample;
using Microsoft.Extensions.Logging.LightWeight;


var thirdPartyDi = new SillyDi();

thirdPartyDi.AddTransient<IUpperCaseService, UpperCaseService>();
thirdPartyDi.AddTransient<IAddDateService, AddDateService>();
thirdPartyDi.AddTransient<Echo>();

var provider = new SillyDiResolver(thirdPartyDi);


var builder = new ConsoleApplicationBuilder();

builder
    .Describe("Di Example", "This application represents di usage.")
    .UseResolver(provider)
    .UseLogger(new ConsoleLogger());

var app = builder.Build();

await app.Execute(["echo","Hey!"],CancellationToken.None);
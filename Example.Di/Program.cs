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

var app = new Application();

app.UseResolver(provider);

app.Logger = new ConsoleLogger();

app.ExecuteInteractive();
using CoreCommandLine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.LightWeight;


var builder = new ConsoleApplicationBuilder()
    .UseLogger(new ConsoleLogger())
    .Describe("Example hello world application", "\tType --help or -h for help \n\tType exit for exit.")
    .BeforeCommandExecutes(a => a.Application.Logger.LogInformation($"-------------- <{a.Command.GetCommandName().Value.Name}> -----------------"))
    .AfterCommandExecutes(a => a.Application.Logger.LogInformation($"-------------- </{a.Command.GetCommandName().Value.Name}> -----------------"));

var app = builder.Build();

app.ExecuteInteractive();
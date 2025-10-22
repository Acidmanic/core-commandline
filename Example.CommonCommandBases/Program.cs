using CoreCommandLine;
using Microsoft.Extensions.Logging.LightWeight;


var app = new ConsoleApplicationBuilder()
    .Describe("Example", "3 Common Commands")
    .UseLogger(new ConsoleLogger().Shorten())
    .Build();

await app.ExecuteInteractive(CancellationToken.None);
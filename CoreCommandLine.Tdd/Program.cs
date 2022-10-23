using  System;
using Microsoft.Extensions.Logging;


namespace CoreCommandLine.Tdd
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new ExampleApplication
            {
                ApplicationTitle = "Example hello world application",
                ApplicationDescription = "\tType --help or -h for help \n\tType exit for exit."
            };

            app.Output = s => app.Logger.LogInformation("\n{Text}",s);
            
            app.ExecuteInteractive();
        }
    }
}

using  System;
using Microsoft.Extensions.Logging;


namespace CoreCommandLine.Tdd
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new ExampleApplication();

            app.Output = s => app.Logger.LogInformation("\n{Text}",s);
            
            app.ExecuteInteractive();
        }
    }
}

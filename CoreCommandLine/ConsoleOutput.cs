using System;
using Microsoft.Extensions.Logging.LightWeight;

namespace CoreCommandLine
{
    public class ConsoleOutput:LoggerAdapter
    {
        public ConsoleOutput():base(Console.WriteLine)
        {
            
        }
    }
}
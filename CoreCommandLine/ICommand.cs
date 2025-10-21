using System;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    public interface ICommand
    {
        
        bool Execute(Context context, string[] args);
        
        string Description { get; }

        void SetLogger(ILogger logger);
        
    }
}

using System;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    public interface ICommand
    {
        /// <returns>Number of consumed arguments</returns>
        int Execute(Context context, string[] args);
        
        /// <returns>Number of consumed arguments</returns>
        Task<int> Execute(Context context, string[] args,CancellationToken cancellationToken);

        string Description { get; }

        void SetLogger(ILogger logger);
    }
}
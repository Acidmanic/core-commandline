using System;
using Microsoft.Extensions.Logging;

namespace CoreCommandLine
{
    public interface ICommand
    {
        /// <returns>Number of consumed arguments</returns>
        int Execute(Context context, string[] args);

        string Description { get; }

        void SetLogger(ILogger logger);
    }
}
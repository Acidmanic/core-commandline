using System;

namespace CoreCommandLine
{
    public interface ICommand
    {
        
        bool Execute(Context context, string[] args);
        
        string Name { get; }
        
        string ShortName { get; }
        
        string Description { get; }
    }
}

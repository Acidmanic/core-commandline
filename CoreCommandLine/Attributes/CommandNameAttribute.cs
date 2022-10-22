using System;

namespace CoreCommandLine.Attributes
{
    public class CommandNameAttribute:Attribute
    {
        
        public CommandNameAttribute(string name):this(name,name){ }

        public CommandNameAttribute(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
        }

        public string Name { get; }
        
        public string ShortName { get; }
        
        
    }
}
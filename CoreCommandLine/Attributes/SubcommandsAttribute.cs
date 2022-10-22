using System;
using System.Collections.Generic;

namespace CoreCommandLine.Attributes
{
    public class SubcommandsAttribute:Attribute
    {
        public List<Type> Children { get; } = new List<Type>();

        public SubcommandsAttribute(params Type[] children)
        {
            Children.AddRange(children);
        }
    }
}
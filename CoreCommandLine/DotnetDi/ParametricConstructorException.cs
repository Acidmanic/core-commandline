using System;

namespace CoreCommandLine.DotnetDi
{
    public class ParametricConstructorException:Exception
    {
        public ParametricConstructorException(string name):base(name + " Must have a non-parametric constructor.")
        {
        }
    }
}
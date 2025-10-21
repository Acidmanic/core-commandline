using System;

namespace CoreCommandLine.Di
{
    public class NullResolver:IResolver
    {
        public object Resolve(Type type)
        {
            return null;
        }

        public IServiceProvider Services => this.ToServiceProvider();
    }
}
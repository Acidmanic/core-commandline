using System;

namespace CoreCommandLine.Di
{
    public class DotnetServiceProviderResolver:IResolver
    {

        private readonly IServiceProvider _serviceProvider;

        public DotnetServiceProviderResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? Resolve(Type type)
        {
            try
            {
                return _serviceProvider.GetService(type);
                
            }
            catch  { /* ignore */ }

            return null;
        }

        public IServiceProvider Services => _serviceProvider;
    }
}
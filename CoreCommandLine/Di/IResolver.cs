using System;

namespace CoreCommandLine.Di
{
    public interface IResolver
    {

        object? Resolve(Type type);
        
        public IServiceProvider Services { get; }
    }


    internal class ServiceProviderResolver : IServiceProvider
    {
        private readonly IResolver _resolver;

        public ServiceProviderResolver(IResolver resolver)
        {
            _resolver = resolver;
        }

        public object? GetService(Type serviceType)
        {
            return _resolver.Resolve(serviceType);
        }
    }
    
    
    public static class ResolverDotnetDiExtensions
    {
        public static IServiceProvider ToServiceProvider(this IResolver resolver) =>  new ServiceProviderResolver(resolver);
    }
}


using System;
using System.Collections.Generic;
using CoreCommandLine.Di;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CoreCommandLine.DotnetDi
{
    public class DotnetCommandlineApplicationBuilder<TApplication> where TApplication : CommandLineApplication, new()
    {
        private readonly List<Type> _startupTypes;
        private ILogger _logger;
        private string _description=null;
        private string _title=null;

        public DotnetCommandlineApplicationBuilder()
        {
            _startupTypes = new List<Type>();
            _logger = NullLogger.Instance;
        }

        /// <summary>
        /// Marks the given type as the only startup class where optional configuration methods are expected to be found.
        /// This will remove any other startup and only uses this one.
        /// </summary>
        /// <typeparam name="TStartup">Represents the type of the startup class which implements the configuration methods.</typeparam>
        /// <returns>The current builder.</returns>
        public DotnetCommandlineApplicationBuilder<TApplication> UseStartup<TStartup>() where TStartup : class
        {
            _startupTypes.Clear();
            _startupTypes.Add(typeof(TStartup));

            return this;
        }

        /// <summary>
        /// Adds the given type to the startup classes, where optional configuration methods are expected to be found.
        /// With this method it's possible to use more than one startup class and they all will be configured in the
        /// order they have been added. 
        /// </summary>
        /// <typeparam name="TStartup">Represents the type of the startup class which implements the configuration methods.</typeparam>
        /// <returns>The current builder.</returns>
        public DotnetCommandlineApplicationBuilder<TApplication> AddStartup<TStartup>() where TStartup : class
        {
            _startupTypes.Add(typeof(TStartup));

            return this;
        }

        /// <summary>
        /// Builds an instance of Commandline application that can be started.
        /// </summary>
        /// <returns></returns>
        public CommandLineApplication Build()
        {
            var invokers = CreateMethodInvokers();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_logger);

            invokers.ForEach(i => i.InvokeSatisfiedMethods<IServiceCollection, ILogger>(serviceCollection, _logger));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var resolver = new DotnetServiceProviderResolver(serviceProvider);

            invokers.ForEach(i =>
                i.InvokeSatisfiedMethods<IServiceProvider, IResolver, ILogger>(serviceProvider, resolver, _logger));

            var application = new TApplication();

            application.UseDotnetResolver(serviceProvider);

            application.Logger = _logger;

            application.ApplicationDescription = _description ?? application.ApplicationDescription;

            application.ApplicationTitle = _title ?? application.ApplicationTitle;

            application.Output = Console.WriteLine;

            return application;
        }

        private List<IMethodInvoker> CreateMethodInvokers()
        {
            var invokers = new List<IMethodInvoker>();

            foreach (var startupType in _startupTypes)
            {
                try
                {
                    var invoker = new MethodInvoker(startupType);

                    invoker.AddFilter(method =>
                    {
                        var lowerName = method.Name.ToLower();
                        return lowerName.StartsWith("configure") ||
                               lowerName.StartsWith("register");
                    });
                    
                    invokers.Add(invoker);
                }
                catch (Exception _)
                {
                }
            }

            return invokers;
        }


        public DotnetCommandlineApplicationBuilder<TApplication> Title(string title)
        {
            _title = title;

            return this;
        }

        public DotnetCommandlineApplicationBuilder<TApplication> Description(string description)
        {
            _description = description;

            return this;
        }

        public DotnetCommandlineApplicationBuilder<TApplication> Describe(string title, string description)
        {
            _description = description;

            _title = title;

            return this;
        }

        public DotnetCommandlineApplicationBuilder<TApplication> UseLogger(ILogger logger)
        {
            _logger = logger;

            return this;
        }
    }
}
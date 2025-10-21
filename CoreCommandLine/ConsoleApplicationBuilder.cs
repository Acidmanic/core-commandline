using System.Reflection;
using CoreCommandLine.Di;
using CoreCommandLine.DotnetDi;
using CoreCommandLine.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CoreCommandLine
{
    public class ConsoleApplicationBuilder
    {
        private enum Resolver
        {
            DotnetResolver=0,
            CustomResolver=1,
        }
        
        private ILogger _logger;
        private string _description = null;
        private string _title = null;
        private readonly ServiceCollectorProxy _services = new ServiceCollectorProxy();
        private IConfigurationBuilder configurationBuilder;
        private readonly List<Assembly> assemblies = [];

        private Action<ExecutionActionAssets> onBeforeExecute = _ => { };

        private Action<ExecutionActionAssets> onAfterExecute = _ => { };

        private Resolver resolver = Resolver.DotnetResolver;
        private IResolver? customResolver;
        
        public ConsoleApplicationBuilder()
        {
            Clear();
        }

        /// <summary>
        /// Allows introducing assemblies ouside of the entry applicatoin to be added manually, for example for when you
        /// have commands declared in external class library
        /// </summary>
        /// <param name="assembly">assembly instance to be added</param>
        /// <returns></returns>
        public ConsoleApplicationBuilder IncludeAssembly(Assembly assembly)
        {
            assemblies.Add(assembly);

            return this;
        }

        public ConsoleApplicationBuilder UseDotnetResolver()
        {
            resolver = Resolver.DotnetResolver;

            return this;
        }

        public ConsoleApplicationBuilder UseResolver(IResolver r)
        {
            resolver = Resolver.CustomResolver;
            customResolver = r;
            return this;
        }
        
        /// <summary>
        /// Builds an instance of Commandline application that can be started.
        /// </summary>
        /// <returns></returns>
        public CommandLineApplication Build()
        {
            var serviceCollection = _services.BuildServiceCollection();

            var configuration = configurationBuilder.Build();

            serviceCollection.AddSingleton(_logger);

            serviceCollection.AddSingleton(configuration);

            IResolver selectedResolver;
            
            if (resolver == Resolver.CustomResolver && customResolver is {} cr)
            {
                selectedResolver = cr;
            }
            else
            {
                var serviceProvider = serviceCollection.BuildServiceProvider();

                selectedResolver = new DotnetServiceProviderResolver(serviceProvider);
            }
            
            serviceCollection.AddTransient<IResolver>(sp => new DotnetServiceProviderResolver(sp));

            CommandInstantiator.Instance.UseResolver(selectedResolver);
            
            var application = new CommandLineApplication(assemblies);

            application.Logger = _logger;

            application.ApplicationDescription = _description ?? application.ApplicationDescription;

            application.ApplicationTitle = _title ?? application.ApplicationTitle;

            application.BeforeExecute = onBeforeExecute;
            application.AfterExecute = onAfterExecute;

            return application;
        }


        public ConsoleApplicationBuilder Title(string title)
        {
            _title = title;

            return this;
        }

        public ConsoleApplicationBuilder Description(string description)
        {
            _description = description;

            return this;
        }

        public ConsoleApplicationBuilder Describe(string title, string description)
        {
            _description = description;

            _title = title;

            return this;
        }

        public ConsoleApplicationBuilder UseLogger(ILogger logger)
        {
            _logger = logger;

            return this;
        }

        /// <summary>
        /// The action set here would be performed just before each command gets executed
        /// </summary>
        /// <param name="action">the action to be performed</param>
        /// <returns></returns>
        public ConsoleApplicationBuilder BeforeCommandExecutes(Action<ExecutionActionAssets> action)
        {
            onBeforeExecute = action;

            return this;
        }

        /// <summary>
        /// The action set here would be performed right after each command gets executed
        /// </summary>
        /// <param name="action">the action to be performed</param>
        /// <returns></returns>
        public ConsoleApplicationBuilder AfterCommandExecutes(Action<ExecutionActionAssets> action)
        {
            onAfterExecute = action;

            return this;
        }

        /// <summary>
        /// Resets the builder for creating new instances
        /// </summary>
        public void Clear()
        {
            initializeAssemblies();
            _services.Clear();
            createDefaultConfigurationBuilder();
            _logger = NullLogger.Instance;
            resolver = Resolver.DotnetResolver;
        }

        public IServiceCollection Services => _services;

        private void createDefaultConfigurationBuilder(string[]? args = null)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? []);

            if (environment is { } e)
            {
                var configurationFile = $"appsettings.{e}.json";

                if (File.Exists(configurationFile))
                {
                    configurationBuilder.AddJsonFile(configurationFile);
                }
            }
        }


        private void initializeAssemblies()
        {
            assemblies.Clear();

            assemblies.Add(Assembly.GetCallingAssembly());

            if (Assembly.GetEntryAssembly() is { } entryAssembly) assemblies.Add(entryAssembly);

            assemblies.Add(Assembly.GetExecutingAssembly());
        }

        public IConfigurationBuilder ConfigurationBuilder => configurationBuilder;
    }
}
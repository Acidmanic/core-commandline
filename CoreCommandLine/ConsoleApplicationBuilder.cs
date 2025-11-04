using System.Reflection;
using CoreCommandLine.Di;
using CoreCommandLine.DotnetDi;
using CoreCommandLine.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CoreCommandLine
{
    public class ConsoleApplicationBuilder
    {
        private enum Resolver
        {
            DotnetResolver = 0,
            CustomResolver = 1,
        }

        private ILogger _logger;
        private string? _description = null;
        private string? _title = null;

        private readonly List<Assembly> assemblies = [];

        private Action<ExecutionActionAssets> onBeforeExecute = _ => { };

        private Action<ExecutionActionAssets> onAfterExecute = _ => { };

        private Action<Context> onInitializeContext = _ => { };

        private Resolver resolverType = Resolver.DotnetResolver;

        private IResolver? customResolver;

        private ServiceCollection serviceCollection;

        private IConfiguration serviceConfigurationInterface;

        private readonly List<Action<IConfigurationBuilder>> configurationBuilderActions = [];

        public ConsoleApplicationBuilder(Action<IConfigurationBuilder>? configuration = null)
        {
            Clear(configuration);
        }

        /// <summary>
        /// Allows introducing assemblies outside the entry application to be added manually, for example for when you
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
            resolverType = Resolver.DotnetResolver;

            return this;
        }

        public ConsoleApplicationBuilder UseResolver(IResolver r)
        {
            resolverType = Resolver.CustomResolver;
            customResolver = r;
            return this;
        }

        /// <summary>
        /// Builds an instance of Commandline application that can be started.
        /// </summary>
        /// <returns></returns>
        public CommandLineApplication Build()
        {
            var hostBuilder = Host.CreateDefaultBuilder([]).ConfigureHostConfiguration(performAllConfigurationActions);

            serviceCollection.AddSingleton(_logger);

            serviceCollection.AddSingleton(serviceConfigurationInterface);

            var useDotnetDi = !(resolverType == Resolver.CustomResolver && customResolver != null);

            if (useDotnetDi)
            {
                hostBuilder.ConfigureServices((hostContext, services) =>
                {
                    foreach (var serviceDescriptor in serviceCollection)
                    {
                        services.Add(serviceDescriptor);
                    }

                    services.AddTransient<IResolver>(sp => new DotnetServiceProviderResolver(sp));
                });
            }

            var host = hostBuilder.Build();

            var selectedResolver = useDotnetDi ? new DotnetServiceProviderResolver(host.Services) : customResolver!;

            var application = new CommandLineApplication(assemblies, selectedResolver, host);

            application.Logger = _logger;

            application.ApplicationDescription = _description ?? application.ApplicationDescription;

            application.ApplicationTitle = _title ?? application.ApplicationTitle;

            application.BeforeExecute = onBeforeExecute;
            application.AfterExecute = onAfterExecute;
            application.InitializeContext = onInitializeContext;

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
        /// The action set here would be performed when ever application re-initializes it's context which includes inter-commands data
        /// </summary>
        /// <param name="action">the action to be performed</param>
        /// <returns></returns>
        public ConsoleApplicationBuilder InitializeContext(Action<Context> action)
        {
            onInitializeContext = action;

            return this;
        }

        /// <summary>
        /// Resets the builder for creating new instances
        /// </summary>
        public void Clear(Action<IConfigurationBuilder>? configureConfigurations)
        {
            initializeAssemblies();
            configurationBuilderActions.Clear();
            if (configureConfigurations is { } c) configurationBuilderActions.Add(c);
            configurationBuilderActions.Add(defaultAppSettingsConfigurations);
            _logger = NullLogger.Instance;
            resolverType = Resolver.DotnetResolver;
            serviceCollection ??= new();


            var configurationBuilder = new ConfigurationBuilder();
            performAllConfigurationActions(configurationBuilder);
            serviceConfigurationInterface = configurationBuilder.Build();
        }

        public IServiceCollection Services => serviceCollection;

        private void performAllConfigurationActions(IConfigurationBuilder builder)
        {
            configurationBuilderActions.ForEach(a => a(builder));
        }

        private void defaultAppSettingsConfigurations(IConfigurationBuilder configurationBuilder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            configurationBuilder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

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

        public IConfiguration Configuration => serviceConfigurationInterface;
    }
}
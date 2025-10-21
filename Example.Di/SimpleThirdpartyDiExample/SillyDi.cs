using System.Collections.Immutable;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Example.Di.SimpleThirdpartyDiExample;

public class SillyDi
{
    private readonly Dictionary<Type, Func<SillyDi, object?>> instanciators = new();
    private readonly Dictionary<Type, Type> implementationMapped = new();


    public void AddTransient(Type interfaceType, Func<SillyDi, object?> factory)
    {
        instanciators[interfaceType] = factory;
    }

    public void AddTransient(Type interfaceType, Type implementationType)
    {
        implementationMapped[interfaceType] = implementationType;
    }

    public void AddTransient(Type type)
    {
        implementationMapped[type] = type;
    }
    
    
    public void AddTransient<T>(Func<SillyDi, object?> factory) => AddTransient(typeof(T),factory);

    public void AddTransient<TAbstraction,TImplementation>() => AddTransient(typeof(TAbstraction), typeof(TImplementation));

    public void AddTransient<T>() => AddTransient(typeof(T));

    
    public object? Resolve(Type type)
    {
        if (instanciators.TryGetValue(type, out var instanciator))
        {
            return instanciator(this);
        }

        var implementation = type;

        if (implementationMapped.ContainsKey(type)) implementation = implementationMapped[type];

        var constructors = implementation.GetConstructors().OrderBy(c => c.GetParameters().Length).ToArray();

        foreach (var constructor in constructors)
        {
            var instance = ResolveConstructor(constructor);

            if (instance is { } resolved) return resolved;
        }

        return null;
    }


    private object? ResolveConstructor(ConstructorInfo constructor)
    {
        var parameters = constructor.GetParameters();

        var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

        var resolvedParameters = parameterTypes.Select(Resolve).ToArray();

        if (resolvedParameters.Any(p => p is null)) return null;

        var parameterInstances = resolvedParameters.Select(p => p!).ToArray();

        try
        {
            var instance = constructor.Invoke(parameterInstances);

            return instance;
        }
        catch
        {
            /* ignore */
        }

        return null;
    }
}
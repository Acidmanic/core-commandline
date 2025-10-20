using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CoreCommandLine.DotnetDi;

internal class ServiceCollectorProxy:IServiceCollection
{
    private readonly List<ServiceDescriptor> descriptors = new();
    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        return descriptors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(ServiceDescriptor item)
    {
        descriptors.Add(item);
    }

    public void Clear()
    {
        descriptors.Clear();
    }

    public bool Contains(ServiceDescriptor item)
    {
        return descriptors.Contains(item);
    }

    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        descriptors.CopyTo(array, arrayIndex);
    }

    public bool Remove(ServiceDescriptor item)
    {
        return descriptors.Remove(item);
        
    }

    public int Count => descriptors.Count;
    public bool IsReadOnly => false;
    
    public int IndexOf(ServiceDescriptor item) => descriptors.IndexOf(item);

    public void Insert(int index, ServiceDescriptor item)
    {
        descriptors.Insert(index,item);
    }

    public void RemoveAt(int index)
    {
        descriptors.RemoveAt(index);
    }

    public ServiceDescriptor this[int index]
    {
        get => descriptors[index];
        set => descriptors[index] = value;
    }

    public ServiceCollection BuildServiceCollection()
    {
        var services = new ServiceCollection();
        
        descriptors.ForEach(d => services.Add(d));
        
        return services;
    }
}
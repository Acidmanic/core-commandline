using CoreCommandLine.Di;
using Example.Di.SimpleThirdpartyDiExample;

namespace Example.Di;

public class SillyDiResolver:IResolver
{
    private readonly SillyDi _sillyDi;

    public SillyDiResolver(SillyDi sillyDi)
    {
        _sillyDi = sillyDi;
    }

    public object? Resolve(Type type)=> _sillyDi.Resolve(type);

    public IServiceProvider Services => this.ToServiceProvider();
}
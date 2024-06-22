using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

namespace Frank.SimpleInstaller.Cli.DependencyInjection;

public class TypeResolver : ITypeResolver, IDisposable
{
    private readonly ServiceProvider _provider;

    public TypeResolver(ServiceProvider provider)
    {
        _provider = provider;
    }

    public object Resolve(Type type)
    {
        return _provider.GetRequiredService(type);
    }

    public void Dispose()
    {
        _provider.Dispose();
    }
}
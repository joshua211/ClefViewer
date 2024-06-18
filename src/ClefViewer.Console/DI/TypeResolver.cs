using Spectre.Console.Cli;

namespace ClefViewer.Console.DI;

/// <summary>
///     SRC:
///     https://github.com/spectreconsole/spectre.console/blob/main/examples/Cli/Injection/Infrastructure/TypeResolver.cs
/// </summary>
public sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public void Dispose()
    {
        if (_provider is IDisposable disposable) disposable.Dispose();
    }

    public object Resolve(Type type)
    {
        if (type == null) return null;

        return _provider.GetService(type);
    }
}
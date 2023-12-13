namespace ClefViewer.Core.Models;

public interface ICanUnwrap
{
    IReadOnlyCollection<WrappedPrimitive> Unwrap();
}
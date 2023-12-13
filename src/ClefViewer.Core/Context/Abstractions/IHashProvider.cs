namespace ClefViewer.Core.Context.Abstractions;

public interface IHashProvider
{
    string ComputeHash(string input);
}
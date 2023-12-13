namespace ClefViewer.Core.Models;

public class TextException : Exception
{
    private readonly string text;

    public TextException(string text)
    {
        this.text = text;
    }

    public override string ToString()
    {
        return text;
    }
}
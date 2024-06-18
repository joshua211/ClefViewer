using System.Security.Cryptography;
using System.Text;
using ClefViewer.Core.Context.Abstractions;

namespace ClefViewer.Core.Context;

public class HashProvider : IHashProvider
{
    private static readonly HashAlgorithm hashAlgorithm = SHA256.Create();

    public string ComputeHash(string input)
    {
        var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sBuilder = new StringBuilder();

        for (var i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
}
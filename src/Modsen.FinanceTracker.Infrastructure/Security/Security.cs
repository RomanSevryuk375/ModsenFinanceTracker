using System.Security.Cryptography;
using System.Text;

namespace Modsen.FinanceTracker.Infrastructure.Security;

public static class PasswordHasher
{
    public static string ComputeSha256Hash(string rawData)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));

        StringBuilder builder = new();
        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
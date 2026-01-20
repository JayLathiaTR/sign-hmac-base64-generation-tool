using System.Security.Cryptography;
using System.Text;

namespace SignHmacTutorial.Services;

public static class HmacSigner
{
    public static string ComputeHmacBase64(string secretKey, string rawBody)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
        byte[] messageBytes = Encoding.UTF8.GetBytes(rawBody);

        using var hmac = new HMACSHA256(keyBytes);
        byte[] hashBytes = hmac.ComputeHash(messageBytes);
        return Convert.ToBase64String(hashBytes);
    }
}

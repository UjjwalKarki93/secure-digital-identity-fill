using System.Security.Cryptography;
using System.Text;
using Banking.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Banking.Infrastructure.Security;

public class HmacService : IHmacService
{
    private readonly byte[] _keyBytes;

    public HmacService(IConfiguration configuration)
    {
        var secret = configuration["Security:HmacSecretKey"]
            ?? throw new InvalidOperationException("HMAC secret key is not configured.");
        _keyBytes = Encoding.UTF8.GetBytes(secret);
    }

    public string Sign(string payload)
    {
        using var hmac = new HMACSHA256(_keyBytes);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }

    public bool Verify(string payload, string signature)
    {
        var expected = Sign(payload);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(signature));
    }
}

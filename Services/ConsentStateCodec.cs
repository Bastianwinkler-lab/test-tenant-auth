using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TestTenantAuth.Models;

namespace TestTenantAuth.Services;

public static class ConsentStateCodec
{
    public static string Encode(string tenantId)
    {
        var payload = new ConsentStatePayload
        {
            TenantId = tenantId,
            TimestampUtc = DateTimeOffset.UtcNow,
            Nonce = Convert.ToHexString(RandomNumberGenerator.GetBytes(12)).ToLowerInvariant()
        };

        var json = JsonSerializer.Serialize(payload);
        return Base64UrlEncode(Encoding.UTF8.GetBytes(json));
    }

    public static ConsentStatePayload? Decode(string? encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded))
        {
            return null;
        }

        try
        {
            var bytes = Base64UrlDecode(encoded);
            return JsonSerializer.Deserialize<ConsentStatePayload>(bytes);
        }
        catch
        {
            return null;
        }
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
        return Convert.FromBase64String(padded);
    }
}

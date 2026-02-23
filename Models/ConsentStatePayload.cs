namespace TestTenantAuth.Models;

public sealed class ConsentStatePayload
{
    public string TenantId { get; set; } = string.Empty;
    public DateTimeOffset TimestampUtc { get; set; }
    public string Nonce { get; set; } = string.Empty;
}

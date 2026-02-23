namespace TestTenantAuth.Models;

public sealed class TenantRecord
{
    public string TenantId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public bool ConsentGranted { get; set; }
    public DateTimeOffset? ConsentGrantedAt { get; set; }
}

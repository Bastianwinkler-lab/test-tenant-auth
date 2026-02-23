namespace TestTenantAuth.Models;

public sealed class TenantCustomer
{
    public string TenantId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}

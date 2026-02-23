using TestTenantAuth.Models;

namespace TestTenantAuth.Services;

public interface ITenantStore
{
    IReadOnlyCollection<TenantRecord> GetAll();
    TenantRecord? GetByTenantId(string tenantId);
    void Add(TenantRecord record);
    void Update(TenantRecord record);
    void Delete(string tenantId);
    void MarkConsentGranted(string tenantId, DateTimeOffset grantedAt);
}

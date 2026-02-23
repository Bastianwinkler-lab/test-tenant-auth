using TestTenantAuth.Models;

namespace TestTenantAuth.Services;

public interface ITenantCustomerStore
{
    IReadOnlyCollection<TenantCustomer> GetAll();
    IReadOnlyCollection<TenantCustomer> GetByTenant(string tenantId);
    void Add(TenantCustomer item);
}

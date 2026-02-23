using TestTenantAuth.Models;

namespace TestTenantAuth.Services;

public sealed class InMemoryTenantCustomerStore : ITenantCustomerStore
{
    private readonly List<TenantCustomer> _items =
    [
        new() { TenantId = "11111111-1111-1111-1111-111111111111", CustomerName = "Musterkunde A" },
        new() { TenantId = "22222222-2222-2222-2222-222222222222", CustomerName = "Musterkunde B" }
    ];

    private readonly object _sync = new();

    public IReadOnlyCollection<TenantCustomer> GetAll()
    {
        lock (_sync)
        {
            return _items
                .Select(x => new TenantCustomer { TenantId = x.TenantId, CustomerName = x.CustomerName })
                .ToList();
        }
    }

    public IReadOnlyCollection<TenantCustomer> GetByTenant(string tenantId)
    {
        lock (_sync)
        {
            return _items
                .Where(x => string.Equals(x.TenantId, tenantId, StringComparison.OrdinalIgnoreCase))
                .Select(x => new TenantCustomer { TenantId = x.TenantId, CustomerName = x.CustomerName })
                .ToList();
        }
    }

    public void Add(TenantCustomer item)
    {
        lock (_sync)
        {
            _items.Add(new TenantCustomer { TenantId = item.TenantId, CustomerName = item.CustomerName });
        }
    }
}

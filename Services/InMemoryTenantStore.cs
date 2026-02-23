using System.Collections.Concurrent;
using TestTenantAuth.Models;

namespace TestTenantAuth.Services;

public sealed class InMemoryTenantStore : ITenantStore
{
    private readonly ConcurrentDictionary<string, TenantRecord> _records = new(StringComparer.OrdinalIgnoreCase);

    public InMemoryTenantStore()
    {
        Add(new TenantRecord { TenantId = "11111111-1111-1111-1111-111111111111", CustomerName = "Contoso GmbH" });
        Add(new TenantRecord { TenantId = "22222222-2222-2222-2222-222222222222", CustomerName = "Fabrikam AG" });
    }

    public IReadOnlyCollection<TenantRecord> GetAll() =>
        _records.Values
            .OrderBy(x => x.CustomerName)
            .Select(Clone)
            .ToList();

    public TenantRecord? GetByTenantId(string tenantId)
    {
        if (_records.TryGetValue(tenantId, out var found))
        {
            return Clone(found);
        }

        return null;
    }

    public void Add(TenantRecord record)
    {
        var normalized = Normalize(record);
        _records[normalized.TenantId] = normalized;
    }

    public void Update(TenantRecord record)
    {
        var normalized = Normalize(record);
        _records.AddOrUpdate(normalized.TenantId, normalized, (_, existing) =>
        {
            existing.CustomerName = normalized.CustomerName;
            return existing;
        });
    }

    public void Delete(string tenantId)
    {
        _records.TryRemove(tenantId, out _);
    }

    public void MarkConsentGranted(string tenantId, DateTimeOffset grantedAt)
    {
        _records.AddOrUpdate(
            tenantId,
            _ => new TenantRecord
            {
                TenantId = tenantId,
                CustomerName = "(Unbekannter Tenant)"
            },
            (_, existing) => existing);

        _records.AddOrUpdate(
            tenantId,
            _ => new TenantRecord
            {
                TenantId = tenantId,
                CustomerName = "(Unbekannter Tenant)",
                ConsentGranted = true,
                ConsentGrantedAt = grantedAt
            },
            (_, existing) =>
            {
                existing.ConsentGranted = true;
                existing.ConsentGrantedAt = grantedAt;
                return existing;
            });
    }

    private static TenantRecord Normalize(TenantRecord record) => new()
    {
        TenantId = record.TenantId.Trim(),
        CustomerName = record.CustomerName.Trim(),
        ConsentGranted = record.ConsentGranted,
        ConsentGrantedAt = record.ConsentGrantedAt
    };

    private static TenantRecord Clone(TenantRecord record) => new()
    {
        TenantId = record.TenantId,
        CustomerName = record.CustomerName,
        ConsentGranted = record.ConsentGranted,
        ConsentGrantedAt = record.ConsentGrantedAt
    };
}

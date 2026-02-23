using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestTenantAuth.Services;

namespace TestTenantAuth.Pages;

[Authorize]
public sealed class TestModel(ITenantStore tenantStore) : PageModel
{
    public string DisplayName { get; set; } = "-";
    public string TenantId { get; set; } = "-";
    public string ObjectId { get; set; } = "-";
    public string Issuer { get; set; } = "-";

    public bool TenantKnown { get; set; }
    public string? CustomerName { get; set; }

    public void OnGet()
    {
        DisplayName = User.FindFirstValue("name")
            ?? User.FindFirstValue("preferred_username")
            ?? "-";

        TenantId = User.FindFirstValue("tid") ?? "-";
        ObjectId = User.FindFirstValue("oid") ?? "-";
        Issuer = User.FindFirstValue("iss") ?? "-";

        if (TenantId != "-")
        {
            var tenant = tenantStore.GetByTenantId(TenantId);
            TenantKnown = tenant is not null;
            CustomerName = tenant?.CustomerName;
        }
    }
}

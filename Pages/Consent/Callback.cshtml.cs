using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestTenantAuth.Services;

namespace TestTenantAuth.Pages.Consent;

public sealed class CallbackModel(ITenantStore tenantStore) : PageModel
{
    public string AdminConsent { get; set; } = "-";
    public string TenantId { get; set; } = "-";
    public string Error { get; set; } = "-";
    public string ErrorDescription { get; set; } = "-";
    public bool ConsentSucceeded { get; set; }

    public void OnGet([FromQuery(Name = "admin_consent")] string? adminConsent,
        [FromQuery(Name = "tenant")] string? tenant,
        [FromQuery(Name = "error")] string? error,
        [FromQuery(Name = "error_description")] string? errorDescription,
        [FromQuery(Name = "state")] string? state)
    {
        AdminConsent = adminConsent ?? "-";
        Error = error ?? "-";
        ErrorDescription = errorDescription ?? "-";

        var decoded = ConsentStateCodec.Decode(state);
        TenantId = tenant ?? decoded?.TenantId ?? "-";

        if (string.Equals(adminConsent, "true", StringComparison.OrdinalIgnoreCase) && TenantId != "-")
        {
            tenantStore.MarkConsentGranted(TenantId, DateTimeOffset.UtcNow);
            ConsentSucceeded = true;
        }
    }
}

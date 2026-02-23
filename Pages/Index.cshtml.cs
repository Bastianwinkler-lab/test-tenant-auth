using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestTenantAuth.Models;
using TestTenantAuth.Services;

namespace TestTenantAuth.Pages;

public sealed class IndexModel(ITenantStore tenantStore, IConfiguration configuration) : PageModel
{
    [BindProperty]
    public TenantForm Form { get; set; } = new();

    public bool IsEditMode { get; set; }
    public IReadOnlyCollection<TenantRecord> Records { get; set; } = [];
    public string? ErrorMessage { get; set; }

    public void OnGet(string? error = null)
    {
        Load(error);
    }

    public IActionResult OnPostSave()
    {
        if (!ModelState.IsValid)
        {
            Load();
            return Page();
        }

        if (tenantStore.GetByTenantId(Form.TenantId) is null)
        {
            tenantStore.Add(new TenantRecord { TenantId = Form.TenantId, CustomerName = Form.CustomerName });
        }
        else
        {
            tenantStore.Update(new TenantRecord { TenantId = Form.TenantId, CustomerName = Form.CustomerName });
        }

        return RedirectToPage();
    }

    public IActionResult OnPostEdit(string tenantId)
    {
        var existing = tenantStore.GetByTenantId(tenantId);
        if (existing is null)
        {
            return RedirectToPage();
        }

        Form = new TenantForm
        {
            TenantId = existing.TenantId,
            CustomerName = existing.CustomerName
        };
        IsEditMode = true;
        Records = tenantStore.GetAll();
        return Page();
    }

    public IActionResult OnPostDelete(string tenantId)
    {
        tenantStore.Delete(tenantId);
        return RedirectToPage();
    }

    public string BuildAdminConsentLink(string tenantId)
    {
        var clientId = configuration["EntraId:ClientId"] ?? string.Empty;
        var redirectUri = $"{Request.Scheme}://{Request.Host}/consent/callback";
        var state = ConsentStateCodec.Encode(tenantId);

        return $"https://login.microsoftonline.com/{Uri.EscapeDataString(tenantId)}/v2.0/adminconsent?client_id={Uri.EscapeDataString(clientId)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&state={Uri.EscapeDataString(state)}";
    }

    private void Load(string? error = null)
    {
        Records = tenantStore.GetAll();
        ErrorMessage = error switch
        {
            "tenant_not_onboarded" => "Anmeldung blockiert: Tenant ist nicht onboarded.",
            _ => null
        };
    }

    public sealed class TenantForm
    {
        [Required]
        [Display(Name = "TenantId (GUID)")]
        public string TenantId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "CustomerName")]
        public string CustomerName { get; set; } = string.Empty;
    }
}

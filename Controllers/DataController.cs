using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestTenantAuth.Services;
using TestTenantAuth.ViewModels;

namespace TestTenantAuth.Controllers;

[AllowAnonymous]
public class DataController(ITenantCustomerStore store, IConfiguration configuration) : Controller
{
    [HttpGet("/data")]
    public IActionResult Index()
    {
        return View(BuildViewModel());
    }

    [HttpPost("/data")]
    [ValidateAntiForgeryToken]
    public IActionResult Index(DataPageViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var invalidModel = BuildViewModel();
            invalidModel.TenantId = model.TenantId;
            invalidModel.CustomerName = model.CustomerName;
            return View(invalidModel);
        }

        store.Add(new Models.TenantCustomer
        {
            TenantId = model.TenantId,
            CustomerName = model.CustomerName
        });

        TempData["Message"] = "Datensatz erfolgreich angelegt.";
        return RedirectToAction(nameof(Index));
    }

    private DataPageViewModel BuildViewModel()
    {
        var azureAd = configuration.GetSection("AzureAd");
        var instance = (azureAd["Instance"] ?? "https://login.microsoftonline.com/").TrimEnd('/');
        var clientId = azureAd["ClientId"] ?? string.Empty;
        var redirectUri = configuration["Consent:RedirectUri"] ?? "https://localhost:5001/signin-oidc";

        var consentUrl = $"{instance}/common/adminconsent?client_id={Uri.EscapeDataString(clientId)}&redirect_uri={Uri.EscapeDataString(redirectUri)}";

        return new DataPageViewModel
        {
            ExistingItems = store.GetAll(),
            ConsentUrl = consentUrl
        };
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestTenantAuth.Services;

namespace TestTenantAuth.Controllers;

[Authorize]
public class HomeController(ITenantCustomerStore store) : Controller
{
    [HttpGet("/")]
    [AllowAnonymous]
    public IActionResult Root() => RedirectToAction(nameof(Index));

    [HttpGet("/index")]
    public IActionResult Index()
    {
        var tenantId = User.FindFirstValue("tid") ?? User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return View(new IndexViewModel
            {
                TenantId = "(keine TenantID im Token gefunden)",
                Customers = []
            });
        }

        return View(new IndexViewModel
        {
            TenantId = tenantId,
            Customers = store.GetByTenant(tenantId)
        });
    }

    [AllowAnonymous]
    [HttpGet("/signin")]
    public IActionResult SignIn() => Challenge();

    [HttpGet("/signout")]
    public IActionResult SignOutApp() => SignOut("Cookies", "OpenIdConnect");
}

public sealed class IndexViewModel
{
    public string TenantId { get; set; } = string.Empty;
    public IReadOnlyCollection<Models.TenantCustomer> Customers { get; set; } = [];
}

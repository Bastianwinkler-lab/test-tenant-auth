using System.ComponentModel.DataAnnotations;

namespace TestTenantAuth.Options;

public sealed class ConsentOptions
{
    [Required]
    public string RedirectUri { get; set; } = "https://localhost:5001/signin-oidc";
}

using System.ComponentModel.DataAnnotations;

namespace TestTenantAuth.Options;

public sealed class AzureAdOptions
{
    [Required]
    public string Instance { get; set; } = "https://login.microsoftonline.com/";

    [Required]
    public string TenantId { get; set; } = "common";

    [Required]
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    [Required]
    public string CallbackPath { get; set; } = "/signin-oidc";
}

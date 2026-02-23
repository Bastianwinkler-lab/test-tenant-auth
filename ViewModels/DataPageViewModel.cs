using System.ComponentModel.DataAnnotations;
using TestTenantAuth.Models;

namespace TestTenantAuth.ViewModels;

public sealed class DataPageViewModel
{
    [Required]
    [Display(Name = "TenantID")]
    public string TenantId { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Kundenname")]
    public string CustomerName { get; set; } = string.Empty;

    public string ConsentUrl { get; set; } = string.Empty;

    public IReadOnlyCollection<TenantCustomer> ExistingItems { get; set; } = [];
}

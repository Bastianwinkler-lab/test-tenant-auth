using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using TestTenantAuth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ITenantCustomerStore, InMemoryTenantCustomerStore>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        var azureAd = builder.Configuration.GetSection("AzureAd");
        var instance = azureAd["Instance"] ?? "https://login.microsoftonline.com/";
        var tenantId = azureAd["TenantId"] ?? "common";

        options.Authority = $"{instance.TrimEnd('/')}/{tenantId}/v2.0";
        options.ClientId = azureAd["ClientId"] ?? string.Empty;
        options.ClientSecret = azureAd["ClientSecret"] ?? string.Empty;
        options.CallbackPath = azureAd["CallbackPath"] ?? "/signin-oidc";

        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;

        options.TokenValidationParameters.ValidateIssuer = false;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using TestTenantAuth.Options;
using TestTenantAuth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ITenantCustomerStore, InMemoryTenantCustomerStore>();

builder.Services
    .AddOptions<AzureAdOptions>()
    .Bind(builder.Configuration.GetSection("AzureAd"))
    .ValidateDataAnnotations()
    .Validate(options => !string.Equals(options.ClientId, "YOUR-CLIENT-ID", StringComparison.OrdinalIgnoreCase), "AzureAd:ClientId muss auf die echte App-Registrierung gesetzt werden.")
    .ValidateOnStart();

builder.Services
    .AddOptions<ConsentOptions>()
    .Bind(builder.Configuration.GetSection("Consent"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, (options, provider) =>
    {
        var azureAd = provider.GetRequiredService<IOptions<AzureAdOptions>>().Value;

        options.Authority = $"{azureAd.Instance.TrimEnd('/')}/{azureAd.TenantId}/v2.0";
        options.ClientId = azureAd.ClientId;
        options.ClientSecret = azureAd.ClientSecret;
        options.CallbackPath = azureAd.CallbackPath;

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

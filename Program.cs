using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using TestTenantAuth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<ITenantStore, InMemoryTenantStore>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        var entraId = builder.Configuration.GetSection("EntraId");

        options.Authority = entraId["Authority"] ?? "https://login.microsoftonline.com/organizations/v2.0";
        options.ClientId = entraId["ClientId"] ?? string.Empty;
        options.ClientSecret = entraId["ClientSecret"] ?? string.Empty;
        options.CallbackPath = entraId["CallbackPath"] ?? "/signin-oidc";
        options.ResponseType = "code";
        options.SaveTokens = true;

        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = context =>
            {
                var tenantStore = context.HttpContext.RequestServices.GetRequiredService<ITenantStore>();
                var tid = context.Principal?.FindFirstValue("tid");

                if (string.IsNullOrWhiteSpace(tid) || tenantStore.GetByTenantId(tid) is null)
                {
                    context.Fail("Tenant not onboarded");
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                if (string.Equals(context.Exception?.Message, "Tenant not onboarded", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Redirect("/?error=tenant_not_onboarded");
                    context.HandleResponse();
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

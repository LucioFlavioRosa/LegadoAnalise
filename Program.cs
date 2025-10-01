using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Services.Auth;
using Services.Associados;
using Services.Auth;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Azure Key Vault
builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration["KeyVaultUri"]), new Azure.Identity.DefaultAzureCredential());

// Microsoft Identity Web
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");
builder.Services.AddAuthorization();

// Serviços customizados
builder.Services.AddScoped<IAuthCallbackService, AuthCallbackService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<AssociadosService>();

// Cache distribuído
builder.Services.AddDistributedMemoryCache(); // Para produção, usar AddStackExchangeRedisCache

// Autenticação por cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
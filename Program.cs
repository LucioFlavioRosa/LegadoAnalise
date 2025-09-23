using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Services;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.Associados.Common;
using Peers.Moderno.Services.Common.Auth;
using Peers.Moderno.Services.Cargos;
using Peers.Moderno.Services.Cargos.Common;
using Peers.Moderno.Services.Clientes;
using Peers.Moderno.Services.Clientes.Common;
using Peers.Moderno.Services.Competencias;
using Peers.Moderno.Services.Competencias.Common;
using Peers.Moderno.Services.Complexidades;
using Peers.Moderno.Services.Complexidades.Common;
using Peers.Moderno.Services.Consolidacao;
using Peers.Moderno.Services.Consolidacao.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Application Insights
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:ConnectionString"]);

// Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HttpClient for Auth Services
builder.Services.AddHttpClient();

// Authentication Services
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    var azureAdConfig = builder.Configuration.GetSection("Authentication:AzureAd");
    options.Authority = $"{azureAdConfig["ADInstance"]}{azureAdConfig["TenantId"]}/v2.0";
    options.ClientId = azureAdConfig["ClientId"];
    options.ClientSecret = azureAdConfig["ClientSecret"];
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
});

// Common Services
builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddScoped<IMessageBoxService, MessageBoxService>();

// Auth Common Services
builder.Services.AddScoped<ITokenDecoder, TokenDecoder>();
builder.Services.AddScoped<IAuthCallbackService, AuthCallbackService>();

// Business Services - Dependency Injection
builder.Services.AddScoped<ICompetenciasService, CompetenciasService>();
builder.Services.AddScoped<ICargosService, CargosService>();
builder.Services.AddScoped<IEixosService, EixosService>();
builder.Services.AddScoped<ISubCompetenciasService, SubCompetenciasService>();
builder.Services.AddScoped<IDimensoesService, DimensoesService>();
builder.Services.AddScoped<IAvaliacoesService, AvaliacoesService>();
builder.Services.AddScoped<IExportFileService, ExportFileService>();
builder.Services.AddScoped<IPremissasService, PremissasService>();

// Competencias Services
builder.Services.AddScoped<Services.Competencias.ICompetenciasService, Services.Competencias.CompetenciasService>();
builder.Services.AddScoped<ICompetenciasImportExportUtil, CompetenciasImportExportUtil>();
builder.Services.AddScoped<ICompetenciasValidator, CompetenciasValidator>();

// Cargos Services
builder.Services.AddScoped<Services.Cargos.ICargosService, Services.Cargos.CargosService>();
builder.Services.AddScoped<Services.Cargos.Common.IDropdownService, Services.Cargos.Common.DropdownService>();
builder.Services.AddScoped<Services.Cargos.Common.IExportService, Services.Cargos.Common.ExportService>();

// Cargos Common Services - Novo serviço adicionado
builder.Services.AddScoped<ICargoLookupService, CargoLookupService>();

// Associados Services
builder.Services.AddScoped<IAssociadosService, AssociadosService>();
builder.Services.AddScoped<AssociadosService>();

// Associados Common Services
builder.Services.AddScoped<IFotoService, FotoService>();
builder.Services.AddScoped<FotoService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<Services.Associados.Common.IDropdownService, Services.Associados.Common.DropdownService>();
builder.Services.AddScoped<Services.Associados.Common.DropdownService>();
builder.Services.AddScoped<IPromocaoService, PromocaoService>();
builder.Services.AddScoped<PromocaoService>();

// Clientes Services
builder.Services.AddScoped<Services.Clientes.Common.IClientesService, Services.Clientes.ClientesService>();

// Complexidades Services
builder.Services.AddScoped<IComplexidadeService, ComplexidadesService>();

// Consolidacao Services
builder.Services.AddScoped<IConsolidacaoService, ConsolidacaoService>();

// Consolidacao Common Services
builder.Services.AddScoped<Services.Consolidacao.Common.IExportFileService, Services.Consolidacao.Common.ExportFileService>();
builder.Services.AddScoped<Services.Consolidacao.Common.IImportFileService, Services.Consolidacao.Common.ImportFileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
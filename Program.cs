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
using Peers.Moderno.Services.Dashboard;
using Peers.Moderno.Services.Dashboard.Common;
using Peers.Moderno.Services.Dimensoes;
using Peers.Moderno.Services.Dimensoes.Common;
using Peers.Moderno.Services.DisparoMassivoRH;
using Peers.Moderno.Services.DisparoMassivoRH.Common;
using Peers.Moderno.Services.Eixos;
using Peers.Moderno.Services.Eixos.Common;
using Peers.Moderno.Services.Avaliacoes;
using Peers.Moderno.Services.Avaliacoes.Common;
using Peers.Moderno.Services.FrentesInternas;
using Peers.Moderno.Services.FrentesInternas.Common;
using Peers.Moderno.Services.Perfis;
using Peers.Moderno.Services.Performance;
using Peers.Moderno.Services.Performance.Common;
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

// Session support for migration compatibility
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpContextAccessor for session access
builder.Services.AddHttpContextAccessor();

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

// New Common Services - Index Page Migration
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// Auth Common Services - Login Migration
builder.Services.AddScoped<ITokenDecoder, TokenDecoder>();
builder.Services.AddScoped<IAuthCallbackService, AuthCallbackService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Auth Common Services - Logout Migration
builder.Services.AddScoped<ILogoutService, LogoutService>();

// Footer Services - Login Migration
builder.Services.AddScoped<IFooterLinksService, FooterLinksService>();

// Perguntas Encerramento Services - Nova funcionalidade
builder.Services.AddScoped<IPerguntasEncerramentoService, PerguntasEncerramentoService>();

// Business Services - Dependency Injection
builder.Services.AddScoped<ICompetenciasService, CompetenciasService>();
builder.Services.AddScoped<ICargosService, CargosService>();
builder.Services.AddScoped<ISubCompetenciasService, SubCompetenciasService>();
builder.Services.AddScoped<IAvaliacoesService, AvaliacoesService>();
builder.Services.AddScoped<IExportFileService, ExportFileService>();
builder.Services.AddScoped<IPremissasService, PremissasService>();

// Eixos Services
builder.Services.AddScoped<Services.Eixos.IEixosService, Services.Eixos.EixosService>();
builder.Services.AddScoped<Services.Eixos.Common.IExportFileService, Services.Eixos.Common.ExportFileService>();

// Dimensoes Services
builder.Services.AddScoped<Services.Dimensoes.Common.IDimensoesService, Services.Dimensoes.DimensoesService>();
builder.Services.AddScoped<IDimensoesExportService, DimensoesExportService>();

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

// Dashboard Services
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Dashboard Common Services
builder.Services.AddScoped<IChartJsonUtil, ChartJsonUtil>();
builder.Services.AddScoped<IPeriodoUtil, PeriodoUtil>();

// DisparoMassivoRH Services
builder.Services.AddScoped<IDisparoMassivoRHService, DisparoMassivoRHService>();

// Avaliacoes Services - Novos serviços adicionados
builder.Services.AddScoped<IEnvioAvaliacoesService, EnvioAvaliacoesService>();
builder.Services.AddScoped<IEvolucaoAssociadoService, EvolucaoAssociadoService>();

// Avaliacoes Common Services - Novos serviços adicionados
builder.Services.AddScoped<IEmailUtils, EmailUtils>();
builder.Services.AddScoped<IWorkflowUtils, WorkflowUtils>();

// FrentesInternas Services - Novos serviços adicionados
builder.Services.AddScoped<IFrentesInternasService, FrentesInternasService>();

// FrentesInternas Common Services - Novos serviços adicionados
builder.Services.AddScoped<IStatusHelper, StatusHelper>();
builder.Services.AddScoped<IExportHelper, ExportHelper>();

// Perfis Services - Novos serviços adicionados
builder.Services.AddScoped<IPerfisService, PerfisService>();

// Performance Services - Novos serviços adicionados
builder.Services.AddScoped<IPerformanceService, PerformanceService>();

// Performance Common Services - Novos serviços adicionados
builder.Services.AddScoped<IPerformanceImportExportUtil, PerformanceImportExportUtil>();
builder.Services.AddScoped<IPerformanceValidationUtil, PerformanceValidationUtil>();
builder.Services.AddScoped<IPerformanceComboHelper, PerformanceComboHelper>();

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

// Session middleware
app.UseSession();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
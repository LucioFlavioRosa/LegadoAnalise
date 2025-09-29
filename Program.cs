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
using Peers.Moderno.Services.Prazos;
using Peers.Moderno.Services.Prazos.Common;
using Peers.Moderno.Services.Premissas;
using Peers.Moderno.Services.Premissas.Common;
using Peers.Moderno.Services.Projetos;
using Peers.Moderno.Services.Projetos.Common;
using Peers.Moderno.Services.SubCompetencias;
using Peers.Moderno.Services.SubCompetencias.Common;
using Peers.Moderno.Services.Common.Avaliacoes;
using Peers.Moderno.Services.AutoAvaliacao;
using Peers.Moderno.Services.AutoAvaliacao.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.AI;
using Peers.Moderno.Services.Mentoria;
using Peers.Moderno.Services.Mentoria.Common;
using Peers.Moderno.Services.Feedback;
using Peers.Moderno.Services.Feedback.Common;
using Services.FeedbackPerformance;
using Services.FeedbackPerformance.Common;
using Services.AvaliacoesGestor;
using Services.AvaliacoesGestor.Common;
using Services.Avaliacoes;
using Services.Avaliacoes.Common;
using Services.AvaliacoesGestorasCegas;
using Services.AvaliacaoMentorCompetencia;
using Services.AvaliacaoMentorCompetencia.Common;
using Services.Performance.IPerformanceMentorService;
using Services.Performance.PerformanceMentorService;
using Services.Resultados;
using Services.Resultados.Common;
using Services.ResultadoLideranca;
using Services.ResultadoLideranca.Common;
using Services.Common;
using Services.Comentarios;
using Services.Comentarios.Common;
using Services.Common;
using Services.Avaliacoes;
using Services.Avaliacoes.Common;
using Services.Avaliacoes.EvolucaoAssociadoService;
using Services.Common.EmailService;
using Services.AvaliacoesExportacao;
using Services.AvaliacoesExportacao.Common;
using Services.Pendencias.Common;
using Services.Periodos;
using Services.Periodos.Common;
using Services.Radar;
using Services.Radar.Common;
using Services.Common;
using Services.Common.ChartHelper;
using Services.Competencias;
using Services.Competencias.Common;
using Services.Workflow;
using Peers.Moderno.Services.Avaliacoes.Common;
using Services.Avaliacoes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:ConnectionString"]);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();
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

if (builder.Configuration.GetValue<bool>("AutoAvaliacao:Features:EnableAIIntegration"))
{
    builder.Services.AddSingleton<IChatClient>(serviceProvider =>
    {
        return new ChatClientBuilder()
            .UseFunctionInvocation()
            .Build();
    });
}

builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddScoped<IMessageBoxService, MessageBoxService>();
builder.Services.AddScoped<FormatHelper>();
builder.Services.AddScoped<ComboHelper>();
builder.Services.AddScoped<IConsultaAvancadaService, ConsultaAvancadaService>();
builder.Services.AddScoped<IResultadoService, ResultadoService>();
builder.Services.AddScoped<IResultadoHelper, ResultadoHelper>();
builder.Services.AddScoped<IResultadoLiderancaService, ResultadoLiderancaService>();
builder.Services.AddScoped<IResultadoLiderancaHelper, ResultadoLiderancaHelper>();
builder.Services.AddScoped<ChartJsInteropService>();
builder.Services.AddScoped<IRadarService, RadarService>();
builder.Services.AddScoped<IRadarHelper, RadarHelper>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITokenDecoder, TokenDecoder>();
builder.Services.AddScoped<IAuthCallbackService, AuthCallbackService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILogoutService, LogoutService>();
builder.Services.AddScoped<IFooterLinksService, FooterLinksService>();
builder.Services.AddScoped<IPerguntasEncerramentoService, PerguntasEncerramentoService>();
builder.Services.AddScoped<ITotalAvaliacaoService, TotalAvaliacaoService>();
builder.Services.AddScoped<IComponentBaseService, ComponentBaseService>();
builder.Services.AddScoped<IBlazorNavigationService, BlazorNavigationService>();
builder.Services.AddScoped<ICompetenciasService, CompetenciasService>();
builder.Services.AddScoped<ICargosService, CargosService>();
builder.Services.AddScoped<ISubCompetenciasService, SubCompetenciasService>();
builder.Services.AddScoped<IAvaliacoesService, AvaliacoesService>();
builder.Services.AddScoped<IExportFileService, ExportFileService>();
builder.Services.AddScoped<Services.Premissas.Common.IPremissasService, Services.Premissas.PremissasService>();
builder.Services.AddScoped<Services.SubCompetencias.ISubCompetenciasService, Services.SubCompetencias.SubCompetenciasService>();
builder.Services.AddScoped<Services.SubCompetencias.Common.ISubCompetenciasValidator, Services.SubCompetencias.Common.SubCompetenciasValidator>();
builder.Services.AddScoped<Services.SubCompetencias.Common.ISubCompetenciasExportService, Services.SubCompetencias.Common.SubCompetenciasExportService>();
builder.Services.AddScoped<Services.SubCompetencias.Common.ISubCompetenciasImportService, Services.SubCompetencias.Common.SubCompetenciasImportService>();
builder.Services.AddScoped<Services.Eixos.IEixosService, Services.Eixos.EixosService>();
builder.Services.AddScoped<Services.Eixos.Common.IExportFileService, Services.Eixos.Common.ExportFileService>();
builder.Services.AddScoped<Services.Dimensoes.Common.IDimensoesService, Services.Dimensoes.DimensoesService>();
builder.Services.AddScoped<IDimensoesExportService, DimensoesExportService>();
builder.Services.AddScoped<Services.Competencias.ICompetenciasService, Services.Competencias.CompetenciasService>();
builder.Services.AddScoped<ICompetenciasImportExportUtil, CompetenciasImportExportUtil>();
builder.Services.AddScoped<ICompetenciasValidator, CompetenciasValidator>();
builder.Services.AddScoped<Services.Cargos.ICargosService, Services.Cargos.CargosService>();
builder.Services.AddScoped<Services.Cargos.Common.IDropdownService, Services.Cargos.Common.DropdownService>();
builder.Services.AddScoped<Services.Cargos.Common.IExportService, Services.Cargos.Common.ExportService>();
builder.Services.AddScoped<ICargoLookupService, CargoLookupService>();
builder.Services.AddScoped<IAssociadosService, AssociadosService>();
builder.Services.AddScoped<AssociadosService>();
builder.Services.AddScoped<IFotoService, FotoService>();
builder.Services.AddScoped<FotoService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<Services.Associados.Common.IDropdownService, Services.Associados.Common.DropdownService>();
builder.Services.AddScoped<Services.Associados.Common.DropdownService>();
builder.Services.AddScoped<IPromocaoService, PromocaoService>();
builder.Services.AddScoped<PromocaoService>();
builder.Services.AddScoped<Services.Clientes.Common.IClientesService, Services.Clientes.ClientesService>();
builder.Services.AddScoped<IComplexidadeService, ComplexidadesService>();
builder.Services.AddScoped<IConsolidacaoService, ConsolidacaoService>();
builder.Services.AddScoped<Services.Consolidacao.Common.IExportFileService, Services.Consolidacao.Common.ExportFileService>();
builder.Services.AddScoped<Services.Consolidacao.Common.IImportFileService, Services.Consolidacao.Common.ImportFileService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IChartJsonUtil, ChartJsonUtil>();
builder.Services.AddScoped<IPeriodoUtil, PeriodoUtil>();
builder.Services.AddScoped<IDisparoMassivoRHService, DisparoMassivoRHService>();
builder.Services.AddScoped<IEnvioAvaliacoesService, EnvioAvaliacoesService>();
builder.Services.AddScoped<IEvolucaoAssociadoService, EvolucaoAssociadoService>();
builder.Services.AddScoped<IEvolucaoAssociadoViewService, EvolucaoAssociadoViewService>();
builder.Services.AddScoped<IEvolucaoAssociadoPerformanceService, EvolucaoAssociadoPerformanceService>();
builder.Services.AddScoped<IEvolucaoAssociadoViewService, EvolucaoAssociadoViewService>();
builder.Services.AddScoped<IEvolucaoAssociadoPerformanceService, EvolucaoAssociadoPerformanceService>();
builder.Services.AddScoped<IEvolucaoAssociadoService, EvolucaoAssociadoService>();
builder.Services.AddScoped<IExportarAvaliacoesService, ExportarAvaliacoesService>();
builder.Services.AddScoped<IExportarAvaliacoesHelper, ExportarAvaliacoesHelper>();
builder.Services.AddScoped<IExportarAvaliacoesExcelService, ExportarAvaliacoesExcelService>();
builder.Services.AddScoped<IPendenciasService, PendenciasService>();
builder.Services.AddScoped<IPeriodoService, PeriodoService>();
builder.Services.AddScoped<IPeriodoValidator, PeriodoValidator>();
builder.Services.AddScoped<IPeriodoFormatHelper, PeriodoFormatHelper>();
builder.Services.AddScoped<IAvaliacoesSinalizadasService, AvaliacoesSinalizadasService>();
builder.Services.AddScoped<Services.Competencias.Common.ICompetenciaHelper, Services.Competencias.Common.CompetenciaHelper>();
builder.Services.AddScoped<Services.Competencias.ICompetenciasService, Services.Competencias.CompetenciasService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IWorkflowComboHelper, WorkflowComboHelper>();

// Registro único e correto do serviço de EvoluçãoAssociadoService
builder.Services.AddScoped<Peers.Moderno.Services.Avaliacoes.Common.IEvolucaoAssociadoService, Services.Avaliacoes.EvolucaoAssociadoService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

if (app.Environment.IsProduction())
{
    app.UseResponseCompression();
    app.UseResponseCaching();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();

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
// Novos serviços para resultado de avaliação
using Services.Resultados;
using Services.Resultados.Common;
// Novos serviços para resultado de liderança
using Services.ResultadoLideranca;
using Services.ResultadoLideranca.Common;
using Services.Common;
using Services.Comentarios;
using Services.Comentarios.Common;
// Importação para Consulta Avançada
using Services.Common;
// Importação dos novos serviços de retroceder avaliações
using Services.Avaliacoes;
using Services.Avaliacoes.Common;

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

// AI Services - Preparação para integração futura
if (builder.Configuration.GetValue<bool>("AutoAvaliacao:Features:EnableAIIntegration"))
{
    builder.Services.AddSingleton<IChatClient>(serviceProvider =>
    {
        // Configuração stub para futura integração com Azure OpenAI ou outros provedores
        return new ChatClientBuilder()
            .UseFunctionInvocation()
            .Build();
    });
}

// Common Services
builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddScoped<IMessageBoxService, MessageBoxService>();
// Serviços e helpers comuns já existentes
builder.Services.AddScoped<FormatHelper>();
builder.Services.AddScoped<ComboHelper>();
// Registro do serviço de consulta avançada
builder.Services.AddScoped<IConsultaAvancadaService, ConsultaAvancadaService>();
// Serviços de resultado de avaliação (novos para tela de resultado)
builder.Services.AddScoped<IResultadoService, ResultadoService>();
builder.Services.AddScoped<IResultadoHelper, ResultadoHelper>();
// Serviços de resultado de liderança (novos para tela de resultado de liderança)
builder.Services.AddScoped<IResultadoLiderancaService, ResultadoLiderancaService>();
builder.Services.AddScoped<IResultadoLiderancaHelper, ResultadoLiderancaHelper>();
// Serviço de ChartJsInterop reutilizável
builder.Services.AddScoped<ChartJsInteropService>();

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

// TotalAvaliacao Services - Nova migração de Web Forms para Blazor
builder.Services.AddScoped<ITotalAvaliacaoService, TotalAvaliacaoService>();

// WebForm4 Common Services - Nova migração de Web Forms para Blazor
builder.Services.AddScoped<IComponentBaseService, ComponentBaseService>();
builder.Services.AddScoped<IBlazorNavigationService, BlazorNavigationService>();

// Business Services - Dependency Injection
builder.Services.AddScoped<ICompetenciasService, CompetenciasService>();
builder.Services.AddScoped<ICargosService, CargosService>();
builder.Services.AddScoped<ISubCompetenciasService, SubCompetenciasService>();
builder.Services.AddScoped<IAvaliacoesService, AvaliacoesService>();
builder.Services.AddScoped<IExportFileService, ExportFileService>();
builder.Services.AddScoped<Services.Premissas.Common.IPremissasService, Services.Premissas.PremissasService>();

// SubCompetencias Services - Novos serviços adicionados conforme passo 8
builder.Services.AddScoped<Services.SubCompetencias.ISubCompetenciasService, Services.SubCompetencias.SubCompetenciasService>();
builder.Services.AddScoped<Services.SubCompetencias.Common.ISubCompetenciasValidator, Services.SubCompetencias.Common.SubCompetenciasValidator>();
builder.Services.AddScoped<Services.SubCompetencias.Common.ISubCompetenciasExportService, Services.SubCompetencias.Common.SubCompetenciasExportService>();
builder.Services.AddScoped<Services.SubCompetencias.Common.ISubCompetenciasImportService, Services.SubCompetencias.Common.SubCompetenciasImportService>();

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

// AutoAvaliacao Services - Novos serviços para migração de Web Forms
builder.Services.AddScoped<IAutoAvaliacaoService, AutoAvaliacaoService>();
builder.Services.AddScoped<IAutoAvaliacaoPerformanceService, AutoAvaliacaoPerformanceService>();

// AutoAvaliacao Common Services - Novos serviços reutilizáveis (passo 12)
builder.Services.AddScoped<INotaHelper, NotaHelper>();
builder.Services.AddScoped<IValidationHelper, ValidationHelper>();
builder.Services.AddScoped<IAccordionHelper, AccordionHelper>();

// AI Services for AutoAvaliacao - Preparação para integração futura
builder.Services.AddScoped<IAvaliacaoIAService, AvaliacaoIAService>();

// FrentesInternas Services - Novos serviços adicionados
builder.Services.AddScoped<IFrentesInternasService, FrentesInternasService>();
// Helpers e modelos comuns de FrentesInternas
builder.Services.AddScoped<Services.FrentesInternas.Common.Helpers.FrentesInternasHelper>();
// Models já estão em Services.FrentesInternas.Common.Models

// FrentesInternas Common Services - Novos serviços adicionados
builder.Services.AddScoped<IStatusHelper, StatusHelper>();
builder.Services.AddScoped<IExportHelper, ExportHelper>();

// Perfis Services - Novos serviços adicionados
builder.Services.AddScoped<IPerfisService, PerfisService>();

// Performance Services - Novos serviços adicionados
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
// Registro do novo serviço de feedback de performance
builder.Services.AddScoped<IFeedbackPerformanceService, FeedbackPerformanceService>();
// Registro do novo serviço de performance mentor
builder.Services.AddScoped<Services.Performance.IPerformanceMentorService, Services.Performance.PerformanceMentorService>();

// Performance Common Services - Novos serviços adicionados
builder.Services.AddScoped<IPerformanceImportExportUtil, PerformanceImportExportUtil>();
builder.Services.AddScoped<IPerformanceValidationUtil, PerformanceValidationUtil>();
builder.Services.AddScoped<IPerformanceComboHelper, PerformanceComboHelper>();

// FeedbackPerformance Services - Registro dos novos serviços e helpers
builder.Services.AddScoped<IFeedbackPerformanceService, FeedbackPerformanceService>();
builder.Services.AddScoped<IFeedbackPerformanceHelper, FeedbackPerformanceHelper>();
builder.Services.AddScoped<IFeedbackPerformanceValidator, FeedbackPerformanceValidator>();

// Prazos Services - Novos serviços adicionados
builder.Services.AddScoped<IPrazosService, PrazosService>();

// Prazos Common Services - Novos serviços adicionados
builder.Services.AddScoped<IPrazosValidationHelper, PrazosValidationHelper>();

// Projetos Services - Novos serviços adicionados
builder.Services.AddScoped<IProjetosService, ProjetosService>();

// Projetos Common Services - Novos serviços adicionados
builder.Services.AddScoped<IProjetosComboHelper, ProjetosComboHelper>();

// Tipos Projetos Services - Migração de Web Forms para Blazor
builder.Services.AddScoped<ITiposProjetosService, TiposProjetosService>();

// Avaliacoes Services - Registro dos novos serviços do fluxo de avaliação
builder.Services.AddScoped<Services.Avaliacoes.IAvaliacaoService, Services.Avaliacoes.AvaliacaoService>();
builder.Services.AddScoped<Services.Avaliacoes.Common.IAvaliacaoUtils, Services.Avaliacoes.Common.AvaliacaoUtils>();

// Mentoria Services - Registro dos novos serviços e helpers
builder.Services.AddScoped<IMentoriaService, MentoriaService>();
builder.Services.AddScoped<IMentoriaHelper, MentoriaHelper>();

// Feedback Services - Registro dos novos serviços e helpers
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IFeedbackFinalizationService, FeedbackFinalizationService>();
builder.Services.AddScoped<IFeedbackComboHelper, FeedbackComboHelper>();

// AvaliacoesGestor Services - Registro dos novos serviços e helpers (Passo 8)
builder.Services.AddScoped<Services.AvaliacoesGestor.Common.IAvaliacoesGestorService, Services.AvaliacoesGestor.Common.AvaliacoesGestorService>();
builder.Services.AddScoped<Services.AvaliacoesGestor.Common.IAvaliacoesGestorHelper, Services.AvaliacoesGestor.Common.AvaliacoesGestorHelper>();

// AvaliacaoCompetenciaService - Registro do serviço de avaliação de competências (Passo 8)
builder.Services.AddScoped<Services.Avaliacoes.IAvaliacaoCompetenciaService, Services.Avaliacoes.AvaliacaoCompetenciaService>();

// AvaliacoesGestorasCegas Performance Services - Passo 6
builder.Services.AddScoped<IAvaliacaoGestorasCegasPerformanceService, AvaliacaoGestorasCegasPerformanceService>();

// AvaliacaoMentorCompetencia Services - Passo 5
builder.Services.AddScoped<IAvaliacaoMentorCompetenciaService, AvaliacaoMentorCompetenciaService>();
builder.Services.AddScoped<AvaliacaoMentorCompetenciaHelper>();

// REGISTRO DOS NOVOS SERVIÇOS DE RESULTADO DE AVALIAÇÃO
builder.Services.AddScoped<IResultadoService, ResultadoService>();
builder.Services.AddScoped<IResultadoHelper, ResultadoHelper>();
// REGISTRO DOS NOVOS SERVIÇOS DE RESULTADO DE LIDERANÇA
builder.Services.AddScoped<IResultadoLiderancaService, ResultadoLiderancaService>();
builder.Services.AddScoped<IResultadoLiderancaHelper, ResultadoLiderancaHelper>();
// Serviço de ChartJsInterop reutilizável
builder.Services.AddScoped<ChartJsInteropService>();

// Registro dos serviços de Comentários (Migração Comentarios)
builder.Services.AddScoped<IComentariosService, ComentariosService>();
builder.Services.AddScoped<ComentariosHelper>();

// Registro dos serviços de retroceder avaliações
builder.Services.AddScoped<IRetrocederAvaliacaoService, RetrocederAvaliacaoService>();
builder.Services.AddScoped<IRetrocederAvaliacaoValidator, RetrocederAvaliacaoValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// AOT Optimization - Configuração condicional para otimização em produção
if (app.Environment.IsProduction())
{
    app.UseResponseCompression();
    app.UseResponseCaching();
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

using Peers.Moderno.Models;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.FrentesInternas;
using Peers.Moderno.Services.Avaliacoes;

namespace Peers.Moderno.Services.Common;

public interface IMenuService
{
    Task<List<MenuItem>> GetMenuItemsAsync(int userId, int userProfileId);
    Task<List<QuickAccessItem>> GetQuickAccessItemsAsync(int userId, int userProfileId);
    Task<List<EvaluationMenuItem>> GetEvaluationMenuItemsAsync(int userId, int userProfileId);
    Task<List<MentorshipMenuItem>> GetMentorshipMenuItemsAsync(int userId);
}

public class MenuService : IMenuService
{
    private readonly IAssociadosService _associadosService;
    private readonly IFrentesInternasService _frentesInternasService;
    private readonly IEnvioAvaliacoesService _envioAvaliacoesService;
    private readonly ITelemetryService _telemetryService;

    public MenuService(
        IAssociadosService associadosService,
        IFrentesInternasService frentesInternasService,
        IEnvioAvaliacoesService envioAvaliacoesService,
        ITelemetryService telemetryService)
    {
        _associadosService = associadosService;
        _frentesInternasService = frentesInternasService;
        _envioAvaliacoesService = envioAvaliacoesService;
        _telemetryService = telemetryService;
    }

    public async Task<List<MenuItem>> GetMenuItemsAsync(int userId, int userProfileId)
    {
        try
        {
            var menuItems = new List<MenuItem>();

            if (userProfileId > 2)
            {
                menuItems.AddRange(GetBasicAssociateMenuItems());
                menuItems.AddRange(GetBasicProjectMenuItems());
                menuItems.AddRange(GetProjectManagementMenuItems());
                menuItems.AddRange(GetEvaluationSendingMenuItems());
            }

            _telemetryService.TrackEvent("MenuItemsLoaded", new Dictionary<string, string>
            {
                { "UserId", userId.ToString() },
                { "ProfileId", userProfileId.ToString() },
                { "ItemCount", menuItems.Count.ToString() }
            });

            return menuItems;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetMenuItemsAsync" },
                { "UserId", userId.ToString() }
            });
            return new List<MenuItem>();
        }
    }

    public async Task<List<QuickAccessItem>> GetQuickAccessItemsAsync(int userId, int userProfileId)
    {
        try
        {
            var quickAccessItems = new List<QuickAccessItem>
            {
                new QuickAccessItem
                {
                    Id = "pendencias",
                    Title = "Pendências",
                    Url = "pendencias.aspx",
                    CssClass = "btn btn-dark",
                    IsVisible = true
                }
            };

            var hasProjects = await _envioAvaliacoesService.HasUserProjectsAsync(userId);
            if (hasProjects)
            {
                quickAccessItems.Add(new QuickAccessItem
                {
                    Id = "gerenciarProjetos",
                    Title = "Projetos",
                    Url = "GerenciarProjetos",
                    CssClass = "btn btn-dark",
                    IsVisible = true
                });
            }

            var isInternalFrontLeader = await _frentesInternasService.IsUserInternalFrontLeaderAsync(userId);
            if (isInternalFrontLeader)
            {
                quickAccessItems.Add(new QuickAccessItem
                {
                    Id = "frenteInterna",
                    Title = "Alocações Internas",
                    Url = "frentesinternas",
                    CssClass = "btn btn-dark",
                    IsVisible = true
                });
            }

            return quickAccessItems;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetQuickAccessItemsAsync" },
                { "UserId", userId.ToString() }
            });
            return new List<QuickAccessItem>();
        }
    }

    public async Task<List<EvaluationMenuItem>> GetEvaluationMenuItemsAsync(int userId, int userProfileId)
    {
        try
        {
            var evaluationItems = new List<EvaluationMenuItem>
            {
                new EvaluationMenuItem
                {
                    Id = "autoAvaliacao",
                    Title = "Auto Avaliação",
                    Url = "autoavalizacao.aspx",
                    CssClass = "btn btn-info",
                    IsVisible = true
                },
                new EvaluationMenuItem
                {
                    Id = "evolucaoAssociado",
                    Title = "Evolução Associado",
                    Url = "EvolucaoAssociado.aspx",
                    CssClass = "btn btn-info",
                    IsVisible = true
                },
                new EvaluationMenuItem
                {
                    Id = "avaliacoesLideranca",
                    Title = "Avaliações de Liderança",
                    Url = "avalizacao_gestor.aspx",
                    CssClass = "btn btn-info",
                    IsVisible = true
                }
            };

            if (userProfileId > 1)
            {
                evaluationItems.AddRange(new List<EvaluationMenuItem>
                {
                    new EvaluationMenuItem
                    {
                        Id = "cegas",
                        Title = "Avaliação às Cegas",
                        Url = "avalizacao_gestorascegas.aspx",
                        CssClass = "btn btn-info",
                        IsVisible = true
                    },
                    new EvaluationMenuItem
                    {
                        Id = "gestor",
                        Title = "Avaliação do Gestor",
                        Url = "avalizacao_gestor.aspx",
                        CssClass = "btn btn-info",
                        IsVisible = true
                    },
                    new EvaluationMenuItem
                    {
                        Id = "feedback",
                        Title = "Feedback",
                        Url = "avalizacao_feedback.aspx",
                        CssClass = "btn btn-info",
                        IsVisible = true
                    },
                    new EvaluationMenuItem
                    {
                        Id = "resultadoLideranca",
                        Title = "Resultado Av. de Liderança",
                        Url = "avalizacao_resultado_lideranca",
                        CssClass = "btn btn-info",
                        IsVisible = true
                    }
                });
            }

            return evaluationItems;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetEvaluationMenuItemsAsync" },
                { "UserId", userId.ToString() }
            });
            return new List<EvaluationMenuItem>();
        }
    }

    public async Task<List<MentorshipMenuItem>> GetMentorshipMenuItemsAsync(int userId)
    {
        try
        {
            var mentorshipItems = new List<MentorshipMenuItem>();

            var mentorados = await _associadosService.ObterMentoradosAsync(userId);
            if (mentorados.Any())
            {
                mentorshipItems.AddRange(new List<MentorshipMenuItem>
                {
                    new MentorshipMenuItem
                    {
                        Id = "mentor",
                        Title = "Cartilha do Mentor",
                        Url = "avalizacao_mentor.aspx",
                        CssClass = "btn btn-info",
                        IsVisible = true,
                        IsMentorSection = true
                    },
                    new MentorshipMenuItem
                    {
                        Id = "resultadoMentor",
                        Title = "Resultado Mentoria",
                        Url = "resultado_mentoria",
                        CssClass = "btn btn-info",
                        IsVisible = true,
                        IsMentorSection = true
                    }
                });
            }

            mentorshipItems.AddRange(new List<MentorshipMenuItem>
            {
                new MentorshipMenuItem
                {
                    Id = "pdi",
                    Title = "PDI",
                    Url = "avaliacao_PDI",
                    CssClass = "btn btn-info",
                    IsVisible = true,
                    IsMentorSection = false
                },
                new MentorshipMenuItem
                {
                    Id = "avMentor",
                    Title = "Avaliar Mentoria",
                    Url = "avaliacao_mentoria",
                    CssClass = "btn btn-info",
                    IsVisible = true,
                    IsMentorSection = false
                }
            });

            return mentorshipItems;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "GetMentorshipMenuItemsAsync" },
                { "UserId", userId.ToString() }
            });
            return new List<MentorshipMenuItem>();
        }
    }

    private List<MenuItem> GetBasicAssociateMenuItems()
    {
        return new List<MenuItem>
        {
            new MenuItem
            {
                Id = "associados",
                Title = "Associados",
                Url = "Associados.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Associados"
            },
            new MenuItem
            {
                Id = "cargos",
                Title = "Cargos",
                Url = "Cargos.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Associados"
            },
            new MenuItem
            {
                Id = "perfilAcesso",
                Title = "Perfil de Acesso",
                Url = "Perfis.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Associados"
            }
        };
    }

    private List<MenuItem> GetBasicProjectMenuItems()
    {
        return new List<MenuItem>
        {
            new MenuItem
            {
                Id = "clientes",
                Title = "Clientes",
                Url = "Clientes.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Projetos"
            },
            new MenuItem
            {
                Id = "tiposProjetos",
                Title = "Tipos de Projetos",
                Url = "TiposProjetos.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Projetos"
            },
            new MenuItem
            {
                Id = "complexidade",
                Title = "Complexidade",
                Url = "Complexidade.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Projetos"
            },
            new MenuItem
            {
                Id = "eixo",
                Title = "Eixo",
                Url = "Eixo.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Projetos"
            },
            new MenuItem
            {
                Id = "dimensoes",
                Title = "Dimensões",
                Url = "Dimensoes.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Projetos"
            },
            new MenuItem
            {
                Id = "subCompetencias",
                Title = "Sub Competências",
                Url = "SubCompetencias.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Projetos"
            },
            new MenuItem
            {
                Id = "competencias",
                Title = "Competências",
                Url = "Competencias.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Projetos"
            },
            new MenuItem
            {
                Id = "performance",
                Title = "Performance",
                Url = "Performance.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro Básico de Projetos"
            }
        };
    }

    private List<MenuItem> GetProjectManagementMenuItems()
    {
        return new List<MenuItem>
        {
            new MenuItem
            {
                Id = "projetos",
                Title = "Projetos",
                Url = "CadastroProjetos.aspx",
                CssClass = "btn btn-info",
                Category = "Cadastro de Projetos"
            }
        };
    }

    private List<MenuItem> GetEvaluationSendingMenuItems()
    {
        return new List<MenuItem>
        {
            new MenuItem
            {
                Id = "envioAvaliacoes",
                Title = "Envio de Avaliações",
                Url = "EnvioAvaliacoes.aspx",
                CssClass = "btn btn-info",
                Category = "Envio de Avaliações"
            }
        };
    }
}

public class MenuItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string CssClass { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
}

public class QuickAccessItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string CssClass { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
}

public class EvaluationMenuItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string CssClass { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
}

public class MentorshipMenuItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string CssClass { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public bool IsMentorSection { get; set; }
}
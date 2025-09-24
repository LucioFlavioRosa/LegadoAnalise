using Peers.Moderno.Models;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.FrentesInternas;

namespace Peers.Moderno.Services.Common;

public interface IMenuService
{
    Task<List<MenuItem>> GetMenuItemsAsync(int userId);
    Task<List<QuickAccessItem>> GetQuickAccessItemsAsync(int userId);
    Task<List<EvaluationItem>> GetEvaluationItemsAsync(int userId);
    Task<List<MentorshipItem>> GetMentorshipItemsAsync(int userId);
}

public class MenuService : IMenuService
{
    private readonly IAssociadosService _associadosService;
    private readonly IFrentesInternasService _frentesInternasService;

    public MenuService(
        IAssociadosService associadosService,
        IFrentesInternasService frentesInternasService)
    {
        _associadosService = associadosService;
        _frentesInternasService = frentesInternasService;
    }

    public async Task<List<MenuItem>> GetMenuItemsAsync(int userId)
    {
        var associado = await _associadosService.ObterAssociadoAsync(userId);
        if (associado == null || associado.IdPerfil <= 2) return new List<MenuItem>();

        return new List<MenuItem>
        {
            new MenuItem
            {
                Title = "Cadastro Básico de Associados",
                Items = new List<MenuSubItem>
                {
                    new MenuSubItem { Text = "Associados", Action = "Associados", CssClass = "btn btn-info" },
                    new MenuSubItem { Text = "Cargos", Action = "Cargos", CssClass = "btn btn-info" },
                    new MenuSubItem { Text = "Perfil de Acesso", Action = "Perfis", CssClass = "btn btn-info" }
                }
            },
            new MenuItem
            {
                Title = "Cadastro Básico de Projetos",
                Items = new List<MenuSubItem>
                {
                    new MenuSubItem { Text = "Clientes", Action = "Clientes", CssClass = "btn btn-info" },
                    new MenuSubItem { Text = "Tipos de Projetos", Action = "TiposProjetos", CssClass = "btn btn-info" },
                    new MenuSubItem { Text = "Complexidade", Action = "Complexidade", CssClass = "btn btn-info" },
                    new MenuSubItem { Text = "Eixo", Action = "Eixo", CssClass = "btn btn-info" },
                    new MenuSubItem { Text = "Dimensões", Action = "Dimensoes", CssClass = "btn btn-info" },
                    new MenuSubItem { Text = "Sub Competências", Action = "SubCompetencias", CssClass = "btn btn-info" },
                    new MenuSubItem { Text = "Competências", Action = "Competencias", CssClass = "btn btn-info" },
                    new MenuSubItem { Text = "Performance", Action = "Performance", CssClass = "btn btn-info" }
                }
            },
            new MenuItem
            {
                Title = "Cadastro de Projetos",
                Items = new List<MenuSubItem>
                {
                    new MenuSubItem { Text = "Projetos", Action = "CadastroProjetos", CssClass = "btn btn-info" }
                }
            },
            new MenuItem
            {
                Title = "Envio de Avaliações",
                Items = new List<MenuSubItem>
                {
                    new MenuSubItem { Text = "Envio de Avaliações", Action = "EnvioAvaliacoes", CssClass = "btn btn-info" }
                }
            }
        };
    }

    public async Task<List<QuickAccessItem>> GetQuickAccessItemsAsync(int userId)
    {
        var items = new List<QuickAccessItem>
        {
            new QuickAccessItem { Text = "Pendências", Action = "pendencias", CssClass = "btn btn-dark" }
        };

        var associado = await _associadosService.ObterAssociadoAsync(userId);
        if (associado != null)
        {
            var projetosGestor = await _associadosService.ObterProjetosGestorAsync(userId);
            if (projetosGestor.Any())
            {
                items.Add(new QuickAccessItem { Text = "Projetos", Action = "GerenciarProjetos", CssClass = "btn btn-dark" });
            }

            var frentesLider = await _frentesInternasService.ObterLideresFrentesInternasAsync();
            if (frentesLider.Any(x => x.IdAssociado == userId))
            {
                items.Add(new QuickAccessItem { Text = "Alocações Internas", Action = "frentesinternas", CssClass = "btn btn-dark" });
            }
        }

        return items;
    }

    public async Task<List<EvaluationItem>> GetEvaluationItemsAsync(int userId)
    {
        var associado = await _associadosService.ObterAssociadoAsync(userId);
        if (associado == null) return new List<EvaluationItem>();

        var items = new List<EvaluationItem>
        {
            new EvaluationItem { Text = "Auto Avaliação", Action = "autoavalizacao", CssClass = "btn btn-info" },
            new EvaluationItem { Text = "Evolução Associado", Action = "EvolucaoAssociado", CssClass = "btn btn-info" },
            new EvaluationItem { Text = "Avaliações de Liderança", Action = "avalizacao_gestor", CssClass = "btn btn-info" }
        };

        if (associado.IdPerfil > 1)
        {
            items.AddRange(new List<EvaluationItem>
            {
                new EvaluationItem { Text = "Avaliação às Cegas", Action = "avalizacao_gestorascegas", CssClass = "btn btn-info" },
                new EvaluationItem { Text = "Avaliação do Gestor", Action = "avalizacao_gestor", CssClass = "btn btn-info" },
                new EvaluationItem { Text = "Feedback", Action = "avalizacao_feedback", CssClass = "btn btn-info" },
                new EvaluationItem { Text = "Resultado Av. de Liderança", Action = "avalizacao_resultado_lideranca", CssClass = "btn btn-info" }
            });
        }

        return items;
    }

    public async Task<List<MentorshipItem>> GetMentorshipItemsAsync(int userId)
    {
        var items = new List<MentorshipItem>();
        
        var mentorados = await _associadosService.ObterMentoradosAsync(userId);
        if (mentorados.Any())
        {
            items.AddRange(new List<MentorshipItem>
            {
                new MentorshipItem { Text = "Cartilha do Mentor", Action = "avalizacao_mentor", CssClass = "btn btn-info" },
                new MentorshipItem { Text = "Resultado Mentoria", Action = "resultado_mentoria", CssClass = "btn btn-info" }
            });
        }

        items.AddRange(new List<MentorshipItem>
        {
            new MentorshipItem { Text = "PDI", Action = "avaliacao_PDI", CssClass = "btn btn-info" },
            new MentorshipItem { Text = "Avaliar Mentoria", Action = "avaliacao_mentoria", CssClass = "btn btn-info" }
        });

        return items;
    }
}

public class MenuItem
{
    public string Title { get; set; } = string.Empty;
    public List<MenuSubItem> Items { get; set; } = new();
}

public class MenuSubItem
{
    public string Text { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string CssClass { get; set; } = string.Empty;
}

public class QuickAccessItem
{
    public string Text { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string CssClass { get; set; } = string.Empty;
}

public class EvaluationItem
{
    public string Text { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string CssClass { get; set; } = string.Empty;
}

public class MentorshipItem
{
    public string Text { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string CssClass { get; set; } = string.Empty;
}
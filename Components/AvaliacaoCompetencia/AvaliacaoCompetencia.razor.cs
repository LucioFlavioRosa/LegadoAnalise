using Microsoft.AspNetCore.Components;
using Services.Competencias;
using Services.Competencias.Common;
using Services.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Components.AvaliacaoCompetencia
{
    public partial class AvaliacaoCompetencia : ComponentBase
    {
        [Inject] public CompetenciasService CompetenciasService { get; set; }
        [Inject] public CompetenciaHelper CompetenciaHelper { get; set; }
        [Inject] public ComboHelper ComboHelper { get; set; }
        [Inject] public MessageBoxService MessageBoxService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected bool IsLoading { get; set; } = true;
        protected string? ErroMensagem { get; set; }
        protected List<CompetenciaModel> Competencias { get; set; } = new();
        protected List<ComboItem> NotasCompetencia { get; set; } = new();
        protected List<string> CabecalhoTitulos { get; set; } = new();
        protected List<string> CabecalhoDescricoesAtuais { get; set; } = new();
        protected List<string> CabecalhoDescricoesProximos { get; set; } = new();
        protected string CabecalhoCargoAtual { get; set; } = string.Empty;
        protected string CabecalhoCargoProximo { get; set; } = string.Empty;
        protected string CabecalhoFuncaoAtual { get; set; } = string.Empty;
        protected string CabecalhoFuncaoProximo { get; set; } = string.Empty;
        protected string CabecalhoAutonomiaAtual { get; set; } = string.Empty;
        protected string CabecalhoAutonomiaProximo { get; set; } = string.Empty;
        protected string CabecalhoEscopoAtual { get; set; } = string.Empty;
        protected string CabecalhoEscopoProximo { get; set; } = string.Empty;
        protected string CabecalhoInterlocucaoAtual { get; set; } = string.Empty;
        protected string CabecalhoInterlocucaoProximo { get; set; } = string.Empty;
        protected Dictionary<string, bool> AccordionOpen { get; set; } = new();

        protected AssociadoDetalhesModel? DetalhesAssociado { get; set; }
        protected ProjetoDetalhesModel? DetalhesProjeto { get; set; }
        protected PeriodoDetalhesModel? DetalhesPeriodo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                await CarregarDadosIniciais();
            }
            catch (System.Exception ex)
            {
                ErroMensagem = $"Erro ao carregar dados: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CarregarDadosIniciais()
        {
            // Carrega combos de notas reutilizando ComboHelper
            NotasCompetencia = ComboHelper.GetNotasCompetenciaItems();

            // Carrega detalhes do contexto (associado, projeto, periodo, etc)
            var contexto = await CompetenciasService.ObterContextoAvaliacaoAsync();
            DetalhesAssociado = contexto.Associado;
            DetalhesProjeto = contexto.Projeto;
            DetalhesPeriodo = contexto.Periodo;

            // Carrega cabeçalho (descrições, cargos, funções, etc)
            var cabecalho = await CompetenciasService.ObterCabecalhoAvaliacaoAsync();
            CabecalhoTitulos = cabecalho.Titulos;
            CabecalhoDescricoesAtuais = cabecalho.DescricoesAtuais;
            CabecalhoDescricoesProximos = cabecalho.DescricoesProximos;
            CabecalhoCargoAtual = cabecalho.CargoAtual;
            CabecalhoCargoProximo = cabecalho.CargoProximo;
            CabecalhoFuncaoAtual = cabecalho.FuncaoAtual;
            CabecalhoFuncaoProximo = cabecalho.FuncaoProximo;
            CabecalhoAutonomiaAtual = cabecalho.AutonomiaAtual;
            CabecalhoAutonomiaProximo = cabecalho.AutonomiaProximo;
            CabecalhoEscopoAtual = cabecalho.EscopoAtual;
            CabecalhoEscopoProximo = cabecalho.EscopoProximo;
            CabecalhoInterlocucaoAtual = cabecalho.InterlocucaoAtual;
            CabecalhoInterlocucaoProximo = cabecalho.InterlocucaoProximo;

            // Carrega competências e avaliações já existentes
            Competencias = await CompetenciasService.ObterCompetenciasAvaliacaoAsync();

            // Inicializa estados dos accordions
            AccordionOpen.Clear();
            foreach (var comp in Competencias)
            {
                AccordionOpen[$"det_{comp.IdCompetencia}"] = false;
                AccordionOpen[$"nivel_{comp.IdCompetencia}"] = false;
                AccordionOpen[$"detPrNivel_{comp.IdCompetencia}"] = false;
                AccordionOpen[$"prNivel_{comp.IdCompetencia}"] = false;
                AccordionOpen[$"cons_{comp.IdCompetencia}"] = false;
            }
            AccordionOpen["detalhes"] = false;
            AccordionOpen["cabecalho"] = false;
        }

        protected void ToggleAccordion(string key)
        {
            if (AccordionOpen.ContainsKey(key))
                AccordionOpen[key] = !AccordionOpen[key];
            else
                AccordionOpen[key] = true;
        }

        protected async Task Salvar()
        {
            if (!ValidarAvaliacao())
                return;

            var sucesso = await CompetenciasService.SalvarAvaliacaoAsync(Competencias);
            if (sucesso)
            {
                MessageBoxService.ShowSuccess("Avaliação Salva com Sucesso.");
            }
            else
            {
                MessageBoxService.ShowError("Falha ao Incluir Competência da Avaliação.");
            }
        }

        protected async Task Finalizar()
        {
            if (!ValidarAvaliacao())
                return;

            var sucesso = await CompetenciasService.SalvarAvaliacaoAsync(Competencias, finalizarAvaliacao: true);
            if (sucesso)
            {
                MessageBoxService.ShowSuccess("Avaliação Finalizada com Sucesso.");
                NavigationManager.NavigateTo($"/autoavalizacao?IdProjeto={DetalhesProjeto?.Id}&IdAssociado={DetalhesAssociado?.Id}&IdPeriodo={DetalhesPeriodo?.Id}");
            }
            else
            {
                MessageBoxService.ShowError("Erro ao finalizar avaliação.");
            }
        }

        protected async Task IrPerformance()
        {
            if (!ValidarAvaliacao())
                return;

            var sucesso = await CompetenciasService.SalvarAvaliacaoAsync(Competencias);
            if (sucesso)
            {
                MessageBoxService.ShowSuccess("Avaliação Salva com Sucesso.");
                NavigationManager.NavigateTo("/autoavalizacao_performance");
            }
            else
            {
                MessageBoxService.ShowError("Falha ao Incluir Competência da Avaliação.");
            }
        }

        protected bool ValidarAvaliacao()
        {
            foreach (var comp in Competencias)
            {
                if (comp.NotaNivel1 == "5" && comp.NotaNivel2 != "5")
                {
                    comp.NotaNivel2 = "5";
                    MessageBoxService.ShowError("ATENÇÃO ! Avaliação de Competência com inconsistência. (Detalhe: quando a nota de Competência do Nivel Atual for = Não Se Aplica, o Próximo Nivel deve ser Não Se Aplica). O Sistema ajustou sua avaliação. Favor revalidar sua avaliação !!");
                    return false;
                }
                if (comp.NotaNivel1 != "5" && comp.NotaNivel1 != "0" && comp.NotaNivel2 != "0" && int.Parse(comp.NotaNivel2) < int.Parse(comp.NotaNivel1))
                {
                    MessageBoxService.ShowError("Existem Avaliações de Competências inconsistentes. A nota do nível 2 (próximo nível) não pode ser maior que a nota do nível 1 (nível atual).");
                    return false;
                }
            }
            return true;
        }
    }

    // Modelos auxiliares para binding
    public class AssociadoDetalhesModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string TempoCargo { get; set; } = string.Empty;
        public string TempoPeers { get; set; } = string.Empty;
    }
    public class ProjetoDetalhesModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string TempoRestante { get; set; } = string.Empty;
    }
    public class PeriodoDetalhesModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}

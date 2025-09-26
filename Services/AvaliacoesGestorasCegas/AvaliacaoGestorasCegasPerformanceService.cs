using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.Common;

namespace Services.AvaliacoesGestorasCegas;

public class AvaliacaoGestorasCegasPerformanceService : IAvaliacaoGestorasCegasPerformanceService
{
    private readonly ApplicationDbContext _db;
    private readonly IPerformanceHelper _performanceHelper;

    public AvaliacaoGestorasCegasPerformanceService(ApplicationDbContext db, IPerformanceHelper performanceHelper)
    {
        _db = db;
        _performanceHelper = performanceHelper;
    }

    public async Task<CabecalhoAvaliacaoGestorasCegasPerformanceDto> GetDadosCabecalhoAsync(int idProjeto, int idAssociado, int idPeriodo, int idAvaliacao)
    {
        var projeto = await _db.Projetos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == idProjeto);
        var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
        var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
        var gestor = projeto != null ? await _db.Associados.FirstOrDefaultAsync(a => a.Id == projeto.IdAssociadoGestor) : null;
        var cliente = projeto?.Cliente?.Nome ?? string.Empty;

        // Simulação de métodos utilitários para tempo de cargo/peers
        string tempoCargo = ""; // Implementar lógica se necessário
        string tempoPeers = "";
        if (associado != null)
        {
            tempoCargo = _performanceHelper.ObterTempoAssociado(associado.Id, PerformanceTempoTipo.Cargo);
            tempoPeers = _performanceHelper.ObterTempoAssociado(associado.Id, PerformanceTempoTipo.Peers);
        }

        string tempoRestante = "";
        var avaliacaoEmail = await _db.AvaliacoesEmail.FirstOrDefaultAsync(a => a.idAvaliacao == idAvaliacao);
        if (avaliacaoEmail != null && avaliacaoEmail.DataLiberacao.HasValue)
        {
            // Simula lógica de cálculo de prazo
            var prazo = await _db.Prazos.FirstOrDefaultAsync(p => p.IdPrazo == avaliacaoEmail.PRAZOS);
            if (prazo != null)
            {
                var dataFinal = avaliacaoEmail.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy");
                tempoRestante = DateTime.Today >= DateTime.ParseExact(dataFinal, "dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"))
                    ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoAsCegas).ToString("dd/MM/yyyy")
                    : dataFinal;
            }
        }

        return new CabecalhoAvaliacaoGestorasCegasPerformanceDto
        {
            NomeAssociado = associado?.Nome ?? string.Empty,
            Periodo = periodo?.Nome ?? string.Empty,
            Projeto = projeto?.Nome ?? string.Empty,
            Gestor = gestor?.Nome ?? string.Empty,
            Cliente = cliente,
            TempoCargo = tempoCargo,
            TempoPeers = tempoPeers,
            TempoRestante = tempoRestante
        };
    }

    public async Task<List<PerformanceModel>> GetListaPerformancesAsync(int idAssociado, int idProjeto, int idPeriodo)
    {
        var associado = await _db.Associados.FirstOrDefaultAsync(a => a.Id == idAssociado);
        if (associado == null) return new List<PerformanceModel>();

        var avaliacaoPerformance = await _db.AvaliacoesPerformance.FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo);
        List<Performance> performances;
        if (avaliacaoPerformance != null)
        {
            // Busca as performances já avaliadas
            var listaPerformances = await _db.AvaliacoesPerformance.Where(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPeriodo == idPeriodo).ToListAsync();
            var listaIdPerformances = listaPerformances.Select(perf => perf.IdPerformance).ToList();
            performances = await _db.Performances.Where(p => p.IdEmpresa == associado.IdEmpresa && p.IdCargo == avaliacaoPerformance.IdCargo && p.IdNivel == associado.IdNivel && listaIdPerformances.Contains(p.IdPerformance)).ToListAsync();
        }
        else
        {
            performances = await _db.Performances.Where(p => p.IdEmpresa == associado.IdEmpresa && p.IdCargo == associado.IdCargo && p.IdNivel == associado.IdNivel).ToListAsync();
        }

        // Organiza por abrangência
        var abrangenciasContadas = new HashSet<string>();
        var listaPerformancesModel = new List<PerformanceModel>();
        foreach (var item in performances.OrderBy(p => p.Abrangencia ?? ""))
        {
            var abrangencia = item.Abrangencia?.ToUpper() ?? "";
            var headerSeparador = abrangenciasContadas.Contains(abrangencia) ? false : true;
            if (!abrangenciasContadas.Contains(abrangencia)) abrangenciasContadas.Add(abrangencia);

            listaPerformancesModel.Add(new PerformanceModel
            {
                IdPerformance = item.IdPerformance,
                Descricao = item.Performance,
                Abaixo = item.PerformanceAbaixo,
                Esperado = item.PerformanceEsperado,
                Acima = item.PerformanceAcima,
                Abrangencia = abrangencia,
                SeparadorAbrangencia = headerSeparador ? "" : "hidden",
                Input = item.InputAvaliacaoAsCegas ? "" : "",
                DisclaimerInput = item.InputAvaliacaoAsCegas ? "" : "Esta nota não requer preenchimento do avaliador"
            });
        }
        return listaPerformancesModel;
    }

    public async Task<List<NotaItemDto>> GetNotasAsync()
    {
        return await Task.FromResult(_performanceHelper.GetNotasPerformanceItems());
    }

    public async Task<bool> SalvarAvaliacaoAsync(int idProjeto, int idAssociado, int idPeriodo, List<AvaliacaoPerformanceInputDto> avaliacoes, bool finalizar = false)
    {
        try
        {
            foreach (var input in avaliacoes)
            {
                var avaliacao = await _db.AvaliacoesPerformance.FirstOrDefaultAsync(a => a.IdAssociado == idAssociado && a.IdProjeto == idProjeto && a.IdPerformance == input.IdPerformance && a.IdPeriodo == idPeriodo);
                if (avaliacao == null)
                {
                    avaliacao = new AvaliacaoPerformance
                    {
                        IdAssociado = idAssociado,
                        IdProjeto = idProjeto,
                        IdPerformance = input.IdPerformance,
                        IdPeriodo = idPeriodo,
                        IdNotaNivel1AvaliacaoCegas = input.Nota,
                        ComentariosAvaliacaoCegas = input.Comentario,
                        DataHoraInicioAvaliacaoCegas = DateTime.Now,
                        DHCAvaliacaoCegas = DateTime.Now,
                        USRAvaliacaoCegas = 0 // Ajustar para usuário logado
                    };
                    _db.AvaliacoesPerformance.Add(avaliacao);
                }
                else
                {
                    avaliacao.IdNotaNivel1AvaliacaoCegas = input.Nota;
                    avaliacao.ComentariosAvaliacaoCegas = input.Comentario;
                    avaliacao.DHCAvaliacaoCegas = DateTime.Now;
                    avaliacao.USRAvaliacaoCegas = 0; // Ajustar para usuário logado
                    if (finalizar)
                        avaliacao.DataHoraFimAvaliacaoCegas = DateTime.Now;
                }
            }
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> FinalizarAvaliacaoAsync(int idProjeto, int idAssociado, int idPeriodo, List<AvaliacaoPerformanceInputDto> avaliacoes)
    {
        return await SalvarAvaliacaoAsync(idProjeto, idAssociado, idPeriodo, avaliacoes, true);
    }
}

public class PerformanceModel
{
    public int IdPerformance { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Abaixo { get; set; } = string.Empty;
    public string Esperado { get; set; } = string.Empty;
    public string Acima { get; set; } = string.Empty;
    public string Abrangencia { get; set; } = string.Empty;
    public string SeparadorAbrangencia { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string DisclaimerInput { get; set; } = string.Empty;
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Microsoft.EntityFrameworkCore;
using Services.Pendencias.Common;

namespace Services.Pendencias.Common;

public interface IPendenciasService
{
    Task<List<ProjetoPendenciaModel>> ObterPendenciasAsync(int userId);
}

public class PendenciasService : IPendenciasService
{
    private readonly ApplicationDbContext _db;

    public PendenciasService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProjetoPendenciaModel>> ObterPendenciasAsync(int userId)
    {
        var associado = await _db.Associados
            .Include(a => a.Cargo)
            .FirstOrDefaultAsync(a => a.Id == userId);
        if (associado == null)
            return new List<ProjetoPendenciaModel>();

        var periodosServiceUltimo = await _db.PeriodosAvaliacoes
            .OrderByDescending(p => p.IdPeriodo)
            .FirstOrDefaultAsync();
        if (periodosServiceUltimo == null)
            return new List<ProjetoPendenciaModel>();

        var projetos = await _db.AssociadosProjetos
            .Include(ap => ap.Projeto)
            .Include(ap => ap.Associado)
            .Where(ap => ap.IdAssociado == associado.Id)
            .ToListAsync();

        var listaPendencias = new List<ProjetoPendenciaModel>();

        foreach (var itemProjeto in projetos)
        {
            var periodo = await _db.PeriodosAvaliacoes
                .FirstOrDefaultAsync(p => p.IdPeriodo == periodosServiceUltimo.IdPeriodo &&
                                           p.DataInicio <= itemProjeto.Projeto.DataInicio &&
                                           p.DataFim >= itemProjeto.Projeto.DataFim);
            if (periodo == null)
                continue;

            var avaliacao = await _db.AvaliacoesEmail
                .FirstOrDefaultAsync(a => a.idProjeto == itemProjeto.Projeto.Id &&
                                          a.idAssociado == itemProjeto.Associado.Id &&
                                          a.idPeriodo == periodo.IdPeriodo);
            if (avaliacao == null)
                continue;

            if (avaliacao.PosicaoAtualFluxoAvaliacao == "AFI")
                continue;

            if (avaliacao.PROJETOS != null && avaliacao.PROJETOS.ATV == 0)
                continue;

            string fotoNome = string.Empty;
            var foto = await _db.Associados
                .Where(a => a.Id == avaliacao.ASSOCIADOS.IdAssociado)
                .Select(a => a.FotoNome)
                .FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(foto))
                fotoNome = "assets/images/users/usernophoto.jpg";
            else
                fotoNome = foto;

            var model = new ProjetoPendenciaModel
            {
                FotoNome = fotoNome,
                Nome = itemProjeto.Associado.Nome,
                Projeto = itemProjeto.Projeto.Nome,
                Periodo = periodo.Nome,
                Pendencia = string.Empty,
                DataLimite = string.Empty
            };

            var prazo = await _db.Prazos.FirstOrDefaultAsync(p => p.IdPrazo == avaliacao.IdPrazo);
            if (prazo == null)
                continue;

            if (avaliacao.idAssociado == associado.Id)
            {
                if (avaliacao.PosicaoAtualFluxoAvaliacao == "NaoIniciada" ||
                    avaliacao.PosicaoAtualFluxoAvaliacao == "AutoAvaliacao" ||
                    avaliacao.PosicaoAtualFluxoAvaliacao == "AvaliacaoAsCegas" ||
                    avaliacao.PosicaoAtualFluxoAvaliacao == "EmParalelo")
                {
                    model.Pendencia = "Auto Avaliação e Avaliação às cegas";
                    var dataFinal = avaliacao.DataLiberacao?.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy") ?? string.Empty;
                    model.DataLimite = dataFinal;
                }
                else
                {
                    continue;
                }
            }
            else if (itemProjeto.IdAvaliador == associado.Id || avaliacao.idGestor == associado.Id)
            {
                if (itemProjeto.IdAvaliador == associado.Id && avaliacao.PosicaoAtualFluxoAvaliacao == "EmParalelo")
                {
                    model.Pendencia = "Auto Avaliação e Avaliação às cegas";
                    var dataFinal = avaliacao.DataLiberacao?.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy") ?? string.Empty;
                    model.DataLimite = dataFinal;
                }
                else if (avaliacao.idGestor == associado.Id && avaliacao.PosicaoAtualFluxoAvaliacao == "AvaliacaoGestor")
                {
                    model.Pendencia = avaliacao.TipoAvaliacao == "desempenho" ? "Avaliação do Gestor" : "Avaliação de Liderança";
                    var dataFinal = avaliacao.DataLiberacao?.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy") ?? string.Empty;
                    model.DataLimite = dataFinal;
                }
                else if (avaliacao.PosicaoAtualFluxoAvaliacao == "Feedback")
                {
                    model.Pendencia = "Feedback";
                    var dataFinal = avaliacao.DataLiberacao?.AddDays(prazo.DuracaoFeedback).ToString("dd/MM/yyyy") ?? string.Empty;
                    model.DataLimite = dataFinal;
                }
                else
                {
                    continue;
                }
            }
            else if (avaliacao.PosicaoAtualFluxoAvaliacao == "AvaliacaoMentor")
            {
                model.Pendencia = "Mentoria";
                var dataFinal = avaliacao.DataLiberacao?.AddDays(prazo.DuracaoMentor).ToString("dd/MM/yyyy") ?? string.Empty;
                model.DataLimite = dataFinal;
            }
            else
            {
                continue;
            }

            listaPendencias.Add(model);
        }

        return listaPendencias;
    }
}
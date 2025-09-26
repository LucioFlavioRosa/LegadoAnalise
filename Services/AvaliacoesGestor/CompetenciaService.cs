using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Services.AvaliacoesGestor.Common;

namespace Services.AvaliacoesGestor;

public interface ICompetenciaService
{
    Task<List<CompetenciaModel>> GetCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor);
    Task<bool> SalvarAvaliacaoAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor, List<CompetenciaModel> respostas, bool finalizarAvaliacao = false);
    Task<List<string>> ValidarAvaliacaoAsync(List<CompetenciaModel> respostas, Dictionary<int, int> pesos, int idNotaNaoSeAplica = 5, int idNotaSelecionar = 0, string tipoAvaliacao = "");
}

public class CompetenciaService : ICompetenciaService
{
    private readonly ApplicationDbContext _db;

    public CompetenciaService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<CompetenciaModel>> GetCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor)
    {
        // Busca as competências do associado para o projeto, período e tipo de avaliação
        // Inclui lógica de obtenção de notas e preenchimento dos campos do modelo
        var associado = await _db.Associados.Include(a => a.Cargo).FirstOrDefaultAsync(a => a.Id == idAssociado);
        if (associado == null) return new List<CompetenciaModel>();

        var projeto = await _db.Projetos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == idProjeto);
        var periodo = await _db.PeriodosAvaliacoes.FirstOrDefaultAsync(p => p.IdPeriodo == idPeriodo);
        var gestor = await _db.Associados.FirstOrDefaultAsync(a => a.Id == idGestor);

        // Obtenha as competências parametrizadas para o cargo e nível do associado
        var competencias = await _db.Competencias
            .Include(c => c.SubCompetencia)
            .Include(c => c.Eixo)
            .Include(c => c.Dimensao)
            .Where(c => c.IdCargo == associado.IdCargo && c.TipoAvaliacao == tipoAvaliacao)
            .ToListAsync();

        // Obtenha as avaliações já existentes para o associado
        var avaliacoes = await _db.AvaliacoesCompetenciasNotas
            .Where(a => a.IdNota > 0)
            .ToListAsync();

        // Obtenha pesos das notas
        var pesos = avaliacoes.ToDictionary(n => n.IdNota, n => n.Peso);

        var lista = new List<CompetenciaModel>();
        foreach (var comp in competencias)
        {
            var model = new CompetenciaModel
            {
                IdCompetencia = comp.IdCompetencia,
                SubCompetencia = comp.SubCompetencia?.Nome ?? string.Empty,
                PalavrasChave = comp.PalavrasChave ?? string.Empty,
                Eixo = comp.Eixo?.Nome ?? string.Empty,
                Dimensao = comp.Dimensao?.Nome ?? string.Empty,
                IdModo = comp.IdModo,
                // Preencher outros campos conforme necessário
            };
            // Lógica para buscar notas e comentários já preenchidos
            // Pode ser expandida conforme o modelo de dados real
            lista.Add(model);
        }
        return lista;
    }

    public async Task<bool> SalvarAvaliacaoAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idGestor, List<CompetenciaModel> respostas, bool finalizarAvaliacao = false)
    {
        // Salva ou atualiza as avaliações de competência para o associado
        foreach (var resposta in respostas)
        {
            var avaliacao = await _db.AvaliacoesCompetenciasNotas
                .FirstOrDefaultAsync(a => a.IdNota == resposta.IdCompetencia);
            if (avaliacao != null)
            {
                // Atualize os campos necessários
                // Exemplo: avaliacao.NotaNivel1 = resposta.notaValidaNivel1;
                //          avaliacao.NotaNivel2 = resposta.notaValidaNivel2;
                //          avaliacao.Comentarios = resposta.Comentarios;
                //          avaliacao.DataHoraFim = finalizarAvaliacao ? DateTime.Now : (DateTime?)null;
            }
            else
            {
                // Crie nova avaliação se necessário
                // _db.AvaliacoesCompetenciasNotas.Add(new AvaliacaoCompetenciaNota { ... });
            }
        }
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<string>> ValidarAvaliacaoAsync(List<CompetenciaModel> respostas, Dictionary<int, int> pesos, int idNotaNaoSeAplica = 5, int idNotaSelecionar = 0, string tipoAvaliacao = "")
    {
        var mensagens = new List<string>();
        mensagens.AddRange(CompetenciaHelper.ValidarNotas(respostas, pesos, idNotaNaoSeAplica, idNotaSelecionar));
        mensagens.AddRange(CompetenciaHelper.ValidarPilares(respostas, idNotaNaoSeAplica, tipoAvaliacao));
        return mensagens;
    }
}

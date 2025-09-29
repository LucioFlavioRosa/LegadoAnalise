using Peers.Moderno.Models;
using Peers.Moderno.Data;
using Services.AvaliacoesGestor.Common;
using Services.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Services.AvaliacoesGestor.Common;

public class AvaliacoesGestorService : IAvaliacoesGestorService
{
    private readonly ApplicationDbContext _db;
    private readonly IComboHelper _comboHelper;
    private readonly IAvaliacoesGestorHelper _helper;

    public AvaliacoesGestorService(ApplicationDbContext db, IComboHelper comboHelper, IAvaliacoesGestorHelper helper)
    {
        _db = db;
        _comboHelper = comboHelper;
        _helper = helper;
    }

    public async Task<List<ComboItem>> ObterProjetosComboAsync(int idGestor)
    {
        var projetos = await _db.Projetos
            .Where(p => p.AssociadoGestor != null && p.AssociadoGestor.Id == idGestor && p.Ativo)
            .Select(p => new ComboItem { Value = p.Id.ToString(), Text = p.Nome })
            .ToListAsync();
        projetos.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });
        return projetos;
    }

    public async Task<List<ComboItem>> ObterClientesComboAsync()
    {
        var clientes = await _db.Clientes
            .Where(c => c.Ativo)
            .Select(c => new ComboItem { Value = c.IdCliente.ToString(), Text = c.Nome })
            .ToListAsync();
        clientes.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });
        return clientes;
    }

    public async Task<List<ComboItem>> ObterPeriodosComboAsync(int idEmpresa)
    {
        var periodos = await _db.PeriodosAvaliacoes
            .Where(p => p.IdEmpresa == idEmpresa)
            .Select(p => new ComboItem { Value = p.IdPeriodo.ToString(), Text = p.Nome })
            .ToListAsync();
        periodos.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });
        return periodos;
    }

    public async Task<List<ComboItem>> ObterStatusComboAsync()
    {
        var status = await _db.ProjetosComplexidades
            .Select(s => new ComboItem { Value = s.IdComplexidade.ToString(), Text = s.Nome })
            .ToListAsync();
        status.Insert(0, new ComboItem { Value = "", Text = "[Selecionar]" });
        return status;
    }

    public async Task<List<ProjetoModel>> BuscarProjetosAvaliacoesAsync(int idGestor, int? idProjeto = null, int? idStatus = null, int? idPeriodo = null, int? idCliente = null)
    {
        var query = _db.Projetos
            .Include(p => p.Cliente)
            .Include(p => p.AssociadoResponsavel)
            .Include(p => p.AssociadoGestor)
            .Include(p => p.AssociadosProjeto)
            .ThenInclude(ap => ap.Associado)
            .Where(p => p.AssociadoGestor != null && p.AssociadoGestor.Id == idGestor && p.Ativo)
            .AsQueryable();

        if (idProjeto.HasValue)
            query = query.Where(p => p.Id == idProjeto.Value);
        if (idStatus.HasValue)
            query = query.Where(p => p.Complexidade != null && p.Complexidade.IdComplexidade == idStatus.Value);
        if (idCliente.HasValue)
            query = query.Where(p => p.Cliente != null && p.Cliente.IdCliente == idCliente.Value);

        var projetos = await query.ToListAsync();
        var lista = new List<ProjetoModel>();
        foreach (var p in projetos)
        {
            var model = new ProjetoModel
            {
                Id = p.Id,
                Nome = p.Nome,
                DataInicio = p.DataInicio.ToString("dd/MM/yyyy"),
                DataTermino = p.DataFim?.ToString("dd/MM/yyyy"),
                Gestor = p.AssociadoGestor,
                Responsavel = p.AssociadoResponsavel,
                Cliente = p.Cliente,
                Status = p.Complexidade,
                Associados = new List<ProjetosAssociadosModel>()
            };
            var associadosProjeto = p.AssociadosProjeto.ToList();
            foreach (var ap in associadosProjeto)
            {
                if (!_helper.DeveIncluirAssociadoNoFluxo(ap, idPeriodo))
                    continue;
                var associadoModel = await _helper.MapearProjetosAssociadosModelAsync(ap, idPeriodo, _db);
                if (associadoModel != null)
                    model.Associados.Add(associadoModel);
            }
            if (model.Associados.Any())
                lista.Add(model);
        }
        return lista;
    }

    public async Task<bool> FinalizarAvaliacaoAsync(int idAvaliacao, int usuarioId)
    {
        return await _helper.FinalizarAvaliacaoAsync(idAvaliacao, usuarioId, _db);
    }

    public async Task<ProjetoModel?> ObterProjetoDetalhadoAsync(int idProjeto, int idGestor, int? idPeriodo = null)
    {
        var projetos = await BuscarProjetosAvaliacoesAsync(idGestor, idProjeto, null, idPeriodo, null);
        return projetos.FirstOrDefault();
    }
}
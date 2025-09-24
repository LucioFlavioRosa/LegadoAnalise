using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Avaliacoes.Common;
using Peers.Moderno.Services.Associados;
using Peers.Moderno.Services.Clientes.Common;
using Peers.Moderno.Services.Projetos;
using Peers.Moderno.Services.Common;
using Peers.Moderno.Services.Cargos;

namespace Peers.Moderno.Services.Avaliacoes;

public class AutoAvaliacaoService : IAutoAvaliacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly IAssociadosService _associadosService;
    private readonly IClientesService _clientesService;
    private readonly IProjetosService _projetosService;
    private readonly ICargosService _cargosService;
    private readonly ITelemetryService _telemetryService;
    private readonly IMessageBoxService _messageBoxService;

    public AutoAvaliacaoService(
        ApplicationDbContext context,
        IAssociadosService associadosService,
        IClientesService clientesService,
        IProjetosService projetosService,
        ICargosService cargosService,
        ITelemetryService telemetryService,
        IMessageBoxService messageBoxService)
    {
        _context = context;
        _associadosService = associadosService;
        _clientesService = clientesService;
        _projetosService = projetosService;
        _cargosService = cargosService;
        _telemetryService = telemetryService;
        _messageBoxService = messageBoxService;
    }

    public async Task<List<ComboItem>> CarregarComboProjetosAsync(int usuarioId)
    {
        try
        {
            var usuario = await _associadosService.ObterAssociadoAsync(usuarioId);
            if (usuario == null)
                return new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };

            var projetos = await _context.Projetos
                .Include(p => p.AssociadosProjeto)
                .Where(p => p.AssociadosProjeto.Any(ap => ap.IdAssociado == usuarioId))
                .Where(p => p.Ativo)
                .OrderBy(p => p.Nome)
                .Select(p => new ComboItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome
                })
                .ToListAsync();

            var items = new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
            items.AddRange(projetos);

            _telemetryService.TrackEvent("ComboProjetosCarregado", new Dictionary<string, string>
            {
                { "UsuarioId", usuarioId.ToString() },
                { "QuantidadeProjetos", projetos.Count.ToString() }
            });

            return items;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarComboProjetosAsync" },
                { "UsuarioId", usuarioId.ToString() }
            });
            return new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
        }
    }

    public async Task<List<ComboItem>> CarregarComboClientesAsync()
    {
        try
        {
            var clientes = await _context.Clientes
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .Select(c => new ComboItem
                {
                    Value = c.IdCliente.ToString(),
                    Text = c.Nome
                })
                .ToListAsync();

            var items = new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
            items.AddRange(clientes);

            return items;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarComboClientesAsync" }
            });
            return new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
        }
    }

    public async Task<List<ComboItem>> CarregarComboPeriodosAsync(int empresaId)
    {
        try
        {
            var periodos = await _context.Set<PeriodoAvaliacao>()
                .Where(p => p.IdEmpresa == empresaId)
                .OrderByDescending(p => p.DataInicio)
                .Select(p => new ComboItem
                {
                    Value = p.IdPeriodo.ToString(),
                    Text = p.Periodo
                })
                .ToListAsync();

            var items = new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
            items.AddRange(periodos);

            return items;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarComboPeriodosAsync" },
                { "EmpresaId", empresaId.ToString() }
            });
            return new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
        }
    }

    public async Task<List<ComboItem>> CarregarComboStatusAsync()
    {
        try
        {
            var statusList = await _context.Set<StatusAvaliacao>()
                .OrderBy(s => s.Status)
                .Select(s => new ComboItem
                {
                    Value = s.IdStatus.ToString(),
                    Text = s.Status
                })
                .ToListAsync();

            var items = new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
            items.AddRange(statusList);

            return items;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CarregarComboStatusAsync" }
            });
            return new List<ComboItem> { ComboHelper.GetDefaultSelectionItem() };
        }
    }

    public async Task<List<ProjetoModel>> BuscarAvaliacoesFiltradasAsync(int usuarioId, FiltroAutoAvaliacaoModel filtro)
    {
        try
        {
            var usuario = await _associadosService.ObterAssociadoAsync(usuarioId);
            if (usuario == null)
                return new List<ProjetoModel>();

            var query = _context.Projetos
                .Include(p => p.Cliente)
                .Include(p => p.AssociadoResponsavel)
                .Include(p => p.AssociadoGestor)
                .Include(p => p.AssociadosProjeto)
                    .ThenInclude(ap => ap.Associado)
                        .ThenInclude(a => a.Cargo)
                .Include(p => p.AssociadosProjeto)
                    .ThenInclude(ap => ap.Avaliador)
                .Where(p => p.AssociadosProjeto.Any(ap => ap.IdAssociado == usuarioId));

            if (filtro.IdProjeto.HasValue && filtro.IdProjeto > 0)
                query = query.Where(p => p.Id == filtro.IdProjeto.Value);

            if (filtro.IdCliente.HasValue && filtro.IdCliente > 0)
                query = query.Where(p => p.IdCliente == filtro.IdCliente.Value);

            var projetos = await query.ToListAsync();
            var listaProjetosAvaliacoes = new List<ProjetoModel>();

            foreach (var projeto in projetos)
            {
                var projetoModel = new ProjetoModel
                {
                    Id = projeto.Id,
                    Nome = projeto.Nome,
                    DataInicio = projeto.DataInicio.ToString("dd/MM/yyyy"),
                    DataTermino = projeto.DataTermino?.ToString("dd/MM/yyyy"),
                    Cliente = projeto.Cliente,
                    Responsavel = projeto.AssociadoResponsavel,
                    Gestor = projeto.AssociadoGestor,
                    Status = new StatusProjeto { Status = projeto.Ativo ? "Ativo" : "Inativo" },
                    Associados = new List<ProjetosAssociadosModel>()
                };

                var associadosProjeto = projeto.AssociadosProjeto
                    .Where(ap => ap.IdAssociado == usuarioId && ap.TipoAvaliacao == "desempenho")
                    .ToList();

                foreach (var associadoProjeto in associadosProjeto)
                {
                    var associadoModel = await CriarProjetosAssociadosModelAsync(associadoProjeto, filtro);
                    if (associadoModel != null)
                    {
                        projetoModel.Associados.Add(associadoModel);
                    }
                }

                if (projetoModel.Associados.Any())
                {
                    listaProjetosAvaliacoes.Add(projetoModel);
                }
            }

            _telemetryService.TrackEvent("AvaliacoesFiltradasBuscadas", new Dictionary<string, string>
            {
                { "UsuarioId", usuarioId.ToString() },
                { "QuantidadeProjetos", listaProjetosAvaliacoes.Count.ToString() }
            });

            return listaProjetosAvaliacoes;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "BuscarAvaliacoesFiltradasAsync" },
                { "UsuarioId", usuarioId.ToString() }
            });
            return new List<ProjetoModel>();
        }
    }

    public async Task<bool> FinalizarAvaliacaoAsync(int idAvaliacao, int usuarioId)
    {
        try
        {
            var avaliacaoEmail = await _context.Set<AvaliacaoEmail>()
                .FirstOrDefaultAsync(ae => ae.IdAvaliacao == idAvaliacao);

            if (avaliacaoEmail == null)
            {
                _messageBoxService.ShowError("Avaliação não encontrada");
                return false;
            }

            var avaliacoesCompetencia = await _context.Set<AvaliacaoCompetencia>()
                .Where(ac => ac.IdAssociado == avaliacaoEmail.IdAssociado &&
                           ac.IdProjeto == avaliacaoEmail.IdProjeto &&
                           ac.IdPeriodo == avaliacaoEmail.IdPeriodo &&
                           ac.IdAvaliacao == idAvaliacao)
                .ToListAsync();

            var avaliacoesPerformance = await _context.Set<AvaliacaoPerformance>()
                .Where(ap => ap.IdAssociado == avaliacaoEmail.IdAssociado &&
                           ap.IdProjeto == avaliacaoEmail.IdProjeto &&
                           ap.IdPeriodo == avaliacaoEmail.IdPeriodo)
                .ToListAsync();

            if (!avaliacoesCompetencia.Any())
            {
                _messageBoxService.ShowWarning("Não existem Competências parametrizadas para este cargo. Não é possível dar andamento na avaliação !!");
                return false;
            }

            if (!avaliacoesPerformance.Any())
            {
                _messageBoxService.ShowWarning("Não existem Performances parametrizadas para este cargo ou você ainda não iniciou o preenchimento das Performances. Não é possível dar andamento na avaliação !!");
                return false;
            }

            foreach (var avaliacao in avaliacoesCompetencia)
            {
                if (avaliacao.IdNotaNivel1AutoAvaliacao <= 0 || avaliacao.IdNotaNivel2AutoAvaliacao <= 0)
                {
                    _messageBoxService.ShowWarning("É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação");
                    return false;
                }
            }

            foreach (var avaliacao in avaliacoesPerformance)
            {
                if (avaliacao.IdNotaNivel1AutoAvaliacao <= 0)
                {
                    _messageBoxService.ShowWarning("É obrigatório digitar todas as notas da Avaliação Performance antes de finalizar a Avaliação");
                    return false;
                }
            }

            var dataHoraFinalizacao = DateTime.Now;

            foreach (var avaliacao in avaliacoesPerformance)
            {
                avaliacao.DataHoraFimAutoAvaliacao = dataHoraFinalizacao;
                _context.Update(avaliacao);
            }

            foreach (var avaliacao in avaliacoesCompetencia)
            {
                avaliacao.DataHoraFimAutoAvaliacao = dataHoraFinalizacao;
                _context.Update(avaliacao);
            }

            await _context.SaveChangesAsync();

            _telemetryService.TrackEvent("AvaliacaoFinalizada", new Dictionary<string, string>
            {
                { "IdAvaliacao", idAvaliacao.ToString() },
                { "UsuarioId", usuarioId.ToString() }
            });

            return true;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "FinalizarAvaliacaoAsync" },
                { "IdAvaliacao", idAvaliacao.ToString() },
                { "UsuarioId", usuarioId.ToString() }
            });
            _messageBoxService.ShowError("Erro ao finalizar avaliação");
            return false;
        }
    }

    public async Task<AvaliacaoStatusModel> ObterStatusAvaliacaoAsync(int idAvaliacao)
    {
        try
        {
            var avaliacaoEmail = await _context.Set<AvaliacaoEmail>()
                .FirstOrDefaultAsync(ae => ae.IdAvaliacao == idAvaliacao);

            if (avaliacaoEmail == null)
                return new AvaliacaoStatusModel();

            var avaliacaoCompetencia = await _context.Set<AvaliacaoCompetencia>()
                .FirstOrDefaultAsync(ac => ac.IdAvaliacao == idAvaliacao);

            var status = new AvaliacaoStatusModel
            {
                IdAvaliacao = idAvaliacao,
                PodeSerFinalizada = avaliacaoEmail.Liberado
            };

            if (avaliacaoCompetencia?.DataHoraFimAutoAvaliacao != null)
            {
                status.Status = "Concluído";
                status.RotuloBotao = "Ver Avaliação";
                status.ExibirBotaoFinalizar = false;
            }
            else if (avaliacaoEmail.PosicaoAtualFluxoAvaliacao == "nao_iniciada")
            {
                status.Status = "Não iniciado";
                status.RotuloBotao = "Iniciar Avaliação";
                status.ExibirBotaoFinalizar = false;
            }
            else
            {
                status.Status = "Em andamento";
                status.RotuloBotao = "Continuar Avaliação";
                status.ExibirBotaoFinalizar = true;
            }

            return status;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ObterStatusAvaliacaoAsync" },
                { "IdAvaliacao", idAvaliacao.ToString() }
            });
            return new AvaliacaoStatusModel();
        }
    }

    public async Task<bool> ValidarPermissaoFinalizacaoAsync(int idAvaliacao, int usuarioId)
    {
        try
        {
            var avaliacaoEmail = await _context.Set<AvaliacaoEmail>()
                .FirstOrDefaultAsync(ae => ae.IdAvaliacao == idAvaliacao && ae.IdAssociado == usuarioId);

            return avaliacaoEmail != null && avaliacaoEmail.Liberado;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "ValidarPermissaoFinalizacaoAsync" },
                { "IdAvaliacao", idAvaliacao.ToString() },
                { "UsuarioId", usuarioId.ToString() }
            });
            return false;
        }
    }

    private async Task<ProjetosAssociadosModel?> CriarProjetosAssociadosModelAsync(AssociadoProjeto associadoProjeto, FiltroAutoAvaliacaoModel filtro)
    {
        try
        {
            var modelo = new ProjetosAssociadosModel
            {
                Id = associadoProjeto.Id,
                Associado = associadoProjeto.Associado,
                DataInicio = associadoProjeto.DataInicio.ToString("dd/MM/yyyy"),
                DataTermino = associadoProjeto.DataTermino?.ToString("dd/MM/yyyy") ?? "",
                TipoAvaliacao = associadoProjeto.TipoAvaliacao,
                Escopo = associadoProjeto.Escopo
            };

            if (associadoProjeto.Associado?.FotoNome == null || associadoProjeto.Associado.FotoNome == "")
            {
                modelo.FotoAssociado = new FotoAssociado { Imagem = "assets/images/users/usernophoto.jpg" };
            }
            else
            {
                modelo.FotoAssociado = new FotoAssociado { Imagem = associadoProjeto.Associado.FotoNome.Replace(" ", "%20") };
            }

            var statusAvaliacao = await ObterStatusAvaliacaoAsync(associadoProjeto.Id);
            modelo.Status = new StatusAvaliacao { Status = statusAvaliacao.Status };
            modelo.RotuloBotao = statusAvaliacao.RotuloBotao;
            modelo.ExibirBotaoFinalizar = statusAvaliacao.ExibirBotaoFinalizar;
            modelo.Etapa = statusAvaliacao.Etapa;

            return modelo;
        }
        catch (Exception ex)
        {
            _telemetryService.TrackException(ex, new Dictionary<string, string>
            {
                { "Method", "CriarProjetosAssociadosModelAsync" },
                { "AssociadoProjetoId", associadoProjeto?.Id.ToString() ?? "Unknown" }
            });
            return null;
        }
    }
}
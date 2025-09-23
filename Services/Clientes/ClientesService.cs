using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Services.Clientes.Common;
using Peers.Moderno.Services.Common;

namespace Peers.Moderno.Services.Clientes;

public class ClientesService : IClientesService
{
    private readonly ApplicationDbContext _context;
    private readonly IMessageBoxService _messageBoxService;

    public ClientesService(ApplicationDbContext context, IMessageBoxService messageBoxService)
    {
        _context = context;
        _messageBoxService = messageBoxService;
    }

    public async Task<IEnumerable<ClienteDto>> ObterListaClientesAsync()
    {
        try
        {
            var clientes = await _context.Set<Models.Cliente>()
                .Include(c => c.AssociadoResponsavel)
                .Select(c => new ClienteDto
                {
                    IdCliente = c.IdCliente,
                    Cliente = c.Cliente,
                    Email = c.Email,
                    Telefones = c.Telefones,
                    GestorCliente = c.GestorCliente,
                    IdAssociacoResponsavel = c.IdAssociacoResponsavel,
                    ATV = c.ATV,
                    NomeSocio = c.AssociadoResponsavel != null ? c.AssociadoResponsavel.Nome : ""
                })
                .ToListAsync();

            return clientes;
        }
        catch (Exception ex)
        {
            _messageBoxService.ShowError($"Erro ao carregar lista de clientes: {ex.Message}");
            return new List<ClienteDto>();
        }
    }

    public async Task<ClienteDto?> ObterClienteAsync(int idCliente)
    {
        try
        {
            var cliente = await _context.Set<Models.Cliente>()
                .Include(c => c.AssociadoResponsavel)
                .Where(c => c.IdCliente == idCliente)
                .Select(c => new ClienteDto
                {
                    IdCliente = c.IdCliente,
                    Cliente = c.Cliente,
                    Email = c.Email,
                    Telefones = c.Telefones,
                    GestorCliente = c.GestorCliente,
                    IdAssociacoResponsavel = c.IdAssociacoResponsavel,
                    ATV = c.ATV,
                    IdEmpresa = c.IdEmpresa,
                    USR = c.USR,
                    DHC = c.DHC
                })
                .FirstOrDefaultAsync();

            return cliente;
        }
        catch (Exception ex)
        {
            _messageBoxService.ShowError($"Erro ao carregar cliente: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> InserirClienteAsync(ClienteDto clienteDto)
    {
        try
        {
            if (!await ValidarClienteAsync(clienteDto))
                return false;

            var cliente = new Models.Cliente
            {
                Cliente = clienteDto.Cliente,
                Telefones = clienteDto.Telefones,
                IdAssociacoResponsavel = clienteDto.IdAssociacoResponsavel,
                GestorCliente = clienteDto.GestorCliente,
                Email = clienteDto.Email,
                IdEmpresa = clienteDto.IdEmpresa,
                USR = clienteDto.USR,
                DHC = DateTime.Now,
                ATV = clienteDto.ATV
            };

            _context.Set<Models.Cliente>().Add(cliente);
            await _context.SaveChangesAsync();

            _messageBoxService.ShowSuccess("Cliente inserido com sucesso!");
            return true;
        }
        catch (Exception ex)
        {
            _messageBoxService.ShowError($"Erro ao inserir cliente: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> AlterarClienteAsync(ClienteDto clienteDto)
    {
        try
        {
            if (!await ValidarClienteAsync(clienteDto))
                return false;

            var cliente = await _context.Set<Models.Cliente>()
                .FirstOrDefaultAsync(c => c.IdCliente == clienteDto.IdCliente);

            if (cliente == null)
            {
                _messageBoxService.ShowError("Cliente não encontrado!");
                return false;
            }

            cliente.Cliente = clienteDto.Cliente;
            cliente.Telefones = clienteDto.Telefones;
            cliente.IdAssociacoResponsavel = clienteDto.IdAssociacoResponsavel;
            cliente.GestorCliente = clienteDto.GestorCliente;
            cliente.Email = clienteDto.Email;
            cliente.ATV = clienteDto.ATV;

            await _context.SaveChangesAsync();

            _messageBoxService.ShowSuccess("Cliente alterado com sucesso!");
            return true;
        }
        catch (Exception ex)
        {
            _messageBoxService.ShowError($"Erro ao alterar cliente: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ExcluirClienteAsync(int idCliente)
    {
        try
        {
            var cliente = await _context.Set<Models.Cliente>()
                .FirstOrDefaultAsync(c => c.IdCliente == idCliente);

            if (cliente == null)
            {
                _messageBoxService.ShowError("Cliente não encontrado!");
                return false;
            }

            cliente.ATV = 0;
            await _context.SaveChangesAsync();

            _messageBoxService.ShowSuccess("Cliente inativado com sucesso!");
            return true;
        }
        catch (Exception ex)
        {
            _messageBoxService.ShowError($"Erro ao inativar cliente: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<SocioDto>> ObterSociosAsync()
    {
        try
        {
            var socios = await _context.Associados
                .Where(a => a.Ativo && a.Socio)
                .Select(a => new SocioDto
                {
                    IdAssociado = a.Id,
                    Nome = a.Nome
                })
                .OrderBy(s => s.Nome)
                .ToListAsync();

            return socios;
        }
        catch (Exception ex)
        {
            _messageBoxService.ShowError($"Erro ao carregar sócios: {ex.Message}");
            return new List<SocioDto>();
        }
    }

    public async Task<bool> ValidarClienteAsync(ClienteDto cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Cliente))
        {
            _messageBoxService.ShowWarning("Campo Cliente não preenchido!");
            return false;
        }

        if (cliente.IdAssociacoResponsavel <= 0)
        {
            _messageBoxService.ShowWarning("Campo Sócio Responsável não preenchido!");
            return false;
        }

        if (string.IsNullOrWhiteSpace(cliente.GestorCliente))
        {
            _messageBoxService.ShowWarning("Campo Gestor do Cliente não preenchido!");
            return false;
        }

        if (string.IsNullOrWhiteSpace(cliente.Email))
        {
            _messageBoxService.ShowWarning("Campo E-mail do Cliente não preenchido!");
            return false;
        }

        if (string.IsNullOrWhiteSpace(cliente.Telefones))
        {
            _messageBoxService.ShowWarning("Campo Telefone não preenchido!");
            return false;
        }

        return true;
    }
}
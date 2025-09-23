using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Clientes.Common;

namespace Peers.Moderno.Services.Clientes;

public class ClienteService : IClienteService
{
    private readonly ApplicationDbContext _context;

    public ClienteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClienteDto>> ObterClientesAsync()
    {
        return await _context.Set<Cliente>()
            .Select(c => new ClienteDto
            {
                Id = c.IdCliente,
                Cliente = c.Cliente1,
                Email = c.Email,
                Telefone = c.Telefones,
                GestorCliente = c.GestorCliente,
                IdAssociadoResponsavel = c.IdAssociadoResponsavel,
                Ativo = c.ATV == 1
            })
            .OrderBy(c => c.Cliente)
            .ToListAsync();
    }

    public async Task<ClienteDto?> ObterClientePorIdAsync(int id)
    {
        var cliente = await _context.Set<Cliente>()
            .FirstOrDefaultAsync(c => c.IdCliente == id);

        if (cliente == null)
            return null;

        return new ClienteDto
        {
            Id = cliente.IdCliente,
            Cliente = cliente.Cliente1,
            Email = cliente.Email,
            Telefone = cliente.Telefones,
            GestorCliente = cliente.GestorCliente,
            IdAssociadoResponsavel = cliente.IdAssociadoResponsavel,
            Ativo = cliente.ATV == 1
        };
    }

    public async Task<bool> InserirClienteAsync(ClienteDto clienteDto)
    {
        try
        {
            var cliente = new Cliente
            {
                Cliente1 = clienteDto.Cliente,
                Email = clienteDto.Email,
                Telefones = clienteDto.Telefone,
                GestorCliente = clienteDto.GestorCliente,
                IdAssociadoResponsavel = clienteDto.IdAssociadoResponsavel,
                ATV = clienteDto.Ativo ? 1 : 0,
                DHC = DateTime.Now,
                USR = 1 // TODO: Obter do contexto de autenticação
            };

            _context.Set<Cliente>().Add(cliente);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AlterarClienteAsync(ClienteDto clienteDto)
    {
        try
        {
            var cliente = await _context.Set<Cliente>()
                .FirstOrDefaultAsync(c => c.IdCliente == clienteDto.Id);

            if (cliente == null)
                return false;

            cliente.Cliente1 = clienteDto.Cliente;
            cliente.Email = clienteDto.Email;
            cliente.Telefones = clienteDto.Telefone;
            cliente.GestorCliente = clienteDto.GestorCliente;
            cliente.IdAssociadoResponsavel = clienteDto.IdAssociadoResponsavel;
            cliente.ATV = clienteDto.Ativo ? 1 : 0;
            cliente.DHC = DateTime.Now;
            cliente.USR = 1; // TODO: Obter do contexto de autenticação

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ExcluirClienteAsync(int id)
    {
        try
        {
            var cliente = await _context.Set<Cliente>()
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return false;

            cliente.ATV = 0;
            cliente.DHC = DateTime.Now;
            cliente.USR = 1; // TODO: Obter do contexto de autenticação

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<ClienteDto>> ObterClientesAtivosAsync()
    {
        return await _context.Set<Cliente>()
            .Where(c => c.ATV == 1)
            .Select(c => new ClienteDto
            {
                Id = c.IdCliente,
                Cliente = c.Cliente1,
                Email = c.Email,
                Telefone = c.Telefones,
                GestorCliente = c.GestorCliente,
                IdAssociadoResponsavel = c.IdAssociadoResponsavel,
                Ativo = true
            })
            .OrderBy(c => c.Cliente)
            .ToListAsync();
    }

    public async Task<List<ClienteDto>> ObterClientesPorSocioAsync(int idSocio)
    {
        return await _context.Set<Cliente>()
            .Where(c => c.IdAssociadoResponsavel == idSocio && c.ATV == 1)
            .Select(c => new ClienteDto
            {
                Id = c.IdCliente,
                Cliente = c.Cliente1,
                Email = c.Email,
                Telefone = c.Telefones,
                GestorCliente = c.GestorCliente,
                IdAssociadoResponsavel = c.IdAssociadoResponsavel,
                Ativo = true
            })
            .OrderBy(c => c.Cliente)
            .ToListAsync();
    }
}
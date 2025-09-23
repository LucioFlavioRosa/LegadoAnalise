using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Data;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Associados;

public interface IAssociadosService
{
    Task<List<AssociadoListaItem>> ObterAssociadosListaAsync();
    Task<Associado?> ObterAssociadoPorIdAsync(int id);
    Task<Associado?> ObterAssociadoPorEmailAsync(string email);
    Task<Associado?> ObterUltimoAssociadoAsync();
    Task<bool> InserirAssociadoAsync(Associado associado);
    Task<bool> AlterarAssociadoAsync(Associado associado);
    Task<bool> InativarAssociadoAsync(int id);
}

public class AssociadosService : IAssociadosService
{
    private readonly ApplicationDbContext _context;

    public AssociadosService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AssociadoListaItem>> ObterAssociadosListaAsync()
    {
        return await _context.Associados
            .Include(a => a.Cargo)
            .Include(a => a.Mentor)
            .Where(a => a.Ativo)
            .Select(a => new AssociadoListaItem
            {
                Id = a.Id,
                Nome = a.Nome,
                NomeCargo = a.Cargo != null ? a.Cargo.Nome : string.Empty,
                NomeMentor = a.Mentor != null ? a.Mentor.Nome : string.Empty,
                Ativo = a.Ativo
            })
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Associado?> ObterAssociadoPorIdAsync(int id)
    {
        return await _context.Associados
            .Include(a => a.Cargo)
            .Include(a => a.Mentor)
            .Include(a => a.Perfil)
            .Include(a => a.Vertical)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Associado?> ObterAssociadoPorEmailAsync(string email)
    {
        return await _context.Associados
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<Associado?> ObterUltimoAssociadoAsync()
    {
        return await _context.Associados
            .OrderByDescending(a => a.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> InserirAssociadoAsync(Associado associado)
    {
        try
        {
            associado.DataCriacao = DateTime.Now;
            associado.DataAlteracao = DateTime.Now;
            
            _context.Associados.Add(associado);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AlterarAssociadoAsync(Associado associado)
    {
        try
        {
            var associadoExistente = await _context.Associados.FindAsync(associado.Id);
            if (associadoExistente == null)
                return false;

            associadoExistente.Nome = associado.Nome;
            associadoExistente.Email = associado.Email;
            associadoExistente.IdCargo = associado.IdCargo;
            associadoExistente.IdMentor = associado.IdMentor;
            associadoExistente.IdPerfil = associado.IdPerfil;
            associadoExistente.IdVertical = associado.IdVertical;
            associadoExistente.Senha = associado.Senha;
            associadoExistente.DataAdmissao = associado.DataAdmissao;
            associadoExistente.Ativo = associado.Ativo;
            associadoExistente.FotoBase64 = associado.FotoBase64;
            associadoExistente.DataAlteracao = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> InativarAssociadoAsync(int id)
    {
        try
        {
            var associado = await _context.Associados.FindAsync(id);
            if (associado == null)
                return false;

            associado.Ativo = false;
            associado.DataAlteracao = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
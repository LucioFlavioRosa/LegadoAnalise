using Peers.Moderno.Data;
using Peers.Moderno.Models;
using Peers.Moderno.Services.Feedback.Common;
using Microsoft.EntityFrameworkCore;

namespace Peers.Moderno.Services.Feedback;

public interface IFeedbackService
{
    Task<List<Competencia>> GetCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao);
    Task<bool> SalvarFeedbackAsync(List<FeedbackCompetenciaInput> feedbacks, int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao, bool finalizar = false);
}

public class FeedbackService : IFeedbackService
{
    private readonly ApplicationDbContext _context;
    private readonly IFeedbackValidator _validator;

    public FeedbackService(ApplicationDbContext context, IFeedbackValidator validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<List<Competencia>> GetCompetenciasAsync(int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao)
    {
        // Exemplo: buscar competências vinculadas ao associado/projeto/periodo
        return await _context.Competencias
            .Include(c => c.Cargo)
            .Include(c => c.Eixo)
            .Include(c => c.SubCompetencia)
            .Include(c => c.Dimensao)
            .Where(c => c.Cargo.IdCargo == _context.Associados.Where(a => a.Id == idAssociado).Select(a => a.IdCargo).FirstOrDefault())
            .ToListAsync();
    }

    public async Task<bool> SalvarFeedbackAsync(List<FeedbackCompetenciaInput> feedbacks, int idAssociado, int idProjeto, int idPeriodo, string tipoAvaliacao, string escopo, int idAvaliacao, bool finalizar = false)
    {
        foreach (var feedback in feedbacks)
        {
            var competencia = await _context.Competencias.FindAsync(feedback.IdCompetencia);
            if (competencia == null)
                continue;
            if (!_validator.ValidarNotas(feedback.NotaNivel1, feedback.NotaNivel2))
                return false;
            // Aqui seria feita a atualização dos campos de feedback na entidade apropriada
            // Exemplo: competencia.NotaNivel1Feedback = feedback.NotaNivel1;
            //          competencia.NotaNivel2Feedback = feedback.NotaNivel2;
            //          competencia.ComentariosFeedback = feedback.Comentario;
        }
        await _context.SaveChangesAsync();
        return true;
    }
}

public class FeedbackCompetenciaInput
{
    public int IdCompetencia { get; set; }
    public int NotaNivel1 { get; set; }
    public int NotaNivel2 { get; set; }
    public string Comentario { get; set; } = string.Empty;
}

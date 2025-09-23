using Peers.Moderno.Models;

namespace Peers.Moderno.Services.Competencias.Common;

public interface ICompetenciasService
{
    Task<bool> CadastrarAsync(Competencia competencia);
    Task<bool> AlterarAsync(Competencia competencia);
    Task<bool> ExcluirAsync(int idCompetencia);
    Task<List<Competencia>> ListarAsync(bool apenasAtivas = true);
    Task<Competencia?> ObterPorIdAsync(int idCompetencia);
    Task<byte[]> ExportarAsync();
    Task<ImportResult> ImportarAsync(Stream fileStream, int idEmpresa, int idUsuario);
    Task<int> AgregarCompetenciaAvaliacoesAsync(int idCompetencia);
    Task<RelacaoCargoSubcompetencia?> ObterRelacaoCargoSubcompetenciaAsync(int idCargo, int idSubcompetencia);
}

public class ImportResult
{
    public int LinhasInseridas { get; set; }
    public int LinhasAlteradas { get; set; }
    public int LinhasDesconsideradas { get; set; }
    public int LinhasComErro { get; set; }
    public List<string> Erros { get; set; } = new List<string>();
    public bool Sucesso => LinhasComErro == 0;

    public string ObterResumo()
    {
        return $"Competências importadas com sucesso<br>" +
               $"Inseridas: {LinhasInseridas}<br>" +
               $"Alteradas: {LinhasAlteradas}<br>" +
               $"Desconsideradas: {LinhasDesconsideradas}<br>" +
               $"Com erro: {LinhasComErro}";
    }
}
using Peers.Moderno.Models;
using OfficeOpenXml;

namespace Peers.Moderno.Services.Competencias.Common;

public interface ICompetenciasService
{
    Task<bool> CadastrarAsync(Competencia competencia);
    Task<bool> AlterarAsync(Competencia competencia);
    Task<bool> ExcluirAsync(int idCompetencia);
    Task<List<Competencia>> ListarAsync(bool? ativo = null);
    Task<Competencia?> ObterPorIdAsync(int idCompetencia);
    Task<Competencia?> ObterCompetenciaExistenteAsync(int idEmpresa, int idCargo, int idNivel, int idEixo, int idSubCompetencia, int idDimensao, string detalhamento);
    Task<List<CompetenciaExportModel>> ExportarAsync();
    Task<ImportResult> ImportarAsync(ExcelPackage package, int idEmpresa, int idUsuario);
    Task<int> AgregarCompetenciaAvaliacoesAsync(int idCompetencia);
}

public class CompetenciaExportModel
{
    public int IdCompetencia { get; set; }
    public int IdCargo { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int IdEixo { get; set; }
    public string Eixo { get; set; } = string.Empty;
    public int IdSubCompetencia { get; set; }
    public string SubCompetencia { get; set; } = string.Empty;
    public int IdDimensao { get; set; }
    public string Dimensao { get; set; } = string.Empty;
    public string DetalheNivelAtual { get; set; } = string.Empty;
    public string CompetenciaAtual { get; set; } = string.Empty;
    public string PalavrasChave { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public int ATV { get; set; }
}

public class ImportResult
{
    public int LinhasInseridas { get; set; }
    public int LinhasAlteradas { get; set; }
    public int LinhasDesconsideradas { get; set; }
    public int LinhasComErro { get; set; }
    public bool Sucesso => LinhasComErro == 0;
    public string MensagemResultado => $"Competências importadas com sucesso<br>Inseridas: {LinhasInseridas}<br>Alteradas: {LinhasAlteradas}<br>Desconsideradas: {LinhasDesconsideradas}<br>Com erro: {LinhasComErro}";
}
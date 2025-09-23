namespace Peers.Moderno.Services.Competencias.Common.DTOs;

public class CompetenciaDto
{
    public int IdCompetencia { get; set; }
    public int IdEmpresa { get; set; }
    public int IdCargo { get; set; }
    public int IdNivel { get; set; }
    public int IdEixo { get; set; }
    public int IdSubCompetencia { get; set; }
    public int IdDimensao { get; set; }
    public string NivelAtual { get; set; } = string.Empty;
    public string DetalhamentoNivelAtual { get; set; } = string.Empty;
    public string ProximoNivel { get; set; } = string.Empty;
    public string DetalhamentoProximoNivel { get; set; } = string.Empty;
    public string PalavrasChave { get; set; } = string.Empty;
    public string RelacaoSubcompetencia { get; set; } = string.Empty;
    public string TipoAvaliacao { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public string DetalhamentoLideranca { get; set; } = string.Empty;
    public int IdEscopoLideranca { get; set; }
    public int IdTituloLideranca { get; set; }
    public bool InputAutoAvaliacao { get; set; } = true;
    public bool InputAvaliacaoAsCegas { get; set; } = true;
    public bool InputAvaliacaoGestor { get; set; } = true;
    public bool InputFeedback { get; set; } = true;
    public bool InputNivel1 { get; set; } = true;
    public bool InputNivel2 { get; set; } = true;
    public bool VisivelAutoAvaliacao { get; set; } = true;
    public bool VisivelAvaliacaoAsCegas { get; set; } = true;
    public bool VisivelAvaliacaoGestor { get; set; } = true;
    public bool VisivelFeedback { get; set; } = true;
    public bool VisivelNivel1 { get; set; } = true;
    public bool VisivelNivel2 { get; set; } = true;
    public int IdNotaPadraoNivel1 { get; set; }
    public int IdNotaPadraoNivel2 { get; set; }
    public int IdModoCalculo { get; set; }
    public bool Ativo { get; set; } = true;
}
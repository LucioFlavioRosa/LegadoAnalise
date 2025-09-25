namespace Peers.Moderno.Models;

public class AutoAvaliacaoPerformanceViewModel
{
    public int IdProjeto { get; set; }
    public int IdAssociado { get; set; }
    public int IdPeriodo { get; set; }
    public int IdAvaliacao { get; set; }
    public string NomeAssociado { get; set; } = string.Empty;
    public string NomePeriodo { get; set; } = string.Empty;
    public string NomeProjeto { get; set; } = string.Empty;
    public string NomeGestor { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public string TempoPeers { get; set; } = string.Empty;
    public string TempoCargo { get; set; } = string.Empty;
    public string TempoRestante { get; set; } = string.Empty;
    public bool PodeEditar { get; set; } = true;
    public List<PerformanceItemModel> Performances { get; set; } = new List<PerformanceItemModel>();
}

public class PerformanceItemModel
{
    public int IdPerformance { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Abaixo { get; set; } = string.Empty;
    public string Esperado { get; set; } = string.Empty;
    public string Acima { get; set; } = string.Empty;
    public string Abrangencia { get; set; } = string.Empty;
    public bool MostrarSeparadorAbrangencia { get; set; }
    public bool PodeEditar { get; set; } = true;
    public string DisclaimerInput { get; set; } = string.Empty;
    public int NotaSelecionada { get; set; }
    public string? Observacoes { get; set; }
}

public class AvaliacaoPerformance
{
    public int IdAvaliacaoPerformance { get; set; }
    public int IdEmpresa { get; set; }
    public int IdAssociado { get; set; }
    public int USRAutoAvaliacao { get; set; }
    public int IdCargo { get; set; }
    public int IdNivel { get; set; }
    public int IdProjeto { get; set; }
    public int IdPeriodo { get; set; }
    public int IdPerformance { get; set; }
    public int IdAvaliacaoStatus { get; set; }
    public string PosicaoAtualFluxoAvaliacao { get; set; } = string.Empty;
    public DateTime DataHoraInicio { get; set; }
    public DateTime DHCAutoAvaliacao { get; set; }
    public int USR { get; set; }
    public DateTime DHC { get; set; }
    public int ATV { get; set; }
    public int IdNotaNivel1AutoAvaliacao { get; set; }
    public string ComentariosAutoAvaliacao { get; set; } = string.Empty;
    public DateTime DataHoraInicioAutoAvaliacao { get; set; }
    public DateTime? DataHoraFimAutoAvaliacao { get; set; }
}
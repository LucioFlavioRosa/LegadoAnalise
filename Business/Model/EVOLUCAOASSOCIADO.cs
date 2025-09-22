using System.ComponentModel.DataAnnotations;

namespace Business.Model
{
    public class EVOLUCAOASSOCIADO
    {
        public int Id { get; set; }
        public int IdPeriodo { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public decimal NotaCompetencia { get; set; }
        public decimal? NotaPerfomance { get; set; }
        public string RatingPerformance { get; set; } = string.Empty;
        public string TipoAvaliacao { get; set; } = string.Empty;
        
        public virtual PERIODOSAVALIACOES PERIODOSAVALIACOES { get; set; } = new();
        public virtual List<EVOLUCAOCOMPETENCIAS> EVOLUCAOCOMPETENCIAS { get; set; } = new();
        public virtual List<EVOLUCAOPERFORMANCE> EVOLUCAOPERFORMANCE { get; set; } = new();
    }

    public class PERIODOSAVALIACOES
    {
        public int Id { get; set; }
        public string Periodo { get; set; } = string.Empty;
    }

    public class EVOLUCAOCOMPETENCIAS
    {
        public int Id { get; set; }
        public int IdEixo { get; set; }
        public decimal? Nota { get; set; }
        public decimal NotaNeutra { get; set; }
        public virtual EIXOS EIXOS { get; set; } = new();
    }

    public class EVOLUCAOPERFORMANCE
    {
        public int Id { get; set; }
        public int IdPerformance { get; set; }
        public decimal Nota { get; set; }
        public virtual PERFORMANCES PERFORMANCES { get; set; } = new();
    }

    public class EIXOS
    {
        public int Id { get; set; }
        public string Eixo { get; set; } = string.Empty;
    }

    public class PERFORMANCES
    {
        public int Id { get; set; }
        public string Performance { get; set; } = string.Empty;
    }

    public class AssociadoInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Mentor { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string ProximoCargo { get; set; } = string.Empty;
        public int IdCargo { get; set; }
    }

    public class Premissa
    {
        public int Id { get; set; }
        public int IdCargo { get; set; }
        public decimal ValorRadarPeers { get; set; }
    }
}
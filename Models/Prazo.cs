using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models;

[Table("PRAZOS")]
public class Prazo
{
    [Key]
    public int IdPrazo { get; set; }
    
    [Required]
    [StringLength(500)]
    public string NomeDisparo { get; set; } = string.Empty;
    
    public int DuracaoAutoAvaliacao { get; set; }
    public int GatilhoAutoAvaliacao { get; set; }
    public int CompensadorAutoAvaliacao { get; set; }
    
    public int DuracaoAvaliacaoAsCegas { get; set; }
    public int GatilhoAvaliacaoAsCegas { get; set; }
    public int CompensadorAvaliacaoAsCegas { get; set; }
    
    public int DuracaoAvaliacaoGestor { get; set; }
    public int GatilhoAvaliacaoGestor { get; set; }
    public int CompensadorAvaliacaoGestor { get; set; }
    
    public int DuracaoFeedback { get; set; }
    public int GatilhoFeedback { get; set; }
    public int CompensadorFeedback { get; set; }
    
    public int DuracaoMentor { get; set; }
    public int GatilhoMentor { get; set; }
    public int CompensadorMentor { get; set; }
    
    public DateTime DHC { get; set; }
    public int ATV { get; set; }
    public int USR { get; set; }
    
    [NotMapped]
    public string StatusTexto => ATV == 1 ? "Ativo" : "Inativo";
    
    [NotMapped]
    public bool Ativo => ATV == 1;
}
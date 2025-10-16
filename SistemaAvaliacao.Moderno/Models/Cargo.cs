using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAvaliacao.Moderno.Models
{
    public class Cargo
    {
        [Key]
        public int IdCargo { get; set; }

        [Required]
        [StringLength(100)]
        public string NomeCargo { get; set; } = string.Empty;

        public int? ProximoCargoId { get; set; }

        [ForeignKey("ProximoCargoId")]
        public Cargo? ProximoCargo { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TempoMinimoPromocao { get; set; }

        [Required]
        [StringLength(500)]
        public string Funcao { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Autonomia { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string EscopoAtuacao { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string NivelInterlocucao { get; set; } = string.Empty;

        [Required]
        public bool Ativo { get; set; }
    }
}

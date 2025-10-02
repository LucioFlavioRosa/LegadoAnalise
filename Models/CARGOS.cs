using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Models
{
    public class CARGOS
    {
        [Key]
        public int IdCargo { get; set; }
        [Required]
        public string Cargo { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public ICollection<ASSOCIADOS>? Associados { get; set; }
    }
}
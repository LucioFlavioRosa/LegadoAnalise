using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Models
{
    public class VERTICAL
    {
        [Key]
        public int IdVertical { get; set; }
        [Required]
        public string Descricao { get; set; } = string.Empty;
        public ICollection<ASSOCIADOS>? Associados { get; set; }
    }
}
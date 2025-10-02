using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Models
{
    public class EMPRESAS
    {
        [Key]
        public int IdEmpresa { get; set; }
        [Required]
        public string Empresa { get; set; } = string.Empty;
        public ICollection<ASSOCIADOS>? Associados { get; set; }
    }
}
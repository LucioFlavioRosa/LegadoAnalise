using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peers.Moderno.Models
{
    public class PERFIS
    {
        [Key]
        public int IdPerfil { get; set; }
        [Required]
        public string Perfil { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public ICollection<ASSOCIADOS>? Associados { get; set; }
    }
}
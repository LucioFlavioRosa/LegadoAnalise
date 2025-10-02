using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models
{
    public class FOTOSASSOCIADOS
    {
        [Key]
        public int IdFoto { get; set; }
        public int IdAssociado { get; set; }
        public string Imagem { get; set; } = string.Empty; // Base64
        public string AssociadoFoto { get; set; } = string.Empty;
        public string NomeFoto { get; set; } = string.Empty;

        [ForeignKey("IdAssociado")]
        public ASSOCIADOS? Associado { get; set; }
    }
}
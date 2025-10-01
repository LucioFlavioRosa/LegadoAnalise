using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peers.Moderno.Models
{
    public class FotoAssociado
    {
        [Key]
        public int IdFoto { get; set; }
        public int IdAssociado { get; set; }
        public string Imagem { get; set; } // base64
        public string NomeFoto { get; set; }
        public string AssociadoFoto { get; set; }

        [ForeignKey("IdAssociado")]
        public virtual Associado Associado { get; set; }
    }
}
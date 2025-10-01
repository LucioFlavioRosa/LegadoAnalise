using System;

namespace Peers.Moderno.Models
{
    public class FOTOSASSOCIADOS
    {
        public int IdFoto { get; set; }
        public int IdAssociado { get; set; }
        public string AssociadoFoto { get; set; }
        public string NomeFoto { get; set; }
        public string Imagem { get; set; }
        public DateTime DHC { get; set; }
        public int ATV { get; set; }

        public ASSOCIADOS ASSOCIADOS { get; set; }
    }
}
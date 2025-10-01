using System;
using System.Collections.Generic;

namespace Peers.Moderno.Models
{
    public class CARGOS
    {
        public int IdCargo { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public int ATV { get; set; }
        public DateTime DHC { get; set; }
        public int USR { get; set; }

        // Navegação
        public ICollection<ASSOCIADOS>? ASSOCIADOS { get; set; }
        public ICollection<PROMOCOES>? PROMOCOES_ANTIGO { get; set; } // Cargo antigo
        public ICollection<PROMOCOES>? PROMOCOES_NOVO { get; set; } // Cargo novo
    }
}
using System;
using System.Collections.Generic;

namespace Peers.Moderno.Models
{
    public class VERTICAL
    {
        public int IdVertical { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int ATV { get; set; }
        public DateTime DHC { get; set; }
        public int USR { get; set; }

        // Navegação
        public ICollection<ASSOCIADOS>? ASSOCIADOS { get; set; }
    }
}
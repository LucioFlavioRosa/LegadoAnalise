using System;
using System.Collections.Generic;

namespace Peers.Moderno.Models
{
    public class PERFIS
    {
        public int IdPerfil { get; set; }
        public string Perfil { get; set; } = string.Empty;
        public int ATV { get; set; }
        public DateTime DHC { get; set; }
        public int USR { get; set; }

        // Navegação
        public ICollection<ASSOCIADOS>? ASSOCIADOS { get; set; }
    }
}
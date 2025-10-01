using System;
using System.Collections.Generic;

namespace Peers.Moderno.Models
{
    public class EMPRESAS
    {
        public int IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public int ATV { get; set; }
        public DateTime DHC { get; set; }
        public int USR { get; set; }

        public ICollection<ASSOCIADOS> ASSOCIADOS { get; set; } = new List<ASSOCIADOS>();
    }
}
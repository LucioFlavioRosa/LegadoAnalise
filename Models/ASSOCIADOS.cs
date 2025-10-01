using System;
using System.Collections.Generic;

namespace Peers.Moderno.Models
{
    public class ASSOCIADOS
    {
        public int IdAssociado { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public int IdCargo { get; set; }
        public int IdPerfil { get; set; }
        public int IdAssociadoMentor { get; set; }
        public int IdEmpresa { get; set; }
        public int IdVertical { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public string Vertical { get; set; } = string.Empty;
        public string? FotoNome { get; set; }
        public int ATV { get; set; }
        public int IdStatus { get; set; }
        public int IdNivel { get; set; }
        public int USR { get; set; }
        public DateTime DHC { get; set; }

        // Navegação
        public CARGOS? CARGOS { get; set; }
        public PERFIS? PERFIS { get; set; }
        public ASSOCIADOS? ASSOCIADOS2 { get; set; } // Mentor
        public EMPRESAS? EMPRESAS { get; set; }
        public VERTICAL? VERTICAL { get; set; }
        public ICollection<FOTOSASSOCIADOS>? FOTOSASSOCIADOS { get; set; }
        public ICollection<PROMOCOES>? PROMOCOES { get; set; }
    }
}
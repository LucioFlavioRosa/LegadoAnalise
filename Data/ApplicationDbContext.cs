using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Peers.Moderno.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ASSOCIADOS> Associados { get; set; }
        public DbSet<CARGOS> Cargos { get; set; }
        public DbSet<PERFIS> Perfis { get; set; }
        public DbSet<VERTICAL> Verticais { get; set; }
        public DbSet<FOTOSASSOCIADOS> FotosAssociados { get; set; }
        public DbSet<PROMOCOES> Promocoes { get; set; }
        public DbSet<EMPRESAS> Empresas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.CARGOS)
                .WithMany()
                .HasForeignKey(a => a.IdCargo);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.PERFIS)
                .WithMany()
                .HasForeignKey(a => a.IdPerfil);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.EMPRESAS)
                .WithMany()
                .HasForeignKey(a => a.IdEmpresa);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.ASSOCIADOS2)
                .WithMany()
                .HasForeignKey(a => a.IdAssociadoMentor);

            modelBuilder.Entity<ASSOCIADOS>()
                .HasOne(a => a.VERTICAL)
                .WithMany()
                .HasForeignKey(a => a.IdVertical);

            modelBuilder.Entity<FOTOSASSOCIADOS>()
                .HasOne(f => f.ASSOCIADOS)
                .WithMany()
                .HasForeignKey(f => f.IdAssociado);

            modelBuilder.Entity<PROMOCOES>()
                .HasOne(p => p.ASSOCIADOS)
                .WithMany()
                .HasForeignKey(p => p.idAssociado);
            modelBuilder.Entity<PROMOCOES>()
                .HasOne(p => p.CARGOS)
                .WithMany()
                .HasForeignKey(p => p.idCargoAnterior);
            modelBuilder.Entity<PROMOCOES>()
                .HasOne(p => p.CARGOS1)
                .WithMany()
                .HasForeignKey(p => p.idCargoNovo);
        }
    }

    public class ASSOCIADOS
    {
        public int IdAssociado { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int IdCargo { get; set; }
        public int IdEmpresa { get; set; }
        public int IdPerfil { get; set; }
        public int IdNivel { get; set; }
        public int IdStatus { get; set; }
        public int IdAssociadoMentor { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Vertical { get; set; } = string.Empty;
        public string? FotoNome { get; set; }
        public int ATV { get; set; }
        public int USR { get; set; }
        public DateTime DHC { get; set; }
        public int IdVertical { get; set; }

        public CARGOS? CARGOS { get; set; }
        public PERFIS? PERFIS { get; set; }
        public EMPRESAS? EMPRESAS { get; set; }
        public ASSOCIADOS? ASSOCIADOS2 { get; set; }
        public VERTICAL? VERTICAL { get; set; }
    }

    public class CARGOS
    {
        public int IdCargo { get; set; }
        public string Cargo { get; set; } = string.Empty;
    }

    public class PERFIS
    {
        public int IdPerfil { get; set; }
        public string Perfil { get; set; } = string.Empty;
    }

    public class VERTICAL
    {
        public int IdVertical { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }

    public class FOTOSASSOCIADOS
    {
        public int IdFoto { get; set; }
        public int IdAssociado { get; set; }
        public string Imagem { get; set; } = string.Empty;
        public string AssociadoFoto { get; set; } = string.Empty;
        public string NomeFoto { get; set; } = string.Empty;
        public ASSOCIADOS? ASSOCIADOS { get; set; }
    }

    public class PROMOCOES
    {
        public int idPromocao { get; set; }
        public int idAssociado { get; set; }
        public int idCargoAnterior { get; set; }
        public int idCargoNovo { get; set; }
        public DateTime? DataPromocao { get; set; }
        public string Comentarios { get; set; } = string.Empty;
        public DateTime DHC { get; set; }
        public bool ATV { get; set; }
        public ASSOCIADOS? ASSOCIADOS { get; set; }
        public CARGOS? CARGOS { get; set; }
        public CARGOS? CARGOS1 { get; set; }
    }

    public class EMPRESAS
    {
        public int IdEmpresa { get; set; }
        public string Empresa { get; set; } = string.Empty;
    }
}

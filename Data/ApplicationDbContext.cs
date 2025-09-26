using Microsoft.EntityFrameworkCore;
using Peers.Moderno.Models;

namespace Peers.Moderno.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Competencia> Competencias { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<Eixo> Eixos { get; set; }
    public DbSet<SubCompetencia> SubCompetencias { get; set; }
    public DbSet<Dimensao> Dimensoes { get; set; }
    public DbSet<RelacaoCargoSubcompetencia> RelacoesCargosSubcompetencias { get; set; }
    public DbSet<AvaliacaoCompetenciaNota> AvaliacoesCompetenciasNotas { get; set; }
    public DbSet<ModoCalculoCompetencia> ModosCalculosCompetencias { get; set; }
    public DbSet<PremissasRadar> PremissasRadar { get; set; }
    public DbSet<CargoNivel> CargosNiveis { get; set; }
    public DbSet<Associado> Associados { get; set; }
    public DbSet<Promocao> Promocoes { get; set; }
    public DbSet<Perfil> Perfis { get; set; }
    public DbSet<Vertical> Verticais { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<ProjetoComplexidade> ProjetosComplexidades { get; set; }
    public DbSet<Performance> Performances { get; set; }
    public DbSet<PerguntaEncerramento> PerguntasEncerramento { get; set; }
    public DbSet<Prazo> Prazos { get; set; }
    public DbSet<Projeto> Projetos { get; set; }
    public DbSet<AssociadoProjeto> AssociadosProjetos { get; set; }
    public DbSet<TipoProjeto> TiposProjetos { get; set; }
    // Novos DbSets para PDI
    public DbSet<PDIPeriodo> PDIPeriodos { get; set; }
    public DbSet<PDIQuestao> PDIQuestoes { get; set; }
    public DbSet<PDIResposta> PDIRespostas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // ... (demais configurações existentes)
        // Configurações para PDI
        modelBuilder.Entity<PDIPeriodo>(entity =>
        {
            entity.HasKey(e => e.IdPeriodo);
            entity.ToTable("PDI_PERIODOS");
            entity.Property(e => e.Periodo).IsRequired().HasMaxLength(100);
        });
        modelBuilder.Entity<PDIQuestao>(entity =>
        {
            entity.HasKey(e => e.IdPDIQuestao);
            entity.ToTable("PDI_QUESTOES");
            entity.Property(e => e.Titulo).HasMaxLength(200);
            entity.Property(e => e.Subtitulo).HasMaxLength(200);
            entity.Property(e => e.Icone).HasMaxLength(200);
            entity.HasOne(e => e.Periodo)
                .WithMany(p => p.Questoes)
                .HasForeignKey(e => e.IdPeriodo)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<PDIResposta>(entity =>
        {
            entity.HasKey(e => e.IdPDIResposta);
            entity.ToTable("PDI_RESPOSTAS");
            entity.Property(e => e.Resposta).HasMaxLength(2000);
            entity.Property(e => e.DHC).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.DataAtualizacao);
            entity.HasOne(e => e.Periodo)
                .WithMany(p => p.Respostas)
                .HasForeignKey(e => e.IdPeriodo)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Questao)
                .WithMany(q => q.Respostas)
                .HasForeignKey(e => e.IdPDIQuestao)
                .OnDelete(DeleteBehavior.Restrict);
        });
        // ... (demais configurações existentes)
    }
}

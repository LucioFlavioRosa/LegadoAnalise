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
    // Adicionados DbSets para PDI
    public DbSet<PDI_QUESTOES> PDIQuestoes { get; set; }
    public DbSet<PDI_RESPOSTAS> PDIRespostas { get; set; }
    public DbSet<PERIODOSAVALIACOES> PeriodosAvaliacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // ... (demais configurações existentes)
        // Configuração das entidades PDI
        modelBuilder.Entity<PDI_QUESTOES>(entity =>
        {
            entity.HasKey(e => e.idPDIQuestoes);
            entity.ToTable("PDI_QUESTOES");
        });
        modelBuilder.Entity<PDI_RESPOSTAS>(entity =>
        {
            entity.HasKey(e => e.idPDIRespostas);
            entity.ToTable("PDI_RESPOSTAS");
            entity.HasOne(e => e.PDI_QUESTOES)
                .WithMany(q => q.PDI_RESPOSTAS)
                .HasForeignKey(e => e.idPDIQuestao);
            entity.HasOne(e => e.PERIODOSAVALIACOES)
                .WithMany(p => p.PDI_RESPOSTAS)
                .HasForeignKey(e => e.idPeriodo);
        });
        modelBuilder.Entity<PERIODOSAVALIACOES>(entity =>
        {
            entity.HasKey(e => e.IdPeriodo);
            entity.ToTable("PERIODOSAVALIACOES");
        });
    }
}
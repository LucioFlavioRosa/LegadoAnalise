using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaAvaliacao.Moderno.Models;

namespace SistemaAvaliacao.Moderno.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            await context.Database.MigrateAsync();

            if (!context.Cargos.Any())
            {
                var cargos = new List<Cargo>
                {
                    new Cargo
                    {
                        NomeCargo = "Analista Júnior",
                        TempoMinimoPromocao = 12,
                        Funcao = "Atua em tarefas operacionais sob supervisão.",
                        Autonomia = "Baixa",
                        EscopoAtuacao = "Projetos internos",
                        NivelInterlocucao = "Equipe interna",
                        Ativo = true
                    },
                    new Cargo
                    {
                        NomeCargo = "Analista Pleno",
                        TempoMinimoPromocao = 18,
                        Funcao = "Executa atividades técnicas de média complexidade.",
                        Autonomia = "Média",
                        EscopoAtuacao = "Projetos internos e clientes",
                        NivelInterlocucao = "Equipe e clientes",
                        Ativo = true
                    },
                    new Cargo
                    {
                        NomeCargo = "Analista Sênior",
                        TempoMinimoPromocao = 24,
                        Funcao = "Responsável por projetos e orientação técnica.",
                        Autonomia = "Alta",
                        EscopoAtuacao = "Projetos estratégicos",
                        NivelInterlocucao = "Gestores e clientes",
                        Ativo = true
                    }
                };

                // Relacionamento Próximo Cargo
                cargos[0].ProximoCargo = cargos[1];
                cargos[1].ProximoCargo = cargos[2];

                context.Cargos.AddRange(cargos);
                await context.SaveChangesAsync();
            }
        }
    }
}

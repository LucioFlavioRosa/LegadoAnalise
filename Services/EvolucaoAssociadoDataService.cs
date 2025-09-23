using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class EvolucaoAssociadoDataService : IEvolucaoAssociadoDataService
{
    public async Task<List<EVOLUCAOASSOCIADO>> ObterEvolucaoAsync(int associadoId, string tipoAvaliacao, string escopo)
    {
        await Task.Delay(1);
        
        return new List<EVOLUCAOASSOCIADO>
        {
            new EVOLUCAOASSOCIADO
            {
                Id = 1,
                IdPeriodo = 1,
                Cargo = "Desenvolvedor",
                NotaCompetencia = 85,
                NotaPerfomance = 4.2m,
                RatingPerformance = "Bom",
                TipoAvaliacao = tipoAvaliacao,
                PERIODOSAVALIACOES = new PERIODOSAVALIACOES { Id = 1, Periodo = "2024-Q1" },
                EVOLUCAOCOMPETENCIAS = new List<EVOLUCAOCOMPETENCIAS>
                {
                    new EVOLUCAOCOMPETENCIAS
                    {
                        Id = 1,
                        IdEixo = 1,
                        Nota = 80,
                        NotaNeutra = 80,
                        EIXOS = new EIXOS { Id = 1, Eixo = "Técnico" }
                    }
                },
                EVOLUCAOPERFORMANCE = new List<EVOLUCAOPERFORMANCE>
                {
                    new EVOLUCAOPERFORMANCE
                    {
                        Id = 1,
                        IdPerformance = 1,
                        Nota = 4.2m,
                        PERFORMANCES = new PERFORMANCES { Id = 1, Performance = "Entrega" }
                    }
                }
            }
        };
    }
}
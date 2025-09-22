using Business.Model;
using Business.Services;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Peers.Moderno.Services
{
    public class EvolucaoAssociadoViewModel
    {
        private readonly IEvolucaoAssociadoServices _evolucaoService;
        private readonly IAssociadosService _associadosService;
        private readonly IPremissaService _premissaService;
        private readonly ILogger<EvolucaoAssociadoViewModel> _logger;

        public EvolucaoAssociadoViewModel(
            IEvolucaoAssociadoServices evolucaoService,
            IAssociadosService associadosService,
            IPremissaService premissaService,
            ILogger<EvolucaoAssociadoViewModel> logger)
        {
            _evolucaoService = evolucaoService;
            _associadosService = associadosService;
            _premissaService = premissaService;
            _logger = logger;
        }

        public List<string> TiposAvaliacao => new() { "[Selecionar]", "desempenho" };
        public List<string> Escopos => new() { "[Selecionar]", "projeto" };

        public string FormatPercentagem(decimal nota)
        {
            return nota > 0 ? $"{nota:##0}%" : "0%";
        }

        public string FormatDecimal(decimal nota)
        {
            return Math.Round(nota, 2).ToString();
        }

        public async Task<string> GerarJsonRadarAsync(List<EVOLUCAOASSOCIADO> listProjetos, int idCargo)
        {
            try
            {
                var labels = new List<string>();
                var datasets = new List<object>();
                int count = 1;

                foreach (var projeto in listProjetos.OrderByDescending(x => x.IdPeriodo))
                {
                    var dataset = new List<decimal>();

                    foreach (var item in projeto.EVOLUCAOCOMPETENCIAS)
                    {
                        if (count == 1)
                        {
                            labels.Add(item.EIXOS.Eixo);
                        }
                        dataset.Add(item.Nota.HasValue && item.Nota.Value > 0 ? item.Nota.Value : 0);
                    }

                    datasets.Add(new
                    {
                        periodo = projeto.PERIODOSAVALIACOES.Periodo,
                        dataset = dataset
                    });

                    count++;
                }

                var premissaRadar = await _premissaService.ObterPremissaPorCardoAsync(idCargo);
                decimal stepSize = 200;

                if (premissaRadar != null && premissaRadar.ValorRadarPeers > 1)
                {
                    decimal step = 200m / (premissaRadar.ValorRadarPeers + 1);
                    stepSize = Math.Round(step);
                }

                var result = new
                {
                    labels = labels,
                    datasets = datasets,
                    stepsize = stepSize
                };

                return JsonSerializer.Serialize(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar JSON do radar para cargo {IdCargo}", idCargo);
                throw;
            }
        }
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Peers.Moderno.Models;

namespace Peers.Moderno.Services;

public class EvolucaoAssociadoService : IEvolucaoAssociadoService
{
    private readonly IPremissaService _premissaService;
    private readonly IAssociadosService _associadosService;
    private readonly IEvolucaoAssociadoDataService _dataService;

    public EvolucaoAssociadoService(
        IPremissaService premissaService,
        IAssociadosService associadosService,
        IEvolucaoAssociadoDataService dataService)
    {
        _premissaService = premissaService;
        _associadosService = associadosService;
        _dataService = dataService;
    }

    public async Task<List<EVOLUCAOASSOCIADO>> ObterEvolucaoAsync(int associadoId, string tipoAvaliacao, string escopo)
    {
        return await _dataService.ObterEvolucaoAsync(associadoId, tipoAvaliacao, escopo);
    }

    public async Task<AssociadoInfo> ObterAssociadoMentorCargoAsync(int associadoId)
    {
        return await _associadosService.ObterAssociadoMentorCargoAsync(associadoId);
    }

    public string FormatarPercentagem(decimal nota)
    {
        if (nota > 0)
        {
            return nota.ToString("##0") + "%";
        }
        return "0%";
    }

    public string FormatarDecimal(decimal nota)
    {
        return Math.Round(nota, 2).ToString();
    }

    public string GerarJsonRadar(List<EVOLUCAOASSOCIADO> listaProjetos, int idCargo)
    {
        dynamic obj = new JObject();
        List<string> labels = new List<string>();
        List<JObject> datasets = new List<JObject>();

        int count = 1;

        foreach (var projeto in listaProjetos.OrderByDescending(x => x.IdPeriodo))
        {
            dynamic itemProjeto = new JObject();
            itemProjeto.periodo = projeto.PERIODOSAVALIACOES.Periodo;
            List<decimal> dataset = new List<decimal>();

            foreach (var item in projeto.EVOLUCAOCOMPETENCIAS)
            {
                if (count == 1)
                {
                    labels.Add(item.EIXOS.Eixo);
                }

                dataset.Add(item.Nota.HasValue && item.Nota.Value > 0 ? item.Nota.Value : 0);
            }

            itemProjeto.dataset = new JArray(dataset);
            datasets.Add(itemProjeto);

            count++;
        }

        obj.labels = new JArray(labels);
        obj.datasets = new JArray(datasets);

        var premissaRadar = _premissaService.ObterPremissaPorCargo(idCargo);
        obj.stepsize = 200;

        if (premissaRadar != null)
        {
            if (premissaRadar.ValorRadarPeers > 1)
            {
                decimal step = (200m) / (premissaRadar.ValorRadarPeers + 1);
                obj.stepsize = Math.Round(step);
            }
        }

        return JsonConvert.SerializeObject(obj);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Peers.Moderno.Models;

namespace Services.Workflow;

public interface IWorkflowService
{
    Task<List<WORKFLOW>> ObterListaAsync();
    Task<WORKFLOW?> ObterByPeriodoAsync(int idPeriodo);
    Task<WORKFLOW?> ObterAsync(int idWorkflow);
    Task<bool> InserirAsync(WORKFLOW workflow);
    Task<bool> AlterarAsync(int idWorkflow, WORKFLOW workflow);
    Task<bool> DeletarAsync(int idWorkflow);
}
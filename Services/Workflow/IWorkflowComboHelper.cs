using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Common;

namespace Services.Workflow;

public interface IWorkflowComboHelper
{
    Task<List<ComboItem>> GetPeriodosComboAsync(int empresaId);
}
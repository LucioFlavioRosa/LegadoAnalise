using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Services.Performance;
using Services.Common;

namespace Components.Performance
{
    public partial class MentorPerformance : ComponentBase
    {
        [Inject] private IPerformanceMentorService PerformanceMentorService { get; set; } = default!;
        [Inject] private IMessageBoxService MessageBoxService { get; set; } = default!;

        protected MentorPerformanceViewModel? ViewModel;
        protected bool IsLoading = true;
        protected string? ErrorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                ViewModel = await PerformanceMentorService.GetMentorPerformanceAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                MessageBoxService.ShowError(ErrorMessage ?? "Erro ao carregar dados de performance.", "Erro");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}

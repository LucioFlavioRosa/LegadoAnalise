// ... outros using ...
using System;
using System.Web.UI.WebControls;
// ... outros using ...

namespace SistemaAvaliacao
{
    public partial class Cargos : System.Web.UI.Page
    {
        // Supondo que existe um presenter já injetado
        private ICargoPresenter _presenter;

        protected void Page_Load(object sender, EventArgs e)
        {
            // ... inicialização padrão ...
        }

        protected void rptCargos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Removida lógica condicional do code-behind
                // O status já deve vir formatado do Presenter ou Helper
                var lblATV = (Label)e.Item.FindControl("lblATV");
                if (lblATV != null)
                {
                    // Supondo que o campo DataItem["StatusFormatado"] já está preenchido
                    lblATV.Text = DataBinder.Eval(e.Item.DataItem, "StatusFormatado")?.ToString();
                }
            }
        }

        // ... outros métodos ...
    }
}

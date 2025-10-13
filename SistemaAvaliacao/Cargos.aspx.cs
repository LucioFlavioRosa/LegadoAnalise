using System;
using System.Web.UI.WebControls;
using SistemaAvaliacao.Presenters;
using SistemaAvaliacao.Infrastructure.DependencyInjection;

namespace SistemaAvaliacao
{
    public partial class Cargos : System.Web.UI.Page
    {
        private CargoPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            // Resolve dependências via ServiceLocator
            _presenter = ServiceLocator.GetCargoPresenter();
            _presenter.SetView(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.OnPageLoad();
            }
        }

        protected void rptCargos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // Delegar para o Presenter
            _presenter.OnRepeaterItemCommand(source, e);
        }

        protected void rptCargos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Delegar para o Presenter
            _presenter.OnRepeaterItemDataBound(sender, e);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Delegar exportação para o Presenter
            _presenter.OnExportCargos(this.Response);
        }
    }
}

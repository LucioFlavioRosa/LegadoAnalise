using System;
using System.Collections.Generic;
using SistemaAvaliacao.Presenters;
using SistemaAvaliacao.Domain.Models;
using SistemaAvaliacao.Services;
using SistemaAvaliacao.Repositories;
using SistemaAvaliacao.Infrastructure.DependencyInjection;

namespace SistemaAvaliacao
{
    public partial class Cargos : System.Web.UI.Page, ICargoView
    {
        private CargoPresenter _presenter;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Resolve dependências via ServiceLocator
            _presenter = ServiceLocator.Resolve<CargoPresenter>();
            _presenter.View = this;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.CarregarCargos();
                _presenter.CarregarProximosCargos();
            }
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            _presenter.CadastrarOuAtualizarCargo();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            _presenter.ExportarCargos();
        }

        protected void rptCargos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            _presenter.TratarComandoGrid(e);
        }

        // Métodos da ICargoView
        public void ExibirMensagem(string mensagem, bool sucesso = true)
        {
            MessageBoxHandler.ShowMessage(mensagem, sucesso);
        }

        public void PreencherGrid(IEnumerable<Cargo> cargos)
        {
            rptCargos.DataSource = cargos;
            rptCargos.DataBind();
        }

        public void LimparFormulario()
        {
            txtCargo.Text = string.Empty;
            ddlProximocargo.SelectedIndex = 0;
            txtTempoMinimo.Text = string.Empty;
            txtFuncao.Text = string.Empty;
            txtAutonomia.Text = string.Empty;
            txtEscopoAtuacao.Text = string.Empty;
            txtNivelInterlocucao.Text = string.Empty;
            ddlStatus.SelectedIndex = 0;
            hdIdCargo.Value = string.Empty;
        }

        public Cargo ObterDadosFormulario()
        {
            int.TryParse(hdIdCargo.Value, out int idCargo);
            int.TryParse(ddlProximocargo.SelectedValue, out int proximoCargoId);
            int.TryParse(txtTempoMinimo.Text, out int tempoMinimo);
            int.TryParse(ddlStatus.SelectedValue, out int status);

            return new Cargo
            {
                IdCargo = idCargo,
                NomeCargo = txtCargo.Text.Trim(),
                ProximoCargoId = proximoCargoId > 0 ? (int?)proximoCargoId : null,
                TempoMinimoPromocao = tempoMinimo,
                Funcao = txtFuncao.Text.Trim(),
                Autonomia = txtAutonomia.Text.Trim(),
                EscopoAtuacao = txtEscopoAtuacao.Text.Trim(),
                NivelInterlocucao = txtNivelInterlocucao.Text.Trim(),
                Status = status
            };
        }
    }
}

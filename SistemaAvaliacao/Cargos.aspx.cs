using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using SistemaAvaliacao.Models;
using SistemaAvaliacao.Repositories;
using SistemaAvaliacao.Services;

namespace SistemaAvaliacao
{
    public partial class WebForm2 : Page
    {
        private readonly ICargoRepository _cargoRepository;
        private readonly CargoService _cargoService;
        private readonly IExportService _exportService;

        public WebForm2()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _cargoRepository = new CargoRepository(connectionString);
            _cargoService = new CargoService(_cargoRepository);
            _exportService = new ExportService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarProximosCargos();
                CarregarCargos();
            }
        }

        private void CarregarProximosCargos()
        {
            ddlProximocargo.DataSource = _cargoService.ObterTodos();
            ddlProximocargo.DataTextField = "Nome";
            ddlProximocargo.DataValueField = "IdCargo";
            ddlProximocargo.DataBind();
            ddlProximocargo.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Selecione", ""));
        }

        private void CarregarCargos()
        {
            var cargos = _cargoService.ObterTodos();
            rptCargos.DataSource = cargos.Select(c => new CargoViewModel
            {
                IdCargo = c.IdCargo,
                Nome = c.Nome,
                ProximoCargo = c.ProximoCargoId.HasValue ? _cargoService.ObterPorId(c.ProximoCargoId.Value)?.Nome : string.Empty,
                Status = c.Status ? "Ativo" : "Inativo"
            }).ToList();
            rptCargos.DataBind();
        }

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            var cargo = new Cargo
            {
                IdCargo = string.IsNullOrEmpty(hdIdCargo.Value) ? 0 : int.Parse(hdIdCargo.Value),
                Nome = txtCargo.Text.Trim(),
                ProximoCargoId = string.IsNullOrEmpty(ddlProximocargo.SelectedValue) ? (int?)null : int.Parse(ddlProximocargo.SelectedValue),
                TempoMinimo = int.TryParse(txtTempoMinimo.Text, out int tempo) ? tempo : 0,
                Funcao = txtFuncao.Text.Trim(),
                Autonomia = txtAutonomia.Text.Trim(),
                EscopoAtuacao = txtEscopoAtuacao.Text.Trim(),
                NivelInterlocucao = txtNivelInterlocucao.Text.Trim(),
                Status = ddlStatus.SelectedValue == "1"
            };

            if (cargo.IdCargo == 0)
                _cargoService.Adicionar(cargo);
            else
                _cargoService.Atualizar(cargo);

            LimparCampos();
            CarregarCargos();
        }

        protected void rptCargos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int idCargo = int.Parse(((System.Web.UI.WebControls.Label)e.Item.FindControl("lblIdCargo")).Text);
            if (e.CommandName == "AlterarCargo")
            {
                var cargo = _cargoService.ObterPorId(idCargo);
                if (cargo != null)
                {
                    hdIdCargo.Value = cargo.IdCargo.ToString();
                    txtCargo.Text = cargo.Nome;
                    ddlProximocargo.SelectedValue = cargo.ProximoCargoId?.ToString() ?? "";
                    txtTempoMinimo.Text = cargo.TempoMinimo.ToString();
                    txtFuncao.Text = cargo.Funcao;
                    txtAutonomia.Text = cargo.Autonomia;
                    txtEscopoAtuacao.Text = cargo.EscopoAtuacao;
                    txtNivelInterlocucao.Text = cargo.NivelInterlocucao;
                    ddlStatus.SelectedValue = cargo.Status ? "1" : "0";
                }
            }
            else if (e.CommandName == "ExcluirCargo")
            {
                _cargoService.Inativar(idCargo);
                CarregarCargos();
            }
        }

        protected void rptCargos_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            // Nenhuma lógica de negócio aqui, apenas manipulação de UI se necessário
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var cargos = _cargoService.ObterTodos();
            var columns = new[] { "Código", "Cargo", "Próximo Cargo", "Status" };
            var data = cargos.Select(c => new CargoViewModel
            {
                IdCargo = c.IdCargo,
                Nome = c.Nome,
                ProximoCargo = c.ProximoCargoId.HasValue ? _cargoService.ObterPorId(c.ProximoCargoId.Value)?.Nome : string.Empty,
                Status = c.Status ? "Ativo" : "Inativo"
            });
            var bytes = _exportService.ExportToCsv(data, columns, vm => new object[] { vm.IdCargo, vm.Nome, vm.ProximoCargo, vm.Status });
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("content-disposition", "attachment;filename=cargos.csv");
            Response.BinaryWrite(bytes);
            Response.End();
        }

        private void LimparCampos()
        {
            hdIdCargo.Value = string.Empty;
            txtCargo.Text = string.Empty;
            ddlProximocargo.SelectedIndex = 0;
            txtTempoMinimo.Text = string.Empty;
            txtFuncao.Text = string.Empty;
            txtAutonomia.Text = string.Empty;
            txtEscopoAtuacao.Text = string.Empty;
            txtNivelInterlocucao.Text = string.Empty;
            ddlStatus.SelectedValue = "1";
        }
    }
}
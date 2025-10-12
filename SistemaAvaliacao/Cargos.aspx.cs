using System;
using System.Collections.Generic;
using System.Web.UI;
using SistemaAvaliacao.Models;
using SistemaAvaliacao.Services;
using SistemaAvaliacao.Validators;

namespace SistemaAvaliacao
{
    public partial class Cargos : Page
    {
        private readonly ICargoService _cargoService;
        private readonly IExportService _exportService;
        private readonly CargoValidator _cargoValidator;

        public Cargos() : this(
            DependencyResolver.Resolve<ICargoService>(),
            DependencyResolver.Resolve<IExportService>(),
            new CargoValidator())
        {
        }

        public Cargos(ICargoService cargoService, IExportService exportService, CargoValidator cargoValidator)
        {
            _cargoService = cargoService;
            _exportService = exportService;
            _cargoValidator = cargoValidator;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCargos();
            }
        }

        private void BindCargos()
        {
            var cargos = _cargoService.GetAllCargos();
            rptCargos.DataSource = cargos;
            rptCargos.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            var cargos = _cargoService.GetAllCargos();
            var bytes = _exportService.ExportCargosToExcel(cargos);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=Cargos.xlsx");
            Response.BinaryWrite(bytes);
            Response.End();
        }

        // ... outros métodos e eventos ...
    }
}

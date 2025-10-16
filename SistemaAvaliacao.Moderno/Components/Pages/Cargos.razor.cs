using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SistemaAvaliacao.Moderno.Models;
using SistemaAvaliacao.Moderno.Services;

namespace SistemaAvaliacao.Moderno.Components.Pages
{
    public partial class Cargos : ComponentBase
    {
        [Inject]
        public ICargoService CargoService { get; set; } = default!;

        public List<Cargo> CargosList { get; set; } = new();
        public List<Cargo> CargosAtivos { get; set; } = new();
        public Cargo CargoModel { get; set; } = new();
        public string? Mensagem { get; set; }
        public bool IsEdit { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await CarregarCargosAsync();
            await CarregarCargosAtivosAsync();
        }

        private async Task CarregarCargosAsync()
        {
            CargosList = await CargoService.ObterTodosAsync();
        }

        private async Task CarregarCargosAtivosAsync()
        {
            CargosAtivos = await CargoService.ObterCargosAtivosAsync();
        }

        public async Task SalvarCargo()
        {
            Mensagem = null;
            bool resultado;
            if (CargoModel.IdCargo == 0)
            {
                resultado = await CargoService.CriarAsync(CargoModel);
                Mensagem = resultado ? "Cargo cadastrado com sucesso." : "Erro ao cadastrar cargo.";
            }
            else
            {
                resultado = await CargoService.AtualizarAsync(CargoModel);
                Mensagem = resultado ? "Cargo atualizado com sucesso." : "Erro ao atualizar cargo.";
            }
            if (resultado)
            {
                await CarregarCargosAsync();
                await CarregarCargosAtivosAsync();
                LimparFormulario();
            }
        }

        public async Task EditarCargo(int id)
        {
            var cargo = await CargoService.ObterPorIdAsync(id);
            if (cargo != null)
            {
                CargoModel = new Cargo
                {
                    IdCargo = cargo.IdCargo,
                    NomeCargo = cargo.NomeCargo,
                    ProximoCargoId = cargo.ProximoCargoId,
                    TempoMinimoPromocao = cargo.TempoMinimoPromocao,
                    Funcao = cargo.Funcao,
                    Autonomia = cargo.Autonomia,
                    EscopoAtuacao = cargo.EscopoAtuacao,
                    NivelInterlocucao = cargo.NivelInterlocucao,
                    Ativo = cargo.Ativo
                };
                IsEdit = true;
            }
        }

        public async Task InativarCargo(int id)
        {
            var resultado = await CargoService.InativarAsync(id);
            Mensagem = resultado ? "Cargo inativado com sucesso." : "Erro ao inativar cargo.";
            if (resultado)
            {
                await CarregarCargosAsync();
                await CarregarCargosAtivosAsync();
                LimparFormulario();
            }
        }

        public void LimparFormulario()
        {
            CargoModel = new Cargo();
            IsEdit = false;
        }
    }
}

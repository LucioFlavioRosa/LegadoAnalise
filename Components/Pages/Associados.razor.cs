using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Models;
using Components.Shared;

namespace Components.Pages
{
    public partial class Associados : ComponentBase
    {
        [Inject] public Services.IAssociadosService AssociadosService { get; set; }
        [Inject] public Services.ICargosService CargosService { get; set; }
        [Inject] public Services.IPerfisService PerfisService { get; set; }
        [Inject] public Services.IVerticalService VerticalService { get; set; }
        [Inject] public Services.IFotosAssociadosService FotosService { get; set; }
        [Inject] public Services.IExportFileService ExportService { get; set; }
        [Inject] public Services.IImportFileService ImportService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected List<ASSOCIADOS> Associados { get; set; } = new();
        protected List<CARGOS> Cargos { get; set; } = new();
        protected List<PERFIS> Perfis { get; set; } = new();
        protected List<VERTICAL> Verticais { get; set; } = new();
        protected List<ASSOCIADOS> Mentores { get; set; } = new();
        protected ASSOCIADOS AssociadoAtual { get; set; } = new();
        protected List<PROMOCOES> PromocoesAssociado { get; set; } = new();
        protected bool PromocaoMarcada { get; set; }
        protected MessageBox MessageBoxRef;

        protected List<DataTableComponent<ASSOCIADOS>.ColumnDefinition<ASSOCIADOS>> AssociadosColumns;
        protected List<DataTableComponent<PROMOCOES>.ColumnDefinition<PROMOCOES>> PromocoesColumns;

        protected override async Task OnInitializedAsync()
        {
            await CarregarCombosEGrids();
            DefinirColunas();
        }

        private async Task CarregarCombosEGrids()
        {
            Cargos = await CargosService.ObterListaCargosAsync(true);
            Perfis = await PerfisService.ObterListaPerfisAsync(true);
            Verticais = await VerticalService.ListarVerticaisAsync();
            Mentores = await AssociadosService.ObterAssociadosAsync(true);
            Associados = await AssociadosService.ObterAssociadosAsync();
        }

        private void DefinirColunas()
        {
            AssociadosColumns = new List<DataTableComponent<ASSOCIADOS>.ColumnDefinition<ASSOCIADOS>>
            {
                new() { Header = "Id", CellTemplate = a => (builder) => builder.AddContent(0, a.IdAssociado) },
                new() { Header = "Nome", CellTemplate = a => (builder) => builder.AddContent(0, a.Nome) },
                new() { Header = "Cargo", CellTemplate = a => (builder) => builder.AddContent(0, a.CARGOS?.Cargo ?? "") },
                new() { Header = "Mentor", CellTemplate = a => (builder) => builder.AddContent(0, a.ASSOCIADOS2?.Nome ?? "") },
                new() { Header = "Status(Ativo/Inativo)", CellTemplate = a => (builder) => builder.AddContent(0, a.ATV == 1 ? "Ativo" : "Inativo") }
            };

            PromocoesColumns = new List<DataTableComponent<PROMOCOES>.ColumnDefinition<PROMOCOES>>
            {
                new() { Header = "Id", CellTemplate = p => (builder) => builder.AddContent(0, p.idPromocao) },
                new() { Header = "Data", CellTemplate = p => (builder) => builder.AddContent(0, p.DataPromocao?.ToString("dd/MM/yyyy")) },
                new() { Header = "Cargo Antigo", CellTemplate = p => (builder) => builder.AddContent(0, p.CARGOS?.Cargo ?? "") },
                new() { Header = "Cargo Novo", CellTemplate = p => (builder) => builder.AddContent(0, p.CARGOS1?.Cargo ?? "") },
                new() {
                    Header = "Comentários",
                    CellTemplate = p => (builder) =>
                    {
                        builder.OpenElement(0, "input");
                        builder.AddAttribute(1, "type", "text");
                        builder.AddAttribute(2, "class", "form-control");
                        builder.AddAttribute(3, "value", p.Comentarios);
                        builder.AddAttribute(4, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, e => p.Comentarios = e.Value?.ToString()));
                        builder.CloseElement();
                    }
                }
            };
        }

        protected async Task SalvarAssociado()
        {
            if (string.IsNullOrWhiteSpace(AssociadoAtual.Nome))
            {
                MessageBoxRef.Show("Informe o Campo Nome !!", "", MessageBox.MessageBoxType.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(AssociadoAtual.Email))
            {
                MessageBoxRef.Show("Informe o Campo E-mail !!", "", MessageBox.MessageBoxType.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(AssociadoAtual.Senha))
            {
                MessageBoxRef.Show("Informe o Campo Senha !!", "", MessageBox.MessageBoxType.Warning);
                return;
            }
            if (AssociadoAtual.IdCargo == null || AssociadoAtual.IdCargo == 0)
            {
                MessageBoxRef.Show("Campo Cargo do Associado não Preenchido !!", "", MessageBox.MessageBoxType.Warning);
                return;
            }
            if (AssociadoAtual.IdAssociadoMentor == null || AssociadoAtual.IdAssociadoMentor == 0)
            {
                MessageBoxRef.Show("Campo Mentor não Preenchido !!", "", MessageBox.MessageBoxType.Warning);
                return;
            }
            if (AssociadoAtual.IdPerfil == null || AssociadoAtual.IdPerfil == 0)
            {
                MessageBoxRef.Show("Campo Perfil não Preenchido !!", "", MessageBox.MessageBoxType.Warning);
                return;
            }
            bool novo = AssociadoAtual.IdAssociado == 0;
            bool sucesso = false;
            if (novo)
            {
                sucesso = await AssociadosService.InserirAssociadoAsync(AssociadoAtual);
                if (sucesso)
                {
                    var associadoInserido = await AssociadosService.ObterUltimoAssociadoAsync();
                    if (!string.IsNullOrEmpty(AssociadoAtual.FotoBase64))
                    {
                        var foto = new FOTOSASSOCIADOS
                        {
                            Imagem = AssociadoAtual.FotoBase64,
                            AssociadoFoto = associadoInserido.Nome,
                            NomeFoto = associadoInserido.Nome + ".jpg",
                            IdAssociado = associadoInserido.IdAssociado,
                            DHC = DateTime.Now,
                            ATV = true
                        };
                        await FotosService.AdicionarFotoAsync(foto);
                    }
                    if (PromocaoMarcada)
                    {
                        var promocao = new PROMOCOES
                        {
                            idAssociado = associadoInserido.IdAssociado,
                            idCargoAnterior = associadoInserido.IdCargo,
                            idCargoNovo = associadoInserido.IdCargo,
                            DataPromocao = DateTime.Now,
                            Comentarios = "Contratação",
                            DHC = DateTime.Now,
                            ATV = true
                        };
                        await CargosService.AdicionarPromocaoAsync(promocao);
                    }
                    MessageBoxRef.Show("Associado Inserido com sucesso !!", "", MessageBox.MessageBoxType.Info);
                    await LimparFormulario();
                }
            }
            else
            {
                sucesso = await AssociadosService.AlterarAssociadoAsync(AssociadoAtual.IdAssociado, AssociadoAtual);
                if (sucesso)
                {
                    if (!string.IsNullOrEmpty(AssociadoAtual.FotoBase64))
                    {
                        var fotoExistente = await FotosService.ObterFotoPorAssociadoAsync(AssociadoAtual.IdAssociado);
                        if (fotoExistente == null)
                        {
                            var foto = new FOTOSASSOCIADOS
                            {
                                Imagem = AssociadoAtual.FotoBase64,
                                AssociadoFoto = AssociadoAtual.Nome,
                                NomeFoto = AssociadoAtual.Nome + ".jpg",
                                IdAssociado = AssociadoAtual.IdAssociado,
                                DHC = DateTime.Now,
                                ATV = true
                            };
                            await FotosService.AdicionarFotoAsync(foto);
                        }
                        else
                        {
                            fotoExistente.Imagem = AssociadoAtual.FotoBase64;
                            fotoExistente.AssociadoFoto = AssociadoAtual.Nome;
                            fotoExistente.NomeFoto = AssociadoAtual.Nome + ".jpg";
                            await FotosService.AtualizarFotoAsync(fotoExistente);
                        }
                    }
                    if (PromocaoMarcada)
                    {
                        var antigoAssociado = await AssociadosService.ObterAssociadoAsync(AssociadoAtual.IdAssociado);
                        var promocao = new PROMOCOES
                        {
                            idAssociado = AssociadoAtual.IdAssociado,
                            idCargoAnterior = antigoAssociado.IdCargo,
                            idCargoNovo = AssociadoAtual.IdCargo,
                            DataPromocao = DateTime.Now,
                            Comentarios = "Promoção",
                            DHC = DateTime.Now,
                            ATV = true
                        };
                        await CargosService.AdicionarPromocaoAsync(promocao);
                    }
                    MessageBoxRef.Show("Associado alterado com sucesso !!", "", MessageBox.MessageBoxType.Info);
                    await LimparFormulario();
                }
            }
            if (!sucesso)
            {
                MessageBoxRef.Show("Problema no cadastro/alteração do Associado !!", "", MessageBox.MessageBoxType.Warning);
            }
        }

        protected async Task HandleFotoSelecionada(IBrowserFile file)
        {
            if (file != null)
            {
                using var ms = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(ms);
                var bytes = ms.ToArray();
                AssociadoAtual.FotoBase64 = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
                StateHasChanged();
            }
        }

        protected async Task EditarAssociado(ASSOCIADOS associado)
        {
            AssociadoAtual = await AssociadosService.ObterAssociadoAsync(associado.IdAssociado) ?? new ASSOCIADOS();
            PromocoesAssociado = await CargosService.ObterPromocoesAssociadoAsync(AssociadoAtual.IdAssociado);
            PromocaoMarcada = false;
            var foto = await FotosService.ObterFotoPorAssociadoAsync(AssociadoAtual.IdAssociado);
            if (foto != null)
            {
                AssociadoAtual.FotoBase64 = foto.Imagem;
            }
            StateHasChanged();
        }

        protected async Task InativarAssociado(ASSOCIADOS associado)
        {
            var sucesso = await AssociadosService.InativarAssociadoAsync(associado.IdAssociado);
            if (sucesso)
            {
                MessageBoxRef.Show("Associado Inativado com sucesso !!", "", MessageBox.MessageBoxType.Info);
                await LimparFormulario();
            }
            else
            {
                MessageBoxRef.Show("Erro ao Inativar um Associado !!", "", MessageBox.MessageBoxType.Warning);
            }
        }

        protected async Task LimparFormulario()
        {
            AssociadoAtual = new ASSOCIADOS { Senha = "avaliacao", ATV = 1 };
            PromocoesAssociado = new List<PROMOCOES>();
            PromocaoMarcada = false;
            await CarregarCombosEGrids();
            StateHasChanged();
        }

        protected async Task SalvarComentariosPromocoes()
        {
            foreach (var promo in PromocoesAssociado)
            {
                await CargosService.AlterarPromocaoComentarioAsync(promo.idPromocao, promo.Comentarios);
            }
            MessageBoxRef.Show("Comentários salvos com sucesso!", "", MessageBox.MessageBoxType.Success);
        }

        protected async Task ExportarAssociados()
        {
            var dados = await AssociadosService.ObterAssociadosAsync();
            var bytes = await ExportService.ExportarAssociadosExcelAsync(dados);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Associados.xlsx", bytes);
        }

        protected async Task ExportarPromocoes()
        {
            var dados = await CargosService.ObterListaPromocoesAsync();
            var bytes = await ExportService.ExportarPromocoesExcelAsync(dados);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Promocoes.xlsx", bytes);
        }

        protected async Task ImportarAssociados(IBrowserFile file)
        {
            using var ms = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(ms);
            ms.Position = 0;
            var (inseridos, alterados, desconsiderados) = await ImportService.ImportarAssociadosExcelAsync(ms);
            MessageBoxRef.Show($"Associados importados com sucesso<br>Inseridos: {inseridos}<br>Alterados: {alterados}<br>Desconsiderados: {desconsiderados}", "", MessageBox.MessageBoxType.Info);
            await CarregarCombosEGrids();
        }

        protected async Task ImportarPromocoes(IBrowserFile file)
        {
            using var ms = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(ms);
            ms.Position = 0;
            var (inseridos, alterados, desconsiderados) = await ImportService.ImportarPromocoesExcelAsync(ms);
            MessageBoxRef.Show($"Promoções importadas com sucesso<br>Inseridos: {inseridos}<br>Alterados: {alterados}<br>Desconsiderados: {desconsiderados}", "", MessageBox.MessageBoxType.Info);
            await CarregarCombosEGrids();
        }
    }
}

using AjaxControlToolkit;
using Business.DataAccess;
using Business.Services;
using SistemaAvaliacao.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using System.Drawing;
using System.Security;
using System.Windows;
using System.Web.UI.HtmlControls;
using System.Reflection;
using System.Globalization;
using Business.Model;
using TriaSoftware.Util.Framework.Domain.Service;
using System.Web;
using System.Data.OleDb;
using OfficeOpenXml;
using System.Linq;

namespace SistemaAvaliacao
{
    public partial class Associados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LimpaCampos();
            }
        }

        private void LimpaCampos()
        {
            montaComboCargo();
            montaComboPerfis();
            montaComboMentor();
            montaComboNiveis();
            montaGridAssociados();
            montaComboVerticalAssociado();
            hddIdAssociado.Value = "";
            txtSenha.Text = "avaliacao";
            txtNome.Text = string.Empty;
            txtEmail.Text = string.Empty;
            ddlStatus.SelectedValue = "1";
            fotoCadastro.Src = "";
            this.rptHistoricoPromocoes.DataSource = null;
            this.rptHistoricoPromocoes.DataBind();
        }

        #region MONTA COMBOS E GRIDS
        public void montaComboCargo()
        {
            List<CARGOS> cargos = new CargosService().ObterListaCargos(true);

            this.ddlCargoAssociado.DataSource = cargos;
            this.ddlCargoAssociado.DataValueField = "IdCargo";
            this.ddlCargoAssociado.DataTextField = "Cargo";
            this.ddlCargoAssociado.DataBind();
            this.ddlCargoAssociado.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboNiveis()
        {
            //Tirado para implementar nova matriz de cargos que não possui níveis

            //List<CARGOSNIVEIS> niveis = new CargosNiveisService().ObterListaNiveis(true);

            //this.ddlNiveis.DataSource = niveis;
            //this.ddlNiveis.DataValueField = "IdNivel";
            //this.ddlNiveis.DataTextField = "Nivel";
            //this.ddlNiveis.DataBind();
            //this.ddlNiveis.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboPerfis()
        {
            List<PERFIS> cargos = new PerfisService().ObterListaPerfis(true);

            this.ddlPerfil.DataSource = cargos;
            this.ddlPerfil.DataValueField = "IdPerfil";
            this.ddlPerfil.DataTextField = "Perfil";
            this.ddlPerfil.DataBind();
            this.ddlPerfil.Items.Insert(0, "[Selecionar]");
        }

        public void montaComboMentor()
        {
            List<ASSOCIADOS> associados = new AssociadosService().ObterAssociados(true);
            this.ddlMentor.DataValueField = "IdAssociado";
            this.ddlMentor.DataTextField = "Nome";
            this.ddlMentor.DataSource = associados;
            this.ddlMentor.DataBind();
            this.ddlMentor.Items.Insert(0, "[Selecionar]");
        }

        public void montaGridAssociados()
        {
            List<ASSOCIADOS> associados = new AssociadosService().ObterAssociados();

            this.rptAssociados.DataSource = null;
            this.rptAssociados.DataSource = associados;
            this.rptAssociados.DataBind();
        }
        public void montaComboVerticalAssociado()
        {
            List<VERTICAL> verticais = new VerticalService().ListarVerticais();

            this.ddlVerticalAssociado.DataValueField = "IdVertical";
            this.ddlVerticalAssociado.DataTextField = "Descricao";
            this.ddlVerticalAssociado.DataSource = verticais;
            this.ddlVerticalAssociado.DataBind();
            this.ddlVerticalAssociado.Items.Insert(0, "[Selecionar]");
        }
        #endregion

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            try
            {
                AssociadosService associadoservice = new AssociadosService();
                FotosAssociadosService fotosService = new FotosAssociadosService();
                var cargosService = new CargosService();
                ASSOCIADOS associado = new ASSOCIADOS();
                FOTOSASSOCIADOS foto = new FOTOSASSOCIADOS();

                if (txtNome.Text != "")
                {
                    if (txtEmail.Text != "")
                    {
                        if ((new AssociadosService().ObterAssociado(txtEmail.Text) == null && hddIdAssociado.Value == "") || hddIdAssociado.Value != "")
                        {
                            if (txtSenha.Text != "")
                            {
                                if (ddlCargoAssociado.SelectedItem.Value != "[Selecionar]")
                                {
                                    associado.IdCargo = Convert.ToInt16(ddlCargoAssociado.SelectedItem.Value);

                                    if (ddlMentor.SelectedItem.Value != "[Selecionar]")
                                    {
                                        associado.IdAssociadoMentor = Convert.ToInt16(ddlMentor.SelectedItem.Value);

                                        //Tirado para implementar a nova matriz de cargos que não possui níveis
                                        //associado.IdNivel = Convert.ToInt32(ddlNiveis.SelectedValue);
                                        associado.IdNivel = 1;

                                        if (ddlPerfil.SelectedItem.Value != "[Selecionar]")
                                        {
                                            associado.IdPerfil = Convert.ToInt16(ddlPerfil.SelectedItem.Value);

                                            if (ddlStatus.SelectedValue == "1")
                                                associado.ATV = 1;
                                            else
                                                associado.ATV = 0;

                                            if (ddlStatus.SelectedValue == "1")
                                                associado.IdStatus = 1;
                                            else
                                                associado.IdStatus = 0;

                                            associado.Nome = txtNome.Text;
                                            associado.Email = txtEmail.Text;
                                            associado.DHC = DateTime.Now;
                                            associado.IdEmpresa = Convert.ToInt32(Session["IDEMPRESA"].ToString());
                                            //associado.FotoNome = fotoCadastro.Src;
                                            associado.Vertical = ddlVerticalAssociado.Items[ddlVerticalAssociado.SelectedIndex].Text; //txtVertical.Text;
                                            associado.IdVertical = Convert.ToInt32(ddlVerticalAssociado.Items[ddlVerticalAssociado.SelectedIndex].Value);
                                            associado.DataAdmissao = DateTime.Now;
                                            CultureInfo provider = CultureInfo.InvariantCulture;
                                            if (DateTime.TryParseExact(txtDataAdmissao.Text, "dd/MM/yyyy", provider, DateTimeStyles.None, out DateTime dataConvertida))
                                            {
                                                associado.DataAdmissao = dataConvertida;
                                            }


                                            associado.Senha = txtSenha.Text;
                                            // TODO: Implementar na tela o combo de níveis. 4 = Nivel em Difinição
                                            associado.USR = Convert.ToInt32(Session["IDUSUARIO"].ToString());


                                            if (hddIdAssociado.Value == "")
                                            {
                                                if (associadoservice.InserirAssociado(associado))
                                                {

                                                    var associadoInserido = associadoservice.ObterUltimoAssociado();
                                                    // salvar a foto na base de dados nesse ponto
                                                    if (Session["FOTO_BASE64"] != null)
                                                    {
                                                        foto.Imagem = Session["FOTO_BASE64"].ToString();
                                                        foto.AssociadoFoto = associadoInserido.Nome;
                                                        foto.NomeFoto = associadoInserido.Nome + "." + Session["FOTO_EXT"];
                                                        foto.IdAssociado = associadoInserido.IdAssociado;
                                                        fotosService.AdicionarFoto(foto);

                                                        // Limpa a Session após salvar
                                                        Session.Remove("FOTO_BASE64");
                                                        Session.Remove("FOTO_EXT");
                                                    }


                                                    PROMOCOES promocao = new PROMOCOES();
                                                    promocao.idAssociado = associadoInserido.IdAssociado;
                                                    promocao.idCargoAnterior = associadoInserido.IdCargo;
                                                    promocao.idCargoNovo = associadoInserido.IdCargo;
                                                    promocao.DataPromocao = DateTime.Now;
                                                    promocao.Comentarios = "Contratação";
                                                    promocao.DHC = DateTime.Now;
                                                    promocao.ATV = true;
                                                    if (cargosService.AdicionarPromocao(promocao))
                                                    {
                                                        MessageBox.Show("Associado Inserido com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                                                        LimpaCampos();
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Problema no cadastro do Associado !!", "", TIPO.Warning, MessageBoxHandler);
                                                }
                                            }
                                            else
                                            {
                                                var antigoAssociado = associadoservice.ObterAssociado(int.Parse(hddIdAssociado.Value));
                                                if (associadoservice.AlteraAssociado(Convert.ToInt32(hddIdAssociado.Value), associado))
                                                {
                                                    if (Session["FOTO_BASE64"] != null)
                                                    {
                                                        var fotoExistente = fotosService.ObterFotoPorAssociado(Convert.ToInt32(hddIdAssociado.Value));
                                                        if (fotoExistente == null)
                                                        {
                                                            fotoExistente = new FOTOSASSOCIADOS();
                                                            fotoExistente.Imagem = Session["FOTO_BASE64"]?.ToString() ?? "";
                                                            fotoExistente.AssociadoFoto = associado.Nome;
                                                            fotoExistente.NomeFoto = associado.Nome + "." + Session["FOTO_EXT"];
                                                            fotoExistente.IdAssociado = antigoAssociado.IdAssociado;
                                                            fotosService.AdicionarFoto(fotoExistente);
                                                        }
                                                        else
                                                        {
                                                            fotoExistente.Imagem = Session["FOTO_BASE64"]?.ToString() ?? fotoExistente.Imagem;
                                                            fotoExistente.AssociadoFoto = associado.Nome;
                                                            fotoExistente.NomeFoto = associado.Nome + "." + Session["FOTO_EXT"];
                                                            fotoExistente.IdAssociado = antigoAssociado.IdAssociado;
                                                            fotosService.AtualizarFoto(fotoExistente);
                                                        }
                                                        // Limpa a Session após salvar
                                                        Session.Remove("FOTO_BASE64");
                                                        Session.Remove("FOTO_EXT");
                                                    }

                                                    string conteudoTabela = hiddenHtmlCode.Value;
                                                    conteudoTabela = conteudoTabela.Split(new string[] { "<tbody>" }, StringSplitOptions.None)[1];
                                                    conteudoTabela = conteudoTabela.Split(new string[] { "</tbody>" }, StringSplitOptions.None)[0];
                                                    var linhas = conteudoTabela.Split(new string[] { "<tr role" }, StringSplitOptions.None);
                                                    foreach (var linha in linhas)
                                                    {
                                                        if (linha.Contains("<td") && !linha.Contains("Nenhum registro encontrado"))
                                                        {
                                                            var textIdPromocao = linha.Split(new string[] { "<td" }, StringSplitOptions.None)[1];
                                                            textIdPromocao = textIdPromocao.Split(new string[] { ">" }, StringSplitOptions.None)[1];
                                                            textIdPromocao = textIdPromocao.Split(new string[] { "<" }, StringSplitOptions.None)[0];
                                                            int idPromocao = int.Parse(textIdPromocao);

                                                            var textComentario = linha.Split(new string[] { "<td contenteditable" }, StringSplitOptions.None)[1];
                                                            textComentario = textComentario.Split(new string[] { ">" }, StringSplitOptions.None)[1];
                                                            textComentario = textComentario.Split(new string[] { "<" }, StringSplitOptions.None)[0];

                                                            cargosService.AlterarPromocaoComentario(idPromocao, textComentario);
                                                        }
                                                    }

                                                    if (checkboxPromocao.Checked)
                                                    {
                                                        PROMOCOES promocao = new PROMOCOES();
                                                        promocao.idAssociado = antigoAssociado.IdAssociado;
                                                        promocao.idCargoAnterior = antigoAssociado.IdCargo;
                                                        promocao.idCargoNovo = associado.IdCargo;
                                                        promocao.DataPromocao = DateTime.Now;
                                                        promocao.Comentarios = "Promoção";
                                                        promocao.DHC = DateTime.Now;
                                                        promocao.ATV = true;
                                                        if (cargosService.AdicionarPromocao(promocao))
                                                        {
                                                            MessageBox.Show("Associado alterado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                                                            LimpaCampos();
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Problema ao inserir Promoção !!", "", TIPO.Warning, MessageBoxHandler);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Associado alterado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                                                        LimpaCampos();
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Problema no alteração do Associado !!", "", TIPO.Warning, MessageBoxHandler);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Campo Perfil não Preenchido !!", "", TIPO.Warning, MessageBoxHandler);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Campo Mentor não Preenchido !!", "", TIPO.Warning, MessageBoxHandler);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Campo Cargo do Associado não Preenchido !!", "", TIPO.Warning, MessageBoxHandler);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Informe o Campo Senha !!", "", TIPO.Warning, MessageBoxHandler);
                            }
                        }
                        else
                        {
                            MessageBox.Show("E-mail  já existente na base de dados !!", "", TIPO.Warning, MessageBoxHandler);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Informe o Campo E-mail !!", "", TIPO.Warning, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Informe o Campo Nome !!", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERRO ao tentar salvar o usuário: " + ex.Message, "", TIPO.Warning, MessageBoxHandler);
            }
        }

        #region Botão provisório Migrar Fotos
        //protected void btnMigrarFotos_Click(object sender, EventArgs e)
        //{
        //    string basePath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
        //    string fotosPath = Path.Combine(basePath, "SistemaAvaliacao", "Content", "FotosAssociados");

        //    if (Directory.Exists(fotosPath))
        //    {
        //        var arquivos = Directory.GetFiles(fotosPath);
        //        foreach (var arquivo in arquivos)
        //        {
        //            string nomeArquivo = Path.GetFileName(arquivo);
        //            var extensao = Path.GetExtension(arquivo);
        //            var mimeType = "image/" + extensao.TrimStart('.').ToLower();
        //            string base64 = Convert.ToBase64String(File.ReadAllBytes(arquivo));

        //            string fotoNome = $"Content/FotosAssociados/{nomeArquivo}";

        //            AssociadosService service = new AssociadosService();
        //            var associado = service.ObterAssociadoPeloNomeDaFoto(fotoNome);

        //            if(associado != null)
        //            {
        //                FotosAssociadosService fotosService = new FotosAssociadosService();

        //                var fotoExistente = fotosService.ObterFotoPorAssociado(associado.IdAssociado);

        //                if(fotoExistente == null)
        //                {
        //                    FOTOSASSOCIADOS foto = new FOTOSASSOCIADOS();
        //                    foto.AssociadoFoto = associado.Nome;
        //                    foto.IdAssociado = associado.IdAssociado;
        //                    foto.Imagem = $"data:{mimeType};base64,{base64}";
        //                    foto.NomeFoto = nomeArquivo;

        //                    fotosService.AdicionarFoto(foto);
        //                }

        //            }

        //        }

        //    }
        //    else
        //    {
        //        MessageBox.Show("Migração de fotos não realizada", "Pasta de fotos não encontrada", TIPO.Warning, MessageBoxHandler);
        //    }
        //}

        #endregion

        protected void btnFoto_Click(object sender, EventArgs e)
        {
            if (FileUpLoad1.HasFile)
            {
                var file = FileUpLoad1.PostedFile;
                string extension = Path.GetExtension(file.FileName).ToLower().Replace(".", "");
                string mimeType = "image/jpeg";
                if (extension == "png") mimeType = "image/png";
                else if (extension == "gif") mimeType = "image/gif";
                else if (extension == "bmp") mimeType = "image/bmp";
                else if (extension == "jpg" || extension == "jpeg") mimeType = "image/jpeg";

                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                    string base64String = Convert.ToBase64String(fileBytes);
                    fotoCadastro.Src = $"data:{mimeType};base64,{base64String}";

                    Session["FOTO_BASE64"] = $"data:{mimeType};base64,{base64String}";
                    Session["FOTO_EXT"] = extension;
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('Nenhum arquivo selecionado');", true);
            }
        }
        protected void rptAssociados_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            if (e.CommandName == "AlteraAssociado")
            {
                var idAssociado = ((Label)e.Item.FindControl("lblIAssociado"));
                hddIdAssociado.Value = Convert.ToString(idAssociado.Text);
                MontaCamposAssociados(Convert.ToInt32(idAssociado.Text));

                var cargosService = new CargosService();
                var getPromocoes = cargosService.ObterTodasPromocoesAssociado(int.Parse(hddIdAssociado.Value));
                rptHistoricoPromocoes.DataSource = null;
                rptHistoricoPromocoes.DataSource = getPromocoes;
                rptHistoricoPromocoes.DataBind();
            }
            else
            {
                var idAssociado = ((Label)e.Item.FindControl("lblIAssociado"));

                var associado = new AssociadosService().ObterAssociado(Convert.ToInt32(idAssociado.Text));
                associado.ATV = 0;
                associado.IdStatus = 0;

                AssociadosService associadoservice = new AssociadosService();
                var statusExclusao = associadoservice.ExcluiAssociado(Convert.ToInt32(idAssociado.Text), associado);

                if (statusExclusao)
                {
                    LimpaCampos();
                    MessageBox.Show("Associado Inativado com sucesso !!", "", TIPO.Info, MessageBoxHandler);
                }
                else
                {
                    MessageBox.Show("Erro ao Ecluir um Associado !!", "", TIPO.Warning, MessageBoxHandler);
                }
            }
        }

        private void MontaCamposAssociados(int idAssociado)
        {
            var associado = new AssociadosService().ObterAssociado(Convert.ToInt32(idAssociado));
            var fotoAssociado = new FotosAssociadosService().ObterFotoPorAssociado(associado.IdAssociado);

            txtNome.Text = associado.Nome;

            if (ddlMentor.Items.FindByValue(associado.IdAssociadoMentor.ToString()) != null)
            {
                ddlMentor.SelectedValue = Convert.ToString(associado.IdAssociadoMentor);
            }

            txtEmail.Text = associado.Email;
            ddlCargoAssociado.SelectedValue = Convert.ToString(associado.IdCargo);
            ddlPerfil.SelectedValue = Convert.ToString(associado.IdPerfil);
            ddlStatus.SelectedValue = Convert.ToString(associado.ATV);
            //ddlNiveis.SelectedValue = Convert.ToString(associado.IdNivel);
            txtSenha.Text = associado.Senha;
            txtDataAdmissao.Text = associado.DataAdmissao != null ? ((DateTime)associado.DataAdmissao).ToString("dd'/'MM'/'yyyy") : "";
            ddlVerticalAssociado.SelectedValue = Convert.ToString(associado.IdVertical);

            if (fotoAssociado != null && !string.IsNullOrEmpty(fotoAssociado.Imagem))
            {
                // Exibe a foto do associado
                fotoCadastro.Src = fotoAssociado.Imagem;
                Session["FOTO_BASE64"] = fotoAssociado.Imagem;
                Session["FOTO_EXT"] = fotoAssociado.NomeFoto.Split('.').Last();
            }
            else
            {
                fotoCadastro.Src = "";
                Session.Remove("FOTO_BASE64");
                Session.Remove("FOTO_EXT");
            }

        }

        protected void rptAssociados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (((Label)e.Item.FindControl("lblATV")).Text == "1")
                {
                    ((Label)e.Item.FindControl("lblATV")).Text = "Ativo";
                }
                else
                {
                    ((Label)e.Item.FindControl("lblATV")).Text = "Inativo";
                    ((Button)e.Item.FindControl("btnDeletar")).Visible = false;
                }
            }
        }

        protected void ButtonFoto_Click(object sender, EventArgs e)
        {

        }

        protected void rptHistoricoPromocoes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void rptHistoricoPromocoes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void btnExportAssociados_Click(object sender, EventArgs e)
        {
            List<AssociadosModelExport> listaExport = new List<AssociadosModelExport>();
            var associadosService = new AssociadosService();
            var getAssociados = associadosService.ObterAssociados();

            foreach (var item in getAssociados)
            {
                AssociadosModelExport addItem = new AssociadosModelExport();
                addItem.IdAssociado = item.IdAssociado;
                addItem.Nome = item.Nome;
                addItem.IdCargo = item.IdCargo;
                addItem.Cargo = item.CARGOS.Cargo;
                addItem.IdEmpresa = item.IdEmpresa;
                addItem.Empresa = item.EMPRESAS.Empresa;
                addItem.IdPerfil = item.IdPerfil;
                addItem.Perfil = item.PERFIS.Perfil;
                addItem.IdMentor = item.IdAssociadoMentor;
                addItem.Mentor = item.ASSOCIADOS2.Nome;
                addItem.Email = item.Email;
                addItem.DataAdmissao = item.DataAdmissao;
                addItem.Vertical = item.Vertical;
                addItem.IdVertical = item.IdVertical;
                addItem.ATV = item.ATV;
                listaExport.Add(addItem);
            }

            string fileName = "Associados_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, listaExport); // Caminho não é necessário, só o byte[]

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        protected void btnImportAssociados_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUploadAssociados.HasFile)
                {
                    using (var package = new OfficeOpenXml.ExcelPackage(fileUploadAssociados.FileContent))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            MessageBox.Show("O arquivo Excel não possui nenhuma planilha.", "", TIPO.Warning, MessageBoxHandler);
                            return;
                        }
                        int rowCount = worksheet.Dimension.End.Row;
                        int somaLinhasDesconsideradas = 0, somaLinhasInseridas = 0, somaLinhasAlteradas = 0;
                        for (int row = 2; row <= rowCount; row++) // Começa da linha 2 para ignorar o cabeçalho
                        {
                            int IdAssociado = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                            string Nome = worksheet.Cells[row, 2].GetValue<string>() ?? "";
                            int IdCargo = worksheet.Cells[row, 3].GetValue<int?>() ?? 0;
                            int IdEmpresa = worksheet.Cells[row, 5].GetValue<int?>() ?? 1;
                            int IdPerfil = worksheet.Cells[row, 7].GetValue<int?>() ?? 0;
                            int IdMentor = worksheet.Cells[row, 9].GetValue<int?>() ?? 0;
                            string Email = worksheet.Cells[row, 11].GetValue<string>() ?? "";
                            DateTime? DataAdmissao = worksheet.Cells[row, 12].GetValue<DateTime?>();
                            string Vertical = worksheet.Cells[row, 13].GetValue<string>() ?? "";
                            int IdVertical = worksheet.Cells[row, 14].GetValue<int?>() ?? 1;
                            int ATV = worksheet.Cells[row, 15].GetValue<int?>() ?? 0;

                            if (Nome != "" && IdEmpresa > 0 && IdPerfil > 0 && IdMentor > 0 && Email != "" && DataAdmissao != null)
                            {
                                ASSOCIADOS importItem = new ASSOCIADOS();
                                importItem.IdAssociado = IdAssociado;
                                importItem.Nome = Nome;
                                importItem.IdCargo = IdCargo;
                                importItem.IdEmpresa = IdEmpresa;
                                importItem.IdPerfil = IdPerfil;
                                importItem.IdNivel = 1;
                                importItem.IdStatus = 1;
                                importItem.IdAssociadoMentor = IdMentor;
                                importItem.DataAdmissao = (DateTime)DataAdmissao;
                                importItem.Email = Email;
                                importItem.Senha = "avaliacao";
                                importItem.Vertical = Vertical;
                                importItem.FotoNome = null;
                                importItem.ATV = ATV;
                                importItem.USR = Convert.ToInt32(Session["IDASSOCIADOLOGADO"].ToString());
                                importItem.DHC = DateTime.Now;
                                importItem.IdVertical = IdVertical;

                                var mainService = new AssociadosService();
                                var existeItem = IdAssociado != 0 ? mainService.ObterAssociado(IdAssociado) : null;

                                if (existeItem == null)
                                {
                                    mainService.InserirAssociado(importItem);
                                    somaLinhasInseridas += 1;
                                }
                                else
                                {
                                    importItem.IdAssociado = IdAssociado;
                                    importItem.Senha = existeItem.Senha;
                                    importItem.FotoNome = existeItem.FotoNome;
                                    mainService.AlteraAssociado(IdAssociado, importItem);
                                    somaLinhasAlteradas += 1;
                                }
                            }
                            else
                            {
                                somaLinhasDesconsideradas += 1;
                            }
                        }
                        MessageBox.Show("Associados importados com sucesso<br>Inseridos: " + somaLinhasInseridas.ToString() + "<br>Alterados: " + somaLinhasAlteradas.ToString() +
                            "<br>Desconsiderados: " + somaLinhasDesconsideradas.ToString(), "", TIPO.Info, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Nenhum arquivo selecionado", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "", TIPO.Warning, MessageBoxHandler);
            }
        }

        protected void btnExportPromocoes_Click(object sender, EventArgs e)
        {
            List<PromocoesModelExport> listaExport = new List<PromocoesModelExport>();
            var cargosService = new CargosService();
            var getPromocoes = cargosService.ObterListaPromocoes();

            foreach (var item in getPromocoes)
            {
                PromocoesModelExport addItem = new PromocoesModelExport();
                addItem.IdPromocao = item.idPromocao;
                addItem.IdAssociado = item.idAssociado;
                addItem.Associado = item.idAssociado != null ? item.ASSOCIADOS.Nome : "";
                addItem.IdCargoAnterior = item.idCargoAnterior;
                addItem.CargoAnterior = item.idCargoAnterior != null ? item.CARGOS.Cargo : "";
                addItem.IdCargoNovo = item.idCargoNovo;
                addItem.CargoNovo = item.idCargoNovo != null ? item.CARGOS1.Cargo : "";
                addItem.DataPromocao = item.DataPromocao;
                addItem.Comentarios = item.Comentarios;
                addItem.ATV = (bool)item.ATV;
                listaExport.Add(addItem);
            }

            string fileName = "Promocoes_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
            ExportFileService excel = new ExportFileService();
            var result = excel.GenerateExcelConsideracoesMentor(fileName, listaExport); // Caminho não é necessário, só o byte[]

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName + ";");
            HttpContext.Current.Response.BinaryWrite(result);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        protected void btnImportPromocoes_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileUploadPromocoes.HasFile)
                {
                    using (var package = new OfficeOpenXml.ExcelPackage(fileUploadPromocoes.FileContent))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            MessageBox.Show("O arquivo Excel não possui nenhuma planilha.", "", TIPO.Warning, MessageBoxHandler);
                            return;
                        }
                        int rowCount = worksheet.Dimension.End.Row;
                        int somaLinhasDesconsideradas = 0, somaLinhasInseridas = 0, somaLinhasAlteradas = 0;
                        for (int row = 2; row <= rowCount; row++) // Começa da linha 2 para ignorar o cabeçalho
                        {
                            int IdPromocao = worksheet.Cells[row, 1].GetValue<int?>() ?? 0;
                            int IdAssociado = worksheet.Cells[row, 2].GetValue<int?>() ?? 0;
                            int IdCargoAnterior = worksheet.Cells[row, 4].GetValue<int?>() ?? 0;
                            int IdCargoNovo = worksheet.Cells[row, 6].GetValue<int?>() ?? 0;
                            DateTime? DataPromocao = worksheet.Cells[row, 8].GetValue<DateTime?>();
                            string Comentarios = worksheet.Cells[row, 9].GetValue<string>() ?? "Import";
                            bool ATV = worksheet.Cells[row, 10].GetValue<bool?>() ?? true;

                            if (IdAssociado > 0 && IdCargoAnterior > 0 && IdCargoNovo > 0 && DataPromocao != null)
                            {
                                PROMOCOES importItem = new PROMOCOES();
                                importItem.idAssociado = IdAssociado;
                                importItem.idCargoAnterior = IdCargoAnterior;
                                importItem.idCargoNovo = IdCargoNovo;
                                importItem.DataPromocao = DataPromocao;
                                importItem.Comentarios = Comentarios;
                                importItem.ATV = ATV;
                                importItem.DHC = DateTime.Now;

                                var mainService = new CargosService();
                                var existeItem = IdPromocao != 0 ? mainService.ObterPromocao(IdAssociado, IdCargoAnterior, IdCargoNovo) : null;

                                if (existeItem == null)
                                {
                                    mainService.AdicionarPromocao(importItem);
                                    somaLinhasInseridas += 1;
                                }
                                else
                                {
                                    importItem.idPromocao = IdPromocao;
                                    mainService.AlterarPromocao(importItem);
                                    somaLinhasAlteradas += 1;
                                }
                            }
                            else
                            {
                                somaLinhasDesconsideradas += 1;
                            }
                        }
                        MessageBox.Show("Associados importados com sucesso<br>Inseridos: " + somaLinhasInseridas.ToString() + "<br>Alterados: " + somaLinhasAlteradas.ToString() +
                            "<br>Desconsiderados: " + somaLinhasDesconsideradas.ToString(), "", TIPO.Info, MessageBoxHandler);
                    }
                }
                else
                {
                    MessageBox.Show("Nenhum arquivo selecionado", "", TIPO.Warning, MessageBoxHandler);
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.Message, "", TIPO.Warning, MessageBoxHandler);
            }
        }
    }
}
<%@ Page Title="" Language="C#" Debug="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Competencias.aspx.cs" Inherits="SistemaAvaliacao.Competencias" ValidateRequest="false" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- ============================================================== -->
    <!-- Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Competências</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastro Básico de Projetos</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Competências</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>
    <!-- ============================================================== -->
    <!-- End Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->

    <!-- ============================================================== -->
    <!-- Container fluid  -->
    <!-- ============================================================== -->
    <div class="page-content container-fluid">
        <!-- ============================================================== -->
        <!-- Start Page Content -->
        <!-- ============================================================== -->

        <%--EDITOR DE CAMPOS--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Dados da Competência</h4>
                <form action="#">
                    <div class="form-body">
                        <div class="text-left">
                            <label>Tipo de Avaliação</label><br />
                            <asp:DropDownList ID="ddlTipoAvaliacao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="comboTrocaAvaliacao" CssClass="form-control p-0 select2"
                                Style="width: 25%"></asp:DropDownList><br />
                        </div>
                    </div>

                    <div class="form-body" id="divDesempenho" runat="server">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Cargo</label>
                                    <asp:DropDownList ID="ddlCargo" runat="server" CssClass="form-control p-0 select2" Style="width: 100%"
                                        OnSelectedIndexChanged="ddlSubCompetencia_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Eixo</label>
                                    <asp:DropDownList ID="ddlEixo" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Sub Competência</label>
                                    <asp:DropDownList ID="ddlSubCompetencia" runat="server" CssClass="form-control p-0 select2" Style="width: 100%" 
                                        OnSelectedIndexChanged="ddlSubCompetencia_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Dimensão</label>
                                    <asp:DropDownList ID="ddlDimensao" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                    </asp:DropDownList>

                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-9">
                                <div class="form-group">
                                    <label>Detalhamento Nível Atual</label>
                                    <textarea name="txtDetalhamentoJr" id="txtDetalhamentoJr" rows="5" cols="40" runat="server" class="form-control">Detalhamento</textarea>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <label>Nivel Atual</label>
                                    <asp:TextBox ID="txtJunior" class="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            
                            <div class="col-md-9" hidden>
                                <div class="form-group">
                                    <label>Detalhamento Próximo Nível</label>
                                    <textarea id="txtDetalhamentoPleno" name="txtDetalhamentoPleno" runat="server" rows="5" cols="40" class="form-control"></textarea>
                                </div>
                            </div>
                            <div class="col-md-3" hidden>
                                <div class="form-group">
                                    <label>Próximo Nível</label>
                                    <asp:TextBox ID="txtPleno" class="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm">
                                <div class="form-group">
                                    <label>Palavras-chave</label>
                                    <asp:TextBox ID="txtPalavrasChave" class="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm">
                                <div class="form-group">
                                    <label>Resumo da subcompetência pro cargo</label>
                                    <asp:UpdatePanel id="updateRelacaoSubcompetencia" UpdateMode="Always" runat="server">
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ddlCargo" />
                                            <asp:AsyncPostBackTrigger ControlID="ddlSubCompetencia" />
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:TextBox ID="txtRelacaoSubcompetencia" class="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <asp:Label runat="server" Font-Italic="true" Font-Size="12px" ForeColor="LightGray" Text="O valor acima é compartilhado para todas as competências que possuírem a mesma associação de cargo e subcompetência"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <h4>Configurações de auto preenchimento</h4>
                        </div>
                        <br />
                        <div class="row" style="white-space:nowrap">
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxInputAutoAv" checked/>
                                <label class="labelCbox">Input Auto Avaliação</label>
                            </div>
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxInputAvCegas" checked/>
                                <label class="labelCbox">Input Avaliação as Cegas</label>
                            </div>
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxInputAvGestor" checked/>
                                <label class="labelCbox">Input Avaliação do Gestor</label>
                            </div>
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxInputFeedback" checked/>
                                <label class="labelCbox">Input Feedback</label>
                            </div>
                        </div>
                        <div class="row" style="white-space:nowrap">
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxInputNivel1" checked/>
                                <label class="labelCbox">Input Nível 1</label>
                            </div>
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxInputNivel2" checked/>
                                <label class="labelCbox">Input Nível 2</label>
                            </div>
                        </div>
                        <br />
                        <div class="row" style="white-space:nowrap">
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxVisivelAutoAv" checked/>
                                <label class="labelCbox">Visível Auto Avaliação</label>
                            </div>
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxVisivelAvCegas" checked/>
                                <label class="labelCbox">Visível Avaliação as Cegas</label>
                            </div>
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxVisivelAvGestor" checked/>
                                <label class="labelCbox">Visível Avaliação do Gestor</label>
                            </div>
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxVisivelFeedback" checked/>
                                <label class="labelCbox">Visível Feedback</label>
                            </div>
                        </div>
                        <div class="row" style="white-space:nowrap">
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxVisivelNivel1" checked/>
                                <label class="labelCbox">Visível Nível 1</label>
                            </div>
                            <div class="col-sm">
                                <input type="checkbox" runat="server" class="checkbox" id="cboxVisivelNivel2" checked/>
                                <label class="labelCbox">Visível Nível 2</label>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-md-3">
                                <label class="labelCbox">Nota Padrão Nível 1</label>
                                <asp:DropDownList ID="ddlNotaPadraoNivel1" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <label class="labelCbox">Nota Padrão Nível 2</label>
                                <asp:DropDownList ID="ddlNotaPadraoNivel2" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-md-3">
                                <label class="labelCbox">Modo de Cálculo</label>
                                <asp:DropDownList ID="ddlModoCalculo" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="form-body" id="divLideranca" runat="server">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Escopo</label><br />
                                    <asp:DropDownList ID="ddlEscopoLideranca" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Pilar</label><br />
                                    <asp:DropDownList ID="ddlEixoLideranca" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Título</label><br />
                                    <asp:DropDownList ID="ddlTituloLideranca" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <label>Detalhamento</label>
                                    <asp:TextBox ID="textDetalheLideranca" class="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="form-actions">
                        <div class="text-right">
                            <asp:Button id="btnAgregar" OnClick="btnAgregar_Click" CssClass="btn btn-facebook" runat="server" Text="Agregar" 
                                ToolTip="Adicionar esta competência as avaliações já existentes do período atual que não possuírem esta competência." />
                            <asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />
                        </div>
                    </div>
            </div>
        </div>


        <%--EXPORT E IMPORT--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Exportar e Importar Competências</h4>
                <h6 class="card-subtitle">Utilize o modelo de arquivo exportado para alterar ou adicionar competências.</h6>
                <h6 class="card-subtitle">Mantenha uma linha SEM um valor na coluna IdCompetência para adicionar uma nova competência.</h6>
                <h6 class="card-subtitle">As correlações de cargo, eixo, subcompetência e dimensão devem ser identificadas por seus CÓDIGOS.</h6>
                <h6 class="card-subtitle">NÃO altere a ordem e quantidade das colunas, a quantidade de abas do arquivo ou coloque linhas acima da linha de títulos.</h6>
                <asp:Button runat="server" type="button" ID="btnExport" class="btn btn-info" OnClick="btnExport_Click" Text="Exportar"></asp:Button>
                <asp:Button runat="server" type="button" ID="btnImport" class="btn btn-info" OnClick="btnImport_Click" Text="Importar"></asp:Button>
                <asp:FileUpload ID="fileUpload" runat="server" AllowMultiple="false" accept=".xlsx" class="btn btn-danger" /> 
            </div>
        </div>

        <%--LISTA DE COMPETENCIAS--%>
        <div class="card">

            <div class="card-body">
                <h4 class="card-title">Lista de Competências</h4>
                <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code>competência nível cargo</code>. </h6>
                <div class="table-responsive">
                    
                    <asp:HiddenField ID="hdId" runat="server" />


                <asp:Repeater ID="rptCompetencias" OnItemCommand="rpt_ItemCommand" OnItemDataBound="rpt_ItemDataBound" runat="server">
                        <HeaderTemplate>
                            <table class="table table-striped border dtInit">
                                <thead>
                                    <tr>
                                        <th>Código</th>
                                        <th>Cargo</th>
                                        <th>Escopo</th>
                                        <th>Eixo</th>
                                        <th>Sub Competência</th>
                                        <th>Competencia Jr</th>
                                        <%--<th hidden>Competencia Pl</th>--%>
                                        <%--<th hidden>Competencia Sr</th>--%>
                                        <th>Status(Ativo/Inativo)</th>
                                        <th data-sort="0">Ação</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                

                                <td><asp:Label ID="lblId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "idCompetencia") %>' Visible="false"></asp:Label><%# DataBinder.Eval(Container.DataItem, "IdCompetencia") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "CARGOS.Cargo") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Escopo") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "EIXOS.Eixo") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "SUBCOMPETENCIAS.SubCompetencia") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "CompetenciaJR") %></td>
                                <%--<td hidden><%# DataBinder.Eval(Container.DataItem, "CompetenciaPL") %></td>
                                <td hidden><%# DataBinder.Eval(Container.DataItem, "CompetenciaSR") %></td>--%>
                                <td><asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                <td>
                                   <asp:Button ID="btnAlterar" class="btn btn-info" runat="server" Text="Alterar" CommandName="Alterar" />
                                    <asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Inativar" CommandName="Excluir" />

                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
								<tfoot>
                                    <tr>
                                    <th>Código</th>
                                    <th>Cargo</th>
                                    <th>Escopo</th>
                                    <th>Eixo</th>
                                    <th>Sub Competência</th>
                                    <th>Competencia Jr</th>
                                    <%--<th hidden>Competencia Pl</th>
                                    <th hidden>Competencia Sr</th>--%>
                                    <th>Status(Ativo/Inativo)</th>
                                    <th data-sort="0">Ação</th>
                                </tr>
                                </tfoot>
                            </table>

                        </FooterTemplate>
                    </asp:Repeater>

                </div>
            </div>

        </div>

    </div>
    <!-- ============================================================== -->
    <!-- End PAge Content -->
    <!-- ============================================================== -->
    <!-- ============================================================== -->
    <!-- Right sidebar -->
    <!-- ============================================================== -->
    <!-- .right-sidebar -->
    <!-- ============================================================== -->
    <!-- End Right sidebar -->
    <!-- ============================================================== -->

    <!-- ============================================================== -->
    <!-- End Container fluid  -->
    <!-- ============================================================== -->


    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <!--<script src="assets/libs/popper.js/dist/umd/popper.min.js"></script>
    <script src="assets/libs/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- apps -->
    <script src="dist/js/app.min.js"></script>
    <script src="dist/js/app.init.js"></script>
    <script src="dist/js/app-style-switcher.js"></script>
    <script src="assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js"></script>
    <script src="assets/extra-libs/sparkline/sparkline.js"></script>
    <script src="dist/js/waves.js"></script>
    <!-- <script src="dist/js/sidebarmenu.js"></script>
    <!--Custom JavaScript -->
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/libs/ckeditor/ckeditor.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>
    <script src="assets/libs/ckeditor/ckeditor.js"></script>

    <script src="dist/js/custom.min.js"></script>
    <script>
        var element = document.getElementById('<%=txtDetalhamentoJr.ClientID%>');
        CKEDITOR.replace(element,
            {
                height: 100
            });

        var element = document.getElementById('<%=txtDetalhamentoPleno.ClientID%>');
        CKEDITOR.replace(element,
            {
                height: 100
            });
    </script>
    <script>
        document.getElementById('<%= txtJunior.ClientID %>').addEventListener('input', function() {
            document.getElementById('<%= txtRelacaoSubcompetencia.ClientID %>').value = this.value;
        });
    </script>
    <style>
        .checkbox{
            width:1.7em;
            height:1.7em;
            accent-color:#003150;
        }

        .labelCbox{
            vertical-align:bottom;
        }
    </style>


</asp:Content>

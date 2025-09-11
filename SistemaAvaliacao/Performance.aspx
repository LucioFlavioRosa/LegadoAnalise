<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Performance.aspx.cs" Inherits="SistemaAvaliacao.Performance" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Performance</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastro Básico de Projetos</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Performance</li>
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


        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Dados da performance</h4>
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Cargo </label>
                                <asp:DropDownList ID="ddlCargo" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Status</label>
                                <asp:DropDownList ID="ddlStatus" class="form-control" runat="server" Style="width: 100%">
                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="0">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Performance</label>
                                <asp:TextBox ID="txtPerformance" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Abrangência</label>
                                <asp:DropDownList ID="ddlAbrangencia" class="form-control" runat="server" Style="width: 100%">
                                    <asp:ListItem Value="0">Individual</asp:ListItem>
                                    <asp:ListItem Value="1">Coletivo</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Abaixo</label>
                                <asp:TextBox ID="txtAbaixo" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Esperado</label>
                                <asp:TextBox ID="txtEsperado" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Acima</label>
                                <asp:TextBox ID="txtAcima" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Input auto-avaliação</label>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Input avaliação as cegas</label>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Input avaliação do gestor</label>
                            </div>
                        </div>
                    </div>
                    <div class="row" style="margin-top:-16px">
                        <%--INPUT E NOTA PADRÃO -- AUTO AVALIAÇÃO--%>
                        <div class="col-sm">
                            <div class="form-group">
                                <input type="checkbox" name="chboxInputAutoAvaliacao" ID="chboxInputAutoAvaliacao" style="transform: scale(2.1); margin-left:7px" checked onclick="InputAutoAvaliacao(0)"/>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group" style="margin-left:-100px">
                                <asp:Label ID="labelNotaPadraoAutoAvaliacao" runat="server" Text="Nota padrão:" Font-Italic="true" ForeColor="gray" Style="display:none;"></asp:Label>
                            </div>
                        </div>
                        <div class="col-2">
                            <div class="form-group" style="margin-left:-140px;margin-top:-6px;display:none" id="divDDLNotaPadraoAutoAvaliacao">
                                <asp:DropDownList ID="ddlNotaPadraoAutoAvaliacao" class="form-control" runat="server" Style="width: 100%"></asp:DropDownList>
                            </div>
                        </div>
                        <%--INPUT E NOTA PADRÃO -- AVALIAÇÃO AS CEGAS--%>
                        <div class="col-sm">
                            <div class="form-group">
                                <input type="checkbox" name="chboxInputAvaliacaoAsCegas" ID="chboxInputAvaliacaoAsCegas" style="transform: scale(2.1); margin-left:7px" checked onclick="InputAutoAvaliacao(1)"/>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group" style="margin-left:-100px">
                                <asp:Label ID="labelNotaPadraoAvaliacaoAsCegas" runat="server" Text="Nota padrão:" Font-Italic="true" ForeColor="gray" Style="display:none;"></asp:Label>
                            </div>
                        </div>
                        <div class="col-2">
                            <div class="form-group" style="margin-left:-140px;margin-top:-6px;display:none" id="divDDLNotaPadraoAvaliacaoAsCegas">
                                <asp:DropDownList ID="ddlNotaPadraoAvaliacaoAsCegas" class="form-control" runat="server" Style="width: 100%"></asp:DropDownList>
                            </div>
                        </div>
                        <%--INPUT E NOTA PADRÃO -- AVALIAÇÃO GESTOR--%>
                        <div class="col-sm">
                            <div class="form-group">
                                <input type="checkbox" name="chboxInputAvaliacaoGestor" ID="chboxInputAvaliacaoGestor" style="transform: scale(2.1); margin-left:7px" checked onclick="InputAutoAvaliacao(2)"/>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group" style="margin-left:-100px">
                                <asp:Label ID="labelNotaPadraoAvaliacaoGestor" runat="server" Text="Nota padrão:" Font-Italic="true" ForeColor="gray" Style="display:none;"></asp:Label>
                            </div>
                        </div>
                        <div class="col-2">
                            <div class="form-group" style="margin-left:-140px;margin-top:-6px;display:none" id="divDDLNotaPadraoAvaliacaoGestor">
                                <asp:DropDownList ID="ddlNotaPadraoAvaliacaoAsGestor" class="form-control" runat="server" Style="width: 100%" ></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="form-actions">
                        <div class="text-right">
                            <asp:Button OnClick="btnCadastrar_Click" ID="Button1" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%--EXPORT E IMPORT--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Exportar e Importar Performances</h4>
                <h6 class="card-subtitle">Utilize o modelo de arquivo exportado para alterar ou adicionar competências.</h6>
                <h6 class="card-subtitle">Mantenha uma linha SEM um valor na coluna IdPerformance para adicionar uma nova performance.</h6>
                <h6 class="card-subtitle">O cargo deve ser identificado pelo ID.</h6>
                <h6 class="card-subtitle">Para fazer com que uma etapa possua uma nota padrão sem preenchimento do usuário, indique a coluna INPUT da etapa com o valor 0 e identifique o IdNotaPadrao na próxima coluna.</h6>
                <h6 class="card-subtitle">NÃO altere a ordem e quantidade das colunas, a quantidade de abas do arquivo ou coloque linhas acima da linha de títulos.</h6>
                <asp:Button runat="server" type="button" ID="btnExport" class="btn btn-info" OnClick="btnExport_Click" Text="Exportar"></asp:Button>
                <asp:Button runat="server" type="button" ID="btnImport" class="btn btn-info" OnClick="btnImport_Click" Text="Importar"></asp:Button>
                <asp:FileUpload ID="fileUpload" runat="server" AllowMultiple="false" accept=".xlsx" class="btn btn-danger" /> 
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Lista de Performance</h4>
                <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code>performance código</code>. </h6>
                <div class="table-responsive">

                    <asp:HiddenField ID="hdId" runat="server" />

                        
                    <asp:Repeater ID="rptPerformance" OnItemCommand="rpt_ItemCommand" OnItemDataBound="rpt_ItemDataBound" runat="server">

                        <HeaderTemplate>
                            <table class="table table-striped border dtInit">
                                <thead>
                                    <tr>
                                        <th>Código</th>
                                        <th>Performance</th>
                                        <th>Cargo</th>
                                        <th>Nível</th>
                                        <th>Status(Ativo/Inativo)</th>
                                        <th data-sort="0">Ação</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td><asp:Label ID="lblId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "idPerformance") %>' Visible="false"></asp:Label><%# DataBinder.Eval(Container.DataItem, "IdPerformance") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Performance") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "CARGOS.Cargo") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "CARGOSNIVEIS.Nivel") %></td>
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
                                        <th>Performance</th>
                                        <th>Cargo</th>
                                        <th>Nível</th>
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
    </div>
    <!-- ============================================================== -->
    <!-- End Container fluid  -->
    <!-- ============================================================== -->


    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <!-- <script src="assets/libs/popper.js/dist/umd/popper.min.js"></script>
    <script src="assets/libs/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- apps -->
    <script src="dist/js/app.min.js"></script>
    <script src="dist/js/app.init.js"></script>
    <script src="dist/js/app-style-switcher.js"></script>
    <script src="assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js"></script>
    <script src="assets/extra-libs/sparkline/sparkline.js"></script>
    <script src="dist/js/waves.js"></script>
    <!--<script src="dist/js/sidebarmenu.js"></script>
    <!--Custom JavaScript -->
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>

    <script src="assets/libs/ckeditor/ckeditor.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>

    <script src="dist/js/custom.min.js"></script>
    <script>
        CKEDITOR.replace('editor1', {
            height: 150
        });
    </script>
    <script type="text/javascript">
        function InputAutoAvaliacao(target) {
            var checkbox = document.getElementById("chboxInputAutoAvaliacao");
            var label = document.getElementById('<%= labelNotaPadraoAutoAvaliacao.ClientID %>');
            var ddl = document.getElementById("divDDLNotaPadraoAutoAvaliacao");

            if (target == 1) {
                checkbox = document.getElementById("chboxInputAvaliacaoAsCegas");
                label = document.getElementById('<%= labelNotaPadraoAvaliacaoAsCegas.ClientID %>');
                ddl = document.getElementById("divDDLNotaPadraoAvaliacaoAsCegas");
            }
            else if (target == 2) {
                checkbox = document.getElementById("chboxInputAvaliacaoGestor");
                label = document.getElementById('<%= labelNotaPadraoAvaliacaoGestor.ClientID %>');
                ddl = document.getElementById("divDDLNotaPadraoAvaliacaoGestor");
            }

            if (checkbox.checked) {
                label.style.display = "none";
                ddl.style.display = "none";
            }
            else {
                label.style.display = "inline";
                ddl.style.display = "block";
            }
        }

        function ValorCheckbox(valor1, valor2, valor3) {
            var checkbox = document.getElementById("chboxInputAutoAvaliacao");
            var label = document.getElementById('<%= labelNotaPadraoAutoAvaliacao.ClientID %>');
            var ddl = document.getElementById("divDDLNotaPadraoAutoAvaliacao");
            if (valor1) {
                checkbox.checked = true;
                label.style.display = "none";
                ddl.style.display = "none";
            }
            else {
                checkbox.checked = false;
                label.style.display = "inline";
                ddl.style.display = "block";
            }

            checkbox = document.getElementById("chboxInputAvaliacaoAsCegas");
            label = document.getElementById('<%= labelNotaPadraoAvaliacaoAsCegas.ClientID %>');
            ddl = document.getElementById("divDDLNotaPadraoAvaliacaoAsCegas");
            if (valor2) {
                checkbox.checked = true;
                label.style.display = "none";
                ddl.style.display = "none";
            }
            else {
                checkbox.checked = false;
                label.style.display = "inline";
                ddl.style.display = "block";
            }

            checkbox = document.getElementById("chboxInputAvaliacaoGestor");
            label = document.getElementById('<%= labelNotaPadraoAvaliacaoGestor.ClientID %>');
            ddl = document.getElementById("divDDLNotaPadraoAvaliacaoGestor");
            if (valor3) {
                checkbox.checked = true;
                label.style.display = "none";
                ddl.style.display = "none";
            }
            else {
                checkbox.checked = false;
                label.style.display = "inline";
                ddl.style.display = "block";
            }

        }

        function TesteAlerta(texto, texto2){
            alert(texto + ' ' + texto2);
        }
    </script>

</asp:Content>

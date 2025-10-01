<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Associados.aspx.cs" Inherits="SistemaAvaliacao.Associados" ValidateRequest="false" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Associados</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastros Básicos de Usuário</a></li>
                        <li class="breadcrumb-item active">Associados</li>
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

        <%--CARD DADOS DO ASSOCIADO--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Dados do Associado</h4>
                <div class="form-body">
                    <div class="row">
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Nome</label>
                                <asp:HiddenField ID="hddIdAssociado" runat="server" />
                                <asp:TextBox ID="txtNome" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Mentor</label>
                                <asp:DropDownList ID="ddlMentor" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>E-mail</label>
                                <asp:TextBox ID="txtEmail" class="form-control" runat="server" TextMode="Email"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Vertical</label>
                                <%--<asp:TextBox ID="txtVertical" class="form-control" runat="server"></asp:TextBox>--%>
                                <asp:DropDownList ID="ddlVerticalAssociado" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Senha</label>
                                <asp:TextBox ID="txtSenha" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Data de Admissão</label>
                                <asp:TextBox ID="txtDataAdmissao" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Cargo</label>
                                <asp:DropDownList ID="ddlCargoAssociado" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group" style="border:none">
                                <label>Promoção</label>
                                <br />
                                <input type="checkbox" ID="checkboxPromocao" runat="server" style="transform: scale(2.1); margin-left:7px; margin-top:10px; border:none"/>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Perfil de acesso <a href="javascript:void(0)" data-toggle="modal" data-target="#perfil-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                                <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Status</label>
                                <asp:DropDownList ID="ddlStatus" CssClass="form-control p-0 select2" Style="width: 100%" runat="server">

                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="0">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    
                    <div class="row">
                        <div class="col-md-1">
                            <div class="form-group">
                                <label>Foto</label>
                                <img src="" runat="server" id="fotoCadastro" width="150" height="150"/>
                            </div>
                        </div>
                        <div class="col-md--0" >
                            <div class="form-group" style="margin-top:20%;margin-left:30%">
                                <asp:Button OnClick="btnFoto_Click" ID="ButtonFoto" class="btn btn-info" runat="server" Text="Upload" />
                                <asp:FileUpload id="FileUpLoad1" runat="server" accept=".png,.jpg,.jpeg,.gif" />
                            </div>
                        </div>
                    </div>

                    <div class="form-actions flex flex-row justify-content-between">                        
                        <div class="text-right">
                            <asp:Button OnClientClick="JavaScriptFunction()" OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Cadastrar/Salvar"  />
                        </div>
                    </div>
                    
                </div>
            </div>
        </div>
        
        <%--CARD HISTÓRICO DE PROMOÇÕES--%>
        <div class="card">
            <div class="card-body">
                <asp:HiddenField ID="hiddenHtmlCode" runat="server" />
                <div class="form-body" id="divHistoricoPromocoes">
                <h4 class="card-title">Histórico de promoções do associado</h4>
                <asp:Repeater ID="rptHistoricoPromocoes" runat="server" OnItemCommand="rptHistoricoPromocoes_ItemCommand" OnItemDataBound="rptHistoricoPromocoes_ItemDataBound" >
                    <HeaderTemplate>
                        <table id="tabelaHistoricoPromocoes" class="table table-striped border dtInit">
                            <thead>
                                <tr>
                                    <th>Id</th>
                                    <th>Data</th>
                                    <th>Cargo Antigo</th>
                                    <th>Cargo Novo</th>
                                    <th>Comentários</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>

                    <ItemTemplate>
                        <tr>
                            <td><%# DataBinder.Eval(Container.DataItem, "idPromocao") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "DataPromocao") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "CARGOS.Cargo") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "CARGOS1.Cargo") %></td>
                            <td contenteditable style="background-color:white;font-style:italic"><%# DataBinder.Eval(Container.DataItem, "Comentarios") %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                </div>
            </div>
        </div>

        <%--EXPORT E IMPORT--%>
        <div class="card">
            <div class="card-body">
                <div class="row">
                    <h4 class="card-title">Exportar e Importar Associados e Promoções</h4>
                </div>
                <div class="row">
                    <h6 class="card-subtitle">Utilize os modelos de arquivos exportados para alterar ou adicionar projetos e associações.
                    <br />Linhas SEM um valor nas colunas IdAssociado e/ou IdPromocao serão consideradas inserções.
                    <br />NÃO altere a ordem e quantidade das colunas, a quantidade de abas dos arquivos ou coloque linhas acima da linha de títulos.</h6>
                </div>
                <br />
                <div class="row"> 
                    <asp:Button runat="server" type="button" ID="btnExportAssociados" class="btn btn-info" OnClick="btnExportAssociados_Click" Text="Exportar Associados"></asp:Button>
                    <asp:Button runat="server" type="button" ID="btnImportAssociados" class="btn btn-info" OnClick="btnImportAssociados_Click" Text="Importar Associados"></asp:Button>
                    <asp:FileUpload ID="fileUploadAssociados" runat="server" AllowMultiple="false" accept=".xlsx" class="btn btn-danger" /> 
                </div>
                <br />
                <div class="row">
                    <asp:Button runat="server" type="button" ID="btnExportPromocoes" class="btn btn-info" OnClick="btnExportPromocoes_Click" Text="Exportar Promoções"></asp:Button>
                    <asp:Button runat="server" type="button" ID="btnImportPromocoes" class="btn btn-info" OnClick="btnImportPromocoes_Click" Text="Importar Promoções"></asp:Button>
                    <asp:FileUpload ID="fileUploadPromocoes" runat="server" AllowMultiple="false" accept=".xlsx" class="btn btn-danger" /> 
                </div>
            </div>
        </div>

        <%--CARD LISTA DE ASSOCIADOS --%>
        <div class="card">
            <div class="card-body">
                <div class="form-body">
                    <h4 class="card-title">Lista de Associados</h4>
                    <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code>nome status cargo</code>. </h6>
                    <div class="table-responsive" id="divListaAssociados">
                        <asp:Repeater ID="rptAssociados" OnItemCommand="rptAssociados_ItemCommand" OnItemDataBound="rptAssociados_ItemDataBound" runat="server">
                            <HeaderTemplate>
                                <table class="table table-striped border dtInit">
                                    <thead>
                                        <tr>
                                            <th>Id</th>
                                            <th>Nome</th>
                                            <th>Cargo</th>
                                            <th>Mentor</th>
                                            <th>Status(Ativo/Inativo)</th>
                                            <th data-sort="0">Ação</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>

                            <ItemTemplate>
                                <tr>
                                    <td><asp:Label ID="lblIAssociado" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "idAssociado") %>'></asp:Label></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Nome") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "CARGOS.Cargo") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "ASSOCIADOS2.Nome") %></td>
                                    <td><asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                    <td>
                                        <asp:Button ID="btnAlterar" class="btn btn-info" runat="server" Text="Alterar" CommandName="AlteraAssociado" />
                                        <asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Inativar" CommandName="ExcluiAssociado" />
                                    </td>
                                </tr>
                            </ItemTemplate>

                            <FooterTemplate>
                                </tbody>
									<tfoot>
                                        <tr>
                                            <th>id</th>
                                            <th>Nome</th>
                                            <th>Cargo</th>
                                            <th>Mentor</th>
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

        <div class="card">
            <div class="card-body">
                <div id="cargo-modal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="display: none;">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h4 class="modal-title">Cadastro de Cargo</h4>
                                <button type="button" class="close" data-dismiss="modal" aria-hidden="true" title="Fechar">×</button>
                            </div>
                            <div class="modal-body">
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label>Código</label>
                                                <input type="text" class="form-control">
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label>Cargo</label>
                                                <input type="text" class="form-control">
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-default waves-effect" data-dismiss="modal">Fechar</button>
                                <button type="button" class="btn btn-danger waves-effect waves-light">Cadastrar</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="perfil-modal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="display: none;">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h4 class="modal-title">Cadastro de Perfil</h4>
                                <button type="button" class="close" data-dismiss="modal" aria-hidden="true" title="Fechar">×</button>
                            </div>
                            <div class="modal-body">
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label>Código</label>
                                                <input type="text" class="form-control">
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label>Perfil</label>
                                                <input type="text" class="form-control">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-default waves-effect" data-dismiss="modal">Fechar</button>
                                <button type="button" class="btn btn-danger waves-effect waves-light">Cadastrar</button>
                            </div>
                        </div>
                    </div>
                </div>

                <script src="assets/libs/jquery/dist/jquery.min.js"></script>
                <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
                <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>
                <script src="dist/js/custom.min.js"></script>
                
            </div>
        </div>

    </div>

    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />

    <script type="text/javascript">
        function JavaScriptFunction() {
            document.getElementById('<%= hiddenHtmlCode.ClientID %>').value = document.getElementById('divHistoricoPromocoes').innerHTML;
        }
    </script>
</asp:Content>
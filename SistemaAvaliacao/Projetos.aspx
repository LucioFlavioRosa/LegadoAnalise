<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Projetos.aspx.cs" Inherits="SistemaAvaliacao.Projetos" EnableEventValidation="false" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />

    <!-- ============================================================== -->
    <!-- Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Projetos</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active">Cadastros de Projetos</li>
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
                <h4 class="card-title">Dados do Projeto</h4>
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Código</label>
                                <asp:TextBox ID="txtCodigo" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Projeto</label>
                                <asp:TextBox ID="txtProjeto" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Empresa / Cliente</label>
                                <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-control p-0 select2" DataValueField="0" DataTextField="Selecione" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Início</label>
                                <input type="date" id="txtDataInicio" runat="server" class="form-control">
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Término</label>
                                <input type="date" id="txtDataTermino" runat="server" class="form-control">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Responsável <a href="javascript:void(0)" data-toggle="modal" data-target="#responsavel-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                                <asp:DropDownList ID="ddlResponsavel" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Gestor <a href="javascript:void(0)" data-toggle="modal" data-target="#gestor-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                                <asp:DropDownList ID="ddlGestor" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Status</label>
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="0">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Tipo de Projeto<a href="javascript:void(0)" data-toggle="modal" data-target="#tipo-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a> </label>
                                <asp:DropDownList ID="ddlTipoProjeto" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Complexidade <a href="javascript:void(0)" data-toggle="modal" data-target="#complexiade-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                                <asp:DropDownList ID="ddlComplexidade" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col-md-12">
                            <h4 class="card-title">Associados no Projeto <a href="javascript:void(0)" data-toggle="modal" data-target="#profissionais-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></h4>
                        </div>
                        <div class="table-responsive">

                            <asp:Repeater ID="rptAssociados" OnItemCommand="rpt_ItemCommand" OnItemDataBound="rpt_ItemDataBound" runat="server">
                                
                                       <HeaderTemplate>
                                    <table class="table table-striped border dtInit">
                                        <thead>
                                            <tr>
                                                <th>Associado</th>
                                                <th>Cargo Projeto</th>
                                                <th>Avaliador</th>
                                                <th>Data Início</th>
                                                <th>Data Término</th>
                                                <th>Status(Ativo/Inativo)</th>
                                                <th data-sort="0">Ação</th>
                                               </tr>
                                        </thead>

                                        <tbody>
                                </HeaderTemplate>

                                <ItemTemplate>
                                    <tr>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Nome") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "CARGOS.Cargo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "CARGOS.Cargo") %></td>
                                         <td><%# DataBinder.Eval(Container.DataItem, "CARGOS.Cargo") %></td>
                                         <td><%# DataBinder.Eval(Container.DataItem, "CARGOS.Cargo") %></td>
                                        <td><asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                        <td>
                                            <asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Excluir" CommandName="ExcluirAlocacao" />
                                        </td>
                                    </tr>
                                </ItemTemplate>


                                <FooterTemplate>
                                    </tbody>
											            <tfoot>
                                                            <tr>
                                                                <th>Associado</th>
                                                                <th>Cargo Projeto</th>
                                                                <th>Avaliador</th>
                                                                <th>Status(Ativo/Inativo)</th>
                                                                <th>Data Início</th>
                                                                <th>Data Término</th>
                                                                <th>Ação</th>
                                                            </tr>
                                                        </tfoot>
                                    </table>

                                </FooterTemplate>
                            </asp:Repeater>



                        </div>
                    </div>
                </div>
                <div class="form-actions">
                    <div class="text-right">
                        <p></p>
                        <p></p>
                        <asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />
                    </div>
                </div>
            </div>
        </div>

        <div class="card">

            <div class="card-body">
                <h4 class="card-title">Lista de projetos</h4>
                <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code>projtos status datas</code>. </h6>
                <div class="table-responsive">

                    <asp:Repeater ID="rptProjetos" runat="server">
                        <HeaderTemplate>
                            <table class="table table-striped border dtInit">
                                <thead>
                                    <tr>
                                        <th>Projeto</th>
                                        <th>Data Inicio</th>
                                        <th>Data Término</th>
                                        <th data-sort="0">Ação</th>
                                    </tr>
                                </thead>

                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td><%# DataBinder.Eval(Container.DataItem, "Projeto") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DataInicio") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DataFim") %></td>
                                <td>
                                    <button type="button" class="btn btn-dark btn-outline btn-circle btn-lg m-r-5"><i class="ti-close" title="Remover do projeto"></i></button>
                                </td>
                            </tr>
                        </ItemTemplate>


                        <FooterTemplate>
                            </tbody>
											            <tfoot>
                                                            <tr>
                                                                <th>Projeto</th>
                                                                <th>Data Inicio</th>
                                                                <th>Data Término</th>
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

    <div id="responsavel-modal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Cadastro de Responsável</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" title="Fechar">×</button>
                </div>
                <div class="modal-body">

                    <div class="form-body">
                        <div class="row">
                            <div class="col-md-12">
                                Tela a ser definida pelos desenvolvedores, aqui é apenas um exemplo de como abrir o modal
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

    <div id="gestor-modal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Cadastro de Gestor</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" title="Fechar">×</button>
                </div>
                <div class="modal-body">
                    <div class="form-body">
                        <div class="row">
                            <div class="col-md-12">
                                Tela a ser definida pelos desenvolvedores, aqui é apenas um exemplo de como abrir o modal
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


    <div id="profissionais-modal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Cadastro de Associados</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" title="Fechar">×</button>
                </div>
                <div class="modal-body">
                    <div class="form-body">
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <label>Associado</label>
                                    <asp:DropDownList ID="ddlassociadoProjeto" runat="server" CssClass="form-control p-0 select2" DataValueField="0" DataTextField="Selecione" Style="width: 100%">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Início</label>
                                    <asp:TextBox ID="txtInicioAlocacao" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Término</label>
                                    <asp:TextBox ID="txtTérminoAlocacao" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default waves-effect" data-dismiss="modal">Fechar</button>
                    <asp:Button OnClick="btnCadastrarAlocacao_Click" ID="Button1" class="btn btn-default waves-effect" runat="server" Text="Cadastrar/Salvar" />                                    
									      
                </div>
            </div>
        </div>
    </div>

    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <!--<script src="assets/libs/popper.js/dist/umd/popper.min.js"></script>
    <script src="assets/libs/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- apps-->
    <script src="dist/js/app.min.js"></script>
    <script src="dist/js/app.init.js"></script>
    <script src="dist/js/app-style-switcher.js"></script>
    <script src="assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js"></script>
    <script src="assets/extra-libs/sparkline/sparkline.js"></script>
    <script src="dist/js/waves.js"></script>
    <!--<script src="dist/js/sidebarmenu.js"></script>-->
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


</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="SistemaAvaliacao.Clientes" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Clientes</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastro Básico de Projetos / </a></li>
                        <li class="breadcrumb-item active" aria-current="page">Clientes</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>


    <div class="page-content container-fluid">

        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Dados do cliente</h4>
                <form action="#">
                    <div class="form-body">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Cliente</label>
                                    <asp:TextBox ID="txtCliente" class="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Sócio Responsável <a href="javascript:void(0)" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                                    <asp:DropDownList ID="ddlSocio" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label>Status</label>
                                    <asp:DropDownList ID="ddlStatus" class="form-control" runat="server">
                                        <asp:ListItem Value="1">Ativo</asp:ListItem>
                                        <asp:ListItem Value="0">Inativo</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Gestor do Cliente</label>
                                    <asp:TextBox ID="txtGestorCliente" class="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>E-mail do Cliente</label>
                                    <asp:TextBox ID="txtEmail" class="form-control" runat="server"  TextMode="Email"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Telefone do Cliente</label>
                                    <asp:TextBox ID="txtTelefone" class="form-control" runat="server"  TextMode="Phone"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="form-actions">
                        <div class="text-right">
                            <asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />
                        </div>
                    </div>
                </form>
            </div>
        </div>

        <div class="card">

            <div class="card-body">
                <h4 class="card-title">Lista de clientes</h4>
                <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code>cliente código</code>. </h6>
                <div class="table-responsive">
                    
                    <asp:HiddenField ID="hdIdCliente" runat="server" />

                    <asp:Repeater ID="rptClientes" OnItemCommand="rptClientes_ItemCommand" OnItemDataBound="rptClientes_ItemDataBound" runat="server">

                        <HeaderTemplate>
                            <table class="table table-striped border dtInit">
                                <thead>
                                    <tr>
                                        <th>Código</th>
                                        <th>Cliente</th>
                                        <th>Email</th>
                                        <th>Status(Ativo/Inativo)</th>
                                        <th data-sort="0">Ação</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td><asp:Label ID="lblIdCliente" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IdCliente") %>' Visible="false"></asp:Label>
                                            <%# DataBinder.Eval(Container.DataItem, "IdCliente") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "CLiente") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Email") %></td>
                                <td><asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                <td>
                                    <asp:Button ID="btnAlterar" class="btn btn-info" runat="server" Text="Alterar" CommandName="AlterarCliente" />
                                    <asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Inativar" CommandName="ExcluirCliente" />

                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
							<tfoot>
                                <tr>
                                    <th>Código</th>
                                    <th>Cliente</th>
                                    <th>Email</th>
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
    <script src="dist/js/custom.min.js"></script>



</asp:Content>

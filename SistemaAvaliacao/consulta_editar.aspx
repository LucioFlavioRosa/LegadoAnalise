<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="consulta_editar.aspx.cs" Inherits="SistemaAvaliacao.consulta_editar" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Editar Avaliações</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Editar Avaliações</li>
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

        <%--CARD PESQUISA--%>
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Filtro avaliações</h4>
                        <div class="form-body">
                            <div class="row">
                                <div class="col-md-0" hidden="hidden">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>IdAvaliacao</label>
                                            <asp:TextBox ID="ddlAvaliacaoId" Style="width: 100%" runat="server">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Projeto</label>
                                            <select id="ddlProjetos" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Cliente</label>
                                            <select id="ddlClientes" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Associado</label>
                                            <select id="ddlProfissionais" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Período</label>
                                            <select id="ddlPeriodos" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2 text-right">
                                    <asp:Button runat="server" type="button" ID="Button1" Style="margin-top: 30px"
                                        class="btn btn-danger" OnClick="btnSearch_Click" Text="Listar Avaliações"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%--CARD CAMPOS--%>
        <div class="row" runat="server" id="cardCampos">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Detalhes</h4>
                        <div class="form-body">
                            <div class="row">
                                <div class="col-md-0" hidden="hidden">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>IdAvaliacao</label>
                                            <asp:TextBox ID="txt_detalhes_IdAvaliacao" Style="width: 100%" runat="server">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Projeto</label>
                                            <asp:Label id="txt_detalhes_Projeto" runat="server" class="form-control p-0 select2" style="width: 100%"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Cliente</label>
                                            <select id="Select2" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Associado</label>
                                            <select id="Select3" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Período</label>
                                            <select id="Select4" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2 text-right">
                                    <asp:Button runat="server" type="button" ID="Button2" Style="margin-top: 30px"
                                        class="btn btn-danger" OnClick="btnSearch_Click" Text="Listar Avaliações"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <div class="table-responsive">
                            <asp:Repeater ID="rptProjetos" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-striped border gridResultado">
                                        <thead>
                                            <tr>
                                                <th>Projeto</th>
                                                <th>Cliente</th>
                                                <th>Associado</th>
                                                <th>Cargo</th>
                                                <th>Gestor</th>
                                                <th>Período</th>
                                                <th>Início</th>
                                                <th>Término</th>
                                                <th>Fase</th>
                                                <th>Ação</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>

                                <ItemTemplate>
                                    <tr>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Projeto.Projeto") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Cliente.Cliente") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.Nome") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.CARGOS.Cargo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Gestor.Nome") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Periodo.Periodo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "DataInicio") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "DataTermino") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Fase") %></td>
                                        <td>
                                            <asp:Button type="button" runat="server" ID="btnRetroceder" class="btn btn-dark" Text="Retroceder" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' OnClick="btnRetroceder_Click" />
                                        </td>
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
            </div>
        </div>
        <!-- ============================================================== -->
        <!-- End Page Content -->
        <!-- ============================================================== -->
        <!-- ============================================================== -->
        <!-- Right sidebar -->
        <!-- ============================================================== -->
        <!-- .right-sidebar -->
        <!-- ============================================================== -->
        <!-- End Right sidebar -->
        <!-- ============================================================== -->
    </div>

    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/extra-libs/datatables.net/Buttons-1.7.0/js/dataTables.buttons.min.js"></script>
    <script src="assets/extra-libs/datatables.net/JSZip-2.5.0/jszip.min.js"></script>
    <script src="assets/extra-libs/datatables.net/Buttons-1.7.0/js/buttons.html5.min.js"></script>
    <script src="assets/extra-libs/datatables.net/Buttons-1.7.0/js/buttons.print.min.js"></script>

    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>
    <script src="dist/js/custom.min.js"></script>

    <script src="dist/js/pages/consulta/datatable-consulta.js"></script>

</asp:Content>

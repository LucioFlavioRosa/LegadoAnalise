<%@ Page Language="C#" MasterPageFile="~/Site.Master"  AutoEventWireup="true" CodeBehind="DisparoMassivoRH.aspx.cs" Inherits="SistemaAvaliacao.DisparoMassivoRH" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- ============================================================== -->
    <!-- Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Comitê</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Disparo Massivo RH</li>
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
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <h4 class="card-title">Disparo de e-mail de dados do RH importados</h4>
                        </div>
                        <div class="row">
                            <h6 class="card-subtitle">As linhas abaixo foram identificadas como novos disparos a serem enviados por e-mail aos mentores de cada associado</h6>
                        </div>
                        <div class="row">
                            <asp:Button runat="server" type="button" ID="btnDispararTodos" class="btn btn-danger" OnClick="btnDispararTodos_Click" Text="Disparar todos"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Novos disparos</h4>
                <div class="table-responsive">
                    <table class="table table-striped border gridResultado">
                        <thead>
                            <tr>
                                <th>IdLinha</th>
                                <th>Período</th>
                                <th>Associado</th>
                                <th>Mentor</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptDisparos" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# DataBinder.Eval(Container.DataItem, "idConsideracoesMentor") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Periodo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Mentor") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
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
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>
    <script src="dist/js/custom.min.js"></script>
    <script src="dist/js/pages/resultado/datatable-resultado.js"></script>
</asp:Content>
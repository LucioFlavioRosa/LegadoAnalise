<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="pendencias.aspx.cs" Inherits="SistemaAvaliacao.pendencias" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Pendências</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Pendências</li>
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
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Minhas pendências</h4>
                        
                        <div class="table-responsive">
                        <!-- REPEATER DE PESSOAS -->
                        <asp:Repeater runat="server" id="rptProjetos">
                            <HeaderTemplate>
                                <table class="table table-striped border dtInit">
                                    <thead>
                                        <tr>
                                            <th>Foto</th>
                                            <th>Associado</th>
                                            <th>Projeto</th>
                                            <th>Pendência</th>
                                            <th>Período</th>
                                            <th>Data Limite</th>
                                            <th data-sort="0"></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>

                            <ItemTemplate>
                                <tr>
                                    <td><img src=<%# DataBinder.Eval(Container.DataItem, "FotoNome") %> width="120" height="120"></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Nome") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Projeto") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Pendencia") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "Periodo") %></td>
                                    <td><%# DataBinder.Eval(Container.DataItem, "DataLimite") %></td>
                                    <td style="text-align:right;"></td>
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
    </div>

    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    
    <!-- apps -->
    <script src="dist/js/app.min.js"></script>
    <script src="dist/js/app-style-switcher.js"></script>
    <script src="assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js"></script>
    <script src="assets/extra-libs/sparkline/sparkline.js"></script>
    <script src="dist/js/waves.js"></script>
    <!--Custom JavaScript -->
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/libs/ckeditor/ckeditor.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>

    <script src="dist/js/custom.min.js"></script>
</asp:Content>

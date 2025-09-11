<%@ Page Title="Radar das Avaliações" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="radar.aspx.cs" Inherits="SistemaAvaliacao.radar" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <!-- ============================================================== -->
    <!-- Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Radar das Avaliações</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Radar das Avaliações</li>
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
                        <h4 class="card-title">Filtros</h4>
                        <div class="form-body">
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Período</label>
                                            <asp:DropDownList ID="ddlPeriodos" runat="server" class="form-control p-0 select2" Style="width: 100%" OnSelectedIndexChanged="ddlPeriodos_SelectedIndexChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Projeto</label>
                                            <asp:DropDownList ID="ddlProjetos" runat="server" class="form-control p-0 select2" Style="width: 100%" OnSelectedIndexChanged="ddlProjetos_SelectedIndexChanged" AutoPostBack="true" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Associado</label>
                                            <asp:DropDownList ID="ddlAssociados" runat="server" class="form-control p-0 select2" Style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Tipo de Avaliação</label>
                                            <asp:DropDownList ID="ddlTipo" runat="server" class="form-control p-0 select2" Style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Escopo</label>
                                            <asp:DropDownList ID="ddlEscopo" runat="server" class="form-control p-0 select2" Style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-1">
                                    <div class="form-actions">
                                        <label>&nbsp;</label>
                                        <div>
                                            <asp:Button OnClick="btnMostraGrafico_Click" ID="Button1" class="btn btn-info" runat="server" Text="Exibir Radar" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkRotulo" runat="server" Checked="true" ToolTip="Exibir Valores nos Pontos do Eixo" Text="Rótulos" />
                                    </div>
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkBackground" runat="server" Checked="false" ToolTip="Preencher o Fundo do Radar com as Cores do Eixo" Text="Fundo Colorido" />
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card" style="width: 100%; text-align: center;">
            <div class="card-body">
                <asp:Chart ID="myRadarChart" runat="server" Width="1200px" Height="600px" IsSoftShadows="true" AntiAliasing="All">
                    <ChartAreas>
                        <asp:ChartArea Name="MainChartArea">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
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
    <script src="assets/libs/popper.js/dist/umd/popper.min.js"></script>
    <script src="assets/libs/bootstrap/dist/js/bootstrap.min.js"></script>
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
    <script src="./Scripts/includes.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            var tree = $('#tree').tree({
                primaryKey: 'id',
                uiLibrary: 'bootstrap4',
                dataSource: './dadosexemplo.json',
                checkboxes: true
            });
            $('#btnEnviar').on('click', function () {
                var checkedIds = tree.getCheckedNodes();
                $.ajax({ url: '/Avaliacoes/Disparar', data: { checkedIds: checkedIds }, method: 'POST' })
                    .fail(function () {
                        alert('Falha ao disparar.');
                    });
            });
        });
    </script>

</asp:Content>

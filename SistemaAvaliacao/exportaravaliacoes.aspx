<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="exportaravaliacoes.aspx.cs" Inherits="SistemaAvaliacao.exportaravaliacoes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- ============================================================== -->
    <!-- Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Exportar Avaliações</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Exportar Avaliações</li>
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
                        <h4 class="card-title">Filtro avaliações</h4>
                        <div class="form-body">
                            <div class="row">
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Período</label>
                                            <select id="ddlPeriodos" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Projeto</label>
                                            <select id="ddlProjetos" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Associado</label>
                                            <select id="ddlAssociados" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">&nbsp;</div>
                                <div class="col-md-2 text-right">
                                    <asp:Button runat="server" type="button" ID="btnSearch" Style="margin-top: 30px"
                                        class="btn btn-danger" OnClick="btnSearch_Click" Text="Listar Avaliações"></asp:Button>
                                </div>
                                <div class="col-md-2 text-right">
                                    <asp:Button runat="server" type="button" ID="btnExportar" Style="margin-top: 30px;"
                                        class="btn btn-info" OnClick="btnExportar_Click" Text="Exportar" visible="false" ></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Avaliações Finalizadas</h4>
                <h6 class="card-subtitle">Esta é apenas uma pré-visualização, os dados completos são apresentados no arquivo de exportação.</h6>
                <div class="table-responsive">
                    <table class="table table-striped border dtInit">
                        <thead>
                            <tr>
                                <th>Período</th>
                                <th>Projeto</th>
                                <th>Avaliado</th>
                                <th>Cargo</th>
                                <th>Gestor</th>
                                <th>Tipo</th>
                                <th>Competência/Performance Avaliada</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptProjetos" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Período") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Projeto") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Nome_Avaliado") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Cargo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Nome_Gestor") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Tipo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Nível") %></td>
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

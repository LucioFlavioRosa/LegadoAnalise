<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="workflowdeprojetos.aspx.cs" Inherits="SistemaAvaliacao.workflowdeprojetos" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <!-- ============================================================== -->
    <!-- Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Workflow de Projetos</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Workflow de Projetos</li>
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
                        <h4 class="card-title">Prazo das Avaliações (em dias)</h4>
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
                                            <label>Data de Início</label>
                                            <input runat="server" id="txtDataInicio" class="form-control" type="date">
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Auto-Avaliação</label>
                                            <input runat="server" id="txtAuto" class="form-control text-center" type="number">
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Avaliação às Cegas</label>
                                            <input runat="server" id="txtCegas" class="form-control text-center" type="number">
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Avaliação do Gestor</label>
                                            <input runat="server" id="txtGestor" class="form-control text-center" type="number">
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Feedback</label>
                                            <input runat="server" id="txtFeedback" class="form-control text-center" type="number">
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Alerta Sem Alteração</label>
                                            <input runat="server" id="txtAlerta" class="form-control text-center" type="number">
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-actions">
                                        <div class="text-right bottom-text">
                                            <asp:Button OnClick="btnCadastrar_Click" ID="Button1" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <h4 style="float: left;" class="card-title">Configurações do Workflow</h4>
                <br />
                <br />
                <div class="table-responsive">
                    <asp:Repeater ID="rptWorkflow" runat="server">
                        <HeaderTemplate>
                            <table class="table table-striped border dtInit">
                                <thead>
                                    <tr>
                                        <th>Período</th>
                                        <th>Data Início</th>
                                        <th>Auto-Avaliação</th>
                                        <th>Avaliação Às Cegas</th>
                                        <th>Avaliação do Gestor</th>
                                        <th>Avaliação Feedback</th>
                                        <th data-sort="0">Ação</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# DataBinder.Eval(Container.DataItem, "PERIODOSAVALIACOES.Periodo") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DataInicio", "{0:dd/MM/yyyy}") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DiasAutoAvaliacao") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DiasAvaliacaoCegas") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DiasAvaliacaoGestor") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DiasFeedback") %></td>
                                <td>
                                    <asp:LinkButton OnClick="btnAlterar_Click" ID="btnAlterar" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdWorkflow") %>' class="btn btn-info btn-outline btn-circle btn-lg m-r-5" runat="server" ToolTip="Alterar"><i class="ti-pencil-alt"></i></asp:LinkButton>
                                    <asp:LinkButton OnClick="btnDeletar_Click" ID="btnDeletar" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdWorkflow") %>' class="btn btn-dark btn-outline btn-circle btn-lg m-r-5" runat="server" ToolTip="Deletar"><i class="ti-trash"></i></asp:LinkButton>
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

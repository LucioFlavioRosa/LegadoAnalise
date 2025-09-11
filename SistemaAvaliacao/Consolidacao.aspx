<%@ Page Language="C#" MasterPageFile="~/Site.Master"  AutoEventWireup="true" CodeBehind="Consolidacao.aspx.cs" Inherits="SistemaAvaliacao.Consolidacao" %>
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
                        <li class="breadcrumb-item active" aria-current="page">Resultado Avaliações</li>
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
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Avaliações Finalizadas</h4>
                <div class="table-responsive">
                    <table class="table table-striped border gridResultado">
                        <thead>
                            <tr>
                                <th>Período</th>
                                <th>Avaliado</th>
                                <th>Cargo</th>
                                <th>Projeto</th>                                
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptProjetos" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Periodo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Nome") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Cargo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Projeto") %></td>
                                        
                                        <td class="text-center no-sort">
                                            <a href="avalizacao_consolidacao.aspx?&IdAssociado=<%# DataBinder.Eval(Container.DataItem, "IdAssociado") %>&IdPeriodo=<%# DataBinder.Eval(Container.DataItem, "IdPeriodo") %>&IdProjeto=<%# DataBinder.Eval(Container.DataItem, "IdProjeto") %>&TipoAvaliacao=<%# DataBinder.Eval(Container.DataItem, "TipoAvaliacao") %>&Escopo=<%# DataBinder.Eval(Container.DataItem, "Escopo") %>">
                                                <button type="button" class="btn btn-info">Consolidar</button>
                                            </a>                                            
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>

        </div>

        <%--EXPORT E IMPORT DE CONSIDERAÇÕES DO MENTOR E DADOS DO RH--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Exportar Considerações dos Mentores e Dados do RH</h4>
                <h6 class="card-subtitle">NÃO altere a ordem e quantidade das colunas, a quantidade de abas do arquivo ou coloque linhas acima da linha de títulos.</h6>
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-2">
                            <div class="form-group">
                                <div class="form-group">
                                    <label>Período</label>
                                    <select id="selectExportPeriodo" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                </div>
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <div class="form-group">
                                    <label>Associado</label>
                                    <select id="selectExportAssociado" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <asp:Button runat="server" type="button" ID="btnExport" class="btn btn-info" OnClick="btnExport_Click" Text="Exportar"></asp:Button>
                    <asp:Button runat="server" type="button" ID="btnImport" class="btn btn-info" OnClick="btnImport_Click" Text="Importar"></asp:Button>
                    <asp:FileUpload ID="fileUpload" runat="server" AllowMultiple="false" accept=".xlsx" class="btn btn-danger" /> 
                </div>
            </div>
        </div>

        <%--EXPORT E IMPORT DE NOTAS DE PERFORMANCES--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Exportar notas de performances</h4>
                <h6 class="card-subtitle">NÃO altere a ordem e quantidade das colunas, a quantidade de abas do arquivo ou coloque linhas acima da linha de títulos.</h6>
                <div class="form-body">
                    <asp:Button runat="server" type="button" ID="btn_notas_ExportarPerf" class="btn btn-info" OnClick="btn_notas_ExportarPerf_Click" Text="Exportar notas - performances"></asp:Button>
                    <asp:Button runat="server" type="button" ID="btn_notas_Importar" class="btn btn-info" OnClick="btn_notas_Importar_Click" Text="Importar"></asp:Button>
                    <asp:FileUpload ID="fileUpload_notas" runat="server" AllowMultiple="false" accept=".xlsx" class="btn btn-danger" /> 
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
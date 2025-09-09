<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="envioevolucao.aspx.cs" Inherits="SistemaAvaliacao.envioevolucao" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Evolução</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Envio de Evolução</li>
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
                        <div class="form-body">
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label>Associado</label>
                                        <select id="ddlPeriodos" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label>Associado</label>
                                        <select id="ddlAssociados" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <label>Vertical</label>
                                    <select id="ddlEmpresa" runat="server" class="form-control p-0 select3" style="width: 100%" />
                                </div>
                                <div class="col-md-3">
                                    <asp:Button runat="server" type="button" ID="btnSearch" Style="margin-top: 30px"
                                        class="btn btn-danger" OnClick="btnSearch_Click" Text="Listar Avaliações"></asp:Button>
                                </div>
                        </div>
                    </div>
                </div>
                <asp:Panel ID="SecEnvio" runat="server" Visible="false">
                    <hr />
                        <div Style="padding: 1.57rem;">
                            <div class="row">
                                <div class="col-md-3">                                    
                                    <asp:Button runat="server" type="button" ID="btnGerarTodos"
                                        class="btn btn-info" OnClick="btnGerarTodos_Click" Text="Enviar TODAS Evoluções"></asp:Button>
                                </div>
                                <div class="col-md-12 text-left" Style="margin-top: 16px">
                                    <asp:CheckBox runat="server" ID="chkReenviar" Text="*Reenviar para as Evoluções já Enviadas ?" />
                                </div>
                            </div>
                        </div>
                </asp:Panel>
            </div>
        </div>
            <div class="col-12">
                <div class="card" style="width: 100%">
                    <div class="card-body">
                        <h4 class="card-title">Avaliações Finalizadas</h4>
                        <div class="table-responsive">
                            <table class="table table-striped border gridResultado">
                                <thead>
                                    <tr>
                                        <th>Período</th>
                                        <th>Avaliado</th>
                                        <th>Vertical</th>
                                        <th>Cargo</th>
                                        <th>Mentor</th>
                                        <th>Qtd. Projetos</th>
                                        <th>Enviado</th>
                                        <th>Tipo Avaliação</th>
                                        <th>Escopo</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptProjetos" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Periodo") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Nome") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Vertical") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Cargo") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Mentor") %></td>
                                                <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "QtdProjetos") %></td>
                                                <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Enviado") %></td>
                                                <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "TipoAvaliacao") %></td>
                                                <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Escopo") %></td>
                                                <td class="text-center no-sort">
                                                    <asp:LinkButton OnClick="btnGerarUnico_Click" ID="btnGerarUnico" CommandArgument='<%# string.Format("{0};{1};{2};{3};{4}", DataBinder.Eval(Container.DataItem, "IdAssociado"), DataBinder.Eval(Container.DataItem, "IdPeriodo"), DataBinder.Eval(Container.DataItem, "IdCargo"), DataBinder.Eval(Container.DataItem, "TipoAvaliacao"), DataBinder.Eval(Container.DataItem, "Escopo")) %>'  class="btn btn-facebook btn-outline btn-circle btn-sm m-r-5" runat="server" ToolTip="Gerar e Enviar Email"><i class="ti-email"></i></asp:LinkButton>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
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
    </div>
    <!-- ============================================================== -->
    <!-- End Container fluid  -->
    <!-- ============================================================== -->

    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>
    <script src="dist/js/custom.min.js"></script>
    <script src="dist/js/pages/evolucao/datatable-evolucao.js"></script>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EnvioAvaliacoes.aspx.cs" Inherits="SistemaAvaliacao.EnvioAvaliacoes" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Envio de Avaliações</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Envio de Avaliações</li>
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

        <%--FILTROS--%>
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Período da Avaliação</h4>
                        <div class="form-body">
                            <div class="row">
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label><b>* Período</b></label>
                                            <select id="ddlPeriodos" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Projeto</label>
                                            <select id="ddlProjetos" runat="server"  class="form-control p-0 select2" style="width: 100%"  />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Cliente</label>
                                            <select id="ddlClientes" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Status</label>
                                            <select id="ddlStatus" runat="server" class="form-control p-0 select2" style="width: 100%" />
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
                            </div>

                            <div class="row">
                                <div class="col-md-2">
                                    <label>Disparo (todos)</label>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-2">
                                    <asp:DropDownList runat="server" ID="ddlDisparos" class="form-control p-0 select2" style="width: 100%" ></asp:DropDownList>
                                </div>
                                <div class="col-md-2">
                                    <asp:Button runat="server" type="button" ID="btnSearch"
                                        class="btn btn-danger" OnClick="btnSearch_Click" Text="Listar Avaliações" style="width: 75%" ></asp:Button>
                                </div>
                                <div class="col-md-3">
                                    <asp:Button runat="server" type="button" ID="btnGerarTodos"
                                        class="btn btn-info" OnClick="btnGerarTodos_Click" Text="Gerar/Enviar TODAS as Avaliações" Visible="false"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%--LISTA DE ENVIOS--%>
        <div class="card">
            <div class="card-body">
                <h4 style="float: left;" class="card-title">Projetos para Envio de E-mail</h4>
                <br />
                <br />
                <div class="table-responsive">
                    <asp:Repeater ID="rptAvaliacoes" runat="server">
                        <HeaderTemplate>
                            <table class="table table-striped border dtInit">
                                <thead>
                                    <tr>
                                        <th>Código</th>
                                        <th>Projeto</th>
                                        <th>Cliente</th>
                                        <th>Status(Ativo/Inativo)</th>
                                        <th>Início</th>
                                        <th>Término</th>
                                        <th>Responsável</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td><%# DataBinder.Eval(Container.DataItem, "Id") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Nome") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Cliente.Cliente") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Status.Status") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DataInicio") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "DataTermino") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "Responsavel.Nome") %></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td colspan="6">

                                    <!-- REPEATER DE PESSOAS -->
                                    <asp:Repeater runat="server" ID="rptAssociados" DataSource='<%# Eval("Associados") %>' OnItemCommand="btnGerarUnico_CMD_Command" OnItemDataBound="rptAssociados_ItemDataBound">
                                        <HeaderTemplate>
                                            <table class="table table-striped border dtInit">
                                                <thead>
                                                    <tr>
                                                        <th>Profissional</th>
                                                        <th>Cargo</th>
                                                        <th>Período</th>
                                                        <th>Início</th>
                                                        <th>Término</th>
                                                        <th>Gestor/Liderado</th>
                                                        <th>Avaliador</th>
                                                        <th>Tipo de Avaliação</th>
                                                        <th>Escopo</th>
                                                        <th>Disparo</th>
                                                        <th data-sort="0"></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>

                                        <ItemTemplate>
                                            <tr>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Associado.Nome") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Associado.CARGOS.Cargo") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Periodo.Periodo") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "DataInicio") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "DataTermino") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Gestor.Nome") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Avaliador.Nome") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "TipoAvaliacao") %></td>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Escopo") %></td>
                                                <td><select id="cboxDisparo" runat="server" class="form-control p-0 select2" style="width: 150%" ></select></td>
                                                <td style="width: 170px; text-align: right;">
                                                    <asp:LinkButton Visible="false" OnClick="btnGerarUnico_Click" ID="btnGerarUnico" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdEmail") %>' class="btn btn-facebook btn-outline btn-circle btn-lg m-r-5" runat="server" ToolTip="Gerar e Enviar Email"><i class="ti-email"></i></asp:LinkButton>
                                                    <asp:Button ID="btnGerarUnico_CMD" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdEmail") %>' class="btn btn-facebook btn-outline btn-lg m-r-5" runat="server" ToolTip="Gerar e Enviar Email" Text="Enviar"></asp:Button>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </tbody>
                                                </table>
                                        </FooterTemplate>
                                    </asp:Repeater>
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

        <%--AVALIAÇÕES PENDENTES--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Avaliações pendentes</h4>
                <asp:Button runat="server" ID="btnRedispararTodos" Text="Alertar pendências a todos" OnClick="btnRedispararTodos_Click" class="btn btn-danger" />
                        
                <div class="table-responsive">
                <!-- REPEATER DE PESSOAS -->
                <asp:Repeater runat="server" id="rptProjetos">
                    <HeaderTemplate>
                        <table class="table table-striped border dtInit">
                            <thead>
                                <tr>
                                    <th>Avaliado</th>
                                    <th>Respondente</th>
                                    <th>Projeto</th>
                                    <th>Pendência</th>
                                    <th>Período</th>
                                    <th>Data Limite</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>

                    <ItemTemplate>
                        <tr>
                            <td><%# DataBinder.Eval(Container.DataItem, "Nome") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "Respondente") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "Projeto") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "Pendencia") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "Periodo") %></td>
                            <td><%# DataBinder.Eval(Container.DataItem, "DataLimite") %></td>
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
    <!-- ============================================================== -->
    <!-- End Container fluid  -->
    <!-- ============================================================== -->


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

<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avaliacao.aspx.cs" Inherits="SistemaAvaliacao.avaliacao" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Auto Avaliação</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Avaliação / Auto Avaliação</li>
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
                        <form action="#">
                            <div class="form-body" style="position:sticky;z-index:1;top:0">
                                <div class="row">
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <div class="form-group">
                                                <label>Projeto</label>
                                                <select id="ddlProjetos" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-3">
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
                                                <label>Período</label>
                                                <select id="ddlPeriodos" runat="server" class="form-control p-0 select2" style="width: 100%" />
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
                                        <asp:Button runat="server" type="button" id="btnSearch" style="margin-top: 30px"
                                            class="btn btn-danger" OnClick="btnSearch_Click" Text="Filtrar"></asp:Button>
                                    </div>
                                </div>
                            </div>
                        </form>

                        <div class="table-responsive">
                            <asp:Repeater ID="rptProjetos" runat="server" >
                                <HeaderTemplate>
                                    <table class="table table-striped border dtInit">
                                        <thead>
                                            <tr>
                                                <th>Código</th>
                                                <th>Projeto</th>
                                                <th>Cliente</th>
                                                <th>Status(Ativo/Inativo)</th>
                                                <th>Dt. Início Projeto</th>
                                                <th>Dt. Término Projeto</th>
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

                                            <!-- REPEATER - AV DE DESEMPENHO -->
                                            <asp:Repeater runat="server" DataSource='<%# Eval("Associados") %>' Visible='<%# Eval("IsVisible") %>' >
                                                <HeaderTemplate>
                                                    <table class="table table-striped border dtInit">
                                                        <thead>
                                                            <tr>
                                                                <th>Foto</th>
                                                                <th>Profissional</th>
                                                                <th>Cargo</th>
                                                                <th>Período</th>
                                                                <th>Gestor</th>
                                                                <th oninit="colAvaliador_Load" ID="colAvaliador" runat="server">Avaliador</th>
                                                                <th oninit="colMentor_Init" ID="colMentor" runat="server">Mentor</th>
                                                                <th>Etapa</th>
                                                                <th>Tipo de Avaliação</th>
                                                                <th data-sort="0"></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>

                                                <ItemTemplate>
                                                    <tr>
                                                        <td><img src=<%# DataBinder.Eval(Container.DataItem, "FotoAssociado.Imagem") %> width="120" height="120"></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.Nome") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.CARGOS.Cargo") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Periodo.Periodo") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Gestor.Nome") %></td>
                                                        <td oninit="rowAvaliador_Init" ID="rowAvaliador" runat="server"><%# DataBinder.Eval(Container.DataItem, "Avaliador.Nome") %></td>
                                                        <td oninit="rowMentor_Init" ID="rowMentor" runat="server"><%# DataBinder.Eval(Container.DataItem, "Mentor.Nome") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Etapa") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "TipoAvaliacaoShow") %></td>
                                                        <td style="text-align:right;">
                                                            <br />
                                                            <a href="autoavalizacao_competencia.aspx?IdProjeto=<%# Eval("Projeto.IdProjeto") %>&IdAssociado=<%# Eval("Associado.IdAssociado") %>&IdPeriodo=<%# Eval("Periodo.IdPeriodo") %>&TipoAvaliacao=<%# Eval("TipoAvaliacao") %>&Escopo=<%# Eval("Escopo") %>&idGestor=<%# Eval("Gestor.IdAssociado") %>">
                                                                <button type="button" class="btn btn-info" <%# Eval("ExibirBotaoResponder") %>><%# DataBinder.Eval(Container.DataItem, "RotuloBotao") %></button>
                                                            </a>
                                                            <asp:Button type="button" runat="server" Visible='<%# Eval("ExibirBotaoFinalizar") %>' ID="btnFinalizar" class="btn btn-dark" Text="Finalizar Avaliação" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdEmail") %>' OnClick="btnFinalizar_Click" />
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </tbody>
                                                </table>
                                                </FooterTemplate>
                                            </asp:Repeater>

                                            <!-- REPEATER - AV DE LIDERANÇA -->
                                            <asp:Repeater runat="server" DataSource='<%# Eval("Lideres") %>' Visible='<%# Eval("IsVisible") %>' >
                                                <HeaderTemplate>
                                                    <table class="table table-striped border dtInit">
                                                        <thead>
                                                            <tr>
                                                                <th>Foto</th>
                                                                <th>Profissional</th>
                                                                <th>Cargo</th>
                                                                <th>Período</th>
                                                                <th>Liderado</th>
                                                                <th>Etapa</th>
                                                                <th>Tipo de Avaliação</th>
                                                                <th data-sort="0"></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>

                                                <ItemTemplate>
                                                    <tr>
                                                        <td><img src=<%# DataBinder.Eval(Container.DataItem, "FotoAssociado.Imagem") %> width="120" height="120"></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.Nome") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.CARGOS.Cargo") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Periodo.Periodo") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Gestor.Nome") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Etapa") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "TipoAvaliacaoShow") %></td>
                                                        <td style="text-align:right;">
                                                            <br />
                                                            <a href="autoavalizacao_competencia.aspx?IdProjeto=<%# Eval("Projeto.IdProjeto") %>&IdAssociado=<%# Eval("Associado.IdAssociado") %>&IdPeriodo=<%# Eval("Periodo.IdPeriodo") %>&TipoAvaliacao=<%# Eval("TipoAvaliacao") %>&Escopo=<%# Eval("Escopo") %>&idGestor=<%# Eval("Gestor.IdAssociado") %>">
                                                                <button type="button" class="btn btn-info" <%# Eval("ExibirBotaoResponder") %>><%# DataBinder.Eval(Container.DataItem, "RotuloBotao") %></button>
                                                            </a>
                                                            <asp:Button type="button" runat="server" Visible='<%# Eval("ExibirBotaoFinalizar") %>' ID="btnFinalizar" class="btn btn-dark" Text="Finalizar Avaliação" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IdEmail") %>' OnClick="btnFinalizar_Click" />
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

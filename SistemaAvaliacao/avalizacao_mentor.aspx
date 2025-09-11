<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avalizacao_mentor.aspx.cs" Inherits="SistemaAvaliacao.avalizacao_mentor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Mentor</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Avaliação / Mentor</li>
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

                        <%--FILTRO AVALIAÇÕES--%>
                        <h4 class="card-title">Filtro avaliações</h4>
                        <form action="#">
                            <div class="form-body">
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

                    </div>
                </div>
            </div>

            <%--TABELA MENTORADOS--%>
            <div class="col-12">
            <div class="card">
                <div class="card-body">

                        <h4 class="card-title">Mentorados do período atual</h4>
                        <div class="table-responsive">
                            <asp:Repeater runat="server" ID="rptMentorados" >
                                <HeaderTemplate>
                                    <table class="table table-striped border dtInit">
                                        <thead>
                                            <tr>
                                                <th></th>
                                                <th>Mentorado</th>
                                                <th>Cargo</th>
                                                <th>Mentoria realizada</th>
                                                <th>Dados do RH disponíveis</th>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><img src=<%# DataBinder.Eval(Container.DataItem, "FotoNome") %> width="120" height="120"></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Cargo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "MentoriaRealizada") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "DadosRHLiberados") %></td>
                                        <td style="width:170px; text-align:center;align-content:center;vertical-align:central;align-items:center">
                                            <a href="avalizacao_mentor_competencia.aspx?IdProjeto=<%# Eval("idProjeto") %>&IdAssociado=<%# Eval("idAssociado") %>&IdPeriodo=<%# Eval("idPeriodo") %>&TipoAvaliacao=<%# Eval("TipoAvaliacao") %>&Escopo=<%# Eval("Escopo") %>&idGestor=<%# Eval("idGestor") %>&Exibir=Tudo">
                                                <button type="button" class="btn btn-info">Mentoria</button>
                                            </a>
                                            <a href="avalizacao_mentor_competencia.aspx?IdProjeto=<%# Eval("idProjeto") %>&IdAssociado=<%# Eval("idAssociado") %>&IdPeriodo=<%# Eval("idPeriodo") %>&TipoAvaliacao=<%# Eval("TipoAvaliacao") %>&Escopo=<%# Eval("Escopo") %>&idGestor=<%# Eval("idGestor") %>&Exibir=FeedbackRH">
                                                <button type="button" class="btn btn-facebook" <%# Eval("FeedbackRHVisible") %>>Feedback RH</button>
                                            </a>
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

            <%--TABELA AVALIAÇÕES--%>
            <div class="col-12">
            <div class="card">
                <div class="card-body">

                        <h4 class="card-title">Avaliações de seus mentorados</h4>
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
                                            <asp:Repeater runat="server" DataSource='<%# Eval("Associados") %>' Visible='<%# Eval("IsVisible") %>'>
                                                <HeaderTemplate>
                                                    <table class="table table-striped border dtInit">
                                                        <thead>
                                                            <tr>
                                                                <th>Foto</th>
                                                                <th>Profissional</th>
                                                                <th>Cargo</th>
                                                                <th>Período</th>
                                                                <th>Dt. Início Projeto</th>
                                                                <th>Dt. Término Projeto</th>
                                                                <th>Gestor</th>
                                                                <th>Avaliador</th>
                                                                <th>Mentor</th>
                                                                <th>Etapa</th>
                                                                <th data-sort="0"></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>

                                                <ItemTemplate>
                                                    <tr>
                                                        <td><img src=<%# DataBinder.Eval(Container.DataItem, "Associado.FotoNome") %> width="120" height="120"></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.Nome") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.CARGOS.Cargo") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Periodo.Periodo") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "DataInicio") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "DataTermino") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Gestor.Nome") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Avaliador.Nome") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.ASSOCIADOS3.Nome") %></td>
                                                        <td><%# DataBinder.Eval(Container.DataItem, "Etapa") %></td>
                                                        <td style="width:170px; text-align:right;align-content:center">
                                                            <a href="avalizacao_mentor_competencia.aspx?IdProjeto=<%# Eval("Projeto.IdProjeto") %>&IdAssociado=<%# Eval("Associado.IdAssociado") %>&IdPeriodo=<%# Eval("Periodo.IdPeriodo") %>&TipoAvaliacao=<%# Eval("TipoAvaliacao") %>&Escopo=<%# Eval("Escopo") %>&idGestor=<%# Eval("Gestor.IdAssociado") %>&Exibir=Tudo">
                                                                <button type="button" class="btn btn-info" <%# Eval("ExibirBotaoVerMentor") %>>Ver Avaliação</button>
                                                            </a>
                                                            <a href="avalizacao_mentor_competencia.aspx?IdProjeto=<%# Eval("Projeto.IdProjeto") %>&IdAssociado=<%# Eval("Associado.IdAssociado") %>&IdPeriodo=<%# Eval("Periodo.IdPeriodo") %>&TipoAvaliacao=<%# Eval("TipoAvaliacao") %>&Escopo=<%# Eval("Escopo") %>&idGestor=<%# Eval("Gestor.IdAssociado") %>&Exibir=FeedbackRH">
                                                                <button type="button" class="btn btn-facebook" <%# Eval("ExibirBotaoVerMentor") %> <%# Eval("FeedbackRHVisible") %>>FeedbackRH</button>
                                                            </a>
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

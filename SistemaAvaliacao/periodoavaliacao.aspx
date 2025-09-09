<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="periodoavaliacao.aspx.cs" Inherits="SistemaAvaliacao.periodoavaliacao" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Períodos</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastro de Períodos</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Período</li>
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

        <%--CARD EDITAR CAMPOS--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Período da Avaliação</h4>
                <form action="#">
                    <div class="form-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Empresa</label>
                                    <asp:DropDownList ID="ddlEmpresa" class="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Status</label>
                                    <asp:DropDownList ID="ddlStatus" class="form-control" runat="server">
                                        <asp:ListItem Value="1">Ativo</asp:ListItem>
                                        <asp:ListItem Value="0">Inativo</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Data Início</label>
                                    <asp:TextBox ID="txtDataInicio" TextMode="Date" class="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Data Termino</label>
                                    <asp:TextBox ID="txtDataTermino" TextMode="Date" class="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Código (<b>N</b>SEM-<b>YYYY</b>)</label>
                                    <asp:TextBox ID="txtPeriodo" class="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">                              
                            </div>
                        </div>
                    </div>
                    <div class="form-actions">
                        <div class="text-right">
                            <label style="color:darkred">Ao cadastrar um NOVO período, todas as avaliações que os gestores sinalizaram que devem ocorrer no "próximo período" serão atribuidas a este novo período cadastrado.</label>
                            <asp:Button ID="btnCadastrar" OnClick="btnCadastrar_Click" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />
                        </div>
                    </div>
                </form>
            </div>
        </div>

        <%--CARD LISTA PERÍODOS--%>
        <div class="card">

            <div class="card-body">
                <h4 class="card-title">Lista de Períodos</h4>
                <div class="table-responsive">
                    <asp:HiddenField ID="hdIdPeriodo" runat="server" />
                    <asp:Repeater ID="rptPeriodo" runat="server" OnItemCommand="rptPeriodo_ItemCommand">

                        <HeaderTemplate>
                            <table class="table table-striped border dtInit">
                                <thead>
                                    <tr>
                                        <th>Código</th>
                                        <th>Data Início</th>
                                        <th>Data Término</th>
                                        <th>Status(Ativo/Inativo)</th>
                                        <th data-sort="0" class="text-center">Ação</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# DataBinder.Eval(Container.DataItem, "Codigo") %></td>
                                <td><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DataInicio")).ToString("dd/MM/yyyy") %></td>
                                <td><%# ((DateTime)DataBinder.Eval(Container.DataItem, "DataFim")).ToString("dd/MM/yyyy") %></td>
                                <td><%# FormatStatus((int)DataBinder.Eval(Container.DataItem, "ATV")) %></td>                             
                                <td class="text-center">
                                    <asp:Label ID="lblIdPeriodo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IdPeriodo") %>' Visible="false"></asp:Label>
                                    <asp:Button ID="btnAlterar" class="btn btn-info" runat="server" Text="Alterar" CommandName="AlterarPeriodo" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
								<tfoot>
                                    <tr>
                                        <th>Código</th>
                                        <th>Data Início</th>
                                        <th>Data Término</th>
                                        <th>Status(Ativo/Inativo)</th>
                                        <th data-sort="0">Ação</th>
                                    </tr>
                                </tfoot>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>

        <%--CARD AVALIAÇÕES SINALIZADAS PARA PRÓXIMO PERÍODO--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Avaliações as quais o GESTOR sinalizou que devem ocorrer no próximo período cadastrado:</h4>
                <div class="table-responsive">
                    <asp:Repeater ID="rptAvaliacoesSinalizadas" runat="server">
                        <HeaderTemplate>
                            <table class="table table-striped border">
                                <thead>
                                    <tr>
                                        <th>Projeto</th>
                                        <th>Associado</th>
                                        <th>Avaliador</th>
                                        <th>Início alocação</th>
                                        <th>Término alocação</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                                <tr>
                                    <td><%# Eval("Projeto")%></td>
                                    <td><%# Eval("Respondente")%></td>
                                    <td><%# Eval("Avaliador")%></td>
                                    <td><%# Eval("DataInicio")%></td>
                                    <td><%# Eval("DataTermino")%></td>
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
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="dist/js/custom.min.js"></script>
</asp:Content>

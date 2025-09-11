<%@ Page Title="Cargos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cargos.aspx.cs" Inherits="SistemaAvaliacao.WebForm2" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />

    <!-- ============================================================== -->
    <!-- Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->

    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Cargos</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastro Básico de Usuários</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Cargos</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>

    <!-- ============================================================== -->
    <!-- End Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->



    <div class="page-content container-fluid">
        <!-- ============================================================== -->
        <!-- Start Page Content -->
        <!-- ============================================================== -->
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Dados do Cargo</h4>
                <div class="form-body">
                    <div class="row">
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Cargo</label>
                                <asp:TextBox ID="txtCargo" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Próximo Cargo</label>
                                <asp:DropDownList ID="ddlProximocargo" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Tempo mínimo para promoção (em meses)</label>
                                <asp:TextBox ID="txtTempoMinimo" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Função</label>
                                <asp:TextBox ID="txtFuncao" TextMode="MultiLine" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Autonomia</label>
                                <asp:TextBox ID="txtAutonomia" TextMode="MultiLine" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Escopo de Atuação</label>
                                <asp:TextBox ID="txtEscopoAtuacao" TextMode="MultiLine" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="form-group">
                                <label>Nível de interlocução principal no cliente</label>
                                <asp:TextBox ID="txtNivelInterlocucao" TextMode="MultiLine" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Status</label>
                                <asp:DropDownList ID="ddlStatus" class="form-control" runat="server">
                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="0">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="form-actions">
                    <div class="text-right">
                        <asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />
                    </div>
                </div>

            </div>
        </div>

        <%--EXPORT--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Exportar Cargos</h4>
                <asp:Button runat="server" type="button" ID="btnExport" class="btn btn-info" OnClick="btnExport_Click" Text="Exportar"></asp:Button>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
            <%--TABELA DE CARGOS--%>
            <div class="row mt">
                <div class="col-md">
                    <div class="card-body">
                        <h4 class="card-title">Lista de cargos</h4>
                        <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial: <code>cargo código</code>. </h6>
                        <div class="table-responsive">

                            <asp:HiddenField ID="hdIdCargo" runat="server" />

                            <asp:Repeater ID="rptCargos" OnItemCommand="rptCargos_ItemCommand" OnItemDataBound="rptCargos_ItemDataBound" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-striped border dtInit">
                                        <thead>
                                            <tr>
                                                <th>Código</th>
                                                <th>Cargo</th>
                                                <th>Próximo Cargo</th>
                                                <th>Status(Ativo/Inativo)</th>
                                                <th data-sort="0">Ação</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblIdCargo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "idCargo") %>' Visible="false"></asp:Label>
                                            <%# DataBinder.Eval(Container.DataItem, "IdCargo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Cargo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "CARGOS2.Cargo") %></td>
                                        <td>
                                            <asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                        <td>
                                            <asp:Button ID="btnAlterar" class="btn btn-info" runat="server" Text="Alterar" CommandName="AlterarCargo" />
                                            <asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Inativar" CommandName="ExcluirCargo" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
										<tfoot>
                                            <tr>
                                                <th>Código</th>
                                                <th>Cargo</th>
                                                <th>Próximo Cargo</th>
                                                <th data-sort="0">Ação</th>
                                            </tr>
                                        </tfoot>
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
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="dist/js/custom.min.js"></script>

</asp:Content>


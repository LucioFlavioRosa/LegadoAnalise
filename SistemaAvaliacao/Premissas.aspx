<%@ Page Title="Premissas do Radar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Premissas.aspx.cs" Inherits="SistemaAvaliacao.Premissas" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Competências</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastro Básico de Premissas</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Premissas do Radar</li>
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


        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Dados da Premissa</h4>
                <div class="form-body">

                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Eixo</label>
                                <asp:DropDownList ID="ddlEixo" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Cargo</label>
                                <asp:DropDownList ID="ddlCargo" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Nível</label>
                                <asp:DropDownList ID="ddlNivel" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row">

                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Valor Radar Peers</label>
                                <asp:TextBox ID="txtPeers" TextMode="Number" class="form-control text-right" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Valor Base Avaliado</label>
                                <asp:TextBox ID="txtAvaliado" TextMode="Number" class="form-control text-right" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Valor Base Gestor</label>
                                <asp:TextBox ID="txtGestor" TextMode="Number" class="form-control text-right" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-actions col-md-3">
                            <label>&nbsp;</label>
                            <div class="text-right">
                                <asp:HiddenField ID="hdId" runat="server" />
                                <asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <div class="card">

            <div class="card-body">
                <h4 class="card-title">Lista de Competências</h4>
                <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial. </h6>
                <div class="table-responsive">
                    <asp:Repeater ID="rptPremissas" runat="server" OnItemCommand="rptPremissas_ItemCommand">
                        <HeaderTemplate>
                            <table class="table table-striped border dtInit">
                                <thead>
                                    <tr>
                                        <th>Eixo</th>
                                        <th>Cargo</th>
                                        <th>Nivel</th>
                                        <th>Base Peers</th>
                                        <th>Base Avaliado</th>
                                        <th>Base Gestor</th>
                                        <th>Status(Ativo/Inativo)</th>
                                        <th data-sort="0">Ação</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Label ID="lblIdPremissa" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IdPremissa") %>' Visible="false"></asp:Label>
                                    <%# DataBinder.Eval(Container.DataItem, "EIXOS.Eixo") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "CARGOS.Cargo") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "CARGOSNIVEIS.Nivel") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "ValorRadarPeers") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "ValorBaseAutoAvaliacao") %></td>
                                <td><%# DataBinder.Eval(Container.DataItem, "ValorBaseAvaliacaoGestor") %></td>
                                <td><%# StatusRadar((int)DataBinder.Eval(Container.DataItem, "ATV")) %></td>
                                <td>
                                    <asp:Button ID="btnAlterar" class="btn btn-info" runat="server" Text="Alterar" CommandName="Alterar" />
                                    <asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Ativar/Inativar" CommandName="Inativar" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
									<tfoot>
                                        <tr>
                                            <th>Eixo</th>
                                            <th>Cargo</th>
                                            <th>Nivel</th>
                                            <th>Base Peers</th>
                                            <th>Base Avaliado</th>
                                            <th>Base Gestor</th>
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

    <!-- ============================================================== -->
    <!-- End Container fluid  -->
    <!-- ============================================================== -->


    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>
    <script src="dist/js/custom.min.js"></script>
    
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FrentesInternas.aspx.cs" Inherits="SistemaAvaliacao.FrenteInterna" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">ALOCAÇÕES INTERNAS</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Gerenciar Métricas</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Alocações Internas</li>
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
    <asp:UpdatePanel runat="server" ID="updFrenteInterna" UpdateMode="Always" EnableViewState="true">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="rptFrentesInternas" />
        </Triggers>
        <ContentTemplate>
            <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler1" />
            <div class="page-content container-fluid">
                <%--EDITOR DE CAMPOS--%>
                <div class="card">
                    <div class="card-body">

                        <h4>Dados da Alocação Interna</h4>
                        <asp:HiddenField ID="hdId" runat="server" />
                        <div class="form-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label>Nome</label>
                                        <asp:TextBox ID="txtFrenteInterna" class="form-control" runat="server"></asp:TextBox>
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
                        </div>
                        <div class="form-body" id="divLideres" runat="server">
                            <br />
                            <h4>Líderes da Alocação Interna</h4>
                            <asp:Repeater ID="rptLideres" OnItemCommand="rptLideres_ItemCommand" OnItemDataBound="rptLideres_ItemDataBound" OnItemCreated="rptLideres_ItemCreated" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-striped border dtInit">
                                        <thead>
                                            <tr>
                                                <th>IdAssociado</th>
                                                <th>Líder</th>
                                                <th>Status(Ativo/Inativo)</th>
                                                <th data-sort="0">Ação</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblIdAssociado" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "idAssociado") %>' Visible="false"></asp:Label>
                                            <asp:Label ID="lblIdLiderFrente" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "idLiderFrenteInterna") %>' Visible="false"></asp:Label>
                                            <%# DataBinder.Eval(Container.DataItem, "idAssociado") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "ASSOCIADOS.Nome") %></td>
                                        <td>
                                            <asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                        <td>
                                            <asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Inativar" CommandName="Excluir" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <tr>
                                        <td colspan="3">
                                            <asp:DropDownList runat="server" ID="ddlAssociados" class="form-control"></asp:DropDownList></td>
                                        <td>
                                            <asp:Button ID="btnAdicionar" class="btn btn-dark" runat="server" Text="Adicionar" CommandName="Adicionar" />
                                        </td>
                                    </tr>
                                    </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="form-body" id="divParticipantes" runat="server">
                            <br />
                            <h4>Participantes da Alocação Interna</h4>
                            <asp:Repeater ID="rptParticipantes" OnItemCommand="rptParticipantes_ItemCommand" OnItemDataBound="rptParticipantes_ItemDataBound" OnItemCreated="rptParticipantes_ItemCreated" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-striped border dtInit">
                                        <thead>
                                            <tr>
                                                <th>IdAssociado</th>
                                                <th>Participante</th>
                                                <th>Status(Ativo/Inativo)</th>
                                                <th data-sort="0">Ação</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblIdAssociado" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "idAssociado") %>' Visible="false"></asp:Label>
                                            <asp:Label ID="lblIdParticipante" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "idParticipanteFrenteInterna") %>' Visible="false"></asp:Label>
                                            <%# DataBinder.Eval(Container.DataItem, "idAssociado") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "ASSOCIADOS.Nome") %></td>
                                        <td>
                                            <asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                        <td>
                                            <asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Inativar" CommandName="Excluir" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <tr>
                                        <td colspan="3">
                                            <asp:DropDownList ID="ddlAssociados" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnAdicionar" class="btn btn-dark" runat="server" Text="Adicionar" CommandName="Adicionar" />
                                        </td>
                                    </tr>
                                    </tbody>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="form-actions">
                            <div class="text-right">
                                <asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Cadastrar" />
                            </div>
                        </div>
                    </div>
                </div>

                <%--LISTA DE FRENTES INTERNAS--%>
                <div class="card">

                    <div class="card-body">
                        <h4 class="card-title">Lista de Alocações Internas</h4>
                        <h6 class="card-subtitle">Filtre as informações separadas por espaço para busca em qualquer coluna em qualquer ordem, completo ou parcial. </h6>
                        <div class="table-responsive">
                            <asp:Repeater ID="rptFrentesInternas" OnItemCommand="rptFrentesInternas_ItemCommand" OnItemDataBound="rptFrentesInternas_ItemDataBound" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-striped border dtInit">
                                        <thead>
                                            <tr>
                                                <th>ID</th>
                                                <th>Alocação Interna</th>
                                                <th>Líderes</th>
                                                <th>Status(Ativo/Inativo)</th>
                                                <th data-sort="0">Ação</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "idFrenteInterna") %>' Visible="false"></asp:Label>
                                            <%# DataBinder.Eval(Container.DataItem, "idFrenteInterna") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "FrenteInterna") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Lideres") %></td>
                                        <td>
                                            <asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                                        <td>
                                            <asp:Button ID="btnAlterar" class="btn btn-info" runat="server" Text="Alterar" CommandName="Alterar" />
                                            <asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Inativar" CommandName="Excluir" />
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

                <%--INPUT NOTAS--%>
                <div class="card card-body" runat="server" id="divAvaliar">
                    <h4>Avaliar</h4>
                    <h5 runat="server" id="labelDisclaimerAvaliar">Siga o botão abaixo para avaliar os associados das alocações internas que você for líder no período de </h5>
                    <div class="form-actions">
                        <asp:Button OnClick="btnAvaliar_Click" ID="btnAvaliar" class="btn btn-facebook" runat="server" Text="Avaliar"  style="width:20%" />
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <%--EXPORT--%>
    <div class="card card-body" runat="server" id="divExport">
        <h4 class="card-title">Exportar Avaliações de Alocações Internas</h4>
        <div class="form-actions">
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" CssClass="btn btn-facebook" OnClick="btnExportar_Click" />
        </div>
    </div>
    
    <!-- ============================================================== -->
    <!-- End Container fluid  -->
    <!-- ============================================================== -->

    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>
    <script src="dist/js/custom.min.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>

    <script src="dist/js/app.min.js"></script>
    <script src="dist/js/app.init.js"></script>
    <script src="dist/js/app-style-switcher.js"></script>
    <script src="assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js"></script>
    <script src="assets/extra-libs/sparkline/sparkline.js"></script>
    <script src="dist/js/waves.js"></script>
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>

</asp:Content>

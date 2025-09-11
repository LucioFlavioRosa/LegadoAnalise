<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="resultado.aspx.cs" Inherits="SistemaAvaliacao.resultado" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
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

    <div class="page-content container-fluid">

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
                                <th>Mentor</th>
                                <th>Tipo de Avaliações</th>
                                <th>Escopo</th>
                                <th>Qtd. Projetos</th>
                                <th>Resultado</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptProjetos" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Periodo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Nome") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Cargo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Mentor") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "TipoAvaliacao") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Escopo") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "QtdProjetos") %></td>
                                        <td class="text-center no-sort">
                                            <a href="avalizacao_resultado.aspx?&IdAssociado=<%# DataBinder.Eval(Container.DataItem, "IdAssociado") %>&IdPeriodo=<%# DataBinder.Eval(Container.DataItem, "IdPeriodo") %>&TipoAvaliacao=<%# DataBinder.Eval(Container.DataItem, "TipoAvaliacao") %>&Escopo=<%# DataBinder.Eval(Container.DataItem, "Escopo") %>">
                                                <button type="button" class="btn btn-info">Analisar</button>
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

        <div class="card card-body">
            <h4>Resultados de avaliações de liderança</h4>
            <br />
             <div class="form-group">
                <asp:Button runat="server" ID="btnExtrair" OnClick="btnExtrair_Click" CssClass="btn btn-facebook" style="width:10%" Text="Exportar"/>  
                 <asp:Button runat="server" ID="btnLiberaLideranca" OnClick="btnLiberaLideranca_Click" CssClass="btn btn-facebook" style="width:20%" Visible="false"/>
                 <asp:Button runat="server" ID="btnLiberaMentoria" OnClick="btnLiberaMentoria_Click" CssClass="btn btn-facebook" style="width:20%" Visible="false"/>
            </div>
        </div>

        <div class="card card-body">
            <h4>Exportar resultados de avaliações de desempenho</h4>
            <br />
            <div class="col-md-2">
                <div class="form-group">
                    <label>Período</label>
                    <select id="ddlPeriodo_Export" runat="server" class="form-control p-0 select2" style="width: 100%" />
                </div>
            </div>
            <asp:Button runat="server" ID="btnExportDesempenho" OnClick="btnExportDesempenho_Click" CssClass="btn btn-facebook" style="width:10%" Text="Exportar"/>
        </div>
        
        <div class="card card-body">
            <h4>Exportar resultados de avaliações de mentorado</h4>
            <br />
            <div class="col-md-2">
                <div class="form-group">
                    <label>Período</label>
                    <select id="ddlPeriodo_Mentor_Export" runat="server" class="form-control p-0 select2" style="width: 100%" />
                </div>
            </div>
            <asp:Button runat="server" ID="btnExportMentoria" OnClick="btnExportMentoria_Click" CssClass="btn btn-facebook" style="width:10%" Text="Exportar"/>
        </div>

    </div>

    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>
    <script src="dist/js/custom.min.js"></script>
    <script src="dist/js/pages/resultado/datatable-resultado.js"></script>
</asp:Content>

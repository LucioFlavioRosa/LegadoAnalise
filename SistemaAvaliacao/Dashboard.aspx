<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SistemaAvaliacao.Dashboard" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <asp:HiddenField runat="server" ID="hfAndamentoAvaliacoes" />
    <asp:HiddenField runat="server" ID="hfAvaliadosPeriodo" />
    <asp:HiddenField runat="server" ID="hfCompetencia" />
    <asp:HiddenField runat="server" ID="hfPerformance" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Dashboard</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Dashboard</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>
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
                                        <div class="form-group">
                                            <label>Período</label>
                                            <asp:DropDownList ID="ddlPeriodos" runat="server" CssClass="form-control p-0 select2">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <asp:Button runat="server" type="button" ID="btnSearch" Style="margin-top: 30px"
                                        class="btn btn-danger" OnClick="BtnPeriodo_Click" Text="Trocar Período"></asp:Button>
                                </div>
                                <div class="col-md-7 text-center">
                                    <h4 class="card-title" style="margin-top: 40px">
                                        <asp:Label ID="lblPeriodo" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-xl-3 col-md-6">
                <div class="card border-left-primary shadow">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">Associados</div>
                                <div class="h5 mb-0 font-weight-bold text-gray-800">
                                    <asp:Label ID="lblAssociados" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i class="fas fa-users fa-2x text-gray-300"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xl-2 col-md-6">
                <div class="card border-left-warning shadow">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">Mentores</div>
                                <div class="h5 mb-0 font-weight-bold text-gray-800">
                                    <asp:Label ID="lblMentores" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i class="fas fa-comments fa-2x text-gray-300"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xl-2 col-md-6">
                <div class="card border-left-warning shadow">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">Avaliados</div>
                                <div class="h5 mb-0 font-weight-bold text-gray-800">
                                    <asp:Label ID="lblAvaliacoes" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i class="fas fa-pencil-alt fa-2x text-gray-300"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xl-3 col-md-6">
                <div class="card border-left-success shadow">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-success text-uppercase mb-1">Avaliadores</div>
                                <div class="h5 mb-0 font-weight-bold text-gray-800">
                                    <asp:Label ID="lblAvaliadores" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i class="fas fa-user-secret fa-2x text-gray-300"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xl-2 col-md-6">
                <div class="card border-left-info shadow">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-info text-uppercase mb-1">Gestores</div>
                                <div class="row no-gutters align-items-center">
                                    <div class="col-auto">
                                        <div class="h5 mb-0 mr-3 font-weight-bold text-gray-800">
                                            <asp:Label ID="lblGestores" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i class="fas fa-user-plus fa-2x text-gray-300"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-xl-4 col-lg-4">
                <div class="card shadow ">
                    <div class="card-header py-3 d-flex flex-row align-items-center justify-content-between">
                        <h6 class="m-0 font-weight-bold text-primary">Andamento das Avaliações</h6>
                    </div>
                    <div class="card-body">
                        <div class="table-responsive" style="clear: both;">
                            <table class="table table-hover">
                                <thead>
                                    <tr>
                                        <th>Status</th>
                                        <th class="text-center">Qtd.</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptAndamento" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Status") %></td>
                                                <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "QtdStatus") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xl-8 col-lg-8">
                <div class="card shadow">
                    <div class="card-header py-3 d-flex flex-row align-items-center justify-content-between">
                        <h6 class="m-0 font-weight-bold text-primary">Gráfico Percentagem do Andamento das Avaliações</h6>
                    </div>
                    <div class="card-body">
                        <div class="chart-pie">
                            <canvas id="pieandamentoavaliacoes" height="120"></canvas>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-xl-8 col-lg-8">
                <div class="card shadow ">
                    <div class="card-header py-3">
                        <h6 class="m-0 font-weight-bold text-primary">Avaliados Por Período</h6>
                    </div>
                    <div class="card-body">
                        <div class="chart-bar">
                            <canvas id="baravaliadosperiodo" height="120"></canvas>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xl-4 col-lg-4">
                <div class="card shadow ">
                    <div class="card-header py-3">
                        <h6 class="m-0 font-weight-bold text-primary">Avaliados por Projeto</h6>
                    </div>
                    <div class="card-body" style="overflow-y: scroll; height: 300px; width: auto;">
                        <div class="table-responsive" style="clear: both;">
                            <table class="table table-hover">
                                <thead>
                                    <tr>
                                        <th>Projeto</th>
                                        <th class="text-center">Qtd.</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptProjetos" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# DataBinder.Eval(Container.DataItem, "Status") %></td>
                                                <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "QtdStatus") %></td>
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

        <div class="row">
            <div class="col-xl-6 col-lg-6">
                <div class="card shadow ">
                    <div class="card-header py-3">
                        <h6 class="m-0 font-weight-bold text-primary">
                        Média Geral Consolidado de Competências</h6>
                    </div>
                    <div class="card-body">
                        <div class="chart-bar">
                            <canvas id="radarn1" height="150"></canvas>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-xl-6 col-lg-6">
                <div class="card shadow ">
                    <div class="card-header py-3">
                        <h6 class="m-0 font-weight-bold text-primary">Média Geral Consolidado de Performance</h6>
                    </div>
                    <div class="card-body">
                        <canvas id="linechart" height="150"></canvas>
                    </div>
                </div>
            </div>
        </div>

    </div>
    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/libs/Chart.js-2.9.3/Chart.min.js"></script>
    <script src="assets/libs/Chart.js-2.9.3/chartjs-plugin-datalabels.min.js"></script>
    <script src="dist/js/pages/dashboard/dash.js"></script>
</asp:Content>

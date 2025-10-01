<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EvolucaoAssociado.aspx.cs" Inherits="SistemaAvaliacao.EvolucaoAssociado" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <asp:HiddenField runat="server" ID="hfJsonRadar" /> 
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Associado</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item"><a href="index.aspx">Competência / Perfomance</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Evolução</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>
    <div class="page-content container-fluid">
        <div class="card">
            <div class="card-body">
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-1">
                                    <label><b>Avaliado</b></label>
                                </div>
                                <div class="col-md-4">
                                    <label runat="server" id="lblAssociado"></label>
                                </div>
                                <div class="col-md-2">
                                    <label><b>Mentor</b></label>
                                </div>
                                <div class="col-md-5">
                                    <label runat="server" id="lblMentor"></label>
                                </div>                                
                            </div> 
                            <div class="row">
                                <div class="col-md-1">
                                    <label><b>Cargo</b></label>
                                </div>
                                <div class="col-md-4">
                                    <label runat="server" id="lblCargo"></label>
                                </div>
                                <div class="col-md-2 ffset-md-1">
                                    <label><b>Próximo Cargo</b></label>
                                </div>
                                <div class="col-md-5">
                                    <label runat="server" id="lblProximoCargo"></label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card">
            <div class="card-body">
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-3">
                            <div class="row">
                                <label>Tipo de Avaliações</label>
                            </div>
                            <div class="row">
                                <select ID="ddl_TipoAvaliacao" class="form-control p-0 select2" runat="server" style="width:80%"></select>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="row">
                                <label>Escopo</label>
                            </div>
                            <div class="row">
                                <select ID="ddl_Escopo" class="form-control p-0 select2" runat="server" style="width:80%"></select>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <asp:Button ID="btnGerar" runat="server" Text="Gerar Evolução" OnClick="btnGerar_Click" class="btn btn-info"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="card">
            <div class="card-body">
                <div class="row">
                    <asp:Repeater ID="rptProjetos" runat="server">
                        <ItemTemplate>
                            <div class="col-md-3">
                                <div class="row">
                                    <div class="col-md-12">     
                                        <%--SOMA COMPETENCIAS--%>
                                        <table class="table table-striped border tbresultado">
                                            <tbody>
                                                <tr>                                                    
                                                    <td colspan="2" class="text-center"><%# DataBinder.Eval(Container.DataItem,"Cargo") %></td>
                                                </tr>
                                                <tr>
                                                    <td class="tdresultado">Período</td>
                                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"PERIODOSAVALIACOES.Periodo") %></td>
                                                </tr>
                                                <tr>
                                                    <td class="tdresultado">Nota Competências </td>
                                                    <td class="text-center"><b><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"NotaCompetencia")) %></b></td>
                                                </tr>
                                                <asp:Repeater ID="rptSomaCompetencias" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td><%# Eval("EIXOS.Eixo") %></td>
                                                            <td class="text-center"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"NotaNeutra")) %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>

                                        <%--PERFOMANCE--%>
                                        <div id="divTabelaPerformance" runat="server">
                                            <table class="table table-striped border tbresultado">
                                                <tbody>
                                                    <tr>
                                                        <td class="text-center" colspan="2"><%# DataBinder.Eval(Container.DataItem,"RatingPerformance") %></td>
                                                    </tr>
                                                    <tr>
                                                        <tr>
                                                            <td class="tdresultado">Nota Performance </td>
                                                            <%--<td class="text-center"><b><%# FormatDecimal((decimal)DataBinder.Eval(Container.DataItem,"NotaPerfomance")) %></b></td>--%>
                                                            <td class="text-center">
                                                                <b>
                                                                    <%# DataBinder.Eval(Container.DataItem, "NotaPerfomance") != DBNull.Value && DataBinder.Eval(Container.DataItem, "NotaPerfomance") != null
                                                                        ? FormatDecimal(Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "NotaPerfomance")))
                                                                        : "" %>
                                                                </b>
                                                            </td>

                                                        </tr>
                                                    </tr>
                                                    <asp:Repeater ID="rptNotaPerfomance" runat="server">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td><%# Eval("PERFORMANCES.Performance") %></td>
                                                                <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"Nota") %></td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>
                                        </div>
                                        

                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <br />
                <div class="row" style="display:flex;justify-content:center">
                    <div style="width:70%;height:70%; margin: 15px;">
                        <canvas id="radarassociado" width="120" height="120" padding: "25px"></canvas>
                    </div>
                </div>
            </div>
        </div>
    </div>

     <link href="dist/custom/resultado.css" rel="stylesheet" />
    <script src="assets/libs/jquery/dist/jquery.min.js"></script>      
    <script src="assets/libs/Chart.js-2.9.3/Chart.min.js"></script>
    <script src="dist/js/pages/evolucao/evolucao.js"></script>
</asp:Content>
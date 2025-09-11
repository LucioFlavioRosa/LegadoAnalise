<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avalizacao_resultado.aspx.cs" Inherits="SistemaAvaliacao.avalizacao_resultado" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <%--<asp:HiddenField runat="server" ID="hfJsonRadar" />
    <asp:HiddenField runat="server" ID="hfJsonSomaRadar" />--%>
    <link rel="stylesheet" href="dist/css/sliderAvaliacao.css">
    <div class="page-breadcrumb border-bottom" style="position:sticky;top:50px;z-index:6">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Comitê</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item"><a href="index.aspx">Competência / Perfomance</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Resultado</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>

    <div style="overflow:initial !important" class="container-fluid page-content">

        <%--RÓTULO--%>
        <div class="card" style="position:sticky;top:80px;z-index:4;border-color: rgba(0, 0, 0, .25); border-width: 1px">
            <div class="card-body">
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">

                                <div class="divExpand">
                                    <img runat="server" id="imgUser1Comite" src="assets/images/users/usernophoto.jpg" class="imgExpand">
                                </div>

                                <%--DETALHES AVALIADO--%>
                                <div class="col-sm">
                                    <div class="row">
                                        <div class="col-md-1">
                                            <label><b>Avaliado</b></label>
                                        </div>
                                        <div class="col-md-3">
                                            <label runat="server" id="lblAssociado"></label>
                                        </div>
                                        <div class="col-md-1">
                                            <label><b>Período</b></label>
                                        </div>
                                        <div class="col-md-3">
                                            <label runat="server" id="lblPeriodo"></label>
                                        </div>
                                        <div class="col-md-1">
                                            <label><b>Cargo</b></label>
                                        </div>
                                        <div class="col-md-3">
                                            <label runat="server" id="lblCargo"></label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-1">
                                            <label><b>Próximo Cargo:</b></label>
                                        </div>
                                        <div class="col-md-3">
                                            <label runat="server" id="lblProximoCargo"></label>
                                        </div>
                                        <div class="col-md-1">
                                            <label><b>Tempo de Cargo:</b></label>
                                        </div>
                                        <div class="col-md-3">
                                            <label runat="server" id="lblTempoCargo"></label>
                                        </div>
                                        <div class="col-md-1">
                                            <label><b>Tempo de Peers</b></label>
                                        </div>
                                        <div class="col-md-3">
                                            <label runat="server" id="lblTempoPeers"></label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-1">
                                            <label><b>Mentor</b></label>
                                        </div>
                                        <div class="col-md-3">
                                            <label runat="server" id="lblMentor"></label>
                                        </div>
                                        <div class="col-md-1">
                                            <label><b>Cronômetro</b></label>
                                        </div>
                                        <div class="col-sm">
                                            <div class="container">
                                                <div id="time">
                                                    <span class="digit" id="hr">00</span>
                                                    <span class="txt">:</span>
                                                    <span class="digit" id="min">00</span>
                                                    <span class="txt">:</span>
                                                    <span class="digit" id="sec">00</span>
                                                    <span class="txt">:</span>
                                                    <span class="digit" id="count">00</span>
                                                    <button class="btn btn-facebook" id="start" runat="server" style="margin-left:40px">
                                                        Iniciar</button> 
                                                    <button class="btn btn-facebook" id="stop" runat="server">
                                                        Parar</button>
                                                    <button class="btn btn-facebook" id="reset" runat="server">
                                                        Reiniciar</button>
                                                </div>
                                                <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="start" />
                                                        <asp:AsyncPostBackTrigger ControlID="stop" />
                                                        <asp:AsyncPostBackTrigger ControlID="reset" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <ul class="nav nav-pills mb-3" id="pills-tab" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link active btn btn-facebook" id="tab-competencia-tab" data-toggle="pill" href="#tab-competencia" role="tab" aria-controls="tab-competencia" aria-selected="true">Resultados</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link btn btn-danger" id="tab-performance-tab" data-toggle="pill" href="#tab-performance" role="tab" aria-controls="tab-performance" aria-selected="false">Mentor</a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>

        <%--TABELAS E RADARES--%>
        <div class="card">

            <div class="tab-content" id="pills-tabContent">
                <!-- PILL RADAR -->
                <div class="tab-pane fade active in show" id="tab-competencia" role="tabpanel" aria-labelledby="tab-competencia-tab">
                    <div class="card-body" style="white-space:nowrap !important;overflow-x:scroll !important;overflow-y:hidden !important;float:none !important;display:inline">
                        <div style="white-space:nowrap !important;overflow-x:scroll !important;overflow-y:hidden !important;float:none !important;display:inline" class="row">

                            <asp:Repeater ID="rptProjetos" runat="server">
                                <ItemTemplate>
                                    <div style="display: inline-block !important" class="col-md-3">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <%--COMPETENCIA N1--%>
                                                <table class="table table-striped border tbresultado" id="tb_<%# DataBinder.Eval(Container.DataItem,"IdProjeto") %>">
                                                    <tbody>
                                                        <tr>
                                                            <td class="tdBranco" colspan="2">Projeto <%# string.Format("{0} - {1} - {2}",Container.ItemIndex + 1,((DateTime)DataBinder.Eval(Container.DataItem,"DataInicioAlocado")).ToString("dd/MM"),((DateTime)DataBinder.Eval(Container.DataItem,"DataFimAlocado")).ToString("dd/MM")) %>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="thresultado" colspan="2" title="<%# DataBinder.Eval(Container.DataItem,"ProjetoNome") %>">
                                                                <button type="button" style="padding-left: 1px;" class="btn btn-sm btncollapse" data-id="<%# DataBinder.Eval(Container.DataItem,"IdProjeto") %>">
                                                                    <i class="fas fa-plus-circle"></i>
                                                                </button>
                                                                <%# TruncarTexto((string)DataBinder.Eval(Container.DataItem,"ProjetoNome"),30) %>
                                                                <br />
                                                                <%# TruncarTexto((string)DataBinder.Eval(Container.DataItem,"ClienteNome"),30) %>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdresultado" style="width: 70%">Complexidade</td>
                                                            <td class="text-center" style="width: 30%">
                                                                <a href="#" data-id="<%# DataBinder.Eval(Container.DataItem,"IdProjeto") %>" class="btncomplexidade">
                                                                    <%# DataBinder.Eval(Container.DataItem,"ProjetoComplexidade") %>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdresultado">Nível</td>
                                                            <td class="text-center">1</td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdresultado">Nota Competências</td>
                                                            <td class="text-center"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"NotaCompetencialNivel1")) %></td>
                                                        </tr>
                                                        <asp:Repeater ID="rptCompetenciasN1" runat="server">
                                                            <ItemTemplate>
                                                                <tr class="hidelinha">
                                                                    <td><%# Eval("Eixo") %></td>
                                                                    <td class="text-center"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"PercentualNotaFinalNivel1")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <%--COMPETENCIA N2--%>
                                                <table class="table table-striped border tbresultado">
                                                    <tbody>
                                                        <tr>
                                                            <td class="tdresultado" style="width: 70%">Nível </td>
                                                            <td class="text-center" style="width: 30%">2</td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdresultado">Nota Competências</td>
                                                            <td class="text-center"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"NotaCompetencialNivel2")) %></td>
                                                        </tr>
                                                        <asp:Repeater ID="rptCompetenciasN2" runat="server">
                                                            <ItemTemplate>
                                                                <tr class="hidelinha">
                                                                    <td><%# Eval("Eixo") %></td>
                                                                    <td class="text-center"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"PercentualNotaFinalNivel2")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <%--PERFOMANCE--%>
                                                <table class="table table-striped border tbresultado">
                                                    <tbody>
                                                        <tr>
                                                            <td class="text-center" colspan="2"><%# DataBinder.Eval(Container.DataItem,"RatingPerfomance") %></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdresultado" style="width: 70%">Nota Performance </td>
                                                            <td class="text-center" style="width: 30%"><%# FormatDecimal(DataBinder.Eval(Container.DataItem,"SomaPerfomance")) %></td>
                                                        </tr>
                                                        <asp:Repeater ID="rptNotaPerfomance" runat="server">
                                                            <ItemTemplate>
                                                                <tr class="hidelinha">
                                                                    <td><%# Eval("Perfomance") %></td>
                                                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"NotaPerfomance") %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <%--SOMA COMPETENCIAS--%>
                                                <table class="table table-striped border tbresultado">
                                                    <tbody>
                                                        <tr>
                                                            <td class="tdresultado" style="width: 70%">Nota Competências </td>
                                                            <td class="text-center" style="width: 30%"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"SomaNotaCompetencialN1N2")) %></td>
                                                        </tr>
                                                        <asp:Repeater ID="rptSomaCompetencias" runat="server">
                                                            <ItemTemplate>
                                                                <tr class="hidelinha">
                                                                    <td><%# Eval("Eixo") %></td>
                                                                    <td class="text-center"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"PercentualSomaNotaFinalN1N2")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="row" hidden>
                                            <div class="col-md-12" >
                                                <%--RADAR--%>
                                                <table class="table table-striped border tbresultado" style="width: 100%">
                                                    <tbody>
                                                        <tr>
                                                            <td class="thresultado" style="width: 70%">Competência Radar</td>
                                                            <td class="text-center" style="width: 30%"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"SomaNotaCompetenciaRadar")) %></td>
                                                        </tr>
                                                        <asp:Repeater ID="rptRadar" runat="server">
                                                            <ItemTemplate>
                                                                <tr class="hidelinha">
                                                                    <td><%# Eval("Eixo") %></td>
                                                                    <td class="text-center"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"NotaCompetenciaRadar")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                        <tr style="align-content:center;align-items:center; text-align:center" hidden>
                                                            <td colspan="2">
                                                                <button type="button" class="btn btn-sm btnExpandRadar" data-id="<%# DataBinder.Eval(Container.DataItem,"IdProjeto") %>" hidden>
                                                                    <i class="fas fa-expand"></i>
                                                                </button>
                                                                <canvas width="40" id="radar_<%#Container.ItemIndex %>_<%# DataBinder.Eval(Container.DataItem,"IdProjeto") %>" hidden></canvas>
                                                        
                                                            </td>
                                                        </tr>
                                                        <tr style="align-content:center;align-items:center; text-align:center">
                                                            <td colspan="2">
                                                                <div id="reportContainer_CompEixo_<%# Eval("IdProjeto") %>" style="width: 100%; height: 350px"></div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">&nbsp;</td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdBranco text-center" colspan="2">
                                                                <a href="avalizacao_consolidacao.aspx?&IdProjeto=<%# DataBinder.Eval(Container.DataItem,"IdProjeto") %>&IdAssociado=<%# DataBinder.Eval(Container.DataItem,"IdAssociado") %>&IdPeriodo=<%# DataBinder.Eval(Container.DataItem,"IdPeriodo") %>&TipoAvaliacao=<%# DataBinder.Eval(Container.DataItem,"TipoAvaliacao") %>&Escopo=<%# DataBinder.Eval(Container.DataItem,"Escopo") %>">
                                                                    <button type="button" class="btn btn-sm btn-info">Ajustar Notas Comitê</button>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div style="display: inline-block !important" class="col-md-3">
                                <asp:Repeater ID="rptSomaProjetos" runat="server">
                                    <ItemTemplate>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <%--COMPLEXIDADE COMITE--%>
                                                <table class="table table-striped border tbresultado">
                                                    <tbody>
                                                        <tr>
                                                            <td class="tdBranco" colspan="2">Consolidado
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <tr>
                                                                <td class="tdComite" style="width: 70%">Complexidade Comitê </td>
                                                                <td class="text-center" style="width: 30%"><%# DataBinder.Eval(Container.DataItem,"ComplexidadeComite") %></td>
                                                            </tr>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdBranco" colspan="2">&nbsp;</td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdresultado">Competência no Cargo</td>
                                                            <td class="text-center"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"CompetenciaCargo")) %></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdBranco" colspan="2">&nbsp;</td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdresultado">Competência Cargo Acima</td>
                                                            <td class="text-center"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"CompetenciaProximoCargo")) %></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div>
                                                    <p id="divajuste" class="closeTable"></p>
                                                </div>
                                                <%--PERFOMANCE--%>
                                                <table class="table table-striped border tbresultado">
                                                    <tbody>
                                                        <tr>
                                                            <td class="text-center" colspan="2"><%# DataBinder.Eval(Container.DataItem,"RatingPerfomance") %></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="tdresultado" style="width: 70%">Nota Performance </td>
                                                            <td class="text-center" style="width: 30%"><%# FormatDecimal(DataBinder.Eval(Container.DataItem,"SomaProjetosPerfomance")) %></td>
                                                        </tr>
                                                        <asp:Repeater ID="rptSomaProjetosItems" runat="server">
                                                            <ItemTemplate>
                                                                <tr class="hidelinha">
                                                                    <td><%# Eval("Perfomance") %></td>
                                                                    <td class="text-center"><%# FormatDecimal(DataBinder.Eval(Container.DataItem,"NotaPerfomancePonderada")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <%-- SOMA COMPETENCIAS--%>
                                                <table class="table table-striped border tbresultado">
                                                    <tbody>
                                                        <tr>
                                                            <td class="tdresultado" style="width: 70%">Nota Competências </td>
                                                            <td class="text-center" style="width: 30%"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"SomaProjetosNotaCompetencialN1N2")) %></td>
                                                        </tr>
                                                        <asp:Repeater ID="rptProjetosSomaCompetencias" runat="server">
                                                            <ItemTemplate>
                                                                <tr class="hidelinha">
                                                                    <td><%# Eval("Eixo") %></td>
                                                                    <td class="text-center"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"NotaProjetosCompetencia")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="row" hidden>
                                            <div class="col-md-12">
                                                <%--RADAR--%>
                                                <table class="table table-striped border tbresultado">
                                                    <tbody>
                                                        <tr>
                                                            <td class="thresultado" style="width: 70%">Competência Radar </td>
                                                            <td class="text-center" style="width: 30%"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"SomaProjetosNotaCompetenciaRadar")) %></td>
                                                        </tr>
                                                        <asp:Repeater ID="rptSomaProjetosRadar" runat="server">
                                                            <ItemTemplate>
                                                                <tr class="hidelinha">
                                                                    <td><%# Eval("Eixo") %></td>
                                                                    <td class="text-center"><%# FormatPercentagem(DataBinder.Eval(Container.DataItem,"NotaProjetoCompetenciaRadar")) %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                        <tr hidden>
                                                            <td colspan="2">
                                                                <button type="button" class="btn btn-sm btnExpandSomaRadar">
                                                                    <i class="fas fa-expand"></i>
                                                                </button>
                                                                <%--<canvas id="radar_comite" width="40"></canvas>--%>
                                                            </td>
                                                        </tr>
                                                        <tr style="align-content:center;align-items:center; text-align:center">
                                                            <td colspan="2">
                                                                <div id="reportContainer_Total_1" style="width: 100%; height: 350px"></div>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- PILL CONSIDERAÇÕES DO MENTOR -->
                <div class="tab-pane fade" id="tab-performance" role="tabpanel" aria-labelledby="tab-performance-tab">
                    <%--CONSIDERAÇÕES MENTOR --%>
                    <div class="card">
                        <div class="card-body" style="margin-bottom: 1px">
                            <div class="form-body">
                                <div class="row">
                                    <div class="col-sm">
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 40%;">Associado Avaliado:</label>
                                        <asp:Label runat="server" ID="labelAssociado" Style="background-color: #F2F2F2; font-size: 18px; list-style-type: none; padding-left: 0px; float: right; width: 60%; text-align: center"></asp:Label>
                                    </div>
                                    <div class="col-sm">
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 40%;">Cargo:</label>
                                        <asp:Label runat="server" ID="labelCargo" Style="background-color: #F2F2F2; font-size: 18px; list-style-type: none; padding-left: 0px; float: right; width: 60%; text-align: center"></asp:Label>
                                    </div>
                                    <div class="col-sm">
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 40%;">Vertical:</label>
                                        <asp:Label runat="server" ID="labelVertical" Style="background-color: #F2F2F2; font-size: 18px; list-style-type: none; padding-left: 0px; float: right; width: 60%; text-align: center"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm">
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 40%;">Tempo de Peers:</label>
                                        <asp:Label runat="server" ID="labelTempoDePeers" Style="background-color: #F2F2F2; font-size: 18px; list-style-type: none; padding-left: 0px; float: right; width: 60%; text-align: center"></asp:Label>
                                    </div>
                                    <div class="col-sm">
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 40%;">Tempo de cargo:</label>
                                        <asp:Label runat="server" ID="labelTempoDeCargo" Style="background-color: #F2F2F2; font-size: 18px; list-style-type: none; padding-left: 0px; float: right; width: 60%; text-align: center"></asp:Label>
                                    </div>
                                    <div class="col-sm">
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 100%; position: relative; z-index: 1">Projetos envolvidos no semestre:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm" hidden>
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 40%;">Elegível promoção:</label>
                                        <asp:Label runat="server" ID="labelElegivelPromocao" Text="1 ano" Style="background-color: #F2F2F2; font-size: 18px; list-style-type: none; padding-left: 0px; float: right; width: 60%; text-align: center"></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlElegivelPromocao" Style="background-color: #F2F2F2; font-size: 18px; list-style-type: none; padding-left: 0px; float: right; width: 60%; text-indent: 46.5%"
                                            Visible="false" Enabled="false">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm">
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 40%;">Input de promoção:</label>
                                        <asp:DropDownList runat="server" ID="ddlInputPromocao" Style="background-color: #F2F2F2; font-size: 18px; list-style-type: none; padding-left: 0px; float: right; width: 60%; text-indent: 46.5%"
                                            Enabled="false">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm">
                                    </div>
                                    <div class="col-sm">
                                        <asp:Label runat="server" ID="labelProjetosEnvolvidos" Text="Projetos" Style="background-color: #F2F2F2; font-size: 13px; list-style-type: none; padding-left: 0px; float: right; width: 100%; text-align: left; margin-top: -19px; padding-top: 7px"></asp:Label>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-sm">
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 100%; position: relative; z-index: 1; height: 6px">Trajetória do associado no semestre:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm">
                                        <asp:TextBox TextMode="MultiLine" runat="server" ID="textTrajetoria" Style="background-color: #F2F2F2; font-size: 15px; list-style-type: none; padding-left: 7px; float: right; width: 100%; height: 100px; vertical-align: top; text-align: left; border: none; padding-top: 7px"
                                            Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm">
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 100%; position: relative; z-index: 1; height: 6px">Pontos fortes:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm">
                                        <asp:TextBox TextMode="MultiLine" runat="server" ID="textPontosFortes" Style="background-color: #F2F2F2; font-size: 15px; list-style-type: none; padding-left: 7px; float: right; width: 100%; height: 80px; vertical-align: top; text-align: left; border: none; padding-top: 7px"
                                            Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm">
                                        <label style="font-size: 18px; list-style-type: none; padding-left: 0px; float: left; width: 100%; position: relative; z-index: 1; height: 6px">Pontos de atenção:</label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm">
                                        <asp:TextBox TextMode="MultiLine" runat="server" ID="textPontosFracos" Style="background-color: #F2F2F2; font-size: 15px; list-style-type: none; padding-left: 7px; float: right; width: 100%; height: 80px; vertical-align: top; text-align: left; border: none; padding-top: 7px"
                                            Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <br />
                                <div class="row" id="rowMentoriaRealizada" runat="server" hidden>
                                    <label class="switchRH">
                                        <asp:CheckBox ID="cboxMentoriaRealizada" runat="server" Enabled="false" />
                                        <span class="slider round" style="width: 60px; height: 34px;"></span>
                                    </label>
                                    <asp:Label runat="server" ID="labelLiberadoRH" Style="font-size: 18px; text-align: left; font-style: italic; color: dimgray; vertical-align: central; padding-left: 2px"
                                        Text="Mentoria realizada" Enabled="false"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%--CARD PAINEL PBI--%>
        <div class="card" style="text-align:-webkit-center">
            <div class="card-body" style="width:100%">
                <%--<div id="reportContainer_Total" runat="server" style="width: 90%; height: 700px;align-content:center;align-items:center;text-align:center;"></div>--%>
            </div>
        </div>

    </div>

    <div class="modal fade" id="modalRadar" tabindex="-1" role="dialog" aria-labelledby="modalRadarLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalRadarLabel">Radar</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <canvas id="radar_modal"></canvas>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalComplexidade" tabindex="-1" role="dialog" aria-labelledby="modalComplexidadeLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalComplexidadeLabel"></h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label for="message-text" class="col-form-label">Complexidade:</label>
                        <select class="form-control" id="ddlComplexidade"></select>
                    </div>
                </div>
                <div class="modal-footer">
                    <input type="hidden" id="hdIdProjeto" />
                    <button type="button" class="btn btn-primary" id="btnSalvarcomplexidade">Salvar</button>
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <link href="dist/custom/resultado.css" rel="stylesheet" />
    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/libs/Chart.js-2.9.3/Chart.min.js"></script>
    <script src="dist/js/pages/resultado/resultado.js"></script>
    <script src="JS/node_modules/hacktimer/HackTimer.js"></script>
    <script type="text/javascript" language="javascript" src="https://code.jquery.com/jquery-1.12.4.js"></script>
    <script type="text/javascript" language="javascript" src="https://rawgit.com/Microsoft/PowerBI-JavaScript/master/dist/powerbi.min.js"></script>
    <script>
        function CarregarPainelTotal(idAssociado, idPeriodo) {
            var getEmbedToken = "https://pbiesisavalfunctionapp.azurewebsites.net/api/getEmbedToken?code=LE04Pb15fs3Ozj6jFL98Fep7/XESeBGhPHtm1KI6DiOnRGiL7BjhbA==";

            $.ajax({
                url: getEmbedToken,
                jsonpCallback: 'callback',
                contentType: 'application/javascript',
                dataType: "jsonp",
                success: function (json) {

                    var models = window['powerbi-client'].models;

                    var embedConfiguration = {
                        type: 'report',
                        id: json.ReportId,
                        embedUrl: json.EmbedUrl,
                        tokenType: models.TokenType.Embed,
                        accessToken: json.EmbedToken
                    };

                    var $reportContainer = $('#MainContent_reportContainer_Total');
                    var report = powerbi.embed($reportContainer.get(0), embedConfiguration);
                    report.on("loaded", function () {
                        const newSettings = {
                            panes: {
                                filters: {
                                    visible: false
                                }
                            }
                            //navContentPaneEnabled: false
                        };
                        report.updateSettings(newSettings)
                            .catch(error => { console.log(error) });

                        const filterIdAssociado = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESCOMPETENCIAS",
                                column: "IdAssociado"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: idAssociado
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }
                        const filterIdPeriodo = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESCOMPETENCIAS",
                                column: "IdPeriodo"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: idPeriodo
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }
                        const filterIdAssociadoPerf = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESPERFORMANCES",
                                column: "IdAssociado"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: idAssociado
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }
                        const filterIdPeriodoPerf = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESPERFORMANCES",
                                column: "IdPeriodo"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: idPeriodo
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }

                        report.updateFilters(models.FiltersOperations.Add, [filterIdAssociado]);
                        report.updateFilters(models.FiltersOperations.Add, [filterIdPeriodo]);
                        report.updateFilters(models.FiltersOperations.Add, [filterIdAssociadoPerf]);
                        report.updateFilters(models.FiltersOperations.Add, [filterIdPeriodoPerf]);
                    });



                },
                error: function () {
                    alert("Error");
                }
            });

        };
    </script>
    <script>
        function CarregarPainelProjeto(partePainel, idPainel, idAssociado, idPeriodo, pagina) {
            var getEmbedToken = "https://pbiesisavalfunctionapp.azurewebsites.net/api/getEmbedToken?code=LE04Pb15fs3Ozj6jFL98Fep7/XESeBGhPHtm1KI6DiOnRGiL7BjhbA==";

            $.ajax({
                url: getEmbedToken,
                jsonpCallback: 'callback',
                contentType: 'application/javascript',
                dataType: "jsonp",
                success: function (json) {

                    var models = window['powerbi-client'].models;

                    var embedConfiguration = {
                        type: 'report',
                        id: json.ReportId,
                        embedUrl: json.EmbedUrl,
                        tokenType: models.TokenType.Embed,
                        accessToken: json.EmbedToken
                    };

                    var $reportContainer1 = $('#reportContainer_' + partePainel + '_' + idPainel);
                    var report1 = powerbi.embed($reportContainer1.get(0), embedConfiguration);
                    //report.setPage(pagina);
                    report1.on("loaded", function () {
                        const newSettings = {
                            panes: {
                                filters: {
                                    visible: false
                                }
                            }
                            //navContentPaneEnabled: false
                        };
                        report1.updateSettings(newSettings)
                            .catch(error => { console.log(error) });

                        const filterIdAssociado = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESCOMPETENCIAS",
                                column: "IdAssociado"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: idAssociado
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }
                        const filterIdPeriodo = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESCOMPETENCIAS",
                                column: "IdPeriodo"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: idPeriodo
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }
                        const filterIdProjeto = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESCOMPETENCIAS",
                                column: "IdProjeto"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: idPainel
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }
                        const filterIdProjetoPerf = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESPERFORMANCES",
                                column: "IdProjeto"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: idPainel
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }
                        const filterIdAssociadoPerf = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESPERFORMANCES",
                                column: "IdAssociado"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: idAssociado
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }
                        const filterIdPeriodoPerf = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESPERFORMANCES",
                                column: "IdPeriodo"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: idPeriodo
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }
                        const filterIdTipoAvaliacao = {
                            $schema: "http://powerbi.com/product/schema#advanced",
                            target: {
                                table: "AVALIACOESPERFORMANCES",
                                column: "TipoAvaliacao"
                            },
                            logicalOperator: "And",
                            conditions: [
                                {
                                    operator: "Is",
                                    value: "desempenho"
                                }
                            ],
                            filterType: models.FilterType.AdvancedFilter
                        }
                        report1.updateFilters(models.FiltersOperations.Add, [filterIdAssociado]);
                        report1.updateFilters(models.FiltersOperations.Add, [filterIdPeriodo]);
                        report1.updateFilters(models.FiltersOperations.Add, [filterIdProjeto]);
                        report1.updateFilters(models.FiltersOperations.Add, [filterIdProjetoPerf]);
                        report1.updateFilters(models.FiltersOperations.Add, [filterIdAssociadoPerf]);
                        report1.updateFilters(models.FiltersOperations.Add, [filterIdPeriodoPerf]);
                        report1.updateFilters(models.FiltersOperations.Add, [filterIdTipoAvaliacao]);
                    });



                },
                error: function () {
                    alert(message);
                }
            });

        };
    </script>
    <style>
        .scroll-grid > .row {
            display: block;
            overflow-x: auto;
            white-space: nowrap;
        }

            .scroll-grid > .row > .col-md-3 {
                display: inline-block;
            }
    </style>
    <style>
        .hiddenRow {
            padding: 0 !important;
        }
    </style>
    <script>
        let startBtn = document.getElementById('<%= start.ClientID %>');
        let stopBtn = document.getElementById('<%= stop.ClientID %>');
        let resetBtn = document.getElementById('<%= reset.ClientID %>');

        let hour = 00;
        let minute = 00;
        let second = 00;
        let count = 00;

        startBtn.addEventListener('click', function () {
            timer = true;
            stopWatch();
            return false;
        });

        stopBtn.addEventListener('click', function () {
            timer = false;
            return false;
        });

        resetBtn.addEventListener('click', function () {
            timer = false;
            hour = 0;
            minute = 0;
            second = 0;
            count = 0;
            document.getElementById('hr').innerHTML = "00";
            document.getElementById('min').innerHTML = "00";
            document.getElementById('sec').innerHTML = "00";
            document.getElementById('count').innerHTML = "00";
            return false;
        });

        function stopWatch() {
            if (timer) {
                count++;

                if (count == 100) {
                    second++;
                    count = 0;
                }

                if (second == 60) {
                    minute++;
                    second = 0;
                }

                if (minute == 60) {
                    hour++;
                    minute = 0;
                    second = 0;
                }

                let hrString = hour;
                let minString = minute;
                let secString = second;
                let countString = count;

                if (hour < 10) {
                    hrString = "0" + hrString;
                }

                if (minute < 10) {
                    minString = "0" + minString;
                }

                if (second < 10) {
                    secString = "0" + secString;
                }

                if (count < 10) {
                    countString = "0" + countString;
                }

                document.getElementById('hr').innerHTML = hrString;
                document.getElementById('min').innerHTML = minString;
                document.getElementById('sec').innerHTML = secString;
                document.getElementById('count').innerHTML = countString;
                setTimeout(stopWatch, 10);
            }
        }
    </script>
    <style>
        .divExpand{
            resize: both;
            overflow: hidden;
            line-height: 0;
            width: 100px;
            height: 100px;
        }

        .imgExpand{
          width: 100%;
          height: 100%;
        }
    </style>
</asp:Content>

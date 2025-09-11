<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avalizacao_mentor_competencia.aspx.cs" Inherits="SistemaAvaliacao.avalizacao_mentor_competencia" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <asp:HiddenField runat="server" ID="hfJsonRadar" />
    <link rel="stylesheet" href="dist/css/sliderAvaliacao.css">
    <div class="page-breadcrumb border-bottom" style="position:fixed;z-index:2;width:1653px">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Mentor</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item"><a href="index.aspx">Avaliação / Mentor</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Competência</li>
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

        <!-- CARD TITULO -->
        <div class="card col-md-10" style="position:fixed;z-index:1; margin-top:21px;border-color:rgba(0, 0, 0, .25);border-width:1px">
            <div class="card-body">
                <%-- DETALHES DO AVALIADO --%>
                <div class="accordion" id="accordionDetalhes">
                    <div class="card">
                        <div class="card-header" id="headingDetalhes">
                            <h5 class="mb-0">
                                <button class="btn btn-link" type="button" data-toggle="collapse" data-target="#Detalhes" aria-expanded="true" aria-controls="Detalhes">
                                    Detalhes da avaliação
                                </button>
                            </h5>
                        </div>
                        <div id="Detalhes" class="collapse" aria-labelledby="headingDetalhes" data-parent="#accordionDetalhes">
                            <div class="col-md-5">
                                <div class="text-left">
                                    <button type="button" class="btn btn-facebook" disabled>Competência</button>
                                    <a href="avalizacao_mentor_performance.aspx">
                                        <button id="botaoPerformance" runat="server" type="button" class="btn btn-danger">Performance</button>
                                    </a>
                                    <p></p>
                                </div>
                            </div>
                            <div class="col-sm">
                                <div class="row">
                                    <div class="col-md-2">
                                        <label><b>Avaliado</b></label>
                                    </div>
                                    <div class="col-md-4">
                                        <label runat="server" id="lblAssociado"></label>
                                    </div>
                                    <div class="col-md-2">
                                        <label><b>Período</b></label>
                                    </div>
                                    <div class="col-md-4">
                                        <label runat="server" id="lblPeriodo"></label>
                                    </div>
                                    <div class="col-md-1">
                                        <label><b>Gestor</b></label>
                                    </div>
                                    <div class="col-md-3">
                                        <label runat="server" id="lblGestor"></label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-1">
                                        <label><b>Cargo</b></label>
                                    </div>
                                    <div class="col-md-3">
                                        <label runat="server" id="lblCargo"></label>
                                    </div>
                                    <div class="col-md-2">
                                        <label><b>Tempo de Cargo:</b></label>
                                    </div>
                                    <div class="col-md-4">
                                        <label runat="server" id="lblTempoCargo"></label>
                                    </div>
                                    <div class="col-md-2">
                                        <label><b>Tempo de Peers</b></label>
                                    </div>
                                    <div class="col-md-4">
                                        <label runat="server" id="lblTempoPeers"></label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-1">
                                        <label><b>Projeto</b></label>
                                    </div>
                                    <div class="col-md-3">
                                        <label runat="server" id="lblProjeto"></label>
                                    </div>
                                    <div class="col-md-1">
                                        <label><b>Cliente</b></label>
                                    </div>
                                    <div class="col-md-3">
                                        <label runat="server" id="lblCliente"></label>
                                    </div>
                                    <div class="col-md-2">
                                        <label><b>Tempo restante</b></label>
                                    </div>
                                    <div class="col-md-2">
                                        <label runat="server" id="lblTempo"></label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- DIV DESCRIÇÕES -->
                <div class="accordion" id="accordionCabecalho">
                    <div class="card">
                        <div class="card-header" id="headingCabecalho">
                            <h5 class="mb-0">
                                <button class="btn btn-link" type="button" data-toggle="collapse" data-target="#Cabecalho" aria-expanded="true" aria-controls="Cabecalho">
                                    Descrições nível atual e próximo nível
                                </button>
                            </h5>
                        </div>
                        <div id="Cabecalho" class="collapse" aria-labelledby="headingCabecalho" data-parent="#accordionCabecalho">
                            <table class="table table-striped border" style="width:100%;table-layout:fixed">
                                <thead>
                                    <tr>
                                        <th colspan="1"></th>
                                        <th colspan="2">Cargo</th>
                                        <th colspan="2">Função</th>
                                        <th colspan="2">Autonomia</th>
                                        <th colspan="2">Escopo de Atuação</th>                                
                                        <th colspan="2">Nível de interlocução principal no cliente</th>
                                        <asp:Repeater runat="server" ID="rpt_TabelaCabecalho_Titulos">
                                            <ItemTemplate>
                                                <th colspan="2"><%# Eval("TituloExibicao") %></th>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="1">Nível atual</td>
                                        <td colspan="2"><asp:Label runat="server" ID="text_TabelaCabecalho_CargoAtual"></asp:Label></td>
                                        <td colspan="2"><asp:Label runat="server" ID="text_TabelaCabecalho_FuncaoAtual"></asp:Label></td>
                                        <td colspan="2"><asp:Label runat="server" ID="text_TabelaCabecalho_AutonomiaAtual"></asp:Label></td>
                                        <td colspan="2"><asp:Label runat="server" ID="text_TabelaCabecalho_EscopoAtual"></asp:Label></td>
                                        <td colspan="2"><asp:Label runat="server" ID="text_TabelaCabecalho_InterlocucaoAtual"></asp:Label></td>
                                        <asp:Repeater runat="server" ID="rpt_TabelaCabecalho_CargoAtual">
                                            <ItemTemplate>
                                                <th colspan="2" style="font-weight:normal;"><%# Eval("DescricaoCompetencia_Atual") %></th>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tr>
                                    <tr>
                                        <td colspan="1">Próximo nível</td>
                                        <td colspan="2"><asp:Label runat="server" ID="text_TabelaCabecalho_CargoProximo"></asp:Label></td>
                                        <td colspan="2"><asp:Label runat="server" ID="text_TabelaCabecalho_FuncaoProximo"></asp:Label></td>
                                        <td colspan="2"><asp:Label runat="server" ID="text_TabelaCabecalho_AutonomiaProximo"></asp:Label></td>
                                        <td colspan="2"><asp:Label runat="server" ID="text_TabelaCabecalho_EscopoProximo"></asp:Label></td>
                                        <td colspan="2"><asp:Label runat="server" ID="text_TabelaCabecalho_InterlocucaoProximo"></asp:Label></td>
                                        <asp:Repeater runat="server" ID="rpt_TabelaCabecalho_CargoProximo">
                                            <ItemTemplate>
                                                <th colspan="2" style="font-weight:normal;"><%# Eval("DescricaoCompetencia_Proximo") %></th>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                       
            </div>
        </div>

        <%--CARD COMPETENCIAS--%>
        <div class="card col-md-12" style="margin-top:220px">
            <div class="card-body">

                <%--ACCORDION CONSIDERAÇÕES MENTOR--%>
                <div class="card" style="margin-bottom:1px" runat="server" id="accordionConsideracoesMentor">
                    <div class="card-header" id="headingConsideracoesMentor">
                        <h5 class="mb-0">
                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#consideracoes" aria-expanded="false" aria-controls="collapseThree">
                                Considerações do Mentor
                            </button>
                        </h5>
                    </div>
                    <div id="consideracoes" class="collapse show" aria-labelledby="headingConsideracoesMentor" data-parent="#headingConsideracoesMentor">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Associado Avaliado:</label>
                                    <asp:Label runat="server" id="labelAssociado" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Cargo:</label>
                                    <asp:Label runat="server" id="labelCargo" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Vertical:</label>
                                    <asp:Label runat="server" id="labelVertical" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Tempo de Peers:</label>
                                    <asp:Label runat="server" id="labelTempoDePeers" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Tempo de cargo:</label>
                                    <asp:Label runat="server" id="labelTempoDeCargo" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 100%;position:relative;z-index:1">Projetos envolvidos no semestre:</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm" hidden>
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Elegível promoção:</label>
                                    <asp:Label runat="server" id="labelElegivelPromocao" Text="1 ano" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center" Visible="false"></asp:Label>
                                    <asp:DropDownList runat="server" id="ddlElegivelPromocao" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-indent:46.5%" ></asp:DropDownList>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Input de promoção:</label>
                                    <asp:DropDownList runat="server" id="ddlInputPromocao" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-indent:46.5%"></asp:DropDownList>
                                </div>
                                <div class="col-sm">
                                    </div>
                                <div class="col-sm">
                                    <asp:Label runat="server" id="labelProjetosEnvolvidos" Text="Projetos" style="background-color:#F2F2F2;font-size:13px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 100%;text-align:left;margin-top:-19px;padding-top:7px"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 100%;position:relative;z-index:1;height:6px">Trajetória do associado no semestre:</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <asp:TextBox TextMode="MultiLine" runat="server" id="textTrajetoria" style="background-color:#F2F2F2;font-size:15px; list-style-type: none;
                                        padding-left: 7px;float:right;width: 100%;height: 100px;vertical-align:top;text-align:left;border:none;padding-top:7px"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 100%;position:relative;z-index:1;height:6px">Pontos fortes:</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <asp:TextBox TextMode="MultiLine" runat="server" id="textPontosFortes" style="background-color:#F2F2F2;font-size:15px; list-style-type: none;
                                        padding-left: 7px;float:right;width: 100%;height: 80px;vertical-align:top;text-align:left;border:none;padding-top:7px"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 100%;position:relative;z-index:1;height:6px">Pontos de atenção:</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <asp:TextBox TextMode="MultiLine" runat="server" id="textPontosFracos" style="background-color:#F2F2F2;font-size:15px; list-style-type: none;
                                        padding-left: 7px;float:right;width: 100%;height: 80px;vertical-align:top;text-align:left;border:none;padding-top:7px"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                            <div class="row" id="rowMentoriaRealizada" runat="server">
                                <label class="switchRH">
                                    <asp:CheckBox id="cboxMentoriaRealizada" runat="server" />
                                    <span class="slider round" style="width: 60px; height: 34px;"></span>
                                </label>
                                <asp:Label runat="server" id="labelLiberadoRH" style="font-size:18px; text-align:left;font-style:italic;color:dimgray;vertical-align:central;padding-left:2px"
                                    Text="Mentoria realizada"></asp:Label>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <asp:Button runat="server" id="btnSalvar" class="btn btn-danger" Text="Salvar considerações" style="font-size:18px; list-style-type: none;
                                        padding-left: 0px;float: left;width: 17%;vertical-align: top;text-align: center" OnClick="btnSalvar_Click"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <%--ACCORDION DADOS DO RH--%>
                <div class="card" style="margin-bottom:1px" id="accordionDadosRH" runat="server">
                    <div class="card-header" id="headindDadosRH">
                        <h5 class="mb-0">
                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#dadosRH" aria-expanded="false" aria-controls="collapseThree">
                                Dados do RH
                            </button>
                        </h5>
                    </div>
                    <div id="dadosRH" class="collapse show" aria-labelledby="headindDadosRH" data-parent="#headindDadosRH">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Ação comitê:</label>
                                    <asp:Label runat="server" id="labelAcaoComite" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center" Text="Promover/Não Alterar/Desligar"></asp:Label>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Próximo cargo:</label>
                                    <asp:Label runat="server" id="labelProximoCargo" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center" Text="Proximo Cargo"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Salário atual:</label>
                                    <asp:Label runat="server" id="labelSalarioAtual" Text="R$ 0,00" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Próximo salário:</label>
                                    <asp:Label runat="server" id="labelProximoSalario" Text="R$ 0,00" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Incremento(%):</label>
                                    <asp:Label runat="server" id="labelIncremento" Text="0%" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Regime de contratação atual:</label>
                                    <asp:Label runat="server" id="labelRegimeContratacaoAtual" Text="CLT" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Regime de contratação novo:</label>
                                    <asp:Label runat="server" id="labelRegimeContratacaoNovo" Text="CLT" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 100%;position:relative;z-index:1;height:6px">Pontos Fortes:</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <asp:Label TextMode="MultiLine" runat="server" id="labelPontosFortesRH" style="background-color:#F2F2F2;font-size:15px; list-style-type: none;
                                        padding-left: 7px;float:right;width: 100%;height: 100px;vertical-align:top;text-align:left;border:none;padding-top:7px"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 100%;position:relative;z-index:1;height:6px">Pontos de atenção:</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <asp:Label TextMode="MultiLine" runat="server" id="labelPontosFracosRH" style="background-color:#F2F2F2;font-size:15px; list-style-type: none;
                                        padding-left: 7px;float:right;width: 100%;height: 100px;vertical-align:top;text-align:left;border:none;padding-top:7px"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <%--ACCORDION RADAR--%>
                <div class="accordion" id="accordionCompetencia" runat="server">
                    <div class="card">
                        <div class="card-header" id="headingRadar">
                            <h5 class="mb-0">
                                <button class="btn btn-link" type="button" data-toggle="collapse" data-target="#Radar" aria-expanded="true" aria-controls="Radar">
                                    Radar
                                </button>
                            </h5>
                        </div>
                        <div id="Radar" class="collapse" aria-labelledby="headingRadar" data-parent="#headingRadar">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-12">
                                        <canvas id="radarprojeto" height="110"></canvas>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <%--ACCORDION RESULTADOS--%>
                <div class="accordion" id="accordionResultados" runat="server">
                    <div class="card">
                        <div class="card-header" id="headingResultados">
                            <h5 class="mb-0">
                                <button class="btn btn-link" type="button" data-toggle="collapse" data-target="#Resultados" aria-expanded="true" aria-controls="Resultados">
                                    Resultados do Semestre
                                </button>
                            </h5>
                        </div>
                        <div id="Resultados" class="collapse" aria-labelledby="headingResultados" data-parent="#headingResultados">
                            <div class="card-body resultado-group scroll-grid">
                                    <div class="row">
                                        <asp:Repeater ID="rptProjetos" runat="server">
                                            <ItemTemplate>
                                                <div class="col-md-3">
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
                                                                        <td class="text-center"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"NotaCompetencialNivel1")) %></td>
                                                                    </tr>
                                                                    <asp:Repeater ID="rptCompetenciasN1" runat="server">
                                                                        <ItemTemplate>
                                                                            <tr class="hidelinha">
                                                                                <td><%# Eval("Eixo") %></td>
                                                                                <td class="text-center"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"PercentualNotaFinalNivel1")) %></td>
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
                                                                        <td class="text-center"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"NotaCompetencialNivel2")) %></td>
                                                                    </tr>
                                                                    <asp:Repeater ID="rptCompetenciasN2" runat="server">
                                                                        <ItemTemplate>
                                                                            <tr class="hidelinha">
                                                                                <td><%# Eval("Eixo") %></td>
                                                                                <td class="text-center"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"PercentualNotaFinalNivel2")) %></td>
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
                                                                        <td class="text-center" style="width: 30%"><%# FormatDecimal((decimal)DataBinder.Eval(Container.DataItem,"SomaPerfomance")) %></td>
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
                                                                        <td class="text-center" style="width: 30%"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"SomaNotaCompetencialN1N2")) %></td>
                                                                    </tr>
                                                                    <asp:Repeater ID="rptSomaCompetencias" runat="server">
                                                                        <ItemTemplate>
                                                                            <tr class="hidelinha">
                                                                                <td><%# Eval("Eixo") %></td>
                                                                                <td class="text-center"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"PercentualSomaNotaFinalN1N2")) %></td>
                                                                            </tr>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </tbody>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <div class="col-md-3">
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
                                                                        <td class="text-center"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"CompetenciaCargo")) %></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="tdBranco" colspan="2">&nbsp;</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="tdresultado">Competência Cargo Acima</td>
                                                                        <td class="text-center"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"CompetenciaProximoCargo")) %></td>
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
                                                                        <td class="text-center" style="width: 30%"><%# FormatDecimal((decimal)DataBinder.Eval(Container.DataItem,"SomaProjetosPerfomance")) %></td>
                                                                    </tr>
                                                                    <asp:Repeater ID="rptSomaProjetosItems" runat="server">
                                                                        <ItemTemplate>
                                                                            <tr class="hidelinha">
                                                                                <td><%# Eval("Perfomance") %></td>
                                                                                <td class="text-center"><%# FormatDecimal((decimal)DataBinder.Eval(Container.DataItem,"NotaPerfomancePonderada")) %></td>
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
                                                                        <td class="text-center" style="width: 30%"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"SomaProjetosNotaCompetencialN1N2")) %></td>
                                                                    </tr>
                                                                    <asp:Repeater ID="rptProjetosSomaCompetencias" runat="server">
                                                                        <ItemTemplate>
                                                                            <tr class="hidelinha">
                                                                                <td><%# Eval("Eixo") %></td>
                                                                                <td class="text-center"><%# FormatPercentagem((decimal)DataBinder.Eval(Container.DataItem,"NotaProjetosCompetencia")) %></td>
                                                                            </tr>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
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
                    </div>
                </div>

                <%--ACCORDION COMPETENCIAS--%>
                <div class="card" style="margin-bottom:1px" id="divCompetencias" runat="server">
                    <div class="card-header" id="headingCompetencia">
                        <h5 class="mb-0">
                            <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#competencias" aria-expanded="false" aria-controls="collapseTwo">
                                Competências
                            </button>
                        </h5>
                    </div>
                    <div id="competencias" class="collapse show" aria-labelledby="headingCompetencia" data-parent="#headingCompetencia">
                        <div class="card-body">

                            <!-- DIV TABELA COMPETENCIAS - OLD -->
                            <div class="table-responsive">
                                <table class="table table-striped border">
                                    <thead>
                                        <tr>
                                            <th colspan="5" style="color:#021240;background-color:#e5e019">Nível 1</th>
                                            <th colspan="6" style="color:#021240;background-color:#e5e019">Nível 2</th>
                                        </tr>
                                        <tr>
                                            <th style="color:#021240;background-color:#eae678">Detalhamento Cargo Atual</th>
                                            <th style="color:#021240;background-color:#eae678">Nota Avaliado</th>
                                            <th style="color:#021240;background-color:#eae678">Nota às Cegas</th>
                                            <th style="color:#021240;background-color:#eae678">Nota Gestor</th>
                                            <th style="color:#021240;background-color:#eae678">Nota Feedback</th>

                                            <th runat="server" id="tableDetalheNivel2" style="color:#021240;background-color:#eae678">
                                                <asp:Label ID="lblDetalheNivel2" runat="server" Visible="false" Text="Próximo Nível"></asp:Label>
                                            </th>
                                            <th style="color:#021240;background-color:#eae678">Nota Avaliado</th>
                                            <th style="color:#021240;background-color:#eae678">Nota às Cegas</th>
                                            <th style="color:#021240;background-color:#eae678">Nota Gestor</th>
                                            <th style="color:#021240;background-color:#eae678">Nota Feedback</th>
                                            <th style="color:#021240;background-color:#eae678">Obs.</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptCompetencias" runat="server" OnItemDataBound="rptCompetencias_ItemDataBound">
                                            <ItemTemplate>
                                                <tr <%# Eval("IsHidden") %>>
                                                    <th hidden colspan="2">Competência: <%# DataBinder.Eval(Container.DataItem, "Eixo") %>
                                                    </th>
                                                    <th style="font-style:normal;font-size:21px;font-weight:normal;align-content:center;text-align:center;"
                                                         colspan="11"><%# DataBinder.Eval(Container.DataItem, "SubCompetencia") %>
                                                    </th>
                                                    <th hidden colspan="6">Dimensão: <%# DataBinder.Eval(Container.DataItem, "Dimensao") %>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <asp:HiddenField runat="server" ID="IdCompetenciaItem" Value='<%# Eval("IdCompetencia") %>' />
                                                    <td style="border-top:none;background-color:#f9f9f9">
                                                        <a href="#det_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>"
                                                            data-target="#det_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>" data-toggle="collapse"
                                                            class="accordion-toggle"><%# TruncarTexto((string)DataBinder.Eval(Container.DataItem, "DetalheNivelAtual"),50) %>
                                                            <span style="font-weight:bold">  Ver+</span></a>
                                                        </a>
                                                    </td>
                                                    <td runat="server" id="lblNota1Avaliado" style="border-top:none;background-color:#f9f9f9"></td>
                                                    <td runat="server" id="lblNota1Cegas" style="border-top:none;background-color:#f9f9f9"></td>
                                                    <td runat="server" id="lblNota1Gestor" style="border-top:none;background-color:#f9f9f9"></td>
                                                    <td colspan="2" runat="server" id="lblNota1Feedback" style="border-top:none;background-color:#f9f9f9"></td>
                                                    <td class="text-center" style="border-top:none;background-color:#f9f9f9" hidden>
                                                        <button type="button" data-toggle="collapse" data-target="#nivel_<%# Eval("IdCompetencia") %>"
                                                            class="accordion-toggle"><span class="fas fa-align-justify" aria-hidden="true"></span></button>
                                                    </td>
                                                    <td runat="server" id="lblNota2Avaliado" style="border-top:none;background-color:#f9f9f9"></td>
                                                    <td runat="server" id="lblNota2Cegas" style="border-top:none;background-color:#f9f9f9"></td>
                                                    <td runat="server" id="lblNota2Gestor" style="border-top:none;background-color:#f9f9f9"></td>
                                                    <td runat="server" id="lblNota2Feedback" style="border-top:none;background-color:#f9f9f9"></td>
                                                    <td style="border-top:none;background-color:#f9f9f9">
                                                        <button type="button" data-toggle="collapse" data-target="#obs_<%# Eval("IdCompetencia") %>" class="accordion-toggle" 
                                                            style="background-color:#003150;padding:8px 15px;border:none">
                                                            <span aria-hidden="true" style="color:white;font-size:15px">Considerações</span>
                                                        </button>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="11" class="hiddenRow">
                                                        <div class="accordian-body collapse" id="det_<%# Eval("IdCompetencia") %>">
                                                            <p>
                                                                <strong>Detalhamento Cargo Atual</strong>
                                                                </br>
                                                                <%# DataBinder.Eval(Container.DataItem, "DetalheNivelAtual") %>
                                                            </p>
                                                            <p hidden>
                                                                <strong>Cargo Atual</strong>
                                                                </br>
                                                                <%# DataBinder.Eval(Container.DataItem, "CompetenciaAtual") %>
                                                            </p>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="11" class="hiddenRow">
                                                        <div class="accordian-body collapse" id="nivel_<%# Eval("IdCompetencia") %>">
                                                            <p>
                                                                <strong>Detalhamento Próximo Nível</strong>
                                                                </br>
                                                                <%# DataBinder.Eval(Container.DataItem, "DetalheProximoNivel") %>
                                                            </p>
                                                            <p hidden>
                                                                <strong>Próximo Cargo</strong>
                                                                </br>
                                                                <%# DataBinder.Eval(Container.DataItem, "CompetenciaProximo") %>
                                                            </p>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="11" class="hiddenRow">
                                                        <div class="accordian-body collapse" id="obs_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>">
                                                            <p>
                                                                <strong>Observações do Avaliado:</strong><br>
                                                                <asp:Label ID="lblObservacaoAvaliado" runat="server"></asp:Label>
                                                            </p>
                                                            <p>
                                                                <strong>Observações Feedback:</strong><br>
                                                                <asp:Label ID="lblObservacaoGestor" runat="server"></asp:Label>
                                                            </p>
                                                        </div>
                                                    </td>
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

            <%--DIV BOTÕES ADICIONAIS--%>
            <div class="card">
                <div class="card-body">
                        <div class="col-md-5">
                        <div class="text-left">
                            <button type="button" class="btn btn-facebook" disabled>Competência</button>
                            <a href="avalizacao_mentor_performance.aspx">
                                <button id="btn_Performance_2" runat="server" type="button" class="btn btn-danger">Performance</button>
                            </a>
                        </div>
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
    </div>
    <!-- ============================================================== -->
    <!-- End Container fluid  -->
    <!-- ============================================================== -->

    <link href="dist/custom/resultado.css" rel="stylesheet" />
    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/libs/Chart.js-2.9.3/Chart.min.js"></script>
    <script src="dist/js/pages/feedback/feedback_competencia.js"></script>
    <script src="dist/js/pages/resultado/resultado.js"></script>
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

</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avalizacao_feedback_competencia.aspx.cs" Inherits="SistemaAvaliacao.avalizacao_feedback_competencia" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <asp:HiddenField runat="server" ID="hfJsonRadar" />
    <asp:HiddenField runat="server" ID="hfJsonRadarEvolucao" />
    <div class="page-breadcrumb border-bottom" style="position:fixed;z-index:1;width:1653px">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Feedback</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item"><a href="index.aspx">Avaliação / Feedback</a></li>
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
    <div class="page-content container-fluid" style ="overflow:initial !important">
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
                                    <asp:Button ID="btnIrPerformance" runat="server" Text="Performance" class="btn btn-danger" OnClick="btnIrPerformance_Click" />
                                    <asp:Button ID="btnFinalizar" runat="server" Text="Ir para Finalização" class="btn btn-info" OnClick="btnFinalizar_Click" />
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
                                </div>
                                <div class="row">
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
                <div class="accordion" id="accordionCompetencia">

                    <%--RADAR COMPETEÊNCIAS--%>
                    <div class="card">
                        <div class="card-header" id="headingRadar">
                            <h5 class="mb-0">
                                <button class="btn btn-link" type="button" data-toggle="collapse" data-target="#Radar" aria-expanded="true" aria-controls="Radar">
                                    Radar
                                </button>
                            </h5>
                        </div>

                        <div id="Radar" class="collapse" aria-labelledby="headingRadar" data-parent="#accordionCompetencia">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-12">

                                        <ul class="nav nav-pills mb-3" id="pills-tab" role="tablist" style="gap:15px">
                                            <asp:Repeater runat="server" ID="rptPills">
                                                <ItemTemplate>
                                                    <li class="nav-item">
                                                        <a class="nav-link btn btn-facebook <%#Eval("active") %>" id="<%#Eval("id") %>" data-toggle="pill" href="<%#Eval("href") %>" role="tab"
                                                            aria-controls="<%#Eval("ariacontrols") %>" aria-selected="<%#Eval("ariaselected") %>"><%#Eval("Periodo") %>
                                                        </a>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <li class="nav-item">
                                                <a class="nav-link active btn btn-danger" id="tab-projeto-tab" data-toggle="pill" href="#tab-projeto" role="tab"
                                                    aria-controls="tab-projeto" aria-selected="true"><label runat="server" id="lblPillPeriodoAtual" style="line-height:1px"></label></a>
                                            </li>
                                            <li class="nav-item">
                                                <a class="nav-link btn btn-danger" id="tab-evolucao-tab" data-toggle="pill" href="#tab-evolucao" role="tab"
                                                    aria-controls="tab-evolucao" aria-selected="false">Evolução</a>
                                            </li>
                                        </ul>
                                        
                                        <div class="tab-content" id="pills-tabContent">
                                            <asp:Repeater runat="server" ID="rptPillsRadares" OnItemCreated="rptPillsRadares_ItemCreated">
                                                <ItemTemplate>
                                                    <div class="tab-pane fade" id="<%#Eval("id") %>" role="tabpanel" aria-labelledby="<%#Eval("arialabelled") %>">
                                                        <canvas id="radar<%#Eval("idPeriodo") %>" height="110" style="max-height:536px" ></canvas>
                                                        <label id="radd<%#Eval("idPeriodo") %>"></label>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <div class="tab-pane fade active in show" id="tab-projeto" role="tabpanel" aria-labelledby="tab-projeto-tab">
                                                <canvas id="radarprojeto" height="110" style="max-height:536px"></canvas>
                                            </div>
                                            <div class="tab-pane fade" id="tab-evolucao" role="tabpanel" aria-labelledby="tab-evolucao-tab">
                                                <canvas id="radarevolucao" height="110" style="max-height:536px"></canvas>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <%--LISTA COMPETÊNCIAS--%>
                    <div class="card">
                        <div class="card-header" id="headingCompetencia">
                            <h5 class="mb-0">
                                <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#competencias" aria-expanded="false" aria-controls="collapseTwo">
                                    Feedback - Competências
                                </button>
                            </h5>
                        </div>
                        <div id="competencias" class="collapse show" aria-labelledby="headingCompetencia" data-parent="#accordionCompetencia">
                            <div class="card-body">

                                <%--DIV BOTÃO SALVAR--%>
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="text-right">
                                                <asp:Button ID="btnSalvar" runat="server" Text="Salvar Avaliação" class="btn btn-dark" OnClick="btnSalvar_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <!-- DIV TABELA COMPETENCIAS - NOVA -->
                                <div class="table-responsive" style="overflow-y:auto !important; height:600px">
                                    <table class="table table-striped border" style="width:100%;table-layout:fixed">
                                        <thead>
                                            <tr>
                                                <th colspan="8" style="background-color:#e5e019;color:#021240;position:sticky !important; top:-5px !important">Nível 1</th>
                                                <th colspan="10" style="background-color:#e5e019;color:#021240;position:sticky !important; top:-5px !important">Nível 2</th>
                                            </tr>
                                            <tr>
                                                <th colspan="4" style="color:#021240;background-color:#eae678;position:sticky !important; top:45px !important">Detalhamento Atual</th>
                                                <th hidden>Nivel Atual</th>
                                                <th colspan="1" id="head_NotaAvaliado" runat="server" style="color:#021240;background-color:#eae678;position:sticky !important; top:45px !important">Nota Avaliado</th>
                                                <th colspan="1" style="color:#021240;background-color:#eae678;position:sticky !important; top:45px !important">Nota Gestor</th>
                                                <th colspan="2" style="color:#021240;background-color:#eae678;position:sticky !important; top:45px !important">Feedback</th>
                                                <th runat="server" id="tableDetalheNivel2" colspan="4" style="color:#021240;background-color:#eae678;position:sticky !important; top:45px !important"><asp:Label ID="lblDetalheNivel2" runat="server" Text="Detalhamento Próximo Nível"></asp:Label></th>                                
                                                <th hidden>Próximo Nivel</th>
                                                <th colspan="1" style="color:#021240;background-color:#eae678;position:sticky !important; top:45px !important">Nota Avaliado</th>
                                                <th colspan="1" id="head_NotaGestor2" runat="server" style="color:#021240;background-color:#eae678;position:sticky !important; top:45px !important">Nota Gestor</th>
                                                <th colspan="2" id="head_Feedback" runat="server" style="color:#021240;background-color:#eae678;position:sticky !important; top:45px !important">Feedback</th>
                                                <th colspan="2" style="color:#021240;background-color:#eae678;position:sticky !important; top:45px !important">Considerações</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="rptCompetencias" runat="server" OnItemDataBound="rptCompetencias_ItemDataBound">
                                                <ItemTemplate>
                                                    <tr <%# Eval("IsHidden") %> style="font-style:italic;font-size:14px;font-weight:normal;background-color:#f2f2f2;vertical-align:bottom">
                                                        <%--<th colspan="2" style="font-style:italic;font-size:13px;font-weight:normal">
                                                            Eixo: <%# DataBinder.Eval(Container.DataItem, "Eixo") %>
                                                        </th>--%>
                                                        <th colspan="18" style="font-style:normal;font-size:21px;font-weight:normal;align-content:center;text-align:center;border-top:5px solid;border-color:#003150">   
                                                            <%# DataBinder.Eval(Container.DataItem, "SubCompetencia") %>
                                                        </th>
                                                        <%--<th colspan="2" style="font-style:italic;font-size:13px;font-weight:normal">
                                                            Dimensão: <%# DataBinder.Eval(Container.DataItem, "Dimensao") %>
                                                        </th>--%>
                                                        <%--<th colspan="10">&nbsp;</th>--%>
                                                    </tr>
                                                    <tr style="font-weight:bold;background-color:#f5f5f5;border-top:none">
                                                        <%--PALAVRAS CHAVE--%>
                                                        <td colspan="18" style="text-align:center;border-top:none"><%# DataBinder.Eval(Container.DataItem, "PalavrasChave") %></td>
                                                    </tr>
                                                    <tr style="font-weight:normal;background-color:#f9f9f9;border-top:none">
                                                        <td colspan="4" style="border-top:none;background-color:#f9f9f9">
                                                            <%--HIDDEN ID COMPETÊNCIA--%>
                                                            <asp:HiddenField runat="server" ID="IdCompetenciaItem" Value='<%# Eval("IdCompetencia") %>' /> 

                                                            <%--TEXTO/ACCORDION DETALHE NÍVEL ATUAL--%>
                                                            <a href="#det_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>"
                                                                data-target="#det_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>" data-toggle="collapse" 
                                                                class="accordion-toggle"><%# TruncarTexto((string)DataBinder.Eval(Container.DataItem, "DetalheNivelAtual"),70) %>
                                                                <span style="font-weight:bold">  Ver+</span>
                                                            </a>
                                                        </td>

                                                        <%--ACCORDION TÍTULO NÍVEL ATUAL--%>
                                                        <td hidden colspan="1" style="border-top:none;background-color:#f9f9f9">
                                                            <button type="button" data-toggle="collapse" 
                                                                data-target="#nivel_<%# Eval("IdCompetencia") %>" class="accordion-toggle">
                                                                <span class="fas fa-align-justify" aria-hidden="true"></span>
                                                            </button>
                                                        </td>
                                        
                                                        <%--LABELS RESPOSTAS ANTERIORES - NIVEL ATUAL--%>
                                                        <td colspan="1" runat="server" id="lblNota1Avaliado" style="border-top:none;background-color:#f9f9f9"></td>
                                                        <td colspan="1" runat="server" id="lblNota1Gestor" style="border-top:none;background-color:#f9f9f9"></td>

                                                        <%--COMBOBOX RESPOSTA NÍVEL ATUAL--%>
                                                        <td colspan="2" style="border-top:none;background-color:#f9f9f9">
                                                            <div <%# Eval("disableSelectNivel1") %> <%# Eval("hiddenSelectNivel1") %>>
                                                                <select id="ddlNotaNivel1" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                                            </div>
                                                            <div <%# Eval("hiddenTextBoxNivel1") %>>
                                                                <label id="txtNotaNivel1" runat="server" class="form-control p-0" style="width: 100%; height: 100%; background-color:#E9ECEF"><%# Eval("textoNotaNivel1") %></label>
                                                            </div>
                                                        </td>

                                                        <%--TEXTO/ACCORDION DETALHE PRÓXIMO NÍVEL--%>
                                                        <td colspan="4" style="border-top:none;background-color:#f9f9f9">
                                                            <a href="#detPrNivel_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>" 
                                                                data-target="#detPrNivel_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>" data-toggle="collapse"  class="accordion-toggle">
                                                                <%# TruncarTexto((string)DataBinder.Eval(Container.DataItem, "DetalheProximoNivel"),70) %>
                                                                <span style="font-weight:bold">  Ver+</span>
                                                            </a>
                                                        </td>

                                                        <%--ACCORDION TÍTULO PRÓXIMO NÍVEL--%>
                                                        <td hidden colspan="1" style="border-top:none;background-color:#f9f9f9">
                                                            <button type="button" data-toggle="collapse" data-target="#prNivel_<%# Eval("IdCompetencia") %>" class="accordion-toggle">
                                                                <span class="fas fa-align-justify" aria-hidden="true"></span>
                                                            </button>
                                                        </td>
                                        
                                                        <%--LABELS RESPOSTAS ANTERIORES - PRÓXIMO NIVEL--%>
                                                        <td colspan="1" runat="server" id="lblNota2Avaliado" style="border-top:none;background-color:#f9f9f9"></td>
                                                        <td colspan="1" runat="server" id="lblNota2Gestor" style="border-top:none;background-color:#f9f9f9"></td>

                                                        <%--COMBOBOX RESPOSTA PRÓXIMO NÍVEL--%>
                                                        <td colspan="2" style="border-top:none;background-color:#f9f9f9">
                                                            <div <%# Eval("disableSelectNivel2") %> <%# Eval("hiddenSelectNivel2") %>>
                                                                <select id="ddlNotaNivel2" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                                            </div>
                                                            <div <%# Eval("hiddenTextBoxNivel2") %>>
                                                                <label id="txtNotaNivel2" runat="server" class="form-control p-0" style="width: 100%; height: 100%; background-color:#E9ECEF"><%# Eval("textoNotaNivel2") %></label>
                                                            </div>
                                                        </td>
                                        
                                                        <%--ACCORDION CONSIDERAÇÕES--%>
                                                        <td colspan="2" style="border-top:none;background-color:#f9f9f9;text-align:center">
                                                            <button type="button" data-toggle="collapse" data-target="#cons_<%# Eval("IdCompetencia") %>" class="accordion-toggle" 
                                                                style="background-color:#003150;padding:8px 15px;border:none">
                                                                <span aria-hidden="true" style="color:white;font-size:15px">Considerações</span>
                                                            </button>
                                                        </td>
                                                    </tr>

                                                    <%--DETALHAMENTO ATUAL--%>
                                                    <tr>
                                                        <td colspan="18" class="hiddenRow" style="border-top:none;background-color:#ffffff">
                                                            <div class="accordian-body collapse" id="det_<%# Eval("IdCompetencia") %>">
                                                                <strong>Detalhamento Cargo Atual:</strong><br /><%# DataBinder.Eval(Container.DataItem, "DetalheNivelAtual") %>
                                                            </div>
                                                        </td>
                                                    </tr>

                                                    <%--TÍTULO NIVEL ATUAL--%>
                                                    <tr hidden>
                                                        <td colspan="18" class="hiddenRow" style="border-top:none;background-color:#ffffff">
                                                            <div class="accordian-body collapse" id="nivel_<%# Eval("IdCompetencia") %>">
                                                                <strong>Nivel Atual:</strong></br>
                                                                <%# DataBinder.Eval(Container.DataItem, "CompetenciaAtual") %></div>
                                                        </td>
                                                    </tr>

                                                    <%--DETALHAMENTO DO PRÓXIMO NÍVEL--%>
                                                    <tr>
                                                        <td colspan="18" class="hiddenRow" style="border-top:none;background-color:#ffffff">
                                                            <div class="accordian-body collapse" id="detPrNivel_<%# Eval("IdCompetencia") %>">
                                                                <strong>Detalhamento Próximo Nível:</strong></br>
                                                                <%# DataBinder.Eval(Container.DataItem, "DetalheProximoNivel") %></</div>
                                                        </td>
                                                    </tr>

                                                    <%--TÍTULO PRÓXIMO NÍVEL--%>
                                                    <tr hidden>
                                                        <td colspan="18" class="hiddenRow">
                                                            <div class="accordian-body collapse" id="prNivel_<%# Eval("IdCompetencia") %>">
                                                                <strong>Próximo Nivel:</strong></br>
                                                                <%# DataBinder.Eval(Container.DataItem, "CompetenciaProximo") %></div>
                                                        </td>
                                                    </tr>

                                                    <%--CONSIDERAÇÕES--%>
                                                    <tr>
                                                        <td colspan="18" class="hiddenRow">
                                                            <div class="accordian-body collapse" id="cons_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>">
                                                                <p id="pObsAvaliado" runat="server" ClientIDMode="static">
                                                                    <strong>Observações do avaliado:</strong><br>
                                                                    <asp:Label ID="lblObservacaoAvaliado" runat="server"></asp:Label>
                                                                </p>
                                                                <p id="pObsGestor" runat="server" ClientIDMode="static">
                                                                    <strong>Observações do gestor:</strong><br>
                                                                    <asp:Label ID="lblObservacaoGestor" runat="server"></asp:Label>                                                    
                                                                </p>
                                                                <div class="accordian-body">
                                                                    <strong>Considerações do feedback:</strong></br>
                                                                    <textarea id="txtNivel1" runat="server" rows="5" class="form-control"></textarea>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </div>

                                <%--DIV BOTÃO SALVAR--%>
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="text-right">
                                                <asp:Button ID="btnSalvar2" runat="server" Text="Salvar Avaliação" class="btn btn-dark" OnClick="btnSalvar_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
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
                        <asp:Button ID="Button1" runat="server" Text="Performance" class="btn btn-danger" OnClick="btnIrPerformance_Click" />
                        <asp:Button ID="btn_Finalizar_2" runat="server" Text="Ir para Finalização" class="btn btn-info" OnClick="btnFinalizar_Click" />
                        <p></p>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <link href="dist/custom/consolidacao.css" rel="stylesheet" />
    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/libs/Chart.js-2.9.3/Chart.min.js"></script>
    <script src="dist/js/pages/feedback/feedback_competencia.js"></script>

    <!--SweetAlert-->
    <script src="https://unpkg.com/sweetalert/dist/sweetalert.min.js"></script>

    <script>

        var color = Chart.helpers.color;
        window.chartColors = {
            yellow: 'rgb(252, 176, 25)',
            blue: 'rgb(0, 51, 92)',
            red: 'rgb(255, 99, 132)',
            orange: 'rgb(255, 159, 64)',
            green: 'rgb(75, 192, 192)',
            purple: 'rgb(153, 102, 255)',
            grey: 'rgb(231,233,237)'
        };

        function GeraRadarFeedback(idPeriodo, jsonData) {

            var obj = jsonData;
            var chartObj = document.getElementById('radar' + idPeriodo);
            try
            {
                new Chart(chartObj, {
                    type: 'radar',
                    data: {
                        labels: obj[0].labels,
                        datasets: [
                            {
                                label: 'Avaliação Gestor',
                                backgroundColor: 'rgba(255, 255, 255, 255)',
                                borderColor: window.chartColors.blue,
                                pointBackgroundColor: window.chartColors.blue,
                                data: obj[0].datasetgestor
                            },
                            {
                                label: 'Auto Avaliação',
                                backgroundColor: 'rgba(255, 255, 255, 255)',
                                borderColor: window.chartColors.yellow,
                                pointBackgroundColor: window.chartColors.yellow,
                                data: obj[0].datasetavaliado
                            },
                            {
                                label: 'Nível Atual',
                                backgroundColor: 'rgba(255, 255, 255, 255)',
                                data: obj[0].datasetatual
                            },
                            {
                                label: 'Próximo Nível',
                                backgroundColor: 'rgba(255, 255, 255, 255)',
                                borderColor: window.chartColors.grey,
                                pointBackgroundColor: window.chartColors.grey,
                                data: obj[0].datasetproximonivel
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        scale: {
                            angleLines: {
                                display: false
                            },
                            ticks: {
                                suggestedMin: 0,
                                suggestedMax: 200,
                                maxTicksLimit: 1,
                                display: false
                            }
                        },
                        title: {
                            display: false
                        },
                        legend: {
                            display: true,
                            position: "bottom",
                            fontSize: 5,
                        }
                    }

                });
            }
            catch (e) {
                alert(e.message);
            }
        }

        function GeraRadar(idPeriodo, jsonData) {
            try {
                var ctx = document.getElementById('radd' + idPeriodo);
                ctx.innerHTML = "teste" + idPeriodo;

                var obj = jsonData;
                var chartObj = document.getElementById('radar' + idPeriodo);

                for (var d in obj)
                {
                    new Chart(chartObj,
                        {
                            type: 'radar',
                            data: {
                                labels: obj[d].labels,
                                datasets: [{
                                    label: 'Projeto',
                                    backgroundColor: color(window.chartColors.blue).alpha(0.2).rgbString(),
                                    borderColor: window.chartColors.blue,
                                    pointBackgroundColor: window.chartColors.blue,
                                    data: obj[d].datasetcompetencias
                                }, {
                                    label: 'Nível Atual',
                                    data: obj[d].datasetnivelatual
                                }]
                            },
                            options: {
                                responsive: true,
                                scale: {
                                    angleLines: {
                                        display: false
                                    },
                                    ticks: {
                                        suggestedMin: 50,
                                        suggestedMax: 900
                                    },
                                    pointLabels: {
                                        fontSize: 8,
                                    }
                                },
                                title: {
                                    display: false
                                },
                                legend: {
                                    display: true,
                                    position: "bottom",
                                    "labels": {
                                        "fontSize": 8,
                                    }
                                }
                            }
                        });
                }
            }
            catch (e)
            {
                alert(e.message);
            }
        }

        function hold() {
        }

        var Evolucao = function () {
            var Radar = function () {
                var json = $('input[id$=hfJsonRadarEvolucao]').val();
                var obj = JSON.parse(json);

                var ctx = document.getElementById("radarevolucao").getContext('2d');

                const listCores = ['azul1', 'azul2', 'azul3', 'azul4', 'azul5', 'azul6', 'azul7'];
                var color = Chart.helpers.color;
                window.chartColors = {
                    yellow: 'rgb(252, 176, 25)',
                    blue: 'rgb(252, 176, 25)',
                    red: 'rgb(255, 99, 132)',
                    orange: 'rgb(255, 159, 64)',
                    green: 'rgb(75, 192, 192)',
                    purple: 'rgb(153, 102, 255)',
                    grey: 'rgb(231,233,237)',
                    white: 'rgb(255, 255, 255)',
                    azul1: 'rgb(0, 22, 72)',
                    azul2: 'rgb(0, 41, 132)',
                    azul3: 'rgb(0, 59, 192)',
                    azul4: 'rgb(33, 102, 255)',
                    azul5: 'rgb(113, 157, 255)',
                    azul6: 'rgb(179, 203, 255)',
                    azul7: 'rgb(221, 232, 255)'
                };

                var chart = new Chart(ctx, {
                    type: 'radar',
                    data: {
                        labels: obj.labels,
                    },
                    options: {
                        responsive: true,
                        scale: {
                            angleLines: {
                                display: false
                            },
                            ticks: {
                                beginAtZero: true,
                                max: 200,
                                min: 0,
                                stepSize: obj.stepsize,
                                display: false,
                                fontSize: 40
                            },
                            pointLabels: {
                                fontSize: 22
                            }
                        },
                        title: {
                            display: false
                        },
                        legend: {
                            display: true,
                            position: "bottom",
                            fontSize: 22
                        },
                        labels: {
                            fontSize: 22
                        }
                    }

                });

                for (var d in obj.datasets) {
                    var pavaliado = obj.datasets[d];
                    addData(chart, pavaliado.periodo, color(window.chartColors.white).alpha(0.2).rgbString(), window.chartColors[listCores[d]], pavaliado.dataset);
                }
            }


            function addData(radar, label, hexacolor, rgbcolor, data) {
                radar.data.datasets.push({
                    label: label,
                    backgroundColor: hexacolor,
                    borderColor: rgbcolor,
                    pointBackgroundColor: rgbcolor,
                    data: data
                });

                radar.update();
            }

            return {

                init: function () {
                    Radar();
                }

            };

        }();

        jQuery(document).ready(function () {
            Evolucao.init();
        });
    </script>

    <script>
        if ($("#" + '<%= btnSalvar.ClientID %>').length > 0) {
            fcnSetTimeout();
                function fcnSetTimeout() {
                    setTimeout(function () {
                        Message('success', "SALVANDO!!", 9000);

                        var btnCk = <%= btnSalvar.ClientID %>;

                    $(btnCk).click();

                    fcnSetTimeout();
                }, 900000);
            }
        }
        
        function Message(type, message, _time) {

            swal({
                title: "ATENÇÃO!",
                text: message,
                icon: type,
                timer: _time,
                button: "Ok",
            });
        }

    </script>
</asp:Content>

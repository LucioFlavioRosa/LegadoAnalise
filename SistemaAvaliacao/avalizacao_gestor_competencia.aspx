<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avalizacao_gestor_competencia.aspx.cs" Inherits="SistemaAvaliacao.avalizacao_gestor_competencia" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom" style="position:sticky;top:50px;z-index:6">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Avaliação Gestor</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item"><a href="index.aspx">Avaliação / Avaliação Gestor</a></li>
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
    <div style="overflow:initial !important" class="container-fluid page-content" >
        <!-- ============================================================== -->
        <!-- Start Page Content -->
        <!-- ============================================================== -->

        <!-- CARD TITULO -->
        <div class="card col-md-12" style="position:sticky;z-index:4;top:10.5%;border-color: rgba(0, 0, 0, .25); border-width: 1px">
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
                                        <label id="label_Titulo_Avaliado" runat="server"><b>Avaliado</b></label>
                                    </div>
                                    <div class="col-md-4">
                                        <label runat="server" id="lblAssociado"></label>
                                    </div>
                                    <div class="col-md-1">
                                        <label id="label_Titulo_Gestor" runat="server"><b>Gestor</b></label>
                                    </div>
                                    <div class="col-md-3">
                                        <label runat="server" id="lblGestor"></label>
                                    </div>
                                    <div class="col-md-2">
                                        <label><b>Período</b></label>
                                    </div>
                                    <div class="col-md-4">
                                        <label runat="server" id="lblPeriodo"></label>
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
                                        <label id="label_Titulo_TempoCargo" runat="server"><b>Tempo de Cargo:</b></label>
                                    </div>
                                    <div class="col-md-4">
                                        <label runat="server" id="lblTempoCargo"></label>
                                    </div>
                                    <div class="col-md-2">
                                        <label id="label_Titulo_TempoPeers" runat="server"><b>Tempo de Peers</b></label>
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
                <div id="containerAccordion" runat="server">
                <div class="accordion" id="accordionCabecalho">
                    <div class="card">
                        <div class="card-header" id="headingCabecalho">
                            <h5 class="mb-0">
                                <button class="btn btn-link" type="button" data-toggle="collapse" data-target="#Cabecalho" aria-expanded="true" aria-controls="Cabecalho">
                                    Descrições nível atual e próximo nível
                                    <i class = "fa fa-angle-right"> </i>
                                </button>
                            </h5>
                        </div>
                        <div id="Cabecalho" class="collapse" aria-labelledby="headingCabecalho" data-parent="#accordionCabecalho">
                            <table class="table table-striped border" style="width: 100%; table-layout: fixed">
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
                                        <th colspan="1">Nível atual</th>
                                        <th colspan="2">
                                            <asp:Label runat="server" ID="text_TabelaCabecalho_CargoAtual"></asp:Label></th>
                                        <th colspan="2">
                                            <asp:Label runat="server" ID="text_TabelaCabecalho_FuncaoAtual"></asp:Label></th>
                                        <th colspan="2">
                                            <asp:Label runat="server" ID="text_TabelaCabecalho_AutonomiaAtual"></asp:Label></th>
                                        <th colspan="2">
                                            <asp:Label runat="server" ID="text_TabelaCabecalho_EscopoAtual"></asp:Label></th>
                                        <th colspan="2">
                                            <asp:Label runat="server" ID="text_TabelaCabecalho_InterlocucaoAtual"></asp:Label></th>
                                        <asp:Repeater runat="server" ID="rpt_TabelaCabecalho_CargoAtual">
                                            <ItemTemplate>
                                                <th colspan="2" style="font-weight: normal;"><%# Eval("DescricaoCompetencia_Atual") %></th>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tr>
                                    <tr>
                                        <th colspan="1">Próximo nível</th>
                                        <th colspan="2">
                                            <asp:Label runat="server" ID="text_TabelaCabecalho_CargoProximo"></asp:Label></th>
                                        <th colspan="2">
                                            <asp:Label runat="server" ID="text_TabelaCabecalho_FuncaoProximo"></asp:Label></th>
                                        <th colspan="2">
                                            <asp:Label runat="server" ID="text_TabelaCabecalho_AutonomiaProximo"></asp:Label></th>
                                        <th colspan="2">
                                            <asp:Label runat="server" ID="text_TabelaCabecalho_EscopoProximo"></asp:Label></th>
                                        <th colspan="2">
                                            <asp:Label runat="server" ID="text_TabelaCabecalho_InterlocucaoProximo"></asp:Label></th>
                                        <asp:Repeater runat="server" ID="rpt_TabelaCabecalho_CargoProximo">
                                            <ItemTemplate>
                                                <th colspan="2" style="font-weight: normal;"><%# Eval("DescricaoCompetencia_Proximo") %></th>
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
        </div>

        <%--CARD COMPETENCIAS--%>
        <div class="card col-md-12" >
            <div class="card-body" >

            <!-- DIV TABELA COMPETENCIAS - NOVA -->
            <div class="table-responsive" style="overflow-y:auto !important; height:600px">
                <table class="table table-striped border" style="border-collapse:collapse !important; width: 100% !important" >
                    <thead>
                        <tr>
                            <th colspan="10" 
                                style="background-color: #e5e019; color: #021240; position:sticky !important; top:-5px !important">Nível 1</th>
                            <th colspan="8" id="head_Nivel" runat="server" oninit="CheckTipoAvalInit" 
                                style="background-color: #e5e019; color: #021240;position:sticky !important; top:-5px !important">Nível 2</th>
                            <th colspan="5" id="head_FillingDesempenho" runat="server" oninit="CheckTipoAvalInit" 
                                style="background-color: #e5e019; color: #021240;position:sticky !important; top:-5px !important"></th>
                            <th colspan="13" id="head_FillingLideranca" runat="server" oninit="CheckTipoAvalInit_Reverso" 
                                style="background-color: #e5e019; color: #021240;position:sticky !important; top:-5px !important"></th>
                        </tr>
                        <tr>
                            <th colspan="4" 
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important">Detalhamento Atual</th>
                            <th colspan="5" id="Th1" runat="server" oninit="CheckTipoAvalInit_Reverso" 
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important"></th>
                            <th hidden>Nivel Atual</th>
                            <th colspan="2" id="head_NotaAvaliado" runat="server" oninit="CheckTipoAvalInit" 
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important">Nota Avaliado</th>
                            <th colspan="2" id="head_NotaCegas" runat="server" oninit="CheckTipoAvalInit" 
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important">Nota às Cegas</th>
                            <th colspan="2" id="head_NotaGestor" runat="server" oninit="CheckTipoAvalInit" 
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important">Nota Gestor</th>
                            <th colspan="2" id="head_NotaLiderado" runat="server" oninit="CheckTipoAvalInit_Reverso" 
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important">Nota Liderado</th>
                            <th colspan="5" id="Th2" runat="server" oninit="CheckTipoAvalInit_Reverso"
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important"></th>
                            <th runat="server" id="tableDetalheNivel2" oninit="CheckTipoAvalInit" colspan="4" 
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important">
                                <asp:Label ID="lblDetalheNivel2" runat="server" Text="Detalhamento Próximo Nível"></asp:Label></th>
                            <th hidden>Próximo Nivel</th>
                            <th colspan="2" id="head_NotaAvaliado2" runat="server" oninit="CheckTipoAvalInit" 
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important">Nota Avaliado</th>
                            <th colspan="2" id="head_NotaCegas2" runat="server" oninit="CheckTipoAvalInit"
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important">Nota às Cegas</th>
                            <th colspan="2" id="head_NotaGestor2" runat="server" oninit="CheckTipoAvalInit"
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important">Nota Gestor</th>
                            <th colspan="3" 
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important">Considerações</th>
                            <th colspan="4" id="Th3" runat="server" oninit="CheckTipoAvalInit_Reverso"
                                style="color: #021240; background-color: #eae678;position:sticky !important; top:45px !important"></th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptCompetencias" runat="server" OnItemDataBound="rptCompetencias_ItemDataBound">
                            <ItemTemplate>
                                    
                                <tr <%# Eval("IsHidden") %> style="font-style: italic; font-size: 14px; font-weight: normal; background-color: #f2f2f2; vertical-align: bottom">
                                    <td colspan="23" style="font-style: normal; font-size: 21px; font-weight: normal; align-content: center; text-align: center; border-top: 5px solid; border-color: #003150">
                                        <%# DataBinder.Eval(Container.DataItem, "SubCompetencia") %>
                                    </td>

                                </tr>

                                <tr style="font-weight: bold; background-color: #f5f5f5; border-top: none">
                                    <%--PALAVRAS CHAVE--%>
                                    <td colspan="23" style="text-align: center; border-top: none"><%# DataBinder.Eval(Container.DataItem, "PalavrasChave") %></td>
                                </tr>

                                <tr style="font-weight: normal; background-color: #f9f9f9; border-top: none">

                                    <td colspan="4" style="border-top: none; background-color: #f9f9f9">
                                        <%--HIDDEN ID COMPETÊNCIA--%>
                                        <asp:HiddenField runat="server" ID="IdCompetenciaItem" Value='<%# Eval("IdCompetencia") %>' />

                                        <%--TEXTO/ACCORDION DETALHE NÍVEL ATUAL--%>
                                        <a href="#det_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>" data-target="#det_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>"
                                            data-toggle="collapse" class="accordion-toggle"><%# TruncarTexto((string)DataBinder.Eval(Container.DataItem, "DetalheNivelAtual"),70) %>
                                            <span style="font-weight: bold">Ver+</span></a>
                                        </a>
                                    </td>

                                    <td colspan="5" id="td2" runat="server" oninit="CheckTipoAvalInit_Reverso" style="color: white; border-top: none; background-color: #f9f9f9"></td>

                                    <%--ACCORDION TÍTULO NÍVEL ATUAL--%>
                                    <td hidden colspan="2" style="border-top: none; background-color: #f9f9f9">
                                        <button type="button" data-toggle="collapse" data-target="#nivel_<%# Eval("IdCompetencia") %>"
                                            class="accordion-toggle">
                                            <span class="fas fa-align-justify" aria-hidden="true"></span>
                                        </button>
                                    </td>

                                    <%--LABELS RESPOSTAS ANTERIORES - NIVEL ATUAL--%>
                                    <td colspan="2" runat="server" id="lblNota1Avaliado" oninit="CheckTipoAvalInit" style="border-top: none; background-color: #f9f9f9"></td>
                                    <td colspan="2" runat="server" id="lblNota1Cegas" oninit="CheckTipoAvalInit" style="border-top: none; background-color: #f9f9f9"></td>

                                    <%--COMBOBOX RESPOSTA NÍVEL ATUAL--%>
                                    <td colspan="2" style="border-top: none; background-color: #f9f9f9">
                                        <div <%# Eval("disableSelectNivel1") %> <%# Eval("hiddenSelectNivel1") %>>
                                            <select id="ddlNotaNivel1" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                        <div <%# Eval("hiddenTextBoxNivel1") %>>
                                            <label id="txtNotaNivel1" runat="server" class="form-control p-0" style="width: 100%; height: 100%; background-color:#E9ECEF"><%# Eval("textoNotaNivel1") %></label>
                                        </div>
                                    </td>

                                    <td colspan="5" id="td4" runat="server" oninit="CheckTipoAvalInit_Reverso" style="color: white; border-top: none; background-color: #f9f9f9">

                                    </td>

                                    <%--TEXTO/ACCORDION DETALHE PRÓXIMO NÍVEL--%>
                                    <td colspan="4" style="border-top: none; background-color: #f9f9f9" id="lblDetalheNivel2" runat="server" oninit="CheckTipoAvalInit">
                                        <a href="#detPrNivel_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>" data-target="#detPrNivel_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>"
                                            data-toggle="collapse" class="accordion-toggle"><%# TruncarTexto((string)DataBinder.Eval(Container.DataItem, "DetalheProximoNivel"),70) %>
                                            <span style="font-weight: bold">Ver+</span></a>
                                        </a>
                                    </td>

                                    <%--ACCORDION TÍTULO PRÓXIMO NÍVEL--%>
                                    <td hidden colspan="2" style="border-top: none; background-color: #f9f9f9" id="lblTituloNivel2" runat="server" oninit="CheckTipoAvalInit">
                                        <button type="button" data-toggle="collapse" data-target="#prNivel_<%# Eval("IdCompetencia") %>" class="accordion-toggle">
                                            <span class="fas fa-align-justify" aria-hidden="true"></span>
                                        </button>
                                    </td>

                                    <%--LABELS RESPOSTAS ANTERIORES - PRÓXIMO NIVEL--%>
                                    <td colspan="2" runat="server" id="lblNota2Avaliado" oninit="CheckTipoAvalInit" style="border-top: none; background-color: #f9f9f9"></td>
                                    <td colspan="2" runat="server" id="lblNota2Cegas" oninit="CheckTipoAvalInit" style="border-top: none; background-color: #f9f9f9"></td>

                                    <%--COMBOBOX RESPOSTA PRÓXIMO NÍVEL--%>
                                    <td colspan="2" style="border-top: none; background-color: #f9f9f9" id="comboRespostaNivel2" runat="server" oninit="CheckTipoAvalInit">
                                        <div <%# Eval("disableSelectNivel2") %> <%# Eval("hiddenSelectNivel2") %>>
                                            <select id="ddlNotaNivel2" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                        <div <%# Eval("hiddenTextBoxNivel2") %>>
                                            <label id="txtNotaNivel2" runat="server" class="form-control p-0" style="width: 100%; height: 100%; background-color:#E9ECEF"><%# Eval("textoNotaNivel2") %></label>
                                        </div>
                                    </td>

                                    <%--ACCORDION CONSIDERAÇÕES--%>
                                    <td colspan="3" style="border-top: none; background-color: #f9f9f9; text-align: center">
                                        <button type="button" data-toggle="collapse" data-target="#cons_<%# Eval("IdCompetencia") %>" class="accordion-toggle"
                                            style="background-color: #003150; padding: 8px 15px; border: none">
                                            <span aria-hidden="true" style="color: white; font-size: 15px">Considerações</span>
                                        </button>
                                    </td>
                                    <td colspan="4" id="td1" runat="server" oninit="CheckTipoAvalInit_Reverso" style="color: white; border-top: none; background-color: #f9f9f9"></td>
                                </tr>

                                <%--DETALHAMENTO ATUAL--%>
                                <tr>
                                    <td colspan="23" class="hiddenRow" style="border-top: none; background-color: #ffffff">
                                        <div class="accordian-body collapse" id="det_<%# Eval("IdCompetencia") %>">
                                            <strong>Detalhamento atual:</strong><br />
                                            <%# DataBinder.Eval(Container.DataItem, "DetalheNivelAtual") %>
                                        </div>
                                    </td>
                                </tr>

                                <%--TÍTULO NIVEL ATUAL--%>
                                <tr hidden>
                                    <td colspan="23" class="hiddenRow" style="border-top: none; background-color: #ffffff">
                                        <div class="accordian-body collapse" id="nivel_<%# Eval("IdCompetencia") %>">
                                            <strong>Nivel Atual:</strong></br>
                                            <%# DataBinder.Eval(Container.DataItem, "CompetenciaAtual") %>
                                        </div>
                                    </td>
                                </tr>

                                <%--DETALHAMENTO DO PRÓXIMO NÍVEL--%>
                                <tr>
                                    <td colspan="23" class="hiddenRow" style="border-top: none; background-color: #ffffff">
                                        <div class="accordian-body collapse" id="detPrNivel_<%# Eval("IdCompetencia") %>">
                                            <strong>Detalhamento Próximo Nível:</strong></br>
                                            <%# DataBinder.Eval(Container.DataItem, "DetalheProximoNivel") %></
                                        </div>
                                    </td>
                                </tr>

                                <%--TÍTULO PRÓXIMO NÍVEL--%>
                                <tr hidden>
                                    <td colspan="23" class="hiddenRow">
                                        <div class="accordian-body collapse" id="prNivel_<%# Eval("IdCompetencia") %>">
                                            <strong>Próximo Nivel:</strong></br>
                                            <%# DataBinder.Eval(Container.DataItem, "CompetenciaProximo") %>
                                        </div>
                                    </td>
                                </tr>

                                <%--CONSIDERAÇÕES--%>
                                <tr>
                                    <td colspan="23" class="hiddenRow">
                                        <div class="accordian-body collapse" id="cons_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>">
                                            <p id="pObsAvaliado" runat="server" clientidmode="static">
                                                <strong>Observações do avaliado:</strong><br>
                                                <asp:Label ID="lblObservacaoAvaliado" runat="server"></asp:Label>
                                            </p>
                                            <p id="pObsGestor" runat="server" clientidmode="static">
                                                <strong>Observações do avaliador:</strong><br>
                                                <asp:Label ID="lblObservacaoCegas" runat="server"></asp:Label>
                                            </p>
                                            <div class="accordian-body">
                                                <strong>Considerações:</strong></br>
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

            <%--DIV BOTÕES ADICIONAIS--%>
            <div class="card">
            <div class="card-body">
                    <div class="col-md-5">
                    <div class="text-left">
                        <button type="button" class="btn btn-facebook" disabled>Competência</button>
                        <asp:Button ID="btn_Performance_2" runat="server" Text="Performance" class="btn btn-danger" OnClick="btnIrPerformance_Click" />
                        <asp:Button ID="btn_Finalizar_2" runat="server" Text="Ir para Finalização" class="btn btn-info" OnClick="btnFinalizar_Click" />
                        <p></p>
                    </div>
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
    <script src="assets/libs/jquery/dist/jquery.min.js"></script>


    <style>
        .hiddenRow {
            padding: 0 !important;
        }
    </style>

    <!--SweetAlert-->
    <script src="https://unpkg.com/sweetalert/dist/sweetalert.min.js"></script>

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

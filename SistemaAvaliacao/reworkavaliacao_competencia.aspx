<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="reworkavaliacao_competencia.aspx.cs" Inherits="SistemaAvaliacao.reworkavaliacao_competencia" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom" style="position:fixed;z-index:1;width:1653px">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Auto Avaliação</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item"><a href="index.aspx">Avaliação / Auto Avaliação</a></li>
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

        <!-- CARD COMPETENCIAS -->
        <div class="card col-md-12" style="margin-top:220px">
            <div class="card-body">

                <!-- DIV BOTAO SALVAR -->
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="text-right">
                                <asp:Button ID="btnSalvar" runat="server" Text="Salvar Avaliação" class="btn btn-dark" OnClick="btnSalvar_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <!-- DIV TABELA COMPETENCIAS -->
                <div class="table-responsive">
                    <table class="table table-striped border" style="width:100%;table-layout:fixed">
                        <thead>
                            <tr>
                                <th colspan="7" style="background-color:#e5e019;color:#021240;">Nível 1</th>
                                <th colspan="9" style="background-color:#e5e019;color:#021240;">Nível 2</th>
                            </tr>
                            <tr>
                                <th colspan="4" style="color:#021240;background-color:#eae678;border-bottom:6px solid;border-color:#eae678">Detalhamento Atual</th>
                                <th hidden>Nivel Atual</th>
                                <th colspan="3" style="color:#021240;background-color:#eae678;border-bottom:6px solid;border-color:#eae678">Nota Avaliado</th>
                                
                                <th runat="server" id="tableDetalheNivel2" colspan="4" style="color:#021240;background-color:#eae678;border-bottom:6px solid;border-color:#eae678"><asp:Label ID="lblDetalheNivel2" runat="server" Text="Detalhamento Próximo Nível"></asp:Label></th>                                
                                <th hidden>Próximo Nivel</th>
                                <th colspan="3" style="color:#021240;background-color:#eae678;border-bottom:6px solid;border-color:#eae678">Nota Avaliado</th>
                                <th colspan="2" style="color:#021240;background-color:#eae678;border-bottom:6px solid;border-color:#eae678">Considerações Avaliado</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptCompetencias" runat="server" OnItemDataBound="rptCompetencias_ItemDataBound">
                                <ItemTemplate>
                                    <tr <%# Eval("IsHidden") %> style="font-style:italic;font-size:14px;font-weight:normal;background-color:#f2f2f2;vertical-align:bottom">
                                        <%--<th colspan="2" style="font-style:italic;font-size:13px;font-weight:normal">
                                            Eixo: <%# DataBinder.Eval(Container.DataItem, "Eixo") %>
                                        </th>--%>
                                        <th colspan="16" style="font-style:normal;font-size:21px;font-weight:normal;align-content:center;text-align:center;border-top:5px solid;border-color:#003150">   
                                            <%# DataBinder.Eval(Container.DataItem, "SubCompetencia") %>
                                        </th>
                                        <%--<th colspan="2" style="font-style:italic;font-size:13px;font-weight:normal">
                                            Dimensão: <%# DataBinder.Eval(Container.DataItem, "Dimensao") %>
                                        </th>--%>
                                        <%--<th colspan="10">&nbsp;</th>--%>
                                    </tr>
                                    <tr style="font-weight:bold;background-color:#f5f5f5;border-top:none">
                                        <td colspan="16" style="text-align:center;border-top:none"><%# DataBinder.Eval(Container.DataItem, "PalavrasChave") %></td>
                                    </tr>
                                    <tr style="font-weight:normal;background-color:#f9f9f9;border-top:none">
                                        <td colspan="4" style="border-top:none;background-color:#f9f9f9">
                                            <%--HIDDEN ID COMPETÊNCIA--%>
                                            <asp:HiddenField runat="server" ID="IdCompetenciaItem" Value='<%# Eval("IdCompetencia") %>' /> 

                                            <%--TEXTO/ACCORDION DETALHE NÍVEL ATUAL--%>
                                            <a href="#det_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>" data-target="#det_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>" data-toggle="collapse" 
                                                class="accordion-toggle"><%# TruncarTexto((string)DataBinder.Eval(Container.DataItem, "DetalheNivelAtual"),70) %>
                                                <span style="font-weight:bold">  Ver+</span>
                                            </a>
                                        </td>

                                         <%--ACCORDION TÍTULO NÍVEL ATUAL--%>
                                        <td hidden colspan="1" style="border-top:none;background-color:#f9f9f9"><button type="button" data-toggle="collapse" data-target="#nivel_<%# Eval("IdCompetencia") %>" class="accordion-toggle"><span class="fas fa-align-justify" aria-hidden="true"></span></button></td>
                                        
                                        <%--COMBOBOX RESPOSTA NÍVEL ATUAL--%>
                                        <td colspan="3" style="border-top:none;background-color:#f9f9f9"><select id="ddlNotaNivel1" runat="server" class="form-control p-0 select2" style="width: 100%" /></td>

                                        <%--TEXTO/ACCORDION DETALHE PRÓXIMO NÍVEL--%>
                                        <td colspan="4" style="border-top:none;background-color:#f9f9f9">
                                            <a href="#detPrNivel_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>" data-target="#detPrNivel_<%# DataBinder.Eval(Container.DataItem,"IdCompetencia") %>" 
                                                data-toggle="collapse"  class="accordion-toggle"><%# TruncarTexto((string)DataBinder.Eval(Container.DataItem, "DetalheProximoNivel"),70) %>
                                                <span style="font-weight:bold">  Ver+</span></a>
                                        </td>

                                        <%--ACCORDION TÍTULO PRÓXIMO NÍVEL--%>
                                        <td hidden colspan="1" style="border-top:none;background-color:#f9f9f9">
                                            <button type="button" data-toggle="collapse" data-target="#prNivel_<%# Eval("IdCompetencia") %>" class="accordion-toggle">
                                                <span class="fas fa-align-justify" aria-hidden="true"></span>
                                            </button>
                                        </td>
                                        
                                        <%--COMBOBOX RESPOSTA PRÓXIMO NÍVEL--%>
                                        <td colspan="3" style="border-top:none;background-color:#f9f9f9"><select id="ddlNotaNivel2" runat="server" class="form-control p-0 select2" style="width: 100%" /></td>
                                        
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
                                        <td colspan="16" class="hiddenRow" style="border-top:none;background-color:#ffffff">
                                            <div class="accordian-body collapse" id="det_<%# Eval("IdCompetencia") %>">
                                                <strong>Detalhamento Cargo Atual:</strong><br /><%# DataBinder.Eval(Container.DataItem, "DetalheNivelAtual") %>
                                            </div>
                                        </td>
                                    </tr>

                                    <%--TÍTULO NIVEL ATUAL--%>
                                    <tr hidden>
                                        <td colspan="16" class="hiddenRow" style="border-top:none;background-color:#ffffff">
                                            <div class="accordian-body collapse" id="nivel_<%# Eval("IdCompetencia") %>">
                                                <strong>Nivel Atual:</strong></br>
                                                <%# DataBinder.Eval(Container.DataItem, "CompetenciaAtual") %></div>
                                        </td>
                                    </tr>

                                    <%--DETALHAMENTO DO PRÓXIMO NÍVEL--%>
                                    <tr>
                                        <td colspan="16" class="hiddenRow" style="border-top:none;background-color:#ffffff">
                                            <div class="accordian-body collapse" id="detPrNivel_<%# Eval("IdCompetencia") %>">
                                                <strong>Detalhamento Próximo Nível:</strong></br>
                                                <%# DataBinder.Eval(Container.DataItem, "DetalheProximoNivel") %></</div>
                                        </td>
                                    </tr>

                                    <%--TÍTULO PRÓXIMO NÍVEL--%>
                                    <tr hidden>
                                        <td colspan="16" class="hiddenRow">
                                            <div class="accordian-body collapse" id="prNivel_<%# Eval("IdCompetencia") %>">
                                                <strong>Próximo Nivel:</strong></br>
                                                <%# DataBinder.Eval(Container.DataItem, "CompetenciaProximo") %></div>
                                        </td>
                                    </tr>

                                    <%--CONSIDERAÇÕES--%>
                                    <tr>
                                        <td colspan="16" class="hiddenRow" style="border-top:none;background-color:#ffffff">
                                            <div class="accordian-body collapse" id="cons_<%# Eval("IdCompetencia") %>">
                                                <strong>Considerações:</strong></br>
                                                <textarea id="txtNivel1" runat="server" rows="5" class="form-control"></textarea></div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

                <!-- DIV BOTAO SALVAR -->
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

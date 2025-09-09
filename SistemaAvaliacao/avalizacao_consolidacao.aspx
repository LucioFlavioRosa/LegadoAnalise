<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avalizacao_consolidacao.aspx.cs" Inherits="SistemaAvaliacao.avalizacao_consolidacao" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <asp:HiddenField runat="server" ID="hfJsonRadar" />
    <link rel="stylesheet" href="dist/css/sliderAvaliacao.css">
    <div class="page-breadcrumb border-bottom" style="position:sticky;top:50px;z-index:6">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Consolidação</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Competências / Performance</li>
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

        <div class="card" style="position:sticky;z-index:4;top:79px;border-color: rgba(0, 0, 0, .25); border-width: 1px">
            <div class="card-body">
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <ul class="nav nav-pills mb-3" id="pills-tab" role="tablist">
                                    <li class="nav-item">
                                        <a class="nav-link active btn btn-facebook" id="tab-competencia-tab" data-toggle="pill" href="#tab-competencia" role="tab" aria-controls="tab-competencia" aria-selected="true">Competência</a>
                                    </li>
                                    <li class="nav-item">
                                        <a class="nav-link btn btn-danger" id="tab-performance-tab" data-toggle="pill" href="#tab-performance" role="tab" aria-controls="tab-performance" aria-selected="false">Performance</a>
                                    </li>
                                    <li class="nav-item">
                                        <a class="nav-link btn btn-info" id="tab-dadosRH-tab" data-toggle="pill" href="#tab-dadosRH" role="tab" aria-controls="tab-dadosRH" aria-selected="false">Dados RH</a>
                                    </li>
                                    <li class="nav-item">                       
                                        <asp:Button OnClick="btnResultado_Click" ID="Button1" class="nav-link btn btn-primary" runat="server" Text="Resultado" />
                                    </li>
                                </ul>
                            </div>
                            <div class="row">

                                <%--FOTO--%>
                                <div class="divExpand">
                                    <img runat="server" id="imgUser1Conso" src="assets/images/users/usernophoto.jpg" class="imgExpand">
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
                </div>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <div class="tab-content" id="pills-tabContent">
                    <!-- PILL COMPETENCIAS -->
                    <div class="tab-pane fade active in show" id="tab-competencia" role="tabpanel" aria-labelledby="tab-competencia-tab">
                        <div class="row">
                            <div class="col-md-12">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:Repeater ID="rptCompetencias" runat="server">
                                            <HeaderTemplate>
                                                <table class="table table-hover table-bordered" style="border-collapse: collapse;">
                                                    <thead>
                                                        <tr>
                                                            <th>Competência</th>
                                                            <th>Nível</th>
                                                            <th>Detalhe</th>
                                                            <th>Obs.</th>
                                                            <th>Nota Avaliado</th>
                                                            <th>Nota Gestor</th>
                                                            <th>N. Sub Comp. Avaliado</th>
                                                            <th>N. Sub Comp. Gestor</th>
                                                            <th>N. Comp. Avaliado</th>
                                                            <th>N. Comp. Gestor</th>
                                                            <th>Competência Comitê</th>
                                                            <th>Nota Considerada</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# DataBinder.Eval(Container.DataItem,"Eixo") %></td>
                                                    <td class="text-center">1</td>
                                                    <td class="text-center">
                                                        <button type="button" data-toggle="collapse" data-target="#cn1_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>" class="accordion-toggle"><span class="fas fa-align-justify" aria-hidden="true"></span></button>
                                                    </td>
                                                    <td>
                                                        <button type="button" data-toggle="collapse" data-target="#on1_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>" class="accordion-toggle"><span class="fas fa-align-justify" aria-hidden="true"></span></button>
                                                    </td>
                                                    <td><%# DataBinder.Eval(Container.DataItem,"NotaNivel1AutoAvaliacao") %></td>
                                                    <td><%# DataBinder.Eval(Container.DataItem,"NotaNivel1Feedback") %></td>
                                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"NotaSubcompetenciaAvaliadoN1") %></td>
                                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"NotaSubcompetenciaGestorN1") %></td>
                                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"NotaCompetenciaAvaliado") %></td>
                                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"NotaCompetenciaGestor") %></td>
                                                    <td>
                                                        <div>
                                                            <select id="ddlNotaComite" runat="server" data-id='<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>' data-nivel="1" class="form-control p-0 select2 ddlCompetencia" style="width: 100%" />
                                                        </div>
                                                    </td>
                                                    <td style="text-align: center" id="tdcomiten1_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>">
                                                        <%# DataBinder.Eval(Container.DataItem,"NotaFinalNivel1") %>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="12" class="hiddenRow">
                                                        <div class="accordian-body collapse" id="cn1_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>">
                                                            <p>
                                                                <strong>Detalhamento Cargo Atual:</strong><br></br>
                                                                <%# DataBinder.Eval(Container.DataItem,"DetalheNivelAtual") %>
                                                            </p>
                                                            <p>
                                                                <strong>Cargo Atual:</strong><br></br>
                                                                <%# DataBinder.Eval(Container.DataItem,"CompetenciaAtual") %>
                                                            </p>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="12" class="hiddenRow">
                                                        <div class="accordian-body collapse" id="on1_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>">
                                                            <p>
                                                                <strong>Comentário Avaliado:</strong><br></br>
                                                                <%# DataBinder.Eval(Container.DataItem,"ComentarioAvaliado") %>
                                                            </p>
                                                            <p>
                                                                <strong>Comentário Gestor:</strong><br></br>
                                                                <%# DataBinder.Eval(Container.DataItem,"ComentarioFeedback") %>
                                                            </p>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td><%# DataBinder.Eval(Container.DataItem,"Eixo") %></td>
                                                    <td class="text-center">2</td>
                                                    <td class="text-center">
                                                        <button type="button" data-toggle="collapse" data-target="#cn2_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>" class="accordion-toggle"><span class="fas fa-align-justify" aria-hidden="true"></span></button>
                                                    </td>
                                                    <td>
                                                        <button type="button" data-toggle="collapse" data-target="#on2_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>" class="accordion-toggle"><span class="fas fa-align-justify" aria-hidden="true"></span></button>
                                                    </td>
                                                    <td><%# DataBinder.Eval(Container.DataItem,"NotaNivel2AutoAvaliacao") %></td>
                                                    <td><%# DataBinder.Eval(Container.DataItem,"NotaNivel2Feedback") %></td>
                                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"NotaSubcompetenciaAvaliadoN2") %></td>
                                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"NotaSubcompetenciaGestorN2") %></td>
                                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"NotaCompetenciaAvaliado") %></td>
                                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"NotaCompetenciaGestor") %></td>
                                                    <td>
                                                        <div>
                                                            <select id="ddlNotaComiteNivel2" runat="server" data-id='<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>' data-nivel="2" class="form-control p-0 select2 ddlCompetencia" style="width: 100%" />
                                                        </div>
                                                    </td>
                                                    <td style="text-align: center" id="tdcomiten2_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>"><%# DataBinder.Eval(Container.DataItem,"NotaFinalNivel2") %></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="12" class="hiddenRow">
                                                        <div class="accordian-body collapse" id="cn2_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>">
                                                            <p>
                                                                <strong>Detalhamento Cargo Atual:</strong><br></br>
                                                                <%# DataBinder.Eval(Container.DataItem,"DetalheProximoNivel") %>
                                                            </p>
                                                            <p>
                                                                <strong>Cargo Atual:</strong><br></br>
                                                                <%# DataBinder.Eval(Container.DataItem,"CompetenciaProximo") %>
                                                            </p>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="12" class="hiddenRow">
                                                        <div class="accordian-body collapse" id="on2_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoCompetencia") %>">
                                                            <p>
                                                                <strong>Comentário Avaliado:</strong><br></br>
                                                                <%# DataBinder.Eval(Container.DataItem,"ComentarioAvaliado") %>
                                                            </p>
                                                            <p>
                                                                <strong>Comentário Gestor:</strong><br></br>
                                                                <%# DataBinder.Eval(Container.DataItem,"ComentarioFeedback") %>
                                                            </p>
                                                        </div>
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
                        </div>
                    </div>

                    <!-- PILL PERFORMANCE -->
                    <div class="tab-pane fade" id="tab-performance" role="tabpanel" aria-labelledby="tab-performance-tab">
                        <div class="table-responsive">

                            <asp:Repeater ID="rptPerformance" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-striped table-bordered" style="border-collapse: collapse;">
                                        <thead>
                                            <tr>
                                                <th>Performance</th>
                                                <th>Nota Avaliado</th>
                                                <th>Nota Gestor</th>
                                                <th>Observações</th>
                                                <th>Nota Comitê</th>
                                                <th>Prod. Pon. Perfom</th>
                                                <th>Valor Nota</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# DataBinder.Eval(Container.DataItem,"Perfomance") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem,"NotaNivel1AutoAvaliacao") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem,"NotaNivel1Feedback") %></td>
                                        <td class="text-center">
                                            <button type="button" data-toggle="collapse" data-target="#po_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoPerformance") %>" class="accordion-toggle"><span class="fas fa-align-justify" aria-hidden="true"></span></button>
                                        </td>
                                        <td>
                                            <select id="ddlPerformanceNotaComite" runat="server" data-id='<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoPerformance") %>' class="form-control p-0 select2 ddlPerformance" style="width: 100%" />
                                        </td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem,"NotaPerfomancePonderada") %></td>
                                        <td class="text-center" id="tdperformance_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoPerformance") %>"><%# DataBinder.Eval(Container.DataItem,"NotaPerfomance") %></td>
                                    </tr>
                                    <tr>
                                        <td colspan="7" class="hiddenRow">
                                            <div class="accordian-body collapse" id="po_<%# DataBinder.Eval(Container.DataItem,"IdAvaliacaoPerformance") %>">
                                                <p>
                                                    <strong>Comentário Avaliado:</strong><br></br>
                                                    <%# DataBinder.Eval(Container.DataItem,"ComentariosAutoAvaliacao") %>
                                                </p>
                                                <p>
                                                    <strong>Comentário Gestor:</strong><br></br>
                                                    <%# DataBinder.Eval(Container.DataItem,"ComentarioFeedback") %>
                                                </p>
                                            </div>
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

                    <!-- PILL DADOS RH -->
                    <div class="tab-pane fade" id="tab-dadosRH" role="tabpanel" aria-labelledby="tab-dadosRH-tab">
                        <div class="card-body">
                            <div class="row">
                                <label class="switchRH">
                                    <asp:CheckBox id="cboxLiberadoRH" runat="server" OnCheckedChanged="cboxLiberadoRH_CheckedChanged" AutoPostBack="true"/>
                                    <span class="slider round" style="width: 60px; height: 34px;"></span>
                                </label>
                                <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cboxLiberadoRH" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <div style="padding-top:6px">
                                            <asp:Label runat="server" id="labelLiberadoRH" style="font-size:18px; text-align:left;font-style:italic;color:dimgray;vertical-align:central;padding-left:2px"></asp:Label>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Ação comitê:</label>
                                    <asp:TextBox runat="server" id="txtAcaoComite" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center;border:none"></asp:TextBox>
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
                                    <asp:TextBox runat="server" id="txtSalarioAtual" Text="R$ 0,00" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center;border:none"
                                        OnTextChanged="txtSalarioAtual_TextChanged" AutoPostBack="true"></asp:TextBox>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Próximo salário:</label>
                                    <asp:TextBox runat="server" id="txtProximoSalario" Text="R$ 0,00" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center;border:none"
                                        OnTextChanged="txtSalarioAtual_TextChanged" AutoPostBack="true"></asp:TextBox>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Incremento(%):</label>
                                    <asp:UpdatePanel id="updateLabelIncremento" UpdateMode="Conditional" runat="server">
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="txtSalarioAtual" />
                                            <asp:AsyncPostBackTrigger ControlID="txtProximoSalario" />
                                        </Triggers>
                                        <ContentTemplate>
                                            <asp:Label runat="server" id="labelIncremento" Text="0%" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                                padding-left: 0px;float:right;width: 60%;text-align:center"></asp:Label>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Regime de contratação atual:</label>
                                    <asp:TextBox runat="server" id="txtRegimeContratacaoAtual" Text="CLT" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center;border:none"></asp:TextBox>
                                </div>
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 40%;">Regime de contratação novo:</label>
                                    <asp:TextBox runat="server" id="txtRegimeContratacaoNovo" Text="CLT" style="background-color:#F2F2F2;font-size:18px; list-style-type: none;
                                        padding-left: 0px;float:right;width: 60%;text-align:center;border:none"></asp:TextBox>
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
                                    <asp:TextBox TextMode="MultiLine" runat="server" id="txtPontosFortesRH" style="background-color:#F2F2F2;font-size:15px; list-style-type: none;
                                        padding-left: 7px;float:right;width: 100%;height: 100px;vertical-align:top;text-align:left;border:none;padding-top:7px"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <label style="font-size:18px;list-style-type: none;padding-left: 0px;float:left;width: 100%;position:relative;z-index:1;height:6px">Pontos de atenção:</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm">
                                    <asp:TextBox TextMode="MultiLine" runat="server" id="txtPontosFracosRH" style="background-color:#F2F2F2;font-size:15px; list-style-type: none;
                                        padding-left: 7px;float:right;width: 100%;height: 100px;vertical-align:top;text-align:left;border:none;padding-top:7px"></asp:TextBox>
                                </div>
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



    <link href="dist/custom/consolidacao.css" rel="stylesheet" />
    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/libs/Chart.js-2.9.3/Chart.min.js"></script>
    <script src="dist/js/pages/consolidacao/consolidacao.js"></script>
    <script>
        document.getElementById("inputLiberadoRH").onclick = function () { myFunction() };

        function myFunction() {
            document.getElementById("labelLiberadoRH").innerText = "YOU CLICKED ME!";
            alert("funcionou");
        }
    </script>
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


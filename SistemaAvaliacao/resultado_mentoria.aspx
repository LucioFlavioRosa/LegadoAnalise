<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="resultado_mentoria.aspx.cs" Inherits="SistemaAvaliacao.resultado_mentoria" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">RESULTADO MENTORIA</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Mentoria</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Resultado de Avaliações de Mentoria</li>
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

    <div hidden id="divHIDDENHELPERS">
        <asp:TextBox runat="server" ID="txtIdNota"></asp:TextBox>
        <asp:TextBox runat="server" ID="txtEscala"></asp:TextBox>
        <asp:TextBox runat="server" ID="txtIdResposta"></asp:TextBox>
        <asp:TextBox runat="server" ID="txtComentarios"></asp:TextBox>
        <asp:Button runat="server" ID="btnAtualizaNota" OnClick="btnAtualizaNota_Click" />
        <asp:Button runat="server" ID="btnAtualizaComentario" OnClick="btnAtualizaComentario_Click" />
    </div>

    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" EnableViewState="true">
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdatePanel runat="server" ID="updRespostasMentoria" UpdateMode="Conditional" EnableViewState="true">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAtualizaNota" />
            <asp:AsyncPostBackTrigger ControlID="btnAtualizaComentario" />
        </Triggers>
        <ContentTemplate>
            <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler1" />
            <div class="page-content container-fluid">

                <%--PERIODOS--%>
                <div class="card card-body">
                    <h4 class="card-title">Períodos</h4>
                    <ul class="nav nav-pills mb-3" id="pills-tab2" role="tablist">
                        <asp:Repeater runat="server" ID="rptPills">
                            <ItemTemplate>
                                <li class="nav-item">
                                    <a class="nav-link btn <%#Eval("classe") %> <%#Eval("active") %>" id="<%#Eval("id") %>" data-toggle="pill" href="<%#Eval("href") %>" role="tab"
                                        aria-controls="<%#Eval("ariacontrols") %>" aria-selected="<%#Eval("ariaselected") %>"><%#Eval("Periodo") %>
                                    </a>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>

                <%--LISTA AVALIAÇÕES--%>
                <div class="card">
                    <div class="card-body">
                        <div class="tab-content" id="pills-tabContent">
                            <asp:Repeater runat="server" ID="rptPeriodos" OnItemCreated="rptPeriodos_ItemCreated">
                                <ItemTemplate>
                                    <div class="tab-pane fade <%#Eval("active") %>" id="<%#Eval("id") %>" role="tabpanel" aria-labelledby="<%#Eval("arialabelled") %>">
                                        <h4><%#Eval("Periodo") %></h4>
                                        <div style="display:flex;flex-direction:column;gap:10px; background-color: #D0E4EE; border-radius: 0.25em; padding: 1em;">
                                            <div style="display: flex; width: 100%; flex-direction: row; gap: 10px; align-items:center">
                                                <img src="<%#Eval("MentorFoto") %>" width="100" height="100" style="border-radius: 0.25em" />
                                                <div style="display:flex; flex-direction:column; gap:0.01px;justify-content:center">
                                                    <label style="font-size: 2em"><%# Eval("MentorNome") %></label>
                                                    <label style="font-size: 1em">Você orientou <%# Eval("CountMentorados") %> associados neste período.</label>
                                                    <label style="font-size: 1em">No total, <%# Eval("CountMentores") %> mentores foram avaliados neste período.</label>
                                                </div>
                                            </div>
                                            <asp:Repeater runat="server" ID="rptRespostasPill" DataSource='<%# Eval("resultados") %>' OnItemCreated="rptRespostasPill_ItemCreated">
                                                <ItemTemplate>
                                                    <div style="width: 100%; background-color: #E7F1F7; display: flex; flex-direction: row; padding: 0.5em; margin-bottom: 15px; border-radius: 0.25em; gap:10px">
                                                        <div style="width: 100%; display: flex; flex-direction: column;">
                                                            <label style="font-size: 1.25em"><%# Eval("Pergunta") %></label>
                                                            <div style="display:flex;flex-direction:row;align-items:center;justify-content:space-around;height:100%;gap:4px">
                                                                <div style="display:flex;flex-direction:column; border-radius:0.45em; padding:0.2em;
                                                                        align-items:center;text-align:center; height:100%; width:25%; background-color: white;color:#021240">
                                                                    <label style="font-size: 1.75em">Resultado Mentor</label>
                                                                    <label style="font-size: 2.25em"><%# Convert.ToBoolean(Eval("showNotaMentor")) ? Eval("ResultadoMentor") : "-"%> / <%# Eval("NotaMaxima") %></label>
                                                                    <label style="font-size: 0.75em;font-style:italic">NOTA OBTIDA / NOTA MÁXIMA</label>
                                                                    <div class="container" <%# Eval("HiddenVelocimetro") %>>
                                                                        <div class="chartVeloc" id="chartMentor<%#Eval("idPeriodo") %>_<%#Eval("idResposta") %>" onchange="carregaChart(this, 60);" ></div>
                                                                    </div>
                                                                </div>
                                                                <div style="display:flex;flex-direction:column; border-radius:0.45em; padding:0.2em;
                                                                        align-items:center;text-align:center; height:100%; width:25%; background-color: white;color:#021240">
                                                                    <label style="font-size: 1.75em">Resultado Peers</label>
                                                                    <label style="font-size: 2.25em"><%# Eval("ResultadoPeers") %> / <%# Eval("NotaMaxima") %></label>
                                                                    <label style="font-size: 0.75em;font-style:italic">NOTA OBTIDA / NOTA MÁXIMA</label>
                                                                    <div class="container" <%# Eval("HiddenVelocimetro") %>>
                                                                        <div class="chartVeloc" id="chartPeers<%#Eval("idPeriodo") %>_<%#Eval("idResposta") %>"></div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <div style="width: 100%; background-color: #C0C4CF; display: flex; flex-direction: row; padding: 0.5em; margin-bottom: 15px; border-radius: 0.25em; gap:10px">
                                                <div style="width: 100%; display: flex; flex-direction: column;">
                                                    <label style="font-size: 1.25em">Resultado Periodo</label>
                                                    <div style="display:flex;flex-direction:row;align-items:center;justify-content:space-around;height:100%;gap:4px">
                                                        <div style="display:flex;flex-direction:column; border-radius:0.45em; padding:0.2em;
                                                                align-items:center;text-align:center; height:100%; width:25%; background-color: white;color:#021240">
                                                            <label style="font-size: 1.75em">Resultado Mentor</label>
                                                            <label style="font-size: 2.25em"><%#  Convert.ToInt32(Eval("CountMentorados")) > 1 ? Eval("mediaMentor") : "-" %> / <%# Eval("notaMaxima") %></label>
                                                            <label style="font-size: 0.75em;font-style:italic">NOTA OBTIDA / NOTA MÁXIMA</label>
                                                            <div class="container">
                                                                <div class="chartVeloc" id="chartTotalMentor<%#Eval("idPeriodo") %>" <%# Convert.ToInt32(Eval("CountMentorados")) == 1 ? "style=\"display: none;\"" : "" %>></div>
                                                            </div>
                                                        </div>
                                                        <div style="display:flex;flex-direction:column; border-radius:0.45em; padding:0.2em;
                                                                align-items:center;text-align:center; height:100%; width:25%; background-color: white;color:#021240">
                                                            <label style="font-size: 1.75em">Resultado Peers</label>
                                                            <label style="font-size: 2.25em"><%# Eval("mediaPeers") %> / <%# Eval("notaMaxima") %></label>
                                                            <label style="font-size: 0.75em;font-style:italic">NOTA OBTIDA / NOTA MÁXIMA</label>
                                                            <div class="container">
                                                                <div class="chartVeloc" id="chartTotalPeers<%#Eval("idPeriodo") %>"></div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
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
    <script src="https://cdnjs.cloudflare.com/ajax/libs/echarts/5.0.2/echarts.min.js"></script>

    <script>
        function atualizaNota(idNota, idResposta, Escala) {
            document.getElementById('<%=txtIdNota.ClientID%>').value = idNota;
            document.getElementById('<%=txtIdResposta.ClientID%>').value = idResposta;
            document.getElementById('<%=txtEscala.ClientID%>').value = Escala;

            var clickButton = document.getElementById("<%= btnAtualizaNota.ClientID %>");
            clickButton.click();
        }

        function atualizaComentario(item, idResposta) {
            document.getElementById('<%=txtComentarios.ClientID%>').value = item.value;
            document.getElementById('<%=txtIdResposta.ClientID%>').value = idResposta;

            var clickButton = document.getElementById("<%= btnAtualizaComentario.ClientID %>");
            clickButton.click();
        }

    </script>
    <style>
        .checkboxValid {
            width: 1.7em;
            height: 1.7em;
            accent-color: #003150;
        }
    </style>

    <style>
        .container {
            display: flex;
            align-items: center;
            justify-content: center;
        }

        #chartMentor, #chartPeers, .chartVeloc {
          height: 200px;
          width: 200px;
          margin-bottom: -70px;
          margin-top: -20px;
        }
    </style>

    <script>

        function carregaChart(chart, valor) {
            var dom = document.getElementById(chart);
            var myChart = echarts.init(dom);
            var app = {};
            option = null;

            option = {
                title: {
                    text: '',
                    left: 'center'
                },
                series: [{
                    type: 'gauge',
                    startAngle: 180,
                    endAngle: 0,
                    progress: {
                        show: false,
                        width: 10
                    },
                    axisLine: {
                        roundCap: true,
                        lineStyle: {
                            width: 20,
                            color: [
                                [0.5, '#ca583f'],
                                [0.75, '#FDDD60'],
                                [1, '#1eaa59'],
                            ]
                        }
                    },
                    axisTick: {
                        show: false
                    },
                    splitLine: {
                        length: 15,
                        lineStyle: {
                            width: 0,
                            color: '#999'
                        }
                    },
                    axisLabel: {
                        show: false,
                        distance: 5,
                        color: '#999',
                        fontSize: 14
                    },
                    pointer: {
                        icon: 'path://M2090.36389,615.30999 L2090.36389,615.30999 C2091.48372,615.30999 2092.40383,616.194028 2092.44859,617.312956 L2096.90698,728.755929 C2097.05155,732.369577 2094.2393,735.416212 2090.62566,735.56078 C2090.53845,735.564269 2090.45117,735.566014 2090.36389,735.566014 L2090.36389,735.566014 C2086.74736,735.566014 2083.81557,732.63423 2083.81557,729.017692 C2083.81557,728.930412 2083.81732,728.84314 2083.82081,728.755929 L2088.2792,617.312956 C2088.32396,616.194028 2089.24407,615.30999 2090.36389,615.30999 Z',
                        length: '85%',
                        width: 6,
                        offsetCenter: [0, '0']
                    },
                    anchor: {
                        show: true,
                        showAbove: true,
                        size: 10,
                        itemStyle: {
                            borderWidth: 3
                        }
                    },
                    detail: {
                        show: false,
                        valueAnimation: true,
                        fontSize: 30,
                        offsetCenter: [0, '20%']
                    },
                    data: [{
                        value: valor
                    }]
                }]
            };

            if (option && typeof option === "object") {
                myChart.setOption(option, true);
            }
        }
    </script>

</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avalizacao_resultado_lideranca.aspx.cs" Inherits="SistemaAvaliacao.avalizacao_resultado_lideranca" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <asp:HiddenField runat="server" ID="hfJsonRadar" />
    <asp:HiddenField runat="server" ID="hfJsonSomaRadar" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">RESULTADO LÍDERES</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Resultado Líderes</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>

    <div id="DIVOLDPAINEL" hidden>
    <div class="page-content container-fluid">
        <div class="card">
            <div class="card-body">
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-2">
                                    <img runat="server" id="imgUser1Comite" src="assets/images/users/usernophoto.jpg" width="80" height="80">
                                </div>
                                <div class="col-md-0">
                                    <label><b>Avaliado</b></label>
                                </div>
                                <div class="col-md-3">
                                    <label runat="server" id="lblAssociado"></label>
                                </div>
                                <div class="col-md-0">
                                    <label><b>Mentor</b></label>
                                </div>
                                <div class="col-md-3">
                                    <label runat="server" id="lblMentor"></label>
                                </div>
                                <div class="col-md-0">
                                    <label><b>Período</b></label>
                                </div>
                                <div class="col-md-1">
                                    <label runat="server" id="lblPeriodo"></label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-2">
                                    <label></label>
                                </div>
                                <div class="col-md-0">
                                    <label><b>Cargo</b></label>
                                </div>
                                <div class="col-md-3">
                                    <label runat="server" id="lblCargo"></label>
                                </div>
                                <div class="col-md-2 ffset-md-1">
                                    <label><b>Próximo Cargo</b></label>
                                </div>
                                <div class="col-md-6">
                                    <label runat="server" id="lblProximoCargo"></label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%-- CARD FILTROS --%>
        <div class="card">
             <div class="card-body">
                 <div class="form-body">
                     <div class="row">
                        <div class="col-sm">
                            <div class="col-md-7">
                                <label style="font-size:16px;font-weight:bold">Projetos</label>
                            </div>
                            <div class="col-md-7">
                                <asp:CheckBoxList runat="server" ID="cboxProjetos" SelectionMode="Multiple" Width="200px" OnSelectedIndexChanged="cboxProjetos_SelectedIndexChanged" AutoPostBack="true"></asp:CheckBoxList>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="col-md-7">
                                <label style="font-size:16px;font-weight:bold">Liderados</label>
                            </div>
                            <div class="col-md-7">
                                <asp:CheckBoxList runat="server" ID="cboxLiderados" SelectionMode="Multiple" Width="200px" OnSelectedIndexChanged="cboxProjetos_SelectedIndexChanged" AutoPostBack="true"></asp:CheckBoxList>
                            </div>
                        </div>
                        <div class="col-sm">
                            <div class="col-md-7">
                                <label style="font-size:16px;font-weight:bold">Cargo</label>
                            </div>
                            <div class="col-md-7">
                                <asp:CheckBoxList runat="server" ID="cboxCargos" SelectionMode="Multiple" Width="200px" OnSelectedIndexChanged="cboxProjetos_SelectedIndexChanged" AutoPostBack="true"></asp:CheckBoxList>
                            </div>
                        </div>
                         <div class="col-sm">
                            <div class="col-md-7">
                                <label style="font-size:16px;font-weight:bold">Períodos</label>
                            </div>
                            <div class="col-md-7">
                                <asp:CheckBoxList runat="server" ID="cboxSemestres" SelectionMode="Multiple" Width="200px" OnSelectedIndexChanged="cboxProjetos_SelectedIndexChanged" AutoPostBack="true"></asp:CheckBoxList>
                            </div>
                        </div>
                     </div>
                 </div>
             </div>
        </div>

        <%-- CARD EIXOS --%>
        <div class="card">
             <div class="card-body">
                 <div class="form-body">
                     <div class="row">
                        <div class="col-md-8">
                            <label style="font-size:18px;font-weight:bold">Visão geral da avaliação</label>
                        </div>
                     </div>
                     <div class="row">
                         <div class="col-md-6">
                             <asp:Chart ID="graficoEixos" runat="server" BorderlineWidth="1" Height="340px" Width="460px" BorderlineColor="192, 64, 0" Palette="None" PaletteCustomColors="32, 56, 100">
                                <Series>
                                    <asp:Series Name="serieResultado" XValueMember="eixo" YValueMembers="resultado" BorderWidth="2" ChartType="Bar">  
                                    </asp:Series> 
                                </Series> 
                                <ChartAreas>  
                                    <asp:ChartArea Name="chartResultado">
                                        <axisy Maximum="100">
                                            <MajorGrid Enabled ="False" />
                                        </axisy>
                                        <axisx>
                                            <MajorGrid Enabled="false"/>
                                        </axisx>
                                    </asp:ChartArea>  
                                </ChartAreas>  
                            </asp:Chart> 
                         </div>
                         <div class="col-md-4">
                            <table class="table table-striped border">
                                <asp:Repeater ID="tabelaNotasEixos" runat="server">
                                    <ItemTemplate>
                                        <thead>
                                            <tr>
                                                <th style="width:170px;background-color:#203764;font-weight:normal;color:white">Pilar</th>
                                                <asp:Repeater DataSource='<%# Eval("eixos") %>' runat="server">
                                                    <ItemTemplate>
                                                        <th style="width:125px;background-color:#203764;font-weight:normal;color:white"><%# Eval("eixo") %></th>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                            <tr>
                                                <th style="width:125px;background-color:#A6A6A6;font-weight:normal;color:white;font-size:13px">Geral</th>
                                                <asp:Repeater DataSource='<%# Eval("eixos") %>' runat="server">
                                                    <ItemTemplate>
                                                        <th style="width:125px;background-color:#A6A6A6;font-weight:normal;color:white;font-size:13px"><%# Eval("resultado_percent") %></th>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td>Possui Totalmente</td>
                                                <asp:Repeater DataSource='<%# Eval("eixos") %>' runat="server">
                                                    <ItemTemplate>
                                                        <td style="text-align:center"><%# Eval("notas_2") %></td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                            <tr>
                                                <td>Possui Parcialmente</td>
                                                <asp:Repeater DataSource='<%# Eval("eixos") %>' runat="server">
                                                    <ItemTemplate>
                                                        <td style="text-align:center"><%# Eval("notas_3") %></td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                            <tr>
                                                <td>Não Possui</td>
                                                <asp:Repeater DataSource='<%# Eval("eixos") %>' runat="server">
                                                    <ItemTemplate>
                                                        <td style="text-align:center"><%# Eval("notas_4") %></td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                        </tbody>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                        </div>
                     </div>
                 </div>
             </div>
        </div>

        <%-- CARD SUBCOMPETENCIAS --%>
        <div class="card">
             <div class="card-body">
                 <div class="form-body">
                     <div class="row">
                        <div class="col-md-8">
                            <label style="font-size:18px;font-weight:bold">Visão por pilar</label>
                        </div>
                     </div>
                     <div class="row">
                        <asp:Repeater ID="graficoSubs" runat="server" OnItemDataBound="graficoSubs_ItemDataBound">
                            <ItemTemplate>
                                <div class="col-sm">
                                    <table> 
                                        <thead>
                                            <tr>
                                                <th style="max-width: 50px; height: 85px; line-height: 14px; padding-bottom: 20px; text-align: inherit;margin-bottom:100px">
                                                    <div style="margin-left: -80px; position: absolute; width: 200px; transform: rotate(-90deg); -webkit-transform: rotate(-90deg); -moz-transform: rotate(-90deg);
                                                        -o-transform: rotate(-90deg); -ms-transform: rotate(-90deg);text-align:center;font-size:15px;background-color:#001236;font-weight:normal;color:white">
                                                        <%# Eval("eixo") %>
                                                    </div>
                                                </th>
                                                <th>
                                                    <asp:Chart runat="server" BorderlineWidth="1" Height="340px" Width="460px" BorderlineColor="192, 64, 0"
                                                    DataSource='<%# Eval("subcompetencias") %>' ID="chartSubCompetencias">
                                                    <Series>
                                                        <asp:Series Name="Não possui" XValueMember="subCompetencia" YValueMembers="notas_4_percent" BorderWidth="2" ChartType="StackedBar" Color="#C07681">
                                                        </asp:Series>
                                                        <asp:Series Name="Parcial" XValueMember="subCompetencia" YValueMembers="notas_3_percent" BorderWidth="2" ChartType="StackedBar" Color="#FFC000">
                                                        </asp:Series>
                                                        <asp:Series Name="Total" XValueMember="subCompetencia" YValueMembers="notas_2_percent" BorderWidth="2" ChartType="StackedBar" Color="#355AA5">
                                                        </asp:Series>
                                                    </Series> 
                                                    <ChartAreas>  
                                                        <asp:ChartArea Name="chartResultado">
                                                            <axisy Maximum="1">
                                                                <MajorGrid Enabled ="False" />
                                                                <LabelStyle Format="P0" />
                                                            </axisy>
                                                            <axisx>
                                                                <MajorGrid Enabled="false"/>
                                                            </axisx>
                                                        </asp:ChartArea>  
                                                    </ChartAreas>
                                                    <Legends>
                                                        <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="False" Name="Default" LegendStyle="Row" />
                                                    </Legends>
                                                    <Titles>
                                                        <asp:Title ShadowOffset="3" Name="Items" />
                                                    </Titles>
                                                </asp:Chart> 
                                                </th>
                                            </tr>
                                        </thead>
                                    </table>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                     </div>
                 </div>
             </div>
        </div>

        <%-- CARD RESPOSTAS --%>
        <div class="card">
             <div class="card-body">
                 <div class="form-body">
                     <div class="row">
                        <div class="col-md-12">
                            <label style="font-size:18px;font-weight:bold">Respostas das avaliações</label>
                        </div>
                     </div>
                     <div class="row">
                         <div class="col-md-12">
                            <table class="table table-striped border">
                                <asp:Repeater ID="tabelaRespostas" runat="server">
                                    <ItemTemplate>
                                        <thead>
                                            <tr>
                                                <th rowspan="3" style="width:170px;background-color:#203764;font-weight:normal;color:white;vertical-align:central">Pilar</th>
                                                <th rowspan="3" style="width:170px;background-color:#203764;font-weight:normal;color:white;vertical-align:central">Detalhamento</th>
                                                <asp:Repeater DataSource='<%# Eval("liderados")%>' runat="server">
                                                    <ItemTemplate>
                                                        <th colspan='<%# Eval("numProjetosTabela") %>' style="width:170px;background-color:#203764;font-weight:normal;color:white"><%# Eval("liderado") %></th>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                            <tr>
                                                <asp:Repeater DataSource='<%# Eval("liderados") %>' runat="server">
                                                    <ItemTemplate>
                                                        <asp:Repeater DataSource='<%# Eval("projetos") %>' runat="server">
                                                            <ItemTemplate>
                                                                <th colspan="2" style="width:170px;background-color:#203764;font-weight:normal;color:white"><%# Eval("projeto") %></th>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                            <tr>
                                                <asp:Repeater DataSource='<%# Eval("liderados") %>' runat="server">
                                                    <ItemTemplate>
                                                        <asp:Repeater DataSource='<%# Eval("projetos") %>' runat="server">
                                                            <ItemTemplate>
                                                                    <th style="width:170px;background-color:#203764;font-weight:normal;color:white">Nota liderado</th>
                                                                    <th style="width:170px;background-color:#203764;font-weight:normal;color:white">Comentários</th>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater DataSource='<%# Eval("eixos") %>' runat="server">
                                                <ItemTemplate>
                                                    <asp:Repeater DataSource='<%# Eval("competencias") %>' runat="server">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td style="font-size:14px;vertical-align:middle;text-align:center"><%# Eval("eixo") %></td>
                                                                <td style="font-size:12px"><%# Eval("competencia") %></td>
                                                                <asp:Repeater DataSource='<%# Eval("respostas") %>' runat="server">
                                                                    <ItemTemplate>
                                                                        <td style="vertical-align:middle;text-align:center"><%# Eval("nota") %></td>
                                                                        <td style="vertical-align:middle;text-align:center"><%# Eval("comentario") %></td>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                        </div>
                     </div>
                 </div>
             </div>
        </div>

    </div>
    </div>

    <div id="DIVPAINELNOVO" class="container-fluid">
        <div class="card card-body form-body">
            <h4>Resultado das avaliações de liderança</h4>
            <div class="row">
                <div class="col-sm">
                    <label>Líder: </label>
                    <asp:DropDownList runat="server" ID="ddl_Lider" CssClass="form-control" AutoPostBack="true"
                        OnSelectedIndexChanged="ddlCiclo_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="col-sm">
                    <label>Ciclo: </label>
                    <asp:DropDownList runat="server" ID="ddl_Ciclos" CssClass="form-control" AutoPostBack="true"
                        OnSelectedIndexChanged="ddlCiclo_SelectedIndexChanged">
                        <%--<asp:ListItem Value="" Text="" Enabled="false" />--%>
                    </asp:DropDownList>
                    <div hidden>
                        <asp:Button runat="server" ID="btnHiddenTrigger" />
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <asp:Button runat="server" ID="btn_NotificarLideres" CssClass="btn btn-danger" OnClick="btn_NotificarLideres_Click1" Text="Notificar Líderes" Visible="false"></asp:Button>
            </div>
            <h6>Ao carregar a página ou mudar os filtros, aguarde os painéis atualizarem.</h6>
        </div>
        <div class="card card-body form-body">
            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="updResultados" >
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddl_Ciclos" />
                    <asp:AsyncPostBackTrigger ControlID="ddl_Lider" />
                    <asp:AsyncPostBackTrigger ControlID="btnHiddenTrigger" />
                </Triggers>
                <ContentTemplate>
                    <div style="display:flex;flex-direction:row;align-items: flex-start;justify-content: space-evenly;">
                        <asp:Repeater ID="rptResultadoLider" runat="server">
                            <ItemTemplate>
                                <div style="display:flex;flex-direction:column;align-items: center;gap: 40px">
                                    <div style="display:flex;flex-direction:column;align-items: flex-start;min-height:180px;align-self:center">
                                        <%--HEADER CICLO--%>
                                        <h1 style="text-align:center; align-self:center"><%# Eval("Ciclo")%></h1>

                                        <%--LISTA PROJETOS--%>
                                        <h3 style="text-align:center; align-self:center">PROJETOS AVALIADOS</h3>
                                        <asp:Repeater runat="server" DataSource=<%# Eval("Projetos")%>>
                                            <HeaderTemplate>
                                                <div style="display:flex;flex-direction:column;">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <li style="font-size:1.1em"><%#Eval("Projeto") %></li>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </div>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </div>
                                
                                    <div class="container-graph">
                                        <%--NOTA TOTAL--%>
                                        <h3 style="text-align:center; align-self:center">TOTAL</h3>
                                        <canvas id="graficoNotaTotal_<%#Container.ItemIndex %>" style="width:600px"></canvas>
                                        <br />
                                    </div>

                                    <div class="container-graph">
                                        <%--NOTA PILARES--%>
                                        <h3 style="text-align:center; align-self:center">PILARES</h3>
                                        <canvas id="graficoNotaPilares_<%#Container.ItemIndex %>" style="width:600px; height:500px"></canvas>
                                        <br />
                                    </div>

                                    <div class="container-graph">
                                        <%--NOTA SUBCOMPETENCIAS--%>
                                        <h3 style="text-align:center; align-self:center">SUBCOMPETÊNCIAS</h3>
                                        <canvas id="graficoNotaSubcompetencias_<%#Container.ItemIndex %>" style="width:600px; height:500px"></canvas>
                                        <br />
                                    </div>

                                    <div style="display:flex;flex-direction:row;width:100%;justify-content:center;min-height:550px">
                                        <%--DELTAS--%>
                                        <div style="display:flex;flex-direction:row;width:90%;justify-content:space-around">
                                            <div style="display:flex;flex-direction:column;width:40%;gap:1px">
                                                <h3 style="text-align:center">Maiores notas</h3>
                                                <asp:Repeater ID="rptDeltaMaior" runat="server" DataSource=<%#Eval("deltaMaior") %>>
                                                    <ItemTemplate>
                                                        <div style="width:100%;height:50px;background-color:#021240;color:white;border-radius:4px;text-align:center">
                                                            <label><%#Eval("Subcompetencia") %></label>
                                                            <br />
                                                            <label><%#Eval("Resultado") %></label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                            <div style="display:flex;flex-direction:column;width:40%;gap:1px">
                                                <h3 style="text-align:center">Menores notas</h3>
                                                <asp:Repeater ID="rptDeltaMenor" runat="server" DataSource=<%#Eval("deltaMenor") %>>
                                                    <ItemTemplate>
                                                        <div style="width:100%;height:50px;background-color:#E5F419;color:#021240;border-radius:4px;text-align:center">
                                                            <label><%#Eval("Subcompetencia") %></label>
                                                            <br />
                                                            <label><%#Eval("Resultado") %></label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>
                                    </div>

                                    <div>
                                        <%--NUVEM DE PALAVRAS--%>
                                        <h3 style="text-align:center; align-self:center">COMENTÁRIOS</h3>
                                        <asp:Repeater runat="server" ID="rptNuvemPalavras" DataSource=<%#Eval("Palavras") %>>
                                            <HeaderTemplate>
                                                <ul class="listaCloud">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <li><a data-weight="<%# Eval("DataWeight") %>"> <%# Eval("Palavra") %> </a></li>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </ul>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <link href="dist/custom/resultado.css" rel="stylesheet" />
    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="JS/node_modules/chart.js/dist/chart.umd.js"></script>
    <script src="JS/node_modules/chart.js/auto"></script>
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
    <script>
        (async function () {
            const data = [
                { year: 2010, count: 10 },
                { year: 2011, count: 20 },
                { year: 2012, count: 15 },
                { year: 2013, count: 25 },
                { year: 2014, count: 22 },
                { year: 2015, count: 30 },
                { year: 2016, count: 28 },
            ];

            new Chart(
                document.getElementById('acquisitions'),
                {
                    type: 'bar',
                    data: {
                        labels: data.map(row => row.year),
                        datasets: [
                            {
                                label: 'Acquisitions by year',
                                data: data.map(row => row.count)
                            }
                        ]
                    }
                }
            );
        })();
    </script>
    <script>
        async function helperDDLCiclos() {
            const delay = ms => new Promise(res => setTimeout(res, ms));

            await delay(10);
            var clickButton = document.getElementById("<%= btnHiddenTrigger.ClientID %>");
            clickButton.click();
        }

        async function geraGraficoNotaTotal(idGrafico, data) {
            new Chart(
                document.getElementById('graficoNotaTotal_' + idGrafico),
                {
                    type: 'bar',
                    data: {
                        labels: data.map(row => row.year),
                        datasets: [
                            {
                                label: 'Nota Total',
                                data: data.map(row => row.count)
                            }
                        ]
                    }
                }
            );
        }

        async function geraGraficoNotaPilares(idGrafico, lider, labels, dataLider, dataTodos) {
            new Chart(
                document.getElementById('graficoNotaPilares_' + idGrafico),
                {
                    data: {
                        labels: labels,
                        datasets: [
                            {
                                type: 'bar',
                                label: lider,
                                data: dataLider
                            },
                            {
                                type: 'bar',
                                label: 'Todos',
                                data: dataTodos
                            }
                        ]
                    }
                }
            );
        }

        async function geraGraficoNotaSubcompetencias(idGrafico, lider, labels, dataLider, dataTodos) {
            new Chart(
                document.getElementById('graficoNotaSubcompetencias_' + idGrafico),
                {
                    data: {
                        labels: labels,
                        datasets: [
                            {
                                type: 'radar',
                                label: lider,
                                data: dataLider
                            },
                            {
                                type: 'radar',
                                label: 'Todos',
                                data: dataTodos
                            }
                        ]
                    },
                    options: {
                        scale: {
                            beginAtZero: true,
                            min: 0,
                            max: 100,
                            stepSize: 20
                        }
                    }
                }
            );
        }
    </script>
    <style>
        .listaCloud{
            list-style: none;
            padding-left: 0;

            display: flex;
            flex-wrap: wrap;
            align-items: center;
            justify-content: center;

            line-height: 3.3rem;
            width: 650px;
        }

        .listaCloud a[data-weight="1"] { --size: 1; }
        .listaCloud a[data-weight="2"] { --size: 1; }
        .listaCloud a[data-weight="3"] { --size: 2; }
        .listaCloud a[data-weight="4"] { --size: 3; }
        .listaCloud a[data-weight="5"] { --size: 4; }
        .listaCloud a[data-weight="6"] { --size: 5; }
        .listaCloud a[data-weight="7"] { --size: 6; }
        .listaCloud a[data-weight="8"] { --size: 7; }
        .listaCloud a[data-weight="9"] { --size: 8; }
        .listaCloud a[data-weight="10"] { --size: 9; }
        .listaCloud a[data-weight="11"] { --size: 10; }
        .listaCloud a[data-weight="12"] { --size: 11; }

        .listaCloud a{
            display: block;
            padding: 0.125rem 0.25rem;
            text-decoration: none;
            position: relative;

            --size: 4;
            font-size: calc(var(--size) * 0.70rem + 0.5rem);
            font-weight: calc(var(--size) * 83);
        }
        .container-graph {
            width: 100%;
        }
    </style>
</asp:Content>

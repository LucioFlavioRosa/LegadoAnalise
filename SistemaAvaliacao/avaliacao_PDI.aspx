<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avaliacao_PDI.aspx.cs" Inherits="SistemaAvaliacao.avaliacao_PDI" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Preenchimento PDI</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">PDI</li>
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
            <div class="col-12">
                <div class="card">
                    
                    <div class="card-body">
                        <h4 class="card-title">Períodos</h4>
                        <ul class="nav nav-pills mb-3" id="pills-tab2" role="tablist">
                            <asp:Repeater runat="server" ID="rptPills">
                                <ItemTemplate>
                                    <li class="nav-item">
                                        <a class="nav-link btn btn-facebook <%#Eval("active") %>" id="<%#Eval("id") %>" data-toggle="pill" href="<%#Eval("href") %>" role="tab"
                                            aria-controls="<%#Eval("ariacontrols") %>" aria-selected="<%#Eval("ariaselected") %>"><%#Eval("Periodo") %>
                                        </a>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>

                    <div id="helpersActions" hidden>
                        <asp:TextBox ID="txtAction_IdPDIResposta" runat="server" Text="-1"></asp:TextBox>
                        <asp:TextBox ID="txtAction_Resposta" runat="server" Text="-1"></asp:TextBox>
                        <asp:Button runat="server" ID="btnAction_AtualizaResposta" OnClick="btnAction_AtualizaResposta_Click"/>
                    </div>

                    <div class="card-body">
                        <asp:UpdatePanel runat="server" UpdateMode="Always">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnAction_AtualizaResposta" />
                            </Triggers>
                            <ContentTemplate>
                                
                                <div class="tab-content" id="pills-tabContent">
                                    <asp:Repeater runat="server" ID="rptPeriodos">
                                        <ItemTemplate>
                                            <div class="tab-pane fade <%#Eval("active") %>" id="<%#Eval("id") %>" role="tabpanel" aria-labelledby="<%#Eval("arialabelled") %>">
                                                <h4><%#Eval("Periodo") %></h4>
                                                <div style="display:flex;flex-direction:row;height:900px;width:100%">
                                                    <asp:Repeater runat="server" ID="rptPDIColunas" DataSource=<%#Eval("PDIColunas") %>>
                                                        <ItemTemplate>
                                                            <div style="display:flex;flex-direction:column;flex-grow:1;min-width:16%;max-width:18%">
                                                                <asp:Repeater runat="server" ID="rptPDIRespostas" DataSource=<%#Eval("PDIRespostas") %>>
                                                                    <ItemTemplate>
                                                                        <div style="flex-grow:<%#Eval("FlexGrow") %>;background-color:white;border:solid 0.6em;border-color:#E5F419;<%#Eval("BorderStyle") %>;
                                                                        display:flex;flex-direction:column;justify-content:space-between;gap:2px">
                                                                            <div style="display:flex;flex-direction:row;align-items:center;justify-content:start;min-height:50px">
                                                                                <img width="40px" height="40px" src="<%#Eval("Icone") %>" />
                                                                                <label style="color:<%#Eval("TituloStyle") %>;font-weight:bold"> <%#Eval("Titulo") %> </label>
                                                                            </div>
                                                                            <asp:TextBox ID="txtResposta" TextMode="MultiLine" runat="server" Enabled=<%#Eval("RespostaEnabled") %> Text=<%#Eval("Resposta") %>
                                                                                style="width:90%;align-self:center;height:100%;border:thin solid #F0F0F0" onchange=<%#Eval("OnInput") %>>
                                                                            </asp:TextBox>
                                                                            <label <%#Eval("SubtituloStyle") %>> <%#Eval("Subtitulo") %> </label>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </div>
                                                        </ItemTemplate>
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
            </div>
    </div>

    <script>
        function action_AtualizaResposta(IdPDIResposta, item) {
            document.getElementById('<%=txtAction_IdPDIResposta.ClientID%>').value = IdPDIResposta;
            document.getElementById('<%=txtAction_Resposta.ClientID%>').value = item.value;
            var clickButton = document.getElementById("<%= btnAction_AtualizaResposta.ClientID %>");
            clickButton.click();
        }

        function hold() {

            
        }
    </script>

    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <link href="dist/custom/resultado.css" rel="stylesheet" />
    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/libs/Chart.js-2.9.3/Chart.min.js"></script>
    <script src="dist/js/pages/resultado/resultado.js"></script>
    <!-- apps -->
    <script src="dist/js/app.min.js"></script>
    <script src="dist/js/app-style-switcher.js"></script>
    <script src="assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js"></script>
    <script src="assets/extra-libs/sparkline/sparkline.js"></script>
    <script src="dist/js/waves.js"></script>
    <!--Custom JavaScript -->
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/libs/ckeditor/ckeditor.js"></script>
    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>

    <script src="dist/js/custom.min.js"></script>
</asp:Content>

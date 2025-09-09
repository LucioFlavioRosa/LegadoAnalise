<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="  .aspx.cs" Inherits="SistemaAvaliacao.avaliacao_mentoria" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">AVALIAR MENTORIA</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Mentoria</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Avaliar Mentoria</li>
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
                            <asp:Repeater runat="server" ID="rptPeriodos">
                                <ItemTemplate>
                                    <div class="tab-pane fade <%#Eval("active") %>" id="<%#Eval("id") %>" role="tabpanel" aria-labelledby="<%#Eval("arialabelled") %>">
                                        <h4><%#Eval("Periodo") %></h4>
                                        <h5>As notas e comentários são salvos automaticamente.</h5>
                                        <div style="display:flex;flex-direction:column;gap:10px; background-color: #D0E4EE; border-radius: 0.25em; padding: 1em;">
                                            <div style="display: flex; width: 100%; flex-direction: row; gap: 10px; align-items:center">
                                                <img src="<%#Eval("MentorFoto") %>" width="100" height="100" style="border-radius: 0.25em" />
                                                <label style="font-size: 2em"><%# Eval("MentorNome") %></label>
                                            </div>
                                            <asp:Repeater runat="server" ID="rptRespostasPill" DataSource='<%# Eval("respostas") %>'>
                                                <ItemTemplate>
                                                    <div style="width: 100%; background-color: #E7F1F7; display: flex; flex-direction: row; padding: 0.5em; margin-bottom: 15px; border-radius: 0.25em; gap:10px">
                                                        <div style="display: flex; flex-direction: column; width:75%">
                                                            <label style="font-size: 1.25em"><%# Eval("Pergunta") %></label>
                                                            <div style="display:flex;flex-direction:row;align-items:center;justify-content:space-around;height:100%;gap:4px">
                                                                <asp:Repeater runat="server" ID="rptNotas" DataSource='<%# Eval("notas") %>'>
                                                                    <ItemTemplate>
                                                                        <div style="display:flex;flex-direction:column; border-radius:0.45em; padding:0.2em;flex-basis:100%;max-width:15%;
                                                                                background-color:<%# Eval("BackgroundColor") %>; align-items:center;justify-content:space-evenly;text-align:center; height:100%; 
                                                                                cursor: pointer"
                                                                            onclick=<%# string.Format("atualizaNota({0},{1},{2}); return false;", Eval("IdNota"), Eval("IdResposta"), Eval("Escala")) %>>
                                                                            <label style="font-size: 1em;cursor: pointer"><%# Eval("NotaTexto") %></label>
                                                                            <div <%#Eval("HideIcone") %>>
                                                                                <img src="<%#Eval("Icone") %>" width="20" height="20" style="cursor: pointer"/>
                                                                            </div>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </div>
                                                        </div>
                                                        <div style="display: flex; flex-direction: column; width:25%;align-self:center">
                                                            <label style="font-size: 1em; font-style:italic">Comentários</label>
                                                            <asp:TextBox runat="server" ID="txtComentarios" TextMode="MultiLine" Enabled=<%# Eval("Enabled") %>
                                                                onblur=<%# string.Format("atualizaComentario(this,{0}); return false;", Eval("IdResposta")) %>
                                                                Text=<%# Eval("Comentarios") %>></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
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

</asp:Content>

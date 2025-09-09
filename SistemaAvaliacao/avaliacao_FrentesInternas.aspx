<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avaliacao_FrentesInternas.aspx.cs" Inherits="SistemaAvaliacao.avaliacao_FrentesInternas" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">AVALIAÇÕES ALOCAÇÕES INTERNAS</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Gerenciar Métricas</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Avaliações Alocações Internas</li>
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
        <asp:TextBox runat="server" ID="txtIdAvaliacao"></asp:TextBox>
        <asp:TextBox runat="server" ID="txtIdNota"></asp:TextBox>
        <asp:TextBox runat="server" ID="txtComentarios"></asp:TextBox>
        <asp:TextBox runat="server" ID="txtValidado"></asp:TextBox>
        <asp:Button runat="server" ID="btnAtualizaNota" OnClick="btnAtualizaNota_Click"/>
        <asp:Button runat="server" ID="btnAtualizaComentario" OnClick="btnAtualizaComentario_Click"/>
        <asp:Button runat="server" ID="btnAtualizaValidado" OnClick="btnAtualizaValidado_Click"/>
    </div>

    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" EnableViewState="true">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAtualizaValidado" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:UpdatePanel runat="server" ID="updFrenteInterna" UpdateMode="Conditional" EnableViewState="true">
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
                                        <asp:Repeater runat="server" ID="rptAlocacoesPill" DataSource=<%# Eval("alocacoes") %>>
                                            <ItemTemplate>
                                                <div style="width:100%;background-color:#E7F1F7;display:flex;border-radius:0.25em;display:flex;flex-direction:column;padding-bottom:10px;margin-bottom:15px">
                                                    <div style="width:100%;background-color:#021240;color:white;height:3em;display:flex;align-items:center;padding-top:0.65em;border-radius:0.25em">
                                                    <label style="font-size:1.75em">&nbsp;&nbsp;<%# Eval("Alocacao") %></label>
                                                    </div>
                                                    <div style="margin-left:1.25em;margin-right:5%;padding-top:3px;display:flex;flex-direction:column;gap:15px">
                                                        <asp:Repeater runat="server" ID="rptAvaliacoes" DataSource=<%# Eval("Avaliados") %> OnItemDataBound="rptAvaliacoes_ItemDataBound">
                                                            <ItemTemplate>
                                                                <div style="display:flex;flex-direction:row;align-items:center;gap:10px;justify-content:space-between">
                                                                    <img src=<%# Eval("FotoNome") %> height="70px" width="70px" style="border-radius:7%"/>
                                                                    <label style="font-size:1.45em;width:30%"><%# Eval("Avaliado") %></label>
                                                                    <div style="display:flex;flex-direction:column;flex-grow:1;width:10%">
                                                                        <h4 style="font-style:italic;color:grey">Nota</h4>
                                                                        <asp:DropDownList id="ddlNota" runat="server" class="form-control p-0 select2" style="width: 100%"
                                                                            DataSource=<%# Eval("Notas") %> DataTextField="Descricao" DataValueField="idNotaAlocacaoInterna"
                                                                            onchange='<%# string.Format("atualizaNota({0}, this); return false;", Eval("idAvaliacao")) %>'
                                                                            />
                                                                    </div>
                                                                    <div style="display:flex;flex-direction:column;flex-grow:1;width:30%">
                                                                        <h4 style="font-style:italic;color:grey">Comentários</h4>
                                                                        <asp:TextBox ID="txtComentarios" class="form-control" runat="server" Text=<%# Eval("Comentarios") %> Enabled=<%# Eval("Enabled") %>
                                                                            onchange='<%# string.Format("atualizaComentario({0}, this); return false;", Eval("idAvaliacao")) %>'
                                                                            ></asp:TextBox>
                                                                    </div>
                                                                    <div style="display:flex;flex-direction:column">
                                                                        <h4 style="font-style:italic;color:grey">Validado MD</h4>
                                                                        <input type="checkbox" class="checkboxValid" id="cboxValid2" <%# Eval("ValidadoMD") %>
                                                                            onchange='<%# string.Format("atualizaValidado({0}, this); return false;", Eval("idAvaliacao"))%>'/>
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
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
        function atualizaNota(idAvaliacaoAlocacaoInterna, ddl) {
            document.getElementById('<%=txtIdAvaliacao.ClientID%>').value = idAvaliacaoAlocacaoInterna;
            document.getElementById('<%=txtIdNota.ClientID%>').value = ddl.value;

            var clickButton = document.getElementById("<%= btnAtualizaNota.ClientID %>");
            clickButton.click();
        }

        function atualizaComentario(idAvaliacaoAlocacaoInterna, comentario) {
            document.getElementById('<%=txtIdAvaliacao.ClientID%>').value = idAvaliacaoAlocacaoInterna;
            document.getElementById('<%=txtComentarios.ClientID%>').value = comentario.value;

            var clickButton = document.getElementById("<%= btnAtualizaComentario.ClientID %>");
            clickButton.click();
        }

        function atualizaValidado(idAvaliacaoAlocacaoInterna, input) {
            document.getElementById('<%=txtIdAvaliacao.ClientID%>').value = idAvaliacaoAlocacaoInterna;
            document.getElementById('<%=txtValidado.ClientID%>').value = input.checked;

            var clickButton = document.getElementById("<%= btnAtualizaValidado.ClientID %>");
            clickButton.click();
        }
        
    </script>
    <style>
        .checkboxValid{
            width:1.7em;
            height:1.7em;
            accent-color:#003150;
        }
    </style>

</asp:Content>

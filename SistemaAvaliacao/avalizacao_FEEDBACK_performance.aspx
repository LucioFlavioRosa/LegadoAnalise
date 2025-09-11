<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="avalizacao_feedback_performance.aspx.cs" Inherits="SistemaAvaliacao.avalizacao_feedback_performance" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />

    <!-- ============================================================== -->
    <!-- Bread crumb and right sidebar toggle -->
    <!-- ============================================================== -->
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Feedback</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item"><a href="index.aspx">Avaliação / Feedback</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Performance</li>
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

        <div class="card">
            <div class="card-body">
                <form action="#">
                    <div class="form-body">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="text-left">
                                    <asp:Button ID="btnIrCompetencia" runat="server" Text="Competencia" class="btn btn-danger" OnClick="btnIrCompetencia_Click" />
                                    <button type="button" class="btn btn-facebook" disabled>Performance</button>
                                    <asp:Button ID="btnFinalizar" runat="server" Text="Ir para Finalização" class="btn btn-info" OnClick="btnFinalizar_Click" />
                                    <p></p>
                                </div>
                            </div>
                            <div class="col-md-8">
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
                                    <div class="col-md-3">
                                        <label runat="server" id="lblPeriodo"></label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-2">
                                        <label><b>Projeto</b></label>
                                    </div>
                                    <div class="col-md-4">
                                        <label runat="server" id="lblProjeto"></label>
                                    </div>
                                    <div class="col-md-2">
                                        <label><b>Tempo restante</b></label>
                                    </div>
                                    <div class="col-md-3">
                                        <label runat="server" id="lblTempo"></label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-2">
                                        <label><b>Cliente</b></label>
                                    </div>
                                    <div class="col-md-4">
                                        <label runat="server" id="lblCliente"></label>
                                    </div>
                                    <div class="col-md-2">
                                        <label><b>Gestor</b></label>
                                    </div>
                                    <div class="col-md-3">
                                        <label runat="server" id="lblGestor"></label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>

        <div class="card">
            <div class="card-body">
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-4 offset-md-8">
                            <div class="text-right">
                                <asp:Button ID="btnSalvar" runat="server" Text="Salvar Avaliação" class="btn btn-dark" OnClick="btnSalvar_Click" />
                            </div>
                        </div>
                    </div>
                </div>
                <br />

                <div class="table-responsive">
                    <table class="table table-striped border">
                        <thead>
                            <tr>
                                <th>Performance</th>
                                <th>Abaixo</th>
                                <th>Esperado</th>
                                <th>Acima</th>
                                <th>Nota Avaliado</th>
                                <th>Nota às Cegas</th>
                                <th>Nota Gestor</th>
                                <th>Feedback</th>
                                <th>Observações</th>                              
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptPerformances" runat="server" OnItemDataBound="rptPerformances_ItemDataBound">
                                <ItemTemplate>
                                    <tr <%# Eval("SeparadorAbrangencia") %>>
                                        <td colspan="9" style="text-align:center;font-size:16px"><%# Eval("Abrangencia") %></td>
                                    </tr>
                                    <tr>
                                        <asp:HiddenField runat="server" ID="IdPerformanceItem" Value='<%# Eval("IdPerformance") %>' />
                                        <td>
                                            <%# DataBinder.Eval(Container.DataItem, "Descricao") %>
                                            <br />
                                            <label style="font-size:13px;font-weight:normal;font-style:italic;color:darkred"><%# Eval("DisclaimerInput") %></label>
                                        </td>
                                        <td>
                                            <a href="#abaixo_<%# DataBinder.Eval(Container.DataItem,"IdPerformance") %>" data-target="#abaixo_<%# DataBinder.Eval(Container.DataItem,"IdPerformance") %>" data-toggle="collapse" class="accordion-toggle">
                                                <%# truncaTexto((String)DataBinder.Eval(Container.DataItem, "Abaixo"), 40) %>
                                                <span style="font-weight:bold">  Ver+</span>
                                            </a>
                                        </td>
                                        <td>
                                            <a href="#esperado_<%# DataBinder.Eval(Container.DataItem,"IdPerformance") %>" data-target="#esperado_<%# DataBinder.Eval(Container.DataItem,"IdPerformance") %>" data-toggle="collapse" class="accordion-toggle">
                                                <%# truncaTexto((String)DataBinder.Eval(Container.DataItem, "Esperado"), 40) %>
                                                <span style="font-weight:bold">  Ver+</span>
                                            </a>
                                        </td>
                                        <td>
                                            <a href="#acima_<%# DataBinder.Eval(Container.DataItem,"IdPerformance") %>" data-target="#acima_<%# DataBinder.Eval(Container.DataItem,"IdPerformance") %>" data-toggle="collapse" class="accordion-toggle">
                                                <%# truncaTexto((String)DataBinder.Eval(Container.DataItem, "Acima"), 40) %>
                                                <span style="font-weight:bold">  Ver+</span>
                                            </a>
                                        </td>
                                        <td runat="server" id="lblNotaAvaliado"></td>
                                        <td runat="server" id="lblNotaCegas"></td>
                                        <td runat="server" id="lblNotaGestor"></td>
                                        <td style="width: 120px;">
                                            <select id="ddlNota" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </td>

                                        <td class="text-center">
                                            <button type="button" data-toggle="collapse" data-target="#obs_<%# Eval("IdPerformance") %>" class="accordion-toggle" 
                                                style="background-color:#003150;padding:8px 15px;border:none">
                                                <span aria-hidden="true" style="color:white;font-size:15px">Considerações</span>
                                            </button>
                                        </td>                                      
                                    </tr>
                                    <tr>
                                        <td colspan="9" class="hiddenRow">
                                            <div class="accordian-body collapse" id="abaixo_<%# Eval("IdPerformance") %>">[Abaixo] <%# DataBinder.Eval(Container.DataItem, "Abaixo") %></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="9" class="hiddenRow">
                                            <div class="accordian-body collapse" id="esperado_<%# Eval("IdPerformance") %>">[Esperado] <%# DataBinder.Eval(Container.DataItem, "Esperado") %></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="9" class="hiddenRow">
                                            <div class="accordian-body collapse" id="acima_<%# Eval("IdPerformance") %>">[Acima] <%# DataBinder.Eval(Container.DataItem, "Acima") %></div>
                                        </td>
                                    </tr>                                  
                                    <tr>
                                        <td colspan="9" class="hiddenRow">
                                            <div class="accordian-body collapse" id="obs_<%# Eval("IdPerformance") %>">
                                                <p>
                                                    <strong>Observações do Avaliado:</strong><br>
                                                    <asp:Label ID="lblObservacaoAvaliado" runat="server"></asp:Label>
                                                </p>
                                                <p>
                                                    <strong>Observações do Gestor:</strong><br>
                                                    <asp:Label ID="lblObservacaoGestor" runat="server"></asp:Label>
                                                </p>
                                                <p>
                                                    <strong>Considerações Feedback:</strong><br />
                                                    <textarea id="txtObservacao" runat="server" rows="5" class="form-control"></textarea>
                                                </p>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

                <div class="form-body">
                    <div class="row">
                        <div class="col-md-4 offset-md-8">
                            <div class="text-right">
                                <asp:Button ID="btnSalvar2" runat="server" Text="Salvar Avaliação" class="btn btn-dark" OnClick="btnSalvar_Click" />
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
                        <asp:Button ID="Button1" runat="server" Text="Competencia" class="btn btn-danger" OnClick="btnIrCompetencia_Click" />
                        <button type="button" class="btn btn-facebook" disabled>Performance</button>
                        <asp:Button ID="btn_Finalizar_2" runat="server" Text="Ir para Finalização" class="btn btn-info" OnClick="btnFinalizar_Click" />
                    </div>
                </div>
            </div>
        </div>

    </div>

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

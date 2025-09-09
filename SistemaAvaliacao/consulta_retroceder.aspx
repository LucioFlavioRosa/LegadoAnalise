<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="consulta_retroceder.aspx.cs" Inherits="SistemaAvaliacao.consulta_retroceder" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .content-checkboxes {
            display: flex;
            flex-direction: row;
            gap: 1rem;
        }
    </style>
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Retroceder Avaliações</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Retroceder Avaliações</li>
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

        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Filtro avaliações</h4>
                        <div class="form-body">
                            <div class="row">
                                <div class="col-md-0" hidden="hidden">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>IdAvaliacao</label>
                                            <asp:TextBox ID="ddlAvaliacaoId" Style="width: 100%" runat="server">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Projeto</label>
                                            <select id="ddlProjetos" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Cliente</label>
                                            <select id="ddlClientes" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Associado</label>
                                            <select id="ddlProfissionais" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Período</label>
                                            <select id="ddlPeriodos" runat="server" class="form-control p-0 select2" style="width: 100%" />
                                        </div>
                                    </div>
                                </div>                              
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <div class="form-group">
                                            <label>Fase</label>
                                            <asp:DropDownList 
                                                ID="ddlFase" 
                                                CssClass="form-control p-0" 
                                                Style="width: 100%" 
                                                runat="server">
                                                <asp:ListItem Value="">[Selecionar]</asp:ListItem>
                                                <asp:ListItem Value="AEP">Auto Avaliação e As Cegas</asp:ListItem>
                                                <asp:ListItem Value="AGE">Gestor</asp:ListItem>
                                                <asp:ListItem Value="FED">Feedback</asp:ListItem>
                                                <asp:ListItem Value="AME">Mentor</asp:ListItem>
                                                <asp:ListItem Value="AFI">Finalizada</asp:ListItem>
                                            </asp:DropDownList>                                      
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2 text-right">
                                    <asp:Button runat="server" type="button" ID="Button1" Style="margin-top: 30px"
                                        class="btn btn-danger" OnClick="btnSearch_Click" Text="Listar Avaliações"></asp:Button>
                                </div>
                                <div class="col-md-1 text-right">
                                    <asp:Button runat="server" type="button" ID="btnEfetuarRetrocesso" Style="margin-top: 30px"
                                        class="btn btn-info" OnClick="btnEfetuarRetrocesso_Click" Text="Efetuar Retrocesso" Visible="false"></asp:Button>
                                </div>
                            </div>
                            <%-- Bloco só será exibido se pelo menos um dos checkboxes estiver visível --%>
                            <asp:Panel ID="pnlRetroceder" runat="server" Visible='false'>
                                <div class="row" id="rowRetroceder" runat="server">
                                    <div class="col-md-12">
                                        <h6>Selecione as avaliações que deseja retroceder</h6>
                                    </div>
                                    <div class="col-md-12">
                                        <div class="content-checkboxes">
                                            <asp:CheckBox runat="server" ID="ckb_incluirAutoAvaliacao" Text="Auto Avaliação" AutoPostBack="true" Visible="false"/>
                                            <asp:CheckBox runat="server" ID="ckb_incluirAsCegas" Text="Avaliação às cegas" AutoPostBack="true" Visible="false"/>

                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                            <%--<div class="row" id="rowRetroceder" runat="server">
                                <div class="col-md-12">
                                    <h6>Selecione as avaliações que deseja retroceder</h6>
                                </div>
                                <div class="col-md-2">
                                    <asp:CheckBox runat="server" ID="ckb_incluirAutoAvaliacao" Text="Auto Avaliação" AutoPostBack="true" Visible="false"/>
                                </div>                
                                <div class="col-md-2">
                                    <asp:CheckBox runat="server" ID="ckb_incluirAsCegas" Text="Avaliação às cegas" AutoPostBack="true" Visible="false"/>
                                </div>
                            </div>                            --%>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <div class="table-responsive">
                            <asp:Repeater ID="rptProjetos" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-striped border gridResultado">
                                        <thead>
                                            <tr>
                                                <th>Projeto</th>
                                                <th>Cliente</th>
                                                <th>Associado</th>
                                                <th>Cargo</th>
                                                <th>Gestor</th>
                                                <th>Período</th>
                                                <th>Início</th>
                                                <th>Término</th>
                                                <th>Fase</th>
                                                <th>Ação</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>

                                <ItemTemplate>
                                    <tr>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Projeto.Projeto") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Cliente.Cliente") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.Nome") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Associado.CARGOS.Cargo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Gestor.Nome") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Periodo.Periodo") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "DataInicio") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "DataTermino") %></td>
                                        <td><%# DataBinder.Eval(Container.DataItem, "Fase") %></td>
                                        <td>
                                            <asp:Button type="button" runat="server" ID="btnRetroceder" class="btn btn-dark" Text="Retroceder" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' OnClick="btnRetroceder_Click" />
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
        <!-- ============================================================== -->
        <!-- End Page Content -->
        <!-- ============================================================== -->
        <!-- ============================================================== -->
        <!-- Right sidebar -->
        <!-- ============================================================== -->
        <!-- .right-sidebar -->
        <!-- ============================================================== -->
        <!-- End Right sidebar -->
        <!-- ============================================================== -->
    </div>

    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="assets/extra-libs/datatables.net/Buttons-1.7.0/js/dataTables.buttons.min.js"></script>
    <script src="assets/extra-libs/datatables.net/JSZip-2.5.0/jszip.min.js"></script>
    <script src="assets/extra-libs/datatables.net/Buttons-1.7.0/js/buttons.html5.min.js"></script>
    <script src="assets/extra-libs/datatables.net/Buttons-1.7.0/js/buttons.print.min.js"></script>

    <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>

    <script src="dist/js/pages/consulta/datatable-consulta.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {

            $('#<%= ddlFase.ClientID %>').select2({
                placeholder: "Selecione uma fase",
                allowClear: true
            });


            function toggleCheckboxes() {
                var fase = $('#<%= ddlFase.ClientID %>').val();
                var habilitar = fase === "AEP";
                $('#<%= ckb_incluirAutoAvaliacao.ClientID %>').prop('disabled', !habilitar);
                $('#<%= ckb_incluirAsCegas.ClientID %>').prop('disabled', !habilitar);
                $('#<%= ckb_incluirAutoAvaliacao.ClientID %>').prop('checked', false);
                $('#<%= ckb_incluirAsCegas.ClientID %>').prop('checked', false);
            }
            
            toggleCheckboxes();
            $('#<%= ddlFase.ClientID %>').on('change', function () {
                console.log("chamou change", $("#<%= ddlFase.ClientID %>").val());
                toggleCheckboxes();
            });

            $('select.form-control.select2').select2({
                placeholder: "Selecione",
                allowClear: true
            });
        });
    </script>


</asp:Content>

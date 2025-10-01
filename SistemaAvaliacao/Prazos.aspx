<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Prazos.aspx.cs" Inherits="SistemaAvaliacao.Prazo" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Prazos</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers / Cadastro Básico de Projetos</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Prazos</li>
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

        <%--EDITOR DE CAMPOS--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Dados do Prazo</h4>
                <div class="form-body">
                    <%--TITULO--%>
                    <div class="row">
                        <div class="col-sm">
                            <label>Nome do Disparo</label>
                            <asp:TextBox ID="txt_NomeDisparo" class="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-sm">
                            <label>Status</label>
                            <asp:DropDownList ID="ddlStatus" class="form-control" runat="server">
                                <asp:ListItem Value="1">Ativo</asp:ListItem>
                                <asp:ListItem Value="0">Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <%--LINHA AUTO AVALIACAO--%>
                    <br />
                    <div class="row">
                        <div class="col-sm">
                            <label>Duração Auto Avaliação (em dias)</label>
                            <asp:TextBox ID="txt_Duracao_AUT" class="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-sm">
                            <label>Gatilho Auto Avaliação</label>
                            <select id="ddlGatilho_AUT" runat="server" class="form-control p-0 select2" style="width: 100%" disabled >
                                <option value="0">Ao disparo da auto-avaliação</option>
                            </select>
                        </div>
                        <div class="col-sm">
                            <label>Compensação de Atraso Auto Avaliação (em dias)</label>
                            <asp:TextBox ID="txt_Compensacao_AUT" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <%--LINHA AVALIAÇÃO AS CEGAS--%>
                    <br />
                    <div class="row">
                        <div class="col-sm">
                            <label>Duração Avaliação as Cegas (em dias)</label>
                            <asp:TextBox ID="txt_Duracao_CEG" class="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-sm">
                            <label>Gatilho Avaliação as Cegas</label>
                            <select id="ddlGatilho_CEG" runat="server" class="form-control p-0 select2" style="width: 100%" >
                                <option value="0">Ao disparo da auto-avaliação</option>
                                <option value="1">Ao encerrar a auto-avaliação</option>
                            </select>
                        </div>
                        <div class="col-sm">
                            <label>Compensação de Atraso Avaliação as Cegas (em dias)</label>
                            <asp:TextBox ID="txt_Compensacao_CEG" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <%--LINHA AVALIAÇÃO DO GESTOR--%>
                    <br />
                    <div class="row">
                        <div class="col-sm">
                            <label>Duração Avaliação do Gestor (em dias)</label>
                            <asp:TextBox ID="txt_Duracao_GES" class="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-sm">
                            <label>Gatilho Avaliação do Gestor</label>
                            <select id="ddlGatilho_GES" runat="server" class="form-control p-0 select2" style="width: 100%" >
                                <option value="0">Ao disparo da auto-avaliação</option>
                                <option value="1">Ao encerrar a auto-avaliação</option>
                                <option value="2">Ao encerrar a avaliação as cegas</option>
                            </select>
                        </div>
                        <div class="col-sm">
                            <label>Compensação de Atraso Avaliação do Gestor (em dias)</label>
                            <asp:TextBox ID="txt_Compensacao_GES" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <%--LINHA FEEDBACK--%>
                    <br />
                    <div class="row">
                        <div class="col-sm">
                            <label>Duração Feedback (em dias)</label>
                            <asp:TextBox ID="txt_Duracao_FEE" class="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-sm">
                            <label>Gatilho Feedback</label>
                            <select id="ddlGatilho_FEE" runat="server" class="form-control p-0 select2" style="width: 100%" >
                                <option value="0">Ao disparo da auto-avaliação</option>
                                <option value="1">Ao encerrar a auto-avaliação</option>
                                <option value="2">Ao encerrar a avaliação as cegas</option>
                                <option value="3">Ao encerrar a avaliação do gestor</option>
                            </select>
                        </div>
                        <div class="col-sm">
                            <label>Compensação de Atraso Feedback (em dias)</label>
                            <asp:TextBox ID="txt_Compensacao_FEE" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <%--LINHA MENTOR--%>
                    <br />
                    <div class="row">
                        <div class="col-sm">
                            <label>Duração Consolidação do Mentor (em dias)</label>
                            <asp:TextBox ID="txt_Duracao_MEN" class="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-sm">
                            <label>Gatilho Consolidação do Mentor</label>
                            <select id="ddlGatilho_MEN" runat="server" class="form-control p-0 select2" style="width: 100%" >
                                <option value="0">Ao disparo da auto-avaliação</option>
                                <option value="1">Ao encerrar a auto-avaliação</option>
                                <option value="2">Ao encerrar a avaliação as cegas</option>
                                <option value="3">Ao encerrar a avaliação do gestor</option>
                                <option value="4">Ao encerrar o feedback</option>
                            </select>
                        </div>
                        <div class="col-sm">
                            <label>Compensação de Atraso Consolidação do Mentor (em dias)</label>
                            <asp:TextBox ID="txt_Compensacao_MEN" class="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <br />
                <div class="form-actions">
                    <div class="text-right">
                        <asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />
                    </div>
                </div>
            </div>
        </div>

        <%--LISTA DE PRAZOS--%>
        <div class="card">
            <div class="card-body">
                <h4 class="card-title">Lista de Prazos</h4>
                <div class="table-responsive">
                <asp:HiddenField ID="hdId" runat="server" />
                <asp:Repeater ID="rptPrazos" OnItemCommand="rptEixo_ItemCommand" OnItemDataBound="rptEixo_ItemDataBound" runat="server">
                    <HeaderTemplate>
                        <table class="table table-striped border dtInit">
                            <thead>
                                <tr>
                                    <th>IdPrazo</th>
                                    <th>Nome Disparo</th>
                                    <th>Status(Ativo/Inativo)</th>
                                    <th data-sort="0"></th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Label ID="lblIdPrazo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IdPrazo") %>' Visible="false"></asp:Label>
                                <%# DataBinder.Eval(Container.DataItem, "IdPrazo") %>
                            </td>
                            <td><%# DataBinder.Eval(Container.DataItem, "NomeDisparo") %></td>
                            <td><asp:Label ID="lblATV" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ATV") %>'></asp:Label></td>
                            <td>
                                <asp:Button ID="btnAlterar" class="btn btn-info" runat="server" Text="Alterar" CommandName="AlterarPrazo" />
                                <asp:Button ID="btnDeletar" class="btn btn-dark" runat="server" Text="Inativar" CommandName="InativarPrazo" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
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
    <!--<script src="assets/libs/popper.js/dist/umd/popper.min.js"></script>
    <script src="assets/libs/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- apps -->
    <script src="dist/js/app.min.js"></script>
    <script src="dist/js/app.init.js"></script>
    <script src="dist/js/app-style-switcher.js"></script>
    <script src="assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js"></script>
    <script src="assets/extra-libs/sparkline/sparkline.js"></script>
    <script src="dist/js/waves.js"></script>
    <!--<script src="dist/js/sidebarmenu.js"></script>
    <!--Custom JavaScript -->
    <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
    <script src="dist/js/custom.min.js"></script>


</asp:Content>

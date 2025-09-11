<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TotalizadorAvaliacao.aspx.cs" Inherits="SistemaAvaliacao.TotalizadorAvaliacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Totalizador das Avaliações</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active">Totalizador das Avaliações</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>




    <div class="page-content container-fluid">




        <div class="card">

            <div class="card-body">
                <h4 class="card-title">Dados do Associado</h4>
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Nome</label>
                                <asp:TextBox ID="txtNome" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Mentor</label>
                                <asp:DropDownList ID="ddlMentor" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>E-mail</label>
                                <asp:TextBox ID="txtEmail" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Senha</label>
                                <asp:TextBox ID="txtSenha" type="password" class="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Cargo <a href="javascript:void(0)" data-toggle="modal" data-target="#cargo-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                                <asp:DropDownList ID="ddlCargoAssociado" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Perfil de acesso <a href="javascript:void(0)" data-toggle="modal" data-target="#perfil-modal" class="text-right ml-2"><i class="fa fa-plus" title="Adicionar"></i></a></label>
                                <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="form-control p-0 select2" Style="width: 100%">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Status</label>
                                <asp:DropDownList ID="ddlStatus" CssClass="form-control p-0 select2" Style="width: 100%" runat="server">

                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="0">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="form-actions">
                        <div class="text-right">

                            <asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Cadastrar/Salvar" />
                        </div>
                    </div>
                </div>

                <script src="assets/libs/jquery/dist/jquery.min.js"></script>
                <!-- <script src="assets/libs/popper.js/dist/umd/popper.min.js"></script>
    <script src="assets/libs/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- apps -->
                <script src="dist/js/app.min.js"></script>
                <script src="dist/js/app.init.js"></script>
                <script src="dist/js/app-style-switcher.js"></script>
                <script src="assets/libs/perfect-scrollbar/dist/perfect-scrollbar.jquery.min.js"></script>
                <script src="assets/extra-libs/sparkline/sparkline.js"></script>
                <script src="dist/js/waves.js"></script>
                <!-- <script src="dist/js/sidebarmenu.js"></script>
    <!--Custom JavaScript -->
                <script src="assets/extra-libs/datatables.net/js/jquery.dataTables.min.js"></script>
                <script src="assets/libs/ckeditor/ckeditor.js"></script>
                <script src="assets/libs/select2/dist/js/select2.full.min.js"></script>

                <script src="dist/js/custom.min.js"></script>
                <script>
                    CKEDITOR.replace('editor1', {
                        height: 150
                    });
                </script>
            </div>

        </div>

    </div>

    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />

</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="comentarios.aspx.cs" Inherits="SistemaAvaliacao.comentarios" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler" />
    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-3 col-md-4 col-xs-12 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Preenchimento Comentários</h5>
            </div>
            <div class="col-lg-9 col-md-8 col-xs-12 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active" aria-current="page">Comentários</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>

    <div class="page-content container-fluid">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h2>Deixe seus comentários sobre a ferramenta.</h2>
                        <asp:TextBox runat="server" id="txtComentarios" TextMode="MultiLine" style="height:400px;width:100%"></asp:TextBox>
                        <asp:Button runat="server" ID="btnEnviarComentarios" Text="Enviar" CssClass="btn btn-facebook" OnClick="btnEnviarComentarios_Click" />
                    </div>
                </div>
            </div>
    </div>

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

<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="SistemaAvaliacao.Index" EnableEventValidation="false" %>

<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <uc1:MessageBoxHandler runat="server" ID="MessageBoxHandler1" />

    <div class="page-breadcrumb border-bottom">
        <div class="row">
            <div class="col-lg-6 align-self-center">
                <h5 class="font-medium text-uppercase mb-0">Página Inicial - Navegue nos Botões abaixo ou no Menu ao lado</h5>
            </div>
            <div class="col-lg-5 col-md-8 col-xs-5 align-self-center">
                <nav aria-label="breadcrumb" class="mt-2 float-md-right float-left">
                    <ol class="breadcrumb mb-0 justify-content-end p-0">
                        <li class="breadcrumb-item"><a href="index.aspx">Peers</a></li>
                        <li class="breadcrumb-item active">Página Inicial</li>
                    </ol>
                </nav>
            </div>
        </div>
    </div>

    <div class="page-content container-fluid">

        <asp:Panel runat="server" ID="panelMenu">
            <div class="card">
                <div class="card-body">
                    <h4 class="card-title">Cadastro Básico de Associados</h4>
                    <div class="form-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Button OnClick="btnAssociado_Click" ID="btnAssociado" class="btn btn-info" runat="server" Text="Associados" />
                                    <asp:Button OnClick="btnCargos_Click" ID="btnCargos" class="btn btn-info" runat="server" Text="Cargos" />
                                    <asp:Button OnClick="btnPAcesso_Click" ID="btnPAcesso" class="btn btn-info" runat="server" Text="Perfil de Acesso" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-body">
                    <h4 class="card-title">Cadastro Básico de Projetos</h4>
                    <div class="form-body">
                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:Button OnClick="btnClientes_Click" ID="btnClientes" class="btn btn-info" runat="server" Text="Clientes" />
                                    <asp:Button OnClick="btnCadastrar_Click" ID="btnCadastrar" class="btn btn-info" runat="server" Text="Tipos de Projetos" />
                                    <asp:Button OnClick="btnComplexidade_Click" ID="btnComplexidade" class="btn btn-info" runat="server" Text="Complexidade" />
                                    <asp:Button OnClick="btnEixo_Click" ID="btnEixo" class="btn btn-info" runat="server" Text="Eixo" />
                                    <asp:Button OnClick="btnDimensoes_Click" ID="btnDimensoes" class="btn btn-info" runat="server" Text="Dimensões" />
                                    <asp:Button OnClick="btnSubCompetencia_Click" ID="btnSubCompetencia" class="btn btn-info" runat="server" Text="Sub Competências" />
                                    <asp:Button OnClick="btnCompetencias_Click" ID="btnCompetencias" class="btn btn-info" runat="server" Text="Competências" />
                                    <asp:Button OnClick="btnPerformance_Click" ID="btnPerformance" class="btn btn-info" runat="server" Text="Performance" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-body">
                    <h4 class="card-title">Cadastro de Projetos</h4>
                    <div class="form-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Button OnClick="btnProjetos_Click" ID="btnProjetos" class="btn btn-info" runat="server" Text="Projetos" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-body">

                    <h4 class="card-title">Envio de Avaliações</h4>
                    <div class="form-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Button OnClick="btnEnvioAvaliacoes_Click" ID="btnEnvioAvaliacoes" class="btn btn-info" runat="server" Text="Envio de Avaliações" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </asp:Panel>

        <div class="card" id="divAcessoRapido" runat="server">
            <div class="card-body">
                <h4 class="card-title">Acesso rápido</h4>
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Button ID="btnPendencias" class="btn btn-dark" runat="server" Text="Pendências" OnClick="btnPendencias_Click" />
                                <asp:Button ID="btnGerenciarProjetos" class="btn btn-dark" runat="server" Text="Projetos" OnClick="btnGerenciarProjetos_Click" />
                                <asp:Button ID="btnFrenteInterna" Visible="false" class="btn btn-dark" runat="server" Text="Alocações Internas" OnClick="btnFrenteInterna_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card"  id="divAcessoAvaliacoes" runat="server">
            <div class="card-body">
                <h4 class="card-title">Avaliações</h4>
                <div class="form-body">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Button ID="btnAutoAvaliacao" class="btn btn-info" runat="server" Text="Auto Avaliação" OnClick="btnAutoAvaliacao_Click" />
                                <asp:Button ID="btnEvolucaoAssociado" class="btn btn-info" runat="server" Text="Evolução Associado" OnClick="btnEvolucaoAssociado_Click" />
                                <asp:Button ID="btnAvaliacoesLideranca" class="btn btn-info" runat="server" Text="Avaliações de Liderança" OnClick="btnGestor_Click" />
                                <div runat="server" id="divAvaliacao">
                                    <asp:Button ID="btnCegas" class="btn btn-info" runat="server" Text="Avaliação às Cegas" OnClick="btnCegas_Click" />
                                    <asp:Button ID="btnGestor" class="btn btn-info" runat="server" Text="Avaliação do Gestor" OnClick="btnGestor_Click" />
                                    <asp:Button ID="btnFeedback" class="btn btn-info" runat="server" Text="Feedback" OnClick="btnFeedback_Click" />
                                    <br /><br />
                                    <asp:Button ID="btnResultadoLideranca" class="btn btn-info" runat="server" Text="Resultado Av. de Liderança" OnClick="btnResultadoLideranca_Click" />
                                </div>
                                <br />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card" id="divMentoria" runat="server">
            <div class="card-body">
                <h4 class="card-title">Mentoria</h4>
                <div class="form-body">
                    <div class="row" runat="server" id="divMentor">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Button ID="btnMentor" class="btn btn-info" runat="server" Text="Cartilha do Mentor" OnClick="btnMentor_Click" />
                                <asp:Button ID="btnResultadoMentor" class="btn btn-info" runat="server" Text="Resultado Mentoria" OnClick="btnResultadoMentor_Click"/>
                            </div>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divMentorado">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Button ID="btnPDI" class="btn btn-info" runat="server" Text="PDI" OnClick="btnPDI_Click" />
                                <asp:Button ID="btnAvMentor" class="btn btn-info" runat="server" Text="Avaliar Mentoria" OnClick="btnAvMentor_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    
        <div class="card" id="divAlterarSenha" runat="server">
            <h4 class="modal-title" style="text-align:center">Sua senha foi reiniciada</h4>
            <h4 class="modal-title" style="text-align:center">Altere-a antes de continuar utilizando o portal</h4>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <div class="form-body">
                        <div class="row" hidden>
                            <div class="col-md-2"></div>
                            <div class="col-md-8">
                                <div class="input-group mb-3">
                                    <div class="input-group-prepend">
                                        <span class="input-group-text" id="basic-addon1"><i class="ti-pencil"></i></span>
                                    </div>
                                    <input runat="server" id="txtSenha" type="password" class="form-control form-control-lg" placeholder="Senha Atual" aria-label="Senha Atual" aria-describedby="basic-addon1">
                                </div>
                            </div>
                            <div class="col-md-2"></div>
                        </div>
                        <div class="row">
                            <div class="col-md-2"></div>
                            <div class="col-md-8">
                                <div class="input-group mb-3">
                                    <div class="input-group-prepend">
                                        <span class="input-group-text" id="basic-addon2"><i class="ti-lock"></i></span>
                                    </div>
                                    <input runat="server" id="txtNovaSenha" type="password" class="form-control form-control-lg" placeholder="Nova Senha" aria-label="Nova Senha" aria-describedby="basic-addon2">
                                </div>
                            </div>
                            <div class="col-md-2"></div>
                        </div>
                        <div class="row">
                            <div class="col-md-2"></div>
                            <div class="col-md-8">
                                <div class="input-group mb-3">
                                    <div class="input-group-prepend">
                                        <span class="input-group-text" id="basic-addon3"><i class="ti-check"></i></span>
                                    </div>
                                    <input runat="server" id="txtConfirmaSenha" type="password" class="form-control form-control-lg" placeholder="Confirme a Senha" aria-label="Confirme a Senha" aria-describedby="basic-addon3">
                                </div>
                            </div>
                            <div class="col-md-2"></div>
                        </div>
                    </div>
                    <div class="modal-footer" style="place-content:center">
                        <div class="text-left">
                            <asp:Label ID="lblMensagem" runat="server" Text="" ForeColor="DarkRed" />
                        </div>
                        <asp:Button runat="server" ID="btnAlterar" OnClick="btnAlterar_Click" type="button" class="btn btn-danger" Text="Alterar"></asp:Button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

    <script>
        function hold() {
            document.getElementById("btnFecharAlterarSenha").visibility = 'hidden';
            var clickButton = document.getElementById("btnAlterarSenha");
            clickButton.click();
        }

        function VerificaSenha() {
            alert('123');
        }

        jQuery(document).ready(function () {
            VerificaSenha();
        });
    </script>

</asp:Content>

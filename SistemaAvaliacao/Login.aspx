<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SistemaAvaliacao.Login" %>
<%@ Register Src="~/UserControls/MessageBoxHandler.ascx" TagPrefix="uc1" TagName="MessageBoxHandler" %>

<!DOCTYPE html>
<html dir="ltr">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <!-- Tell the browser to be responsive to screen width -->
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">
    <!-- Favicon icon -->
    <link rel="icon" type="image/png" sizes="16x16" href="assets/images/favicon.png">
    <title>Peers - Login</title>
    <!-- Custom CSS -->
    <link href="dist/css/style.min.css" rel="stylesheet">
    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
    <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
    <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
<![endif]-->
</head>

<body>
    <div class="main-wrapper">
        <!-- ============================================================== -->
        <!-- Preloader - style you can find in spinners.css -->
        <!-- ============================================================== -->
        <div class="preloader">
            <div class="lds-ripple">
                <div class="lds-pos"></div>
                <div class="lds-pos"></div>
            </div>
        </div>
        <!-- ============================================================== -->
        <!-- Preloader - style you can find in spinners.css -->
        <!-- ============================================================== -->
        <!-- ============================================================== -->
        <!-- Login box.scss -->
        <!-- ============================================================== -->
        <div class="auth-wrapper d-flex no-block justify-content-center align-items-center flex-column" style="background:url(assets/images/big/auth-bg.jpg) no-repeat center center;">
            <div class="auth-box">
                <div id="loginform">
                    <div class="logo">
                        <span class="db"><img src="assets/images/logos/logo-icon.png" alt="logo" /></span>
                        <h2 class="font-medium my-3">Bem vindo ao Sistema de Avaliação!</h2>
                    </div>
                    <!-- Form -->
                    <div class="row">
                        <div class="col-12">
                            <form class="form-horizontal mt-3" id="loginform" runat="server">
                                <asp:ScriptManager runat="server"></asp:ScriptManager>
                                <uc1:messageboxhandler runat="server" id="MessageBoxHandler" />
                                <div class="form-group row">
                                    <div class="col-md-12">
                                        <div class="custom-control custom-checkbox">
                                            <input type="checkbox" class="custom-control-input" id="customCheck1">
                                        </div>
                                    </div>
                                </div>
                                <div class="form-group text-center">
                                    <div class="col-xs-12 pb-3">
                                        <asp:Button runat="server" ID="btnLoginSSO" class="btn btn-block btn-lg btn-info" Text="Entrar com minha conta corporativa" OnClick="btnLoginSSO_Click" />
                                    </div>
                                </div>
                           </form>
                        </div>
                    </div>                     
                </div>               
                
                <%--<div id="recoverform">
                    <div class="logo">
                        <span class="db"><img src="assets/images/logos/logo-icon.png" alt="logo" /></span>
                        <h5 class="font-medium mb-3">Recuperar a senha</h5>
                        <span>Informe seu e-mail e siga as instru&ccedil;&otilde;es!</span>
                    </div>
                    <div class="row mt-3">
                        <!-- Form -->
                        <form class="col-12" action="index.html">
                            <!-- email -->
                            <div class="form-group row">
                                <div class="col-12">
                                    <input class="form-control form-control-lg" type="email" required="" placeholder="E-mail">
                                </div>
                            </div>
                            <!-- pwd -->
                            <div class="row mt-3">
                                <div class="col-12">
                                    <button class="btn btn-block btn-lg btn-danger" type="submit" name="action">Enviar</button>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>--%>
            </div>
            <div class="row">
               <div class="col-12">
                   <div>
                       <h5 class="font-medium mt-5 text-center">
                           <asp:Label ID="lblVersion" runat="server" />
                       </h5>                            
                   </div>
               </div>
            </div>
        </div>
        <!-- Rodapé com links dinâmicos -->
        <div class="footer-links" style="background: #e6f1f9; padding: 20px 0; position: fixed;bottom: 0;width: 100%;">
            <div style="display: flex; justify-content: center;">
                <asp:Repeater ID="rptLinks" runat="server">
                    <ItemTemplate>
                        <div style="text-align: center; margin: 0 24px;">
                            <a href='<%# Eval("Url") %>' target="_blank" style="text-decoration: none; color: inherit;">
                                <img src='<%# Eval("Icon") %>' alt='<%# Eval("Name") %>' style="width:32px; height:32px; display:block; margin:0 auto 4px auto;" />
                                <span style="font-size: 16px;"><%# Eval("Name") %></span>
                            </a>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <!-- ============================================================== -->
        <!-- Login box.scss -->
        <!-- ============================================================== -->
        <!-- ============================================================== -->
        <!-- Page wrapper scss in scafholding.scss -->
        <!-- ============================================================== -->
        <!-- ============================================================== -->
        <!-- Page wrapper scss in scafholding.scss -->
        <!-- ============================================================== -->
        <!-- ============================================================== -->
        <!-- Right Sidebar -->
        <!-- ============================================================== -->
        <!-- ============================================================== -->
        <!-- Right Sidebar -->
        <!-- ============================================================== -->
    </div>
    <!-- ============================================================== -->
    <!-- All Required js -->
    <!-- ============================================================== -->
    <script src="assets/libs/jquery/dist/jquery.min.js"></script>
    <!-- Bootstrap tether Core JavaScript -->
    <script src="assets/libs/popper.js/dist/umd/popper.min.js"></script>
    <script src="assets/libs/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- ============================================================== -->
    <!-- This page plugin js -->
    <!-- ============================================================== -->
    <script>
    $('[data-toggle="tooltip"]').tooltip();
    $(".preloader").fadeOut();
    // ============================================================== 
    // Login and Recover Password 
    // ============================================================== 
    $('#to-recover').on("click", function() {
        $("#loginform").slideUp();
        $("#recoverform").fadeIn();
    });
    </script>
</body>

</html>

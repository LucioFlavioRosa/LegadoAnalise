<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MessageBoxHandler.ascx.cs" Inherits="SistemaAvaliacao.UserControls.MessageBoxHandler" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<link href="assets/extra-libs/datatables.net-bs4/css/dataTables.bootstrap4.css" rel="stylesheet">
<link rel="stylesheet" type="text/css" href="assets/libs/ckeditor/samples/toolbarconfigurator/lib/codemirror/neo.css">
<link rel="stylesheet" type="text/css" href="assets/libs/select2/dist/css/select2.min.css">
<link href="dist/css/style.min.css" rel="stylesheet">

<style type="text/css">
    .Shadow
    {
        box-shadow: 6px 6px 10px #333333;
        -webkit-box-shadow: 6px 6px 10px #333333;
        -moz-box-shadow: 6px 6px 10px #333333;
        border-radius: 0.1em;
    }
    
    .modalBackground
    {
        background-color: #999;
        filter: alpha(opacity-]=70);
        opacity: 0.7;
    }
    
    .pnMessage
    {
        border-radius: 0.1em;
        -moz-border-radius: 0.1em;
        -webkit-border-radius: 0.1em;
        box-shadow: 6px 6px 10px #333333;
        -webkit-box-shadow: 6px 6px 10px #333333;
        -moz-box-shadow: 6px 6px 10px #333333;
        font-family: Segoe UI, Calibri, Tahoma, Geneva, sans-serif; /*position: absolute;*/
        left: 1em;
        top: 2em;
        z-index: 99;
        margin-left: 0;
        width: 250px;
        font-size: 12pt;
    }
</style>

<asp:Panel runat="server" ID="pnMensagens" CssClass="pnMessage" Style="width: 300px;
    display: none; background-color: #F7F7F7; border-width: 2px; border-color: #999999;
    border-style: solid; padding: 20px;">
    <div style="text-align: center">
        <div style="vertical-align: middle; margin-left: auto; margin-right: auto; text-align: center;">
            <asp:Image runat="server" ID="imgPop" ImageUrl="~/Images/dialog-message-icon.png" />
        </div>
        <div style="vertical-align: middle; padding-top: 5px; margin-left: auto; margin-right: auto;
            text-align: center;">
            <asp:Label runat="server" ID="lblHeader" Text="Header" Font-Bold="True" ForeColor="Black" />
            <br />
            <br />
            <asp:Label runat="server" ID="lblMensagens" Text="Ty" Font-Bold="False" ForeColor="Black" />
            <br />
            <br />
            <asp:Button runat="server" ID="btnConfirma" Text="OK" class="btn btn-dark" />
        </div>
    </div>
</asp:Panel>

<!-- MSG -->
<ajax:ModalPopupExtender runat="server" ID="ModalMsg" PopupControlID="pnMensagens"
    OkControlID="btnConfirma" TargetControlID="hfGo" BackgroundCssClass="modalBackground"
    BehaviorID="ModalMsg" Enabled="True">
</ajax:ModalPopupExtender>
<asp:HiddenField runat="server" ID="hfGo" />
<!---->

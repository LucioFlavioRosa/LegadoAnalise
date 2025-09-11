<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="SistemaAvaliacao.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.<asp:Button ID="btnCargo" runat="server" Font-Size="Small" Height="24px" Text="Digite o Codigo" Width="96px" />
        <asp:TextBox ID="txtIdCargo" runat="server" Height="20px" Width="63px"></asp:TextBox>
        <asp:Label ID="lclCargo" runat="server" Text="Cargo:"></asp:Label>
    </h2>
    <h3>Your contact page.</h3>
    <address>
        One Microsoft Way<br />
        Redmond, WA 98052-6399<br />
        <abbr title="Phone">P:</abbr>
        425.555.0100
    </address>

    <address>
        <strong>Support:</strong>   <a href="mailto:Support@example.com">Support@example.com</a><br />
        <strong>Marketing:</strong> <a href="mailto:Marketing@example.com">Marketing@example.com</a>
    </address>
</asp:Content>

using AjaxControlToolkit;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaAvaliacao.UserControls
{
    public enum TIPO
    {
        Warning,
        Info,
        Error,
        Default
    }

    public static class MessageBox
    {
        public static void Show(string mensagem, string titulo, TIPO tipo, UserControl ucMensagem)
        {
            Image _img = new Image();
            Label _header = new Label();
            Label _mensagem = new Label();
            Panel _pn = new Panel();

            Style _style = new Style();
            _style.CssClass = "pnMessage";

            _pn = ucMensagem.FindControl("pnMensagens") as Panel;
            _pn.ApplyStyle(_style);

            _img = ucMensagem.FindControl("imgPop") as Image;
            _header = ucMensagem.FindControl("lblHeader") as Label;
            _mensagem = ucMensagem.FindControl("lblMensagens") as Label;

            switch (tipo)
            {
                case TIPO.Info:
                    _img.ImageUrl = "~/Images/dialog-information-icon.png";
                    if (titulo.Trim() == "") titulo = "Aviso";
                    break;
                case TIPO.Warning:
                    _img.ImageUrl = "~/Images/dialog-warning-icon.png";
                    if (titulo.Trim() == "") titulo = "Atenção";
                    break;
                case TIPO.Error:
                    _img.ImageUrl = "~/Images/dialog-error-icon.png";
                    if (titulo.Trim() == "") titulo = "ERRO";
                    break;
                default:
                    _img.ImageUrl = "~/Images/dialog-message-icon.png";
                    if (titulo.Trim() == "") titulo = "Mensagem";
                    break;
            }

            _header.Text = titulo.Trim();
            _mensagem.Text = mensagem.Trim();

            ModalPopupExtender _modalMsg = new ModalPopupExtender();
            _modalMsg = ucMensagem.FindControl("ModalMsg") as ModalPopupExtender;
            _modalMsg.Show();
        }
    }
}
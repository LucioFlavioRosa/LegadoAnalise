using Business.DataAccess;
using Business.Model;
using Business.Services;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using SistemaAvaliacao.UserControls;

namespace SistemaAvaliacao
{
    public partial class comentarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var periodoUltimo = new PeriodoService().ObterPeriodoUltimo();
                var comentariosService = new ComentariosService();
                ValidaComentario();

                var getComentario = comentariosService.ObterComentarios(idAssociado: WebStorage.GetUsuarioLogado().Id, idPeriodo: periodoUltimo.IdPeriodo)[0];
                txtComentarios.Text = getComentario.Comentario;
            }
        }

        public void ValidaComentario()
        {
            var periodoUltimo = new PeriodoService().ObterPeriodoUltimo();
            var comentariosService = new ComentariosService();

            var getComentario = comentariosService.ObterComentarios(idAssociado: WebStorage.GetUsuarioLogado().Id, idPeriodo: periodoUltimo.IdPeriodo);
            if (getComentario == null || getComentario.Count <= 0)
            {
                var addComentario = new COMENTARIOS();
                addComentario.idPeriodo = periodoUltimo.IdPeriodo;
                addComentario.idAssociado = WebStorage.GetUsuarioLogado().Id;
                addComentario.Comentario = "";
                addComentario.DHC = DateTime.Now;

                comentariosService.GerirComentario(addComentario);
            }
        }

        protected void btnEnviarComentarios_Click(object sender, EventArgs e)
        {
            var periodoUltimo = new PeriodoService().ObterPeriodoUltimo();
            var comentariosService = new ComentariosService();
            var getComentario = comentariosService.ObterComentarios(idAssociado: WebStorage.GetUsuarioLogado().Id, idPeriodo: periodoUltimo.IdPeriodo)[0];

            string newComentario = txtComentarios.Text != null ? txtComentarios.Text : getComentario.Comentario;
            getComentario.DHC = DateTime.Now;
            getComentario.Comentario = newComentario;
            comentariosService.GerirComentario(getComentario);

            MessageBox.Show("Comentários enviados.<br/>Obrigado 😃", "Sucesso", TIPO.Info, MessageBoxHandler);

            var redirectUrl = WebStorage.Get("redirectUrl", "index");
            Response.Redirect(redirectUrl);
        }
    }
}
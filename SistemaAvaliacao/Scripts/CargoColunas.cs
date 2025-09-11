using Business.DataAccess;
using Business.Services;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;

namespace SistemaAvaliacao.Scripts
{
    public class CargoColunas
    {
        static public void ConferirCargoColuna(object sender, string colName, string colType)
        {
            // DESCONTINUADO
            /*var usuario = new AssociadosService().ObterAssociado(WebStorage.GetUsuarioLogado().Id);
            List<CARGOSCOLUNAS> listaCargosColunas = new CargosService().ObterListaCargosColunas();
            CARGOSCOLUNAS cargoColuna;

            string texto = sender.ToString();
            HtmlTableCell targetColumn = (HtmlTableCell)sender;
            bool esconder = false;

            cargoColuna = listaCargosColunas.Find(x => x.fkCargo == usuario.IdCargo);
            if (cargoColuna != null)
            {
                if (cargoColuna.colunaAvaliador == 0 && colName == "avaliador") { esconder = true; }
                else if (cargoColuna.colunaMentor == 0 && colName == "mentor") { esconder = true; }

                if (esconder)
                {
                    targetColumn.Visible = false;
                }
            }*/

        }

        static public void ConferirTipoAvaliacaoCampos(object sender, string TipoAvaliacao)
        {
            if (TipoAvaliacao == "lideranca")
            {
                HtmlTableCell targetColumn = (HtmlTableCell)sender;
                targetColumn.Visible = false;
            }
        }

        static public void ConferirTipoAvaliacaoCampos_Reverso(object sender, string TipoAvaliacao)
        {
            if (TipoAvaliacao == "desempenho")
            {
                HtmlTableCell targetColumn = (HtmlTableCell)sender;
                targetColumn.Visible = false;
            }
        }
    }
}
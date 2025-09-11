using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;

namespace Business.Model
{
    public class CompetenciaModel
    {
        public int IdCompetencia { get; set; }
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public int IdEixo { get; set; }
        public string Eixo { get; set; }
        public int IdSubCompetencia { get; set; }
        public string SubCompetencia { get; set; }
        public int IdDimensao { get; set; }
        public string Dimensao { get; set; }
        public string DetalheNivelAtual { get; set; }
        public string CompetenciaAtual { get; set; }
        public string CompetenciaProximo { get; set; }
        public string DetalheProximoNivel { get; set; }
        public int IdModo { get; set; }
        public int ATV { get; set; }
        public string PalavrasChave { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public string IsHidden
        {
            get
            {
                var retorno = "";

                if (String.IsNullOrEmpty(Eixo?.Trim()) && String.IsNullOrEmpty(SubCompetencia?.Trim()) && String.IsNullOrEmpty(Dimensao?.Trim()))
                    retorno = "hidden";

                return retorno;
            }
        }
        public string IsHiddenDetalhamentoProximoNivel { get; set; }

        // VALIDAR NOTAS DO MESMO PILAR
        public int idAvaliacaoCompetencia { get; set; }
        public int notaValidaNivel1 { get; set; }
        public int notaValidaNivel2 { get; set; }
        public HtmlSelect ddl1 { get; set; }
        public HtmlSelect ddl2 { get; set; }

        // NOVAS REGRAS DE AUTO PREENCHIMENTO
        public string hiddenSelectNivel1 { get; set; }
        public string hiddenTextBoxNivel1 { get; set; }
        public string disableSelectNivel1 { get; set; }
        public string textoNotaNivel1 { get; set; }
        public string hiddenSelectNivel2 { get; set; }
        public string hiddenTextBoxNivel2 { get; set; }
        public string disableSelectNivel2 { get; set; }
        public string textoNotaNivel2 { get; set; }
    }

    public class CompetenciaModelExport
    {
        public int IdCompetencia { get; set; }
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public int IdEixo { get; set; }
        public string Eixo { get; set; }
        public int IdSubCompetencia { get; set; }
        public string SubCompetencia { get; set; }
        public int IdDimensao { get; set; }
        public string Dimensao { get; set; }
        public string DetalheNivelAtual { get; set; }
        public string CompetenciaAtual { get; set; }
        public string PalavrasChave { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public int ATV { get; set; }
    }

    public class SubcompetenciaModelExport
    {
        public int IdSubcompetencia { get; set; }
        public string Subcompetencia { get; set; }
        public string TipoAvaliacao { get; set; }
        public int ATV { get; set; }
    }

    public class DimensoesModelExport
    {
        public int IdDimensao { get; set; }
        public string Dimensao { get; set; }
        public string TipoAvaliacao { get; set; }
        public int ATV { get; set; }
    }

    public class EixosModelExport
    {
        public int IdEixo { get; set; }
        public string Eixo { get; set; }
        public string TipoAvaliacao { get; set; }
        public int ATV { get; set; }
    }
}

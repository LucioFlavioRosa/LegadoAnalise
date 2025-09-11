using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class ComplexidadeModel
    {
        public int IdComplexidade { get; set; }
        public string Complexidade { get; set; }
    }

    public class FatorProjetoModel
    {
        public int IdFatorProjeto { get; set; }
        public int IdFator { get; set; }
        public string Fator { get; set; }
        public string FatorDescBaixa { get; set; }
        public string FatorDescMedia { get; set; }
        public string FatorDescAlta { get; set; }
        public int IdProjeto { get; set; }
        public string Projeto { get; set; }
        public int IdNotaFator { get; set; }
        public string NotaFator { get; set; }
        public int ValorSlider { get; set; }
        public bool ValidadoGestor { get; set; }
        public DateTime? DHCValidadoGestor { get; set; }
        public bool ValidadoResponsavel { get; set; }
        public DateTime? DHCValidadoResponsavel { get; set; }
        public string Checked {
            get {
                return ValidadoGestor || ValidadoResponsavel ? "checked" : "";
            }
        }
        public string ValidadoTexto { get; set; }
        public string ValidadorPessoa { get; set; }
    }

    public class FatorComplexidadeExportModel
    {
        public string Projeto { get; set; }
        public string Gestor { get; set; }
        public string Responsavel { get; set; }
        public string Complexidade { get; set; }
        public string Fator { get; set; }
        public string Peso { get; set; }
        public string Nota { get; set; }
        public string DescricaoNota { get; set; }
        public string DescricaoFator { get; set; }
        public DateTime? DHCInput { get; set; }
        public DateTime? DHCValidacao { get; set; }
    }
}

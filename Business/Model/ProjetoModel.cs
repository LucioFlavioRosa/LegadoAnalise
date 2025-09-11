using Business.DataAccess;
using System.Collections.Generic;
using System;

namespace Business.Model
{
    public class ProjetoModel : ModelBase
    {
        public ProjetoModel()
        {
            Associados = new List<ProjetosAssociadosModel>();
            Lideres = new List<ProjetosAssociadosModel>();
        }
        public int IdProjeto { get; set;}
        public string Nome { get; set; }
        public PROJETOSSTATUS Status { get; set; }
        public string DataInicio { get; set; }
        public string DataTermino { get; set; }
        public string FlagPrevisto { get; set; }
        
        public ASSOCIADOS Responsavel { get; set; }
        public ASSOCIADOS Gestor { get; set; }
        public CLIENTES Cliente { get; set; }
        public List<ProjetosAssociadosModel> Associados { get; set; }
        public List<ProjetosAssociadosModel> Lideres { get; set; }
        public bool IsVisible
        {
            get
            {
                return (Associados.Count > 0 || Lideres.Count > 0);
            }
        }
        // PÁGINA DE PENDÊNCIAS
        public string FotoNome { get; set; }
        public string Projeto { get; set; }
        public string Periodo { get; set; }
        public string Pendencia { get; set; }
        public string DataLimite { get; set; }
        public string Respondente { get; set; }
        public string RespondenteEmail { get; set; }
        public int IdAvaliado { get; set; }
        public int IdPeriodo { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public int IdGestor { get; set; }
        // PÁGINA DE GERENCIAR PROJETOS
        public int ContagemAlocados { get;set; }
        // PÁGINA DE PERIODOS (AVALIAÇÕES SINALIZADAS)
        public int IdAvaliador { get; set; }
        public string Avaliador { get; set; }
    }

    public class ProjetoModelExport
    {
        public int IdProjeto { get; set; }
        public string Projeto { get; set; }
        public string Codigo { get; set; }
        public int IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public int IdResponsavel { get; set; }
        public string Responsavel { get; set; }
        public int IdGestor { get; set; }
        public string Gestor { get; set; }
        public int IdStatus { get; set; }
        public string Status { get; set; }
        public int IdTipo { get; set; }
        public string Tipo { get; set; }
        public int IdComplexidade { get; set; }
        public string Complexidade { get; set; }
        public string FatoresComplexidadeValidadosMD { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataTermino { get; set; }
        public int? ATV { get; set; }
    }
}

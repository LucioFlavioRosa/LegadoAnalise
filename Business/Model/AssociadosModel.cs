using Business.DataAccess;
using Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class AssociadosModel
    {
        public string Nome { get; set; }
        public string Mentor { get; set; }
        public string Cargo { get; set; }
        public string ProximoCargo { get; set; }
        public int IdCargo { get; set; }
        public int QtdProjetos { get; set; }
        public string Periodo { get; set; }
        public int IdAssociado { get; set; }
        public int IdPeriodo { get; set; }
        public string Projeto { get; set; }
        public int IdProjeto { get; set; }
        public string Enviado { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public string Vertical { get; set; }
        public int IdVertical { get; set; }
        public FOTOSASSOCIADOS FotoAssociado { get; set; }
    }

    public class AssociadosModelExport
    {
        public int IdAssociado { get; set; }
        public string Nome { get; set; }
        public int IdCargo { get; set; }
        public string Cargo { get; set; }
        public int IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public int IdPerfil { get; set; }
        public string Perfil { get; set; }
        public int IdMentor { get; set; }
        public string Mentor { get; set; }
        public string Email { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public string Vertical { get; set; }
        public int? IdVertical { get; set; }
        public int? ATV { get; set; }
    }

    public class MentoradosModel
    {
        public int idAssociado { get; set; }
        public string Associado { get; set; }
        public int idMentor { get; set; }
        public string Mentor { get; set; }
        public int idCargo { get; set; }
        public string Cargo { get; set; }
        public string FotoNome { get; set; }
        public int idProjeto { get; set; }
        public int idPeriodo { get; set; }
        public int idGestor { get; set; }
        public string TipoAvaliacao { get; set; }
        public string Escopo { get; set; }
        public string MentoriaRealizada
        {
            get
            {
                var associadosService = new AssociadosService();
                var consideracoesService = new ConsideracoesMentorService();
                var periodosService = new PeriodoService();

                var getConsideracoes = consideracoesService.ObterConsideracoesMentorPorAtributos(idMentor, idAssociado, idPeriodo, TipoAvaliacao, Escopo);
                if (getConsideracoes == null)
                {
                    return "Não";
                }
                else
                {
                    return getConsideracoes.MentoriaRealizada ? "Sim" : "Não";
                }
            }
        }
        public string DadosRHLiberados
        {
            get
            {
                var associadosService = new AssociadosService();
                var consideracoesService = new ConsideracoesMentorService();
                var periodosService = new PeriodoService();

                var getConsideracoes = consideracoesService.ObterConsideracoesMentorPorAtributos(idMentor, idAssociado, idPeriodo, TipoAvaliacao, Escopo);
                if (getConsideracoes == null)
                {
                    return "Não";
                }
                else
                {
                    return (bool)getConsideracoes.LiberadoRH ? "Sim" : "Não";
                }
            }
        }
        public string FeedbackRHVisible
        {
            get
            {
                var associadosService = new AssociadosService();
                var consideracoesService = new ConsideracoesMentorService();
                var periodosService = new PeriodoService();

                var getConsideracoes = consideracoesService.ObterConsideracoesMentorPorAtributos(idMentor, idAssociado, idPeriodo, TipoAvaliacao, Escopo);
                if (getConsideracoes == null || getConsideracoes.LiberadoRH != true)
                {
                    return "hidden";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
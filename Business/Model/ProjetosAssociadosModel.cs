using Business.DataAccess;
using Business.Services;
using Business.Util;
using System;
using System.Collections.Generic;

namespace Business.Model
{
    public class ProjetosAssociadosModel : ModelBase
    {
        public string IdEmail { get; set; }
        public int IdAvaliacaoCompetencia { get; set; }
        public int IdAvaliacaoPerformance { get; set; }
        public PROJETOS Projeto { get; set; }
        public ASSOCIADOS Associado { get; set; }
        public ASSOCIADOS Gestor { get; set; }
        public ASSOCIADOS Avaliador { get; set; }
        public ASSOCIADOS Mentor { get; set; }
        public FOTOSASSOCIADOS FotoAssociado { get; set; }
        public PERIODOSAVALIACOES Periodo { get; set; }
        public AVALIACOESSTATUS Status { get; set; }
        public string DataInicio { get; set; }
        public string DataTermino { get; set; }
        public string TipoAvaliacao { get; set; }
        public string TipoAvaliacaoShow { get; set; }
        public string Escopo { get; set; }
        public bool AvaliacaoLiberada { get; set; }
        public string RotuloBotao { get; set; }
        public bool ExibirBotaoFinalizar { get; set; }
        public string ExibirBotaoResponder { get; set; }
        public bool ExibirBotaoLiberarLider { get; set; }
        public string ExibirRotuloEtapa { get; set; }
        public string ExibirBotaoVerMentor { get; set; }
        public string Etapa { get; set; }
        public string ExibirFeedback { get; set; }
        public string ExibirBotaoVer { get; set; }
        public int ATV { get; set; }
        public List<DisparosModel> Disparos { get; set; }
        public string IsHidden
        {
            get
            {
                bool AssociadoDiffLogado = Associado.IdAssociado != WebStorage.GetUsuarioLogado().Id;
                bool IsMeuLiderado = new AssociadosService().ObterLiderado(Associado.IdAssociado) != null;

                PROJETOSASSOCIADOS projeto = new ProjetosService().ObterGestorEAvaliadorDeAssociado(Projeto.IdProjeto, Associado.IdAssociado, Id);
                if (projeto == null) { return "hidden"; }
                
                var retorno = "";

                if (AssociadoDiffLogado && !IsMeuLiderado)
                    retorno = "hidden";

                if (projeto.IdGestor == WebStorage.GetUsuarioLogado().Id)
                    retorno = null;

                if (projeto.IdAvaliador == WebStorage.GetUsuarioLogado().Id)
                    retorno = null;

                if (projeto.PROJETOS.IdAssociadoResponsavel == WebStorage.GetUsuarioLogado().Id)
                    retorno = null;

                if (projeto.PROJETOS.IdAssociadoGestor == WebStorage.GetUsuarioLogado().Id)
                    retorno = null;

                if (!AvaliacaoLiberada)
                    retorno = "hidden";


                return retorno;
            }
            
        }
        // PAGINA DO MENTOR
        public string FeedbackRHVisible { get; set; }
    }

    public class DisparosModel
    {
        public int IdPrazo { get; set; }
        public string NomeDisparo { get; set; }
    }
    // PAGINA: GERENCIAR PROJETOS
    public class HierarquiaAvaliacoesProjetoModel
    {
        public int IdProjeto { get; set; }
        public string Projeto { get; set; }
        public List<HierarquiaAssociadoModel> Associados { get; set; }
    }
    public class HierarquiaAssociadoModel
    {
        public int IdProjeto { get; set; }
        public int IdAssociado { get; set; }
        public string Associado { get; set; }
        public string FotoNome { get; set; }
        public DateTime DataInicioAlocacao { get; set; }
        public string InicioAlocacao { get; set; }
        public DateTime? DataTerminoAlocacao { get; set; }
        public string TerminoAlocacao { get; set; }
        public int IdPeriodoSinalizado { get; set; }
        public string PeriodoSinalizado { get; set; }
        public int IdPeriodoAvaliacao { get; set; }
        public string PeriodoAvaliacao { get; set; }
        public bool RespondeDesempenho { get; set; }
        public bool RespondeLideranca { get; set; }
        public bool HabilitaExcluir { get; set; }
        public string TextoExcluir { get; set; }
        public bool HabilitarAdicionarLiderado { get; set; }
        public string TextoAdicionarLiderado { get; set; }
        public int MarginLeft { get; set; }
        public string MarginLeftText { get; set; }
        public string LeftBorder { get; set; }
        public string LeftBorderLast { get; set; }
        public int IdAvaliador { get; set; }
        public List<HierarquiaAssociadoModel> Avaliados { get; set; }
        public List<int> NovoAvaliado { get; set; }
        public bool HabilitaLimpar { get; set; }
        public string TextoLimpar { get; set; }
        public string ExibeAcoes { get; set; }
        public bool ExibeNome { get; set; }
        public string ExibeSelecionar { get; set; }
        public List<ASSOCIADOS> AssociadosAlterar { get; set; }
    }
}

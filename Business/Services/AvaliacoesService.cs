using Business.DataAccess;
using Business.Model;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading;
using System.Web.UI.WebControls;
using Tria.Framework.Domain.Service;

namespace Business.Services
{
    public class AvaliacoesService
    {
        public string etapaNaoIniciada = "AVM";
        public string etapaAutoAvaliacao = "AAV";
        public string etapaAvaliacaoAsCegas = "ACE";
        public string etapaEmParalelo = "AEP";
        public string etapaAvaliacaoGestor = "AGE";
        public string etapaFeedback = "FED";
        public string etapaAvaliacaoMentor = "AME";
        public string etapaAvaliacaoFinalizada = "AFI";


        public string ProximaEtapa(string etapaAtual, string TipoAvaliacao = "")
        {
            switch (etapaAtual)
            {
                case "AVM":
                case "AAV":
                case "ACE":
                    return etapaEmParalelo;

                case "AEP":
                    return etapaAvaliacaoGestor;


                case "AGE":
                    string returnEtapa = etapaFeedback;
                    if (TipoAvaliacao == "lideranca")
                    {
                        returnEtapa = etapaAvaliacaoFinalizada;
                    }
                    return returnEtapa;

                case "FED":
                    //return etapaAvaliacaoMentor;
                    return etapaAvaliacaoFinalizada;

                case "AME":
                    return etapaAvaliacaoFinalizada;

                default:
                    return etapaNaoIniciada;
            }
        }

        #region Competencias

        /// <summary>
        /// Retorna todas as avaliações de competências de um determinado associado
        /// </summary>
        /// <param name="associado"></param>
        /// <returns></returns>
        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetencias(ASSOCIADOS associado)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
            {
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);
            }

            return ObterAvaliacoes(predicate);
        }

        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetenciasListas(List<int> idsProjetos, List<int> idsAssociados)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.IdAvaliacaoCompetencia != -1;

            if (idsProjetos != null) { predicate = predicate.And(a => idsProjetos.Contains(a.IdProjeto)); }
            if (idsAssociados != null) { predicate = predicate.And(a => idsAssociados.Contains(a.IdAssociado)); }

            return ObterAvaliacoes(predicate);
        }

        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetenciasPeriodoFinalizadas(int idAssociado, int idPeriodo, string TipoAvaliacao, string Escopo)
        {
            string etapaFinalizada = new AvaliacoesService().etapaAvaliacaoFinalizada;
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = a =>
                a.IdAssociado == idAssociado &&
                a.IdPeriodo == idPeriodo &&
                a.TipoAvaliacao == TipoAvaliacao &&
                a.Escopo == Escopo &&
                a.PosicaoAtualFluxoAvaliacao == etapaFinalizada;

            return ObterAvaliacoes(predicate);
        }

        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetencias(PERIODOSAVALIACOES periodoAtual, string etapaAtual)
        {
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = a => a.IdPeriodo == periodoAtual.IdPeriodo
                && a.PosicaoAtualFluxoAvaliacao == etapaAtual && a.ATV == 1;

            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Retorna todas as avaliações de competências de um determinado associado conforme status informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetencias(ASSOCIADOS associado, AVALIACOESSTATUS status)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
            {
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);
            }

            if (status != null)
            {
                predicate = predicate.And(p => p.IdAvaliacaoStatus == status.IdStatus);
            }

            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Retorna todas as avaliações de competências de um determinado associado conforme status e projeto informados informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetencias(ASSOCIADOS associado, AVALIACOESSTATUS status, PROJETOS projeto)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);

            if (status != null)
                predicate = predicate.And(p => p.IdAvaliacaoStatus == status.IdStatus);

            if (projeto != null)
                predicate = predicate.And(p => p.IdProjeto == projeto.IdProjeto);

            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Retorna todas as avaliações de competência de um determinado associado conforme status, projeto e período informados informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetencias(ASSOCIADOS associado, AVALIACOESSTATUS status,
            PROJETOS projeto, PERIODOSAVALIACOES periodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);

            if (status != null)
                predicate = predicate.And(p => p.IdAvaliacaoStatus == status.IdStatus);

            if (projeto != null)
                predicate = predicate.And(p => p.IdProjeto == projeto.IdProjeto);

            if (periodo != null)
                predicate = predicate.And(p => p.IdPeriodo == periodo.IdPeriodo);

            predicate = predicate.And(p => p.ATV == 1);



            return ObterAvaliacoes(predicate);
        }



        /// <summary>
        /// Retorna todas as avaliações de competência de um determinado associado conforme status, projeto e período informados informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACAO> ObterListaAvaliacoes(ASSOCIADOS associado, AVALIACOESSTATUS status,
            PROJETOS projeto, PERIODOSAVALIACOES periodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACAO, bool>> predicate = e => e.idEmpresa == idEmpresa;

            if (associado != null)
                predicate = predicate.And(p => p.idAssociado == associado.IdAssociado);

            if (status != null)
                predicate = predicate.And(p => p.idStatus == status.IdStatus);

            if (projeto != null)
                predicate = predicate.And(p => p.idProjeto == projeto.IdProjeto);

            if (periodo != null)
                predicate = predicate.And(p => p.idPeriodo == periodo.IdPeriodo);

            //predicate = predicate.And(p => p.idAssociado == 87);


            return ObterAvaliacao(predicate);

        }


        /// <summary>
        /// Retorna todas as avaliações de competências de um determinado associado conforme projeto informado informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetencias(ASSOCIADOS associado, PROJETOS projeto)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
            {
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);
            }

            if (projeto != null)
            {
                predicate = predicate.And(p => p.IdProjeto == projeto.IdProjeto);
            }

            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Retorna todas as avaliações de competência de um determinado associado conforme período informados informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetencias(ASSOCIADOS associado, PERIODOSAVALIACOES periodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
            {
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);
            }

            if (periodo != null)
            {
                predicate = predicate.And(p => p.IdPeriodo == periodo.IdPeriodo);
            }

            return ObterAvaliacoes(predicate);
        }

        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetencias(int idAssociado, int idProjeto, int idPeriodo, string etapa, string TipoAvaliacao, string Escopo, int idAvaliacao = 0)
        {
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.IdAssociado == idAssociado;

            //var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            if (idAvaliacao != 0)
                predicate = e => e.IdAssociado == idAssociado
                    && e.IdProjeto == idProjeto
                    && e.IdPeriodo == idPeriodo
                    && e.ATV == 1
                    && e.TipoAvaliacao == TipoAvaliacao
                    && e.Escopo == Escopo
                    && e.idAvaliacao == idAvaliacao;
            else
                predicate = e => e.IdAssociado == idAssociado
                    && e.IdProjeto == idProjeto
                    && e.IdPeriodo == idPeriodo
                    && e.ATV == 1
                    && e.TipoAvaliacao == TipoAvaliacao
                    && e.Escopo == Escopo;

            return ObterAvaliacoes(predicate);
        }
        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetencias(int idAssociado, int idProjeto, int idPeriodo, string TipoAvaliacao, string Escopo, int idAvaliacao)
        {
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.IdAssociado == idAssociado;
            //var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            predicate = e => e.IdAssociado == idAssociado
                && e.IdProjeto == idProjeto
                && e.IdPeriodo == idPeriodo
                && e.ATV == 1
                && e.TipoAvaliacao == TipoAvaliacao
                && e.Escopo == Escopo
                && e.idAvaliacao == idAvaliacao;

            return ObterAvaliacoes(predicate);
        }

        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetencias(int idAssociado, int idProjeto, int idPeriodo, int idCargo = -1)
        {
            //var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.IdAvaliacaoCompetencia != -1;

            if (idAssociado != -1) { predicate = predicate.And(e => e.IdAssociado == idAssociado); }
            if (idProjeto != -1) { predicate = predicate.And(e => e.IdProjeto == idProjeto); }
            if (idPeriodo != -1) { predicate = predicate.And(e => e.IdPeriodo == idPeriodo); }
            if (idCargo != -1) { predicate = predicate.And(e => e.IdCargo == idCargo); }


            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Obtém informações de um determinada avaliação de competência
        /// </summary>
        /// <param name="avaliacao"></param>
        /// <returns></returns>
        public AVALIACOESCOMPETENCIAS ObterAvaliacaoCompetencia(int idAvaliacao)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESCOMPETENCIAS.FirstOrDefault(a => a.IdAvaliacaoCompetencia == idAvaliacao);
        }

        public AVALIACOESCOMPETENCIASNOTAS ObterAvaliacaoCompetenciaNota(int idNota)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESCOMPETENCIASNOTAS.FirstOrDefault(a => a.IdNota == idNota);
        }


        public List<AVALIACOESCOMPETENCIASNOTAS> ObterAvaliacaoCompetenciasNotas(int ATV = 1)
        {
            DataModel context = new DataModel();
            var returnItems = context.AVALIACOESCOMPETENCIASNOTAS.Where(x => x.IdNota != -1).ToList();

            if (ATV != -1) { returnItems = returnItems.Where(x => x.ATV == ATV).ToList(); }

            return returnItems;
        }

        public List<AVALIACOESPERFORMANCESNOTAS> ObterAvaliacaoPerformancesNotas()
        {
            DataModel context = new DataModel();
            return context.AVALIACOESPERFORMANCESNOTAS.Where(a => a.ATV == 1).ToList();
        }


        public AVALIACOESCOMPETENCIAS ObterAvaliacaoCompetencia(int idAssociado, int idProjeto, int idPeriodo, string TipoAvaliacao, string Escopo, int idAvaliacao)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            Expression<Func<AVALIACOESCOMPETENCIAS, bool>>  predicate = e => e.ATV == 1 && e.IdEmpresa == idEmpresa &&
            e.IdAssociado == idAssociado && e.IdProjeto == idProjeto && e.IdPeriodo == idPeriodo &&
            e.TipoAvaliacao == TipoAvaliacao && e.Escopo == Escopo && e.idAvaliacao == idAvaliacao;

            DataModel context = new DataModel();
            return context.AVALIACOESCOMPETENCIAS.FirstOrDefault(predicate);
        }
        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacaoCompetenciaTodos(int idAssociado, int idProjeto, int idPeriodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e => e.ATV == 1 && e.IdEmpresa == idEmpresa &&
            e.IdAssociado == idAssociado && e.IdProjeto == idProjeto && e.IdPeriodo == idPeriodo;

            DataModel context = new DataModel();
            return context.AVALIACOESCOMPETENCIAS.Where(predicate).ToList();
        }

        public List<AVALIACOESCOMPETENCIAS> ListaAvCompetencias()
        {
            DataModel context = new DataModel();
            return context.AVALIACOESCOMPETENCIAS.ToList();
        }

        public AVALIACOESCOMPETENCIAS ObterAvaliacaoCompetencia(int idAssociado, int idProjeto, int idCompetencia, int idPeriodo, string TipoAvaliacao, string Escopo, int idAvaliacao)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            Expression<Func<AVALIACOESCOMPETENCIAS, bool>>  predicate = e => e.IdEmpresa == idEmpresa &&
            e.IdAssociado == idAssociado && e.IdProjeto == idProjeto && e.IdCompetencia == idCompetencia && e.IdPeriodo == idPeriodo &&
            e.TipoAvaliacao == TipoAvaliacao && e.Escopo == Escopo && e.idAvaliacao == idAvaliacao;

            DataModel context = new DataModel();
            return context.AVALIACOESCOMPETENCIAS.FirstOrDefault(predicate);
        }

        private List<AVALIACOESCOMPETENCIAS> ObterAvaliacoes(Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESCOMPETENCIAS.Where(predicate).ToList();
        }

        public List<AVALIACAO> ObterAvaliacao(int idprojeto, int idassociado, string TipoAvaliacao, string Escopo, int idGestor)
        {
            DataModel context = new DataModel();
            Expression<Func<AVALIACAO, bool>> predicate = e => 
                e.idAssociado == idassociado && 
                e.idProjeto == idprojeto && 
                e.TipoAvaliacao == TipoAvaliacao && 
                e.Escopo == Escopo &&
                e.idGestor == idGestor;
            return ObterAvaliacao(predicate);
        }

        public List<AVALIACAO> ObterListaAvaliacao()
        {
            DataModel context = new DataModel();
            Expression<Func<AVALIACAO, bool>> predicate = e => e.idAvaliacao != -1;
            return ObterAvaliacao(predicate);
        }

        public List<AVALIACAO> ObterAvaliacaoHierarquia(int idassociado, int idprojeto, int idperiodo)
        {
            DataModel context = new DataModel();
            Expression<Func<AVALIACAO, bool>> predicate = e =>
                e.idAssociado == idassociado &&
                e.idProjeto == idprojeto &&
                e.idPeriodo == idperiodo;
            return ObterAvaliacao(predicate);
        }

        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacaoCompetenciaQualquerNoPeriodo(int idassociado, int idperiodo, string TipoAvaliacao, string Escopo)
        {
            DataModel context = new DataModel();
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e =>
                e.IdAssociado == idassociado &&
                e.TipoAvaliacao == TipoAvaliacao &&
                e.Escopo == Escopo &&
                e.IdPeriodo == idperiodo;

            return context.AVALIACOESCOMPETENCIAS.Where(predicate).ToList();
        }
        public List<AVALIACOESCOMPETENCIAS> ObterAvaliacoesCompetenciasPeriodo(List<int> idPeriodos, string TipoAvaliacao)
        {
            DataModel context = new DataModel();
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicate = e =>
                e.TipoAvaliacao == TipoAvaliacao &&
                idPeriodos.Contains(e.IdPeriodo);

            return context.AVALIACOESCOMPETENCIAS.Where(predicate).ToList();
        }
        public AVALIACAO ObterAvaliacao(int idAvaliacao)
        {
            DataModel context = new DataModel();
            Expression<Func<AVALIACAO, bool>> predicate = e => e.idAvaliacao == idAvaliacao;
            return ObterAvaliacao(predicate)[0];
        }
        public List<AVALIACAO> ObterAvaliacoesAssociado(int idAssociado, string etapa, string TipoAvaliacao)
        {
            DataModel context = new DataModel();
            Expression<Func<AVALIACAO, bool>> predicate = e => e.idAssociado == idAssociado;

            if (etapa != "") { predicate = predicate.And(e => e.PosicaoAtualFluxoAvaliacao == etapa); }
            if (TipoAvaliacao != "") { predicate = predicate.And(e => e.TipoAvaliacao == TipoAvaliacao); }

            return ObterAvaliacao(predicate);
        }
        public List<AVALIACAO> ObterAvaliacoesTipo(string etapa, string TipoAvaliacao)
        {
            DataModel context = new DataModel();
            Expression<Func<AVALIACAO, bool>> predicate = e => e.TipoAvaliacao == TipoAvaliacao;

            if (etapa != "") { predicate = predicate.And(e => e.PosicaoAtualFluxoAvaliacao == etapa); }

            return ObterAvaliacao(predicate);
        }

        public List<PROJETOSASSOCIADOS> ObterAssociacoesRelacionadas(int idAssociado)
        {
            DataModel context = new DataModel();

            List<int> associadosRelacionados = new List<int>();
            associadosRelacionados.Add(idAssociado);

            List<ASSOCIADOS> mentorados = new AssociadosService().ObterMentorados(idAssociado);
            associadosRelacionados.AddRange(mentorados.Select(m => m.IdAssociado));

            Expression<Func<PROJETOSASSOCIADOS, bool>> predicate = e => (associadosRelacionados.Contains(e.IdAssociado) || e.IdGestor == idAssociado || e.IdAvaliador == idAssociado) && e.ATV == 1;
            return context.PROJETOSASSOCIADOS.Where(predicate).ToList();
        }

        public List<AVALIACAO> ObterAvaliacoesAssociadoSemestre(int idAssociado, int idPeriodo, string TipoAvaliacao)
        {
            DataModel context = new DataModel();
            Expression<Func<AVALIACAO, bool>> predicate = e => e.idAvaliacao != -1;

            if (idAssociado != -1) { predicate = predicate.And(e => e.idAssociado == idAssociado); }
            if (idPeriodo != -1) { predicate = predicate.And(e => e.idPeriodo == idPeriodo); }
            if (TipoAvaliacao != "") { predicate = predicate.And(e => e.TipoAvaliacao == TipoAvaliacao); }
            
            return ObterAvaliacao(predicate);
        }


        private List<AVALIACAO> ObterAvaliacao(Expression<Func<AVALIACAO, bool>> predicate)
        {
            DataModel context = new DataModel();
            return context.AVALIACAO.Where(predicate).ToList();
        }

        /// <summary>
        /// Altera o Status de uma avaliação Competência
        /// </summary>
        /// <param name="avaliacao"></param>
        /// <returns></returns>
        public bool AlterarStatusAvaliacaoCompetencia(int idAvaliacao, AVALIACOESSTATUS novoStatus)
        {
            try
            {
                DataModel context = new DataModel();
                var avaliacaoAtual = context.AVALIACOESCOMPETENCIAS.FirstOrDefault(a => a.IdAvaliacaoCompetencia == idAvaliacao);
                avaliacaoAtual.IdAvaliacaoStatus = novoStatus.IdStatus;
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Altera informações de uma avaliação de competência
        /// </summary>
        /// <param name="avaliacao"></param>
        /// <returns></returns>
        public bool AlterarAvaliacaoCompetencia(int idAvaliacao, AVALIACOESCOMPETENCIAS avaliacao, bool avancaEtapa = false)
        {
            try
            {
                DataModel context = new DataModel();
                var avaliacaoAtual = context.AVALIACOESCOMPETENCIAS.FirstOrDefault(a => a.IdAvaliacaoCompetencia == idAvaliacao);

                if (avaliacao.IdEmpresa != 0) avaliacaoAtual.IdEmpresa = avaliacao.IdEmpresa;
                if (avaliacao.IdAssociado != 0) avaliacaoAtual.IdAssociado = avaliacao.IdAssociado;
                if (avaliacao.IdCargo != 0) avaliacaoAtual.IdCargo = avaliacao.IdCargo;
                if (avaliacao.IdNivel != 0) avaliacaoAtual.IdNivel = avaliacao.IdNivel;
                if (avaliacao.IdProjeto != 0) avaliacaoAtual.IdProjeto = avaliacao.IdProjeto;
                if (avaliacao.IdPeriodo != 0) avaliacaoAtual.IdPeriodo = avaliacao.IdPeriodo;
                if (avaliacao.IdCompetencia != 0) avaliacaoAtual.IdCompetencia = avaliacao.IdCompetencia;
                if (avaliacao.IdAvaliacaoStatus != 0) avaliacaoAtual.IdAvaliacaoStatus = avaliacao.IdAvaliacaoStatus;
                if (avaliacao.PosicaoAtualFluxoAvaliacao != "") avaliacaoAtual.PosicaoAtualFluxoAvaliacao = avancaEtapa ? avaliacao.PosicaoAtualFluxoAvaliacao : avaliacaoAtual.PosicaoAtualFluxoAvaliacao;

                // Alterar a Posição do Fluxo em um método próprio, pois desta forma os blocos abaixo não são executados corretamente
                //avaliacaoAtual.PosicaoAtualFluxoAvaliacao = avaliacao.PosicaoAtualFluxoAvaliacao;

                if (avaliacao.PosicaoAtualFluxoAvaliacao == etapaNaoIniciada ||
                    avaliacao.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoAsCegas ||
                    avaliacao.PosicaoAtualFluxoAvaliacao == etapaAutoAvaliacao || 
                    avaliacao.PosicaoAtualFluxoAvaliacao == etapaEmParalelo)
                {
                //Auto avaliação
                    if (avaliacao.IdNotaNivel1AutoAvaliacao != 0) avaliacaoAtual.IdNotaNivel1AutoAvaliacao = avaliacao.IdNotaNivel1AutoAvaliacao;
                    if (avaliacao.IdNotaNivel2AutoAvaliacao != 0) avaliacaoAtual.IdNotaNivel2AutoAvaliacao = avaliacao.IdNotaNivel2AutoAvaliacao;
                    if (avaliacao.ComentariosAutoAvaliacao != null) avaliacaoAtual.ComentariosAutoAvaliacao = avaliacao.ComentariosAutoAvaliacao;
                    if (avaliacao.DHCAutoAvaliacao != null) avaliacaoAtual.DHCAutoAvaliacao = DateTime.Now;
                    if (avaliacao.USRAutoAvaliacao != 0) avaliacaoAtual.USRAutoAvaliacao = avaliacao.USRAutoAvaliacao;
                    avaliacaoAtual.DataHoraFimAutoAvaliacao = avaliacao.DataHoraFimAutoAvaliacao;

                //Avaliação as cegas
                    if (avaliacao.IdNotaNivel1AvaliacaoCegas != 0) avaliacaoAtual.IdNotaNivel1AvaliacaoCegas = avaliacao.IdNotaNivel1AvaliacaoCegas;
                    if (avaliacao.IdNotaNivel2AvaliacaoCegas != 0) avaliacaoAtual.IdNotaNivel2AvaliacaoCegas = avaliacao.IdNotaNivel2AvaliacaoCegas;
                    if (avaliacao.ComentariosAvaliacaoCegas != null) avaliacaoAtual.ComentariosAvaliacaoCegas = avaliacao.ComentariosAvaliacaoCegas;
                    if (avaliacao.DHCAvaliacaoCegas != null) avaliacaoAtual.DHCAvaliacaoCegas = DateTime.Now;
                    if (avaliacao.USRAvaliacaoCegas != null) avaliacaoAtual.USRAvaliacaoCegas = avaliacao.USRAvaliacaoCegas;
                    avaliacaoAtual.DataHoraInicioAvaliacaoCegas = avaliacao.DataHoraInicioAvaliacaoCegas;
                    avaliacaoAtual.DataHoraFimAvaliacaoCegas = avaliacao.DataHoraFimAvaliacaoCegas;
                }

                //Avaliação do gestor
                if (avaliacao.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoGestor)
                {
                    if (avaliacao.IdNotaNivel1AvaliacaoGestor != 0) avaliacaoAtual.IdNotaNivel1AvaliacaoGestor = avaliacao.IdNotaNivel1AvaliacaoGestor;
                    if (avaliacao.IdNotaNivel2AvaliacaoGestor != 0) avaliacaoAtual.IdNotaNivel2AvaliacaoGestor = avaliacao.IdNotaNivel2AvaliacaoGestor;
                    if (avaliacao.ComentariosAvaliacaoGestor != null) avaliacaoAtual.ComentariosAvaliacaoGestor = avaliacao.ComentariosAvaliacaoGestor;
                    if (avaliacao.DHCAvaliacaoGestor != null) avaliacaoAtual.DHCAvaliacaoGestor = DateTime.Now;
                    if (avaliacao.USRAvaliacaoGestor != null) avaliacaoAtual.USRAvaliacaoGestor = avaliacao.USRAvaliacaoGestor;
                    avaliacaoAtual.DataHoraInicioAvaliacaoGestor = avaliacao.DataHoraInicioAvaliacaoGestor;
                    avaliacaoAtual.DataHoraFimAvaliacaoGestor = avaliacao.DataHoraFimAvaliacaoGestor;

                    // Percentuais Finais para Geração do Excel e Gráfico
                    avaliacaoAtual.NotaSubCompetenciaAvaliado = avaliacao.NotaSubCompetenciaAvaliado;
                    avaliacaoAtual.NotaSubCompetenciaGestor = avaliacao.NotaSubCompetenciaGestor;
                    avaliacaoAtual.NotaCompetenciaAvaliado = avaliacao.NotaCompetenciaAvaliado;
                    avaliacaoAtual.NotaCompetenciaGestor = avaliacao.NotaCompetenciaGestor;
                }

                //Feedback
                if (avaliacao.PosicaoAtualFluxoAvaliacao == etapaFeedback)
                {
                    if (avaliacao.IdNotaNivel1Feedback != 0) avaliacaoAtual.IdNotaNivel1Feedback = avaliacao.IdNotaNivel1Feedback;
                    if (avaliacao.IdNotaNivel2Feedback != 0) avaliacaoAtual.IdNotaNivel2Feedback = avaliacao.IdNotaNivel2Feedback;
                    if (avaliacao.ComentariosFeedback != null) avaliacaoAtual.ComentariosFeedback = avaliacao.ComentariosFeedback;
                    if (avaliacao.DHCFeedback != null) avaliacaoAtual.DHCFeedback = DateTime.Now;
                    if (avaliacao.USRFeedback != null) avaliacaoAtual.USRFeedback = avaliacao.USRFeedback;
                    avaliacaoAtual.DataHoraInicioFeedback = avaliacao.DataHoraInicioFeedback;
                    avaliacaoAtual.DataHoraFimFeedback = avaliacao.DataHoraFimFeedback;
                }

                //Mentor
                if (avaliacao.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoMentor)
                {
                    if (avaliacao.IdNotaNivel1AvaliacaoMentor != 0) avaliacaoAtual.IdNotaNivel1AvaliacaoMentor = avaliacao.IdNotaNivel1AvaliacaoMentor;
                    if (avaliacao.IdNotaNivel2AvaliacaoMentor != 0) avaliacaoAtual.IdNotaNivel2AvaliacaoMentor = avaliacao.IdNotaNivel2AvaliacaoMentor;
                    if (avaliacao.ComentariosAvaliacaoMentor != null) avaliacaoAtual.ComentariosAvaliacaoMentor = avaliacao.ComentariosAvaliacaoMentor;
                    if (avaliacao.DHCAvaliacaoMentor != null) avaliacaoAtual.DHCAvaliacaoMentor = DateTime.Now;
                    if (avaliacao.USRAvaliacaoMentor != null) avaliacaoAtual.USRAvaliacaoMentor = avaliacao.USRAvaliacaoMentor;
                    avaliacaoAtual.DataHoraInicioAvaliacaoMentor = avaliacao.DataHoraInicioAvaliacaoMentor;
                    avaliacaoAtual.DataHoraFimAvaliacaoMentor = avaliacao.DataHoraFimAvaliacaoMentor;
                }

                if (avaliacao.Notificado != null) avaliacaoAtual.Notificado = avaliacao.Notificado;
                if (avaliacao.DataHoraNotificacao != null) avaliacaoAtual.DataHoraNotificacao = avaliacao.DataHoraNotificacao;
                if (avaliacao.USR != 0) avaliacaoAtual.USR = avaliacao.USR;
                if (avaliacao.DHC != null) avaliacaoAtual.DHC = avaliacao.DHC;
                if (avaliacao.Notificado != null) avaliacaoAtual.Notificado = 1;

                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool AvancaProximaEtapaCompetencia(AVALIACOESCOMPETENCIAS avaliacao, AVALIACAO avaliacaoMestre, string TipoAvaliacao = "")
        {
            try
            {
                DataModel context = new DataModel();
                var avaliacaoAlterada = context.AVALIACOESCOMPETENCIAS.FirstOrDefault(a => a.IdAvaliacaoCompetencia == avaliacao.IdAvaliacaoCompetencia);

                switch (avaliacaoMestre.PosicaoAtualFluxoAvaliacao)
                {
                    case "AVM":
                        avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaEmParalelo;
                        break;

                    case "AAV":
                        if (VerificaAAVeACECompetenciaConcluidas(avaliacaoMestre, avaliacao))
                        {
                            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoGestor;
                        }
                        break;

                    case "ACE":
                        if (VerificaAAVeACECompetenciaConcluidas(avaliacaoMestre, avaliacao))
                        {
                            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoGestor;
                        }
                        break;

                    case "AEP":
                        if (VerificaAAVeACECompetenciaConcluidas(avaliacaoMestre, avaliacao))
                        {
                            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoGestor;
                        }
                        break;

                    case "AGE":
                        avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaFeedback;

                        if (TipoAvaliacao == "lideranca")
                        {
                            avaliacaoAlterada.DataHoraTermino = DateTime.Now;
                            avaliacaoAlterada.IdAvaliacaoStatus = new StatusService().ObterStatusAvaliacao("Concluído").IdStatus;
                            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoFinalizada;
                        }
                        break;

                    case "FED":
                        avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoMentor;
                        // Ao chegar no Mentor, já Finaliza a Avaliação, pois não há interação do Mentor
                        avaliacaoAlterada.DataHoraTermino = DateTime.Now;
                        avaliacaoAlterada.IdAvaliacaoStatus = new StatusService().ObterStatusAvaliacao("Concluído").IdStatus;

                        if (TipoAvaliacao == "lideranca")
                        {
                            avaliacaoAlterada.DataHoraTermino = DateTime.Now;
                            avaliacaoAlterada.IdAvaliacaoStatus = new StatusService().ObterStatusAvaliacao("Concluído").IdStatus;
                            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoFinalizada;
                        }
                        break;

                    case "AME":
                        // Finaliza Avaliação após o último Nível 
                        avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoFinalizada;
                        // A linha abaixo subiu para o nível anterior
                        //avaliacaoAlterada.DataHoraTermino = DateTime.Now;
                        break;
                }

                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica se a etapa de Auto-avaliação e Avaliação às cegas foram concuídas
        /// </summary>
        /// <param name="avaliacaoMestre"></param>
        /// <param name="avaliacaoCompetencia"></param>
        /// <returns></returns>
        private bool VerificaAAVeACECompetenciaConcluidas(AVALIACAO avaliacaoMestre, AVALIACOESCOMPETENCIAS avaliacaoCompetencia)
        {
            
            bool aavConcluidaComp = avaliacaoCompetencia.DataHoraFimAutoAvaliacao != null;
            bool aceConcluidaComp = avaliacaoCompetencia.DataHoraFimAvaliacaoCegas != null;

            return aavConcluidaComp && aceConcluidaComp;
        }

        /// <summary>
        /// Salva informações de uma avaliação de competência
        /// </summary>
        /// <param name="avaliacao"></param>
        /// <returns></returns>
        public bool SalvarAvaliacaoCompetencia(AVALIACOESCOMPETENCIAS avaliacao)
        {
            try
            {
                DataModel context = new DataModel();
                context.AVALIACOESCOMPETENCIAS.Add(avaliacao);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Performances

        /// <summary>
        /// Retorna todas as avaliações de performance de um determinado associado
        /// </summary>
        /// <param name="associado"></param>
        /// <returns></returns>
        public List<AVALIACOESPERFORMANCES> ObterAvaliacoesPerformances(ASSOCIADOS associado)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
            {
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);
            }

            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Retorna todas as avaliações de performance de um determinado associado conforme status informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACOESPERFORMANCES> ObterAvaliacoesPerformances(ASSOCIADOS associado, AVALIACOESSTATUS status)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
            {
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);
            }

            if (status != null)
            {
                predicate = predicate.And(p => p.IdAvaliacaoStatus == status.IdStatus);
            }

            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Retorna todas as avaliações de performance de um determinado associado conforme projeto e período
        /// </summary>
        /// <param name="IdAssociado"></param>
        /// <param name="IdProjeto"></param>
        /// <param name="IdPeriodo"></param>
        /// <returns></returns>
        public List<AVALIACOESPERFORMANCES> ObterAvaliacoesPerformances(int IdAssociado, int IdProjeto, int IdPeriodo, bool desempenho)
        {
            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = 
                p => p.IdAssociado == IdAssociado && p.IdProjeto == IdProjeto && p.IdPeriodo == IdPeriodo && p.ATV == 1;

            if (desempenho)
            {
                predicate = predicate.And(a => a.IdPerformance != 364);
            }
            else
            {
                predicate = predicate.And(a => a.IdPerformance == 364);
            }

            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Retorna todas as avaliações de performance de um determinado associado conforme status e projeto informados informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACOESPERFORMANCES> ObterAvaliacoesPerformances(ASSOCIADOS associado, AVALIACOESSTATUS status, PROJETOS projeto)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
            {
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);
            }

            if (status != null)
            {
                predicate = predicate.And(p => p.IdAvaliacaoStatus == status.IdStatus);
            }

            if (projeto != null)
            {
                predicate = predicate.And(p => p.IdProjeto == projeto.IdProjeto);
            }

            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Retorna todas as avaliações de performance de um determinado associado conforme projeto informado informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACOESPERFORMANCES> ObterAvaliacoesPerformances(ASSOCIADOS associado, PROJETOS projeto)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
            {
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);
            }

            if (projeto != null)
            {
                predicate = predicate.And(p => p.IdProjeto == projeto.IdProjeto);
            }

            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Obtém informações de um determinada avaliação de performance
        /// </summary>
        /// <param name="avaliacao"></param>
        /// <returns></returns>
        public AVALIACOESPERFORMANCES ObterAvaliacaoPerformance(int idAvaliacao)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESPERFORMANCES.FirstOrDefault(a => a.IdAvaliacaoPerformance == idAvaliacao);
        }

        public AVALIACOESPERFORMANCES ObterAvaliacaoPerformance(int idAssociado, int idProjeto, int idPerformance, int idPeriodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdEmpresa == idEmpresa &&
            e.IdAssociado == idAssociado && e.IdProjeto == idProjeto && e.IdPerformance == idPerformance;

            if (idPeriodo > 0)
                predicate = predicate.And(a => a.IdPeriodo == idPeriodo);

            DataModel context = new DataModel();

            if (idPeriodo > 0)
                return context.AVALIACOESPERFORMANCES.FirstOrDefault(predicate);
            else
                return context.AVALIACOESPERFORMANCES.Where(predicate).OrderByDescending(o => o.IdPeriodo).FirstOrDefault();
        }


        public AVALIACOESPERFORMANCES ObterAvaliacaoPerformance(int idAssociado, int idProjeto, int idPeriodo, bool desempenho)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdEmpresa == idEmpresa &&
            e.IdAssociado == idAssociado && e.IdProjeto == idProjeto;

            if (idPeriodo > 0)
                predicate = predicate.And(a => a.IdPeriodo == idPeriodo);

            if (desempenho)
            {
                predicate = predicate.And(a => a.IdPerformance != 364);
            }

            DataModel context = new DataModel();

            if (idPeriodo > 0)
                return context.AVALIACOESPERFORMANCES.FirstOrDefault(predicate);
            else
                return context.AVALIACOESPERFORMANCES.Where(predicate).OrderByDescending(o => o.IdPeriodo).FirstOrDefault();
        }

        public AVALIACOESPERFORMANCES ObterAvaliacaoPerformanceEspecifica(int idAssociado, int idProjeto, int idPeriodo, int idPerformance)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdEmpresa == idEmpresa &&
            e.IdAssociado == idAssociado && e.IdProjeto == idProjeto && e.IdPerformance == idPerformance;

            if (idPeriodo > 0)
                predicate = predicate.And(a => a.IdPeriodo == idPeriodo);

            DataModel context = new DataModel();

            if (idPeriodo > 0)
                return context.AVALIACOESPERFORMANCES.FirstOrDefault(predicate);
            else
                return context.AVALIACOESPERFORMANCES.Where(predicate).OrderByDescending(o => o.IdPeriodo).FirstOrDefault();
        }

        /// <summary>
        /// Retorna todas as avaliações de performance de um determinado associado conforme status, projeto e período informados informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACOESPERFORMANCES> ObterAvaliacoesPerformances(ASSOCIADOS associado, AVALIACOESSTATUS status, PROJETOS projeto, PERIODOSAVALIACOES periodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
            {
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);
            }

            if (status != null)
            {
                predicate = predicate.And(p => p.IdAvaliacaoStatus == status.IdStatus);
            }

            if (projeto != null)
            {
                predicate = predicate.And(p => p.IdProjeto == projeto.IdProjeto);
            }

            if (periodo != null)
            {
                predicate = predicate.And(p => p.IdPeriodo == periodo.IdPeriodo);
            }

            return ObterAvaliacoes(predicate);
        }

        public List<AVALIACOESPERFORMANCES> ObterAvaliacoesPerformances(int idAssociado, int idProjeto, int idPeriodo, string etapa, bool desempenho)
        {
            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdAssociado == idAssociado
                    && e.IdProjeto == idProjeto
                    && e.IdPeriodo == idPeriodo;

            if (desempenho)
            {
                predicate = predicate.And(p => p.IdPerformance != 364);
            }

            return ObterAvaliacoes(predicate);
        }

        public List<AVALIACOESPERFORMANCES> ObterAvaliacoesPerformancesLideranca(int idAssociado, int idProjeto, int idPeriodo)
        {
            //var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdAssociado == idAssociado
                    && e.IdProjeto == idProjeto
                    && e.IdPeriodo == idPeriodo
                    && e.IdPerformance == 364;

            return ObterAvaliacoes(predicate);
        }

        /// <summary>
        /// Retorna todas as avaliações de performance de um determinado associado conforme período informados informado
        /// </summary>
        /// <param name="associado"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<AVALIACOESPERFORMANCES> ObterAvaliacoesPerformances(ASSOCIADOS associado, PERIODOSAVALIACOES periodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.IdEmpresa == idEmpresa;

            if (associado != null)
            {
                predicate = predicate.And(p => p.IdAssociado == associado.IdAssociado);
            }

            if (periodo != null)
            {
                predicate = predicate.And(p => p.IdPeriodo == periodo.IdPeriodo);
            }

            return ObterAvaliacoes(predicate);
        }

        private List<AVALIACOESPERFORMANCES> ObterAvaliacoes(Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate)
        {
            DataModel context = new DataModel();
            return context.AVALIACOESPERFORMANCES.Where(predicate).ToList();
        }

        public List<AVALIACOESPERFORMANCES> ObterAvaliacaoPerformanceTodos(int idAssociado, int idProjeto, int idPeriodo)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;

            Expression<Func<AVALIACOESPERFORMANCES, bool>> predicate = e => e.ATV == 1 && e.IdEmpresa == idEmpresa;

            if (idAssociado != -1) { predicate = predicate.And(x => x.IdAssociado == idAssociado); }
            if (idProjeto != -1) { predicate = predicate.And(x => x.IdProjeto == idProjeto); }
            if (idPeriodo != -1) { predicate = predicate.And(x => x.IdPeriodo == idPeriodo); }

            DataModel context = new DataModel();
            return context.AVALIACOESPERFORMANCES.Where(predicate).ToList();
        }

        public List<AVALIACOESPERFORMANCES> ObterAvaliacaoPerformanceLista()
        {
            DataModel context = new DataModel();
            return context.AVALIACOESPERFORMANCES.ToList();
        }



        /// <summary>
        /// Alterar o Status de uma avaliação de performance
        /// </summary>
        /// <param name="avaliacao"></param>
        /// <returns></returns>
        public bool AlterarStatusAvaliacaoPerformance(int idAvaliacao, AVALIACOESSTATUS novoStatus)
        {
            try
            {
                DataModel context = new DataModel();
                var avaliacaoAtual = context.AVALIACOESPERFORMANCES.FirstOrDefault(a => a.IdAvaliacaoPerformance == idAvaliacao);
                avaliacaoAtual.IdAvaliacaoStatus = novoStatus.IdStatus;
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Altera informações de uma avaliação de competência
        /// </summary>
        /// <param name="avaliacao"></param>
        /// <returns></returns>
        public bool AlterarAvaliacaoPerformance(int idAvaliacao, AVALIACOESPERFORMANCES avaliacao)
        {
            try
            {
                DataModel context = new DataModel();
                var avaliacaoAtual = context.AVALIACOESPERFORMANCES.FirstOrDefault(a => a.IdAvaliacaoPerformance == idAvaliacao);

                if (avaliacao.IdEmpresa != 0) avaliacaoAtual.IdEmpresa = avaliacao.IdEmpresa;
                if (avaliacao.IdAssociado != 0) avaliacaoAtual.IdAssociado = avaliacao.IdAssociado;
                if (avaliacao.IdCargo != 0) avaliacaoAtual.IdCargo = avaliacao.IdCargo;
                if (avaliacao.IdNivel != 0) avaliacaoAtual.IdNivel = avaliacao.IdNivel;
                if (avaliacao.IdProjeto != 0) avaliacaoAtual.IdProjeto = avaliacao.IdProjeto;
                if (avaliacao.IdPeriodo != 0) avaliacaoAtual.IdPeriodo = avaliacao.IdPeriodo;
                if (avaliacao.IdPerformance != 0) avaliacaoAtual.IdPerformance = avaliacao.IdPerformance;
                if (avaliacao.IdAvaliacaoStatus != 0) avaliacaoAtual.IdAvaliacaoStatus = avaliacao.IdAvaliacaoStatus;

                // Alterar a Posição do Fluxo em um método próprio, pois desta forma ob blocos abaixo não são executados corretamente
                //avaliacaoAtual.PosicaoAtualFluxoAvaliacao = avaliacao.PosicaoAtualFluxoAvaliacao;

                //Auto avaliação
                if (avaliacao.PosicaoAtualFluxoAvaliacao == etapaNaoIniciada ||
                    avaliacao.PosicaoAtualFluxoAvaliacao == etapaAutoAvaliacao || 
                    avaliacao.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoAsCegas ||
                    avaliacao.PosicaoAtualFluxoAvaliacao == etapaEmParalelo)
                {
                    if (avaliacao.IdNotaNivel1AutoAvaliacao != 0) avaliacaoAtual.IdNotaNivel1AutoAvaliacao = avaliacao.IdNotaNivel1AutoAvaliacao;
                    if (avaliacao.ComentariosAutoAvaliacao != null) avaliacaoAtual.ComentariosAutoAvaliacao = avaliacao.ComentariosAutoAvaliacao;
                    if (avaliacao.DHCAutoAvaliacao != null) avaliacaoAtual.DHCAutoAvaliacao = DateTime.Now;
                    if (avaliacao.USRAutoAvaliacao != 0) avaliacaoAtual.USRAutoAvaliacao = avaliacao.USRAutoAvaliacao;
                    avaliacaoAtual.DataHoraInicioAutoAvaliacao = avaliacao.DataHoraInicioAutoAvaliacao;
                    avaliacaoAtual.DataHoraFimAutoAvaliacao = avaliacao.DataHoraFimAutoAvaliacao;
                }

                //Avaliação as cegas
                if (avaliacao.PosicaoAtualFluxoAvaliacao == etapaNaoIniciada ||
                    avaliacao.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoAsCegas || 
                    avaliacao.PosicaoAtualFluxoAvaliacao == etapaAutoAvaliacao || 
                    avaliacao.PosicaoAtualFluxoAvaliacao == etapaEmParalelo)
                {
                    if (avaliacao.IdNotaNivel1AvaliacaoCegas != 0) avaliacaoAtual.IdNotaNivel1AvaliacaoCegas = avaliacao.IdNotaNivel1AvaliacaoCegas;
                    if (avaliacao.ComentariosAvaliacaoCegas != null) avaliacaoAtual.ComentariosAvaliacaoCegas = avaliacao.ComentariosAvaliacaoCegas;
                    if (avaliacao.DHCAvaliacaoCegas != null) avaliacaoAtual.DHCAvaliacaoCegas = DateTime.Now;
                    if (avaliacao.USRAvaliacaoCegas != null) avaliacaoAtual.USRAvaliacaoCegas = avaliacao.USRAvaliacaoCegas;
                    avaliacaoAtual.DataHoraInicioAvaliacaoCegas = avaliacao.DataHoraInicioAvaliacaoCegas;
                    avaliacaoAtual.DataHoraFimAvaliacaoCegas = avaliacao.DataHoraFimAvaliacaoCegas;
                }

                //Avaliação do gestor
                if (avaliacao.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoGestor)
                {
                    if (avaliacao.IdNotaNivel1AvaliacaoGestor != 0) avaliacaoAtual.IdNotaNivel1AvaliacaoGestor = avaliacao.IdNotaNivel1AvaliacaoGestor;
                    if (avaliacao.ComentariosAvaliacaoGestor != null) avaliacaoAtual.ComentariosAvaliacaoGestor = avaliacao.ComentariosAvaliacaoGestor;
                    if (avaliacao.DHCAvaliacaoGestor != null) avaliacaoAtual.DHCAvaliacaoGestor = DateTime.Now;
                    if (avaliacao.USRAvaliacaoGestor != null) avaliacaoAtual.USRAvaliacaoGestor = avaliacao.USRAvaliacaoGestor;
                    avaliacaoAtual.DataHoraInicioAvaliacaoGestor = avaliacao.DataHoraInicioAvaliacaoGestor;
                    avaliacaoAtual.DataHoraFimAvaliacaoGestor = avaliacao.DataHoraFimAvaliacaoGestor;
                }

                //Feedback
                if (avaliacao.PosicaoAtualFluxoAvaliacao == etapaFeedback)
                {
                    if (avaliacao.IdNotaNivel1Feedback != 0) avaliacaoAtual.IdNotaNivel1Feedback = avaliacao.IdNotaNivel1Feedback;
                    if (avaliacao.ComentariosFeedback != null) avaliacaoAtual.ComentariosFeedback = avaliacao.ComentariosFeedback;
                    if (avaliacao.DHCFeedback != null) avaliacaoAtual.DHCFeedback = DateTime.Now;
                    if (avaliacao.USRFeedback != null) avaliacaoAtual.USRFeedback = avaliacao.USRFeedback;
                    avaliacaoAtual.DataHoraInicioFeedback = avaliacao.DataHoraInicioFeedback;
                    avaliacaoAtual.DataHoraFimFeedback = avaliacao.DataHoraFimFeedback;
                }

                //Mentor
                if (avaliacao.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoMentor)
                {
                    if (avaliacao.IdNotaNivel1AvaliacaoMentor != 0) avaliacaoAtual.IdNotaNivel1AvaliacaoMentor = avaliacao.IdNotaNivel1AvaliacaoMentor;
                    if (avaliacao.ComentariosAvaliacaoMentor != null) avaliacaoAtual.ComentariosAvaliacaoMentor = avaliacao.ComentariosAvaliacaoMentor;
                    if (avaliacao.DHCAvaliacaoMentor != null) avaliacaoAtual.DHCAvaliacaoMentor = DateTime.Now;
                    if (avaliacao.USRAvaliacaoMentor != null) avaliacaoAtual.USRAvaliacaoMentor = avaliacao.USRAvaliacaoMentor;
                    avaliacaoAtual.DataHoraInicioAvaliacaoMentor = avaliacao.DataHoraInicioAvaliacaoMentor;
                    avaliacaoAtual.DataHoraFimAvaliacaoMentor = avaliacao.DataHoraFimAvaliacaoMentor;
                }

                if (avaliacao.Notificado != null) avaliacaoAtual.Notificado = avaliacao.Notificado;
                if (avaliacao.DataHoraNotificacao != null) avaliacaoAtual.DataHoraNotificacao = avaliacao.DataHoraNotificacao;
                if (avaliacao.USR != 0) avaliacaoAtual.USR = avaliacao.USR;
                if (avaliacao.DHC != null) avaliacaoAtual.DHC = avaliacao.DHC;
                if (avaliacao.Notificado != null) avaliacaoAtual.Notificado = 1;

                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool AvancaProximaEtapaPerformance(AVALIACOESPERFORMANCES avaliacao, AVALIACAO avaliacaoMestre, string TipoAvaliacao = "")
        {
            try
            {
                DataModel context = new DataModel();
                var avaliacaoAlterada = context.AVALIACOESPERFORMANCES.FirstOrDefault(a => a.IdAvaliacaoPerformance == avaliacao.IdAvaliacaoPerformance);

                switch (avaliacaoMestre.PosicaoAtualFluxoAvaliacao)
                {
                    case "AVM":
                        avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaEmParalelo;
                        break;

                    case "AAV":
                        if (VerificaAAVeACEPerformanceConcluidas(avaliacaoMestre, avaliacao))
                        {
                            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoGestor;
                        }
                        break;

                    case "ACE":
                        if (VerificaAAVeACEPerformanceConcluidas(avaliacaoMestre, avaliacao))
                        {
                            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoGestor;
                        }
                        break;

                    case "AEP":
                        if (VerificaAAVeACEPerformanceConcluidas(avaliacaoMestre, avaliacao))
                        {
                            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoGestor;
                        }
                        break;

                    case "AGE":
                        avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaFeedback;

                        if (TipoAvaliacao == "lideranca")
                        {
                            avaliacaoAlterada.DataHoraTermino = DateTime.Now;
                            avaliacaoAlterada.IdAvaliacaoStatus = new StatusService().ObterStatusAvaliacao("Concluído").IdStatus;
                            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoFinalizada;
                        }
                        break;

                    case "FED":
                        avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoMentor;
                        // Ao chegar no Mentor, já Finaliza a Avaliação, pois não há interação do Mentor
                        avaliacaoAlterada.DataHoraTermino = DateTime.Now;
                        avaliacaoAlterada.IdAvaliacaoStatus = new StatusService().ObterStatusAvaliacao("Concluído").IdStatus;

                        if (TipoAvaliacao == "lideranca")
                        {
                            avaliacaoAlterada.DataHoraTermino = DateTime.Now;
                            avaliacaoAlterada.IdAvaliacaoStatus = new StatusService().ObterStatusAvaliacao("Concluído").IdStatus;
                            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoFinalizada;
                        }
                        break;

                    case "AME":
                        // Finaliza Avaliação após o último Nível 
                        avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = etapaAvaliacaoFinalizada;
                        // A linha abaixo subiu para o nível anterior
                        //avaliacaoAlterada.DataHoraTermino = DateTime.Now;
                        break;
                }

                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica se a etapa de Auto-avaliação e Avaliação às cegas foram concuídas
        /// </summary>
        /// <param name="avaliacaoMestre"></param>
        /// <param name="avaliacaoCompetencia"></param>
        /// <returns></returns>
        private bool VerificaAAVeACEPerformanceConcluidas(AVALIACAO avaliacaoMestre, AVALIACOESPERFORMANCES avaliacaoPerformance)
        {

            bool aavConcluidaComp = avaliacaoPerformance.DataHoraFimAutoAvaliacao != null;
            bool aceConcluidaComp = avaliacaoPerformance.DataHoraFimAvaliacaoCegas != null;

            return aavConcluidaComp && aceConcluidaComp;
        }

        /// <summary>
        /// Salva informações de uma avaliação de competência
        /// </summary>
        /// <param name="avaliacao"></param>
        /// <returns></returns>
        public bool SalvarAvaliacaoPerformance(AVALIACOESPERFORMANCES avaliacao)
        {
            try
            {
                DataModel context = new DataModel();
                context.AVALIACOESPERFORMANCES.Add(avaliacao);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Avaliações Email

        public AVALIACAO ObterAvaliacaoEmail(int idProjeto, int idAssociado, int idPeriodo, int idEmpresa, string TipoAvaliacao, string Escopo, int idGestor)
        {
            DataModel context = new DataModel();
            if (TipoAvaliacao == "" && Escopo == "")
            {
                return context.AVALIACAO.FirstOrDefault(a => a.idEmpresa == idEmpresa && a.idProjeto == idProjeto &&
                                                        a.idAssociado == idAssociado && a.idPeriodo == idPeriodo);
            }
            else
            {
                if (idGestor == 0)
                {
                    return context.AVALIACAO.FirstOrDefault(a => a.idEmpresa == idEmpresa && a.idProjeto == idProjeto &&
                                                            a.idAssociado == idAssociado && a.idPeriodo == idPeriodo &&
                                                            a.TipoAvaliacao == TipoAvaliacao && a.Escopo == Escopo);
                }
                else
                {
                    return context.AVALIACAO.FirstOrDefault(a => a.idEmpresa == idEmpresa && a.idProjeto == idProjeto &&
                                                            a.idAssociado == idAssociado && a.idPeriodo == idPeriodo &&
                                                            a.TipoAvaliacao == TipoAvaliacao && a.Escopo == Escopo &&
                                                            a.idGestor == idGestor);
                }
            }
        }

        public List<AVALIACAO> ObterAvaliacoesListas(List<int> idsProjetos, List<int> idsAssociados)
        {
            DataModel context = new DataModel();
            Expression<Func<AVALIACAO, bool>> predicate = a => a.idAvaliacao != -1;

            if (idsProjetos != null) { predicate = predicate.And(a => idsProjetos.Contains(a.idProjeto)); }
            if (idsAssociados != null) { predicate = predicate.And(a => idsAssociados.Contains(a.idAssociado)); }

            return ObterAvaliacoes(predicate);
        }
        public AVALIACAO ObterAvaliacaoEmail(int idAvaliacao)
        {
            DataModel context = new DataModel();
            return context.AVALIACAO.FirstOrDefault(a => a.idAvaliacao == idAvaliacao);
        }

        public List<AVALIACAO> ObterAvaliacoesEmail(int idEmpresa, int idPeriodo)
        {
            Expression<Func<AVALIACAO, bool>> predicate = a => a.idEmpresa == idEmpresa && a.idPeriodo == idPeriodo && a.Liberado == true;
            return ObterAvaliacoes(predicate);
        }
        public List<AVALIACAO> ObterAvaliacoesFinalizadasAssociadoPeriodo(int idAssociado, int idPeriodo, string strTipoAvaliacao, string strEscopo)
        {
            Expression<Func<AVALIACAO, bool>> predicate = a =>
                a.idAssociado == idAssociado &&
                a.idPeriodo == idPeriodo &&
                a.Liberado == true &&
                a.TipoAvaliacao == strTipoAvaliacao &&
                a.Escopo == strEscopo &&
                a.PosicaoAtualFluxoAvaliacao == "AFI";
            return ObterAvaliacoes(predicate);
        }
        public List<AVALIACAO> ObterAvaliacoesEmail(PROJETOS projeto, ASSOCIADOS associado, PERIODOSAVALIACOES periodo, AVALIACOESSTATUS status)
        {
            var idEmpresa = WebStorage.GetUsuarioLogado().IdEmpresa;
            Expression<Func<AVALIACAO, bool>> predicate = e => e.idEmpresa == idEmpresa;

            if (projeto != null)
                predicate = predicate.And(p => p.idProjeto == projeto.IdProjeto);

            if (associado != null)
                predicate = predicate.And(p => p.idAssociado == associado.IdAssociado);

            if (periodo != null)
                predicate = predicate.And(p => p.idPeriodo == periodo.IdPeriodo);

            if (status != null)
                predicate = predicate.And(p => p.idStatus == status.IdStatus);

            return ObterAvaliacoes(predicate);
        }

        private List<AVALIACAO> ObterAvaliacoes(Expression<Func<AVALIACAO, bool>> predicate)
        {
            DataModel context = new DataModel();
            return context.AVALIACAO.Where(predicate).ToList();
        }

        public bool SalvarAvaliacaoEmail(AVALIACAO avaliacao)
        {
            try
            {
                DataModel context = new DataModel();
                context.AVALIACAO.Add(avaliacao);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool AlterarAvaliacaoEmail(int idAvaliacao, AVALIACAO avaliacao)
        {
            bool OK = false;

            try
            {
                using (var db = new DataModel())
                {
                    var result = db.AVALIACAO.SingleOrDefault(c => c.idAvaliacao == idAvaliacao);
                    if (result != null)
                    {
                        result.DataLiberacao = avaliacao.DataLiberacao;
                        result.idAssociado = avaliacao.idAssociado;
                        result.idPeriodo = avaliacao.idPeriodo;
                        result.idProjeto = avaliacao.idProjeto;
                        result.Liberado = avaliacao.Liberado;
                        result.USR = avaliacao.USR;
                        result.idStatus = avaliacao.idStatus;
                        result.PosicaoAtualFluxoAvaliacao = avaliacao.PosicaoAtualFluxoAvaliacao;

                        OK = db.SaveChanges() >= 1;
                    }
                }
            }
            catch
            {
                return false;
            }

            return OK;
        }

        public bool AlterarAvaliacaoGestor(AVALIACAO avaliacao, int? idGestor)
        {
            bool OK = false;

            try
            {
                using (var db = new DataModel())
                {
                    var result = db.AVALIACAO.SingleOrDefault(c => c.idAvaliacao == avaliacao.idAvaliacao);
                    if (result != null)
                    {
                        result.idGestor = idGestor;

                        OK = db.SaveChanges() >= 1;
                    }
                }
            }
            catch
            {
                return false;
            }

            return OK;
        }

        public string CalculaTempoRestante(AVALIACAO avaliacao, string etapaAtual)
        {
            DateTime dataLimite = DateTime.MinValue;
            var workflow = new WorkflowService().ObterByPeriodo(avaliacao.idPeriodo);
            if (workflow != null)
            {
                switch (etapaAtual)
                {
                    case "AAV":
                        dataLimite = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas);
                        break;
                    case "AEP":
                        dataLimite = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas);
                        break;
                    case "ACE":
                        dataLimite = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas);
                        break;
                    case "AGE":
                        dataLimite = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor);
                        break;
                    case "FED":
                        dataLimite = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback);
                        break;
                }
            }

            return dataLimite.ToString("dd'd e 'hh'hs'");
        }

        public string CalculaDataTermino(AVALIACAO avaliacao, string etapaFinal)
        {
            DateTime dataLimite = DateTime.MinValue;
            var workflow = new WorkflowService().ObterByPeriodo(avaliacao.idPeriodo);
            if (workflow != null)
            {
                switch (etapaFinal)
                {
                    case "AAV":
                        dataLimite = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas);
                        break;
                    case "AEP":
                        dataLimite = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas);
                        break;
                    case "ACE":
                        dataLimite = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas);
                        break;
                    case "AGE":
                        dataLimite = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor);
                        break;
                    case "FED":
                        dataLimite = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback);
                        break;
                }
            }

            return dataLimite.ToString("dd/MM/yyyy");
        }

        public void AvancaProximaEtapaEmail(AVALIACAO avaliacao, bool avancarEtapa = true, bool enviarEmail = true)
        {
            DataModel context = new DataModel();
            if (avaliacao == null) { return; }
            var avaliacaoAlterada = context.AVALIACAO.FirstOrDefault(a => a.idAvaliacao == avaliacao.idAvaliacao);

            avaliacaoAlterada.PosicaoAtualFluxoAvaliacao = avancarEtapa ? ProximaEtapa(avaliacao.PosicaoAtualFluxoAvaliacao, avaliacao.TipoAvaliacao) : avaliacaoAlterada.PosicaoAtualFluxoAvaliacao;       

            if (avaliacaoAlterada.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoFinalizada)
            {
                // Ao chegar no Mentor, já Finaliza a Avaliação, pois não há interação do Mentor
                avaliacaoAlterada.idStatus = new StatusService().ObterStatusAvaliacao("Concluído").IdStatus;
            }
            context.SaveChanges();

            if (!enviarEmail)
                return;

            if (avaliacaoAlterada.PosicaoAtualFluxoAvaliacao == etapaAutoAvaliacao)
            {
                EnviarEmailAvancoEtapa(avaliacaoAlterada);
                //EnviarEmailAvancoEtapaMentor(avaliacaoAlterada);
                //EnviarEmailAvancoEtapaAvaliador(avaliacaoAlterada);
            }
            if (avaliacaoAlterada.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoAsCegas)
            {
                //EnviarEmailAvancoEtapaGestor(avaliacaoAlterada);
                EnviarEmailAvancoEtapaAvaliador(avaliacaoAlterada);
            }
            if (avaliacaoAlterada.PosicaoAtualFluxoAvaliacao == etapaEmParalelo)
            {
                EnviarEmailAvancoEtapa(avaliacaoAlterada);
                EnviarEmailAvancoEtapaAvaliador(avaliacaoAlterada);
            }
            if (avaliacaoAlterada.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoGestor)
            {
                EnviarEmailAvancoEtapaGestor(avaliacaoAlterada);
                //EnviarEmailAvancoEtapaAvaliador(avaliacaoAlterada);
            }
            if (avaliacaoAlterada.PosicaoAtualFluxoAvaliacao == etapaFeedback)
            {
                EnviarEmailAvancoEtapaGestor(avaliacaoAlterada);
                //EnviarEmailAvancoEtapaAvaliador(avaliacaoAlterada);
            }
            if (avaliacaoAlterada.PosicaoAtualFluxoAvaliacao == etapaAvaliacaoFinalizada)
                EnviarEmailAvancoEtapaMentor(avaliacaoAlterada);

        }

        private void EnviarEmailAvancoEtapa(AVALIACAO avaliacao)
        {
            var workflow = new WorkflowService().ObterByPeriodo(avaliacao.idPeriodo);
            var paramEmail = new EmailParametroService().ObterParametro(avaliacao.idEmpresa);
            var configEmail = new ConfigEmail();
            string etapaEmail = "";
            configEmail.From = paramEmail.RemetenteEmail;
            configEmail.SmtpServer = paramEmail.SMTPServer;
            configEmail.Porta = Convert.ToInt32(paramEmail.Porta);
            configEmail.Dominio = paramEmail.Dominio;
            configEmail.Senha = paramEmail.Password;
            configEmail.UsarSSL = Convert.ToBoolean(paramEmail.UsarSSL);
            configEmail.Remetente = paramEmail.RemetenteNome;

            ASSOCIADOS pessoa = new ASSOCIADOS();
            string dataFinal = "";

            var prazo = new WorkflowService().ObterPrazo(avaliacao.IdPrazo);

            switch (avaliacao.PosicaoAtualFluxoAvaliacao)
            {
                case "AAV":
                    pessoa = avaliacao.ASSOCIADOS;
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");// workflow.DiasAutoAvaliacao).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Auto Avaliação";
                    break;

                case "AEP":
                    pessoa = avaliacao.ASSOCIADOS;
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");// workflow.DiasAutoAvaliacao).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Auto Avaliação";
                    break;

                case "ACE":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoAsCegas).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação as Cegas";
                    break;

                case "AGE":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoGestor).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação Gestor";
                    break;

                case "FED":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoFeedback).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorFeedback).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Feedback";
                    break;

                case "AME":
                    pessoa = new AssociadosService().ObterMentor(avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoMentor).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorMentor).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação Mentor";
                    break;
            }

            var destinatario = new Destinatario();
            destinatario.Nome = pessoa.Nome;
            destinatario.Email = pessoa.Email;

            //xxxx
            var associadoAvaliado = new AssociadosService().ObterAssociado(avaliacao.idAssociado);
            var mentorAssociadoAvaliado = new AssociadosService().ObterAssociado(associadoAvaliado.IdAssociadoMentor);
            var gestorAssociadoAvaliado = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
            var alocacao = new ProjetosService().ObterProjetoAssociadoAtributos(avaliacao.idAssociado, avaliacao.idProjeto, (int)avaliacao.idGestor, avaliacao.idPeriodo, avaliacao.TipoAvaliacao, avaliacao.Escopo);
            var avaliador = new AssociadosService().ObterAssociado((int)alocacao.IdAvaliador);


            // MONTA O EMAIL
            var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
            _corpo = ConfiguraBody(_corpo, associadoAvaliado, avaliacao.PROJETOS, pessoa, avaliacao.PosicaoAtualFluxoAvaliacao, "", "", "", "", dataFinal, mentorAssociadoAvaliado, gestorAssociadoAvaliado,
                avaliador, alocacao);

            var mensagem = new Mensagem();
            mensagem.Titulo = "[RH Peers] Processo de Avaliação - " + etapaEmail + " do/a " + associadoAvaliado.Nome;

            mensagem.Corpo = _corpo;

            var emailService = new EmailService(configEmail, destinatario, mensagem);

            var thread = new Thread(new ThreadStart(emailService.Enviar));
            thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
            thread.IsBackground = true;
            thread.Start();
        }

        private void EnviarEmailAvancoEtapaGestor(AVALIACAO avaliacao)
        {
            var workflow = new WorkflowService().ObterByPeriodo(avaliacao.idPeriodo);
            var paramEmail = new EmailParametroService().ObterParametro(avaliacao.idEmpresa);
            var configEmail = new ConfigEmail();
            string etapaEmail = "";
            configEmail.From = paramEmail.RemetenteEmail;
            configEmail.SmtpServer = paramEmail.SMTPServer;
            configEmail.Porta = Convert.ToInt32(paramEmail.Porta);
            configEmail.Dominio = paramEmail.Dominio;
            configEmail.Senha = paramEmail.Password;
            configEmail.UsarSSL = Convert.ToBoolean(paramEmail.UsarSSL);
            configEmail.Remetente = paramEmail.RemetenteNome;

            ASSOCIADOS pessoa = new ASSOCIADOS();
            string dataFinal = "";

            //List<PROJETOSASSOCIADOS> projetosAssociados = new ProjetosService().ObterListaAssociados(avaliacao.idProjeto, avaliacao.idAssociado);

            var prazo = new WorkflowService().ObterPrazo(avaliacao.IdPrazo);
            switch (avaliacao.PosicaoAtualFluxoAvaliacao)
            {
                case "AAV":
                    pessoa = avaliacao.ASSOCIADOS;
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");// workflow.DiasAutoAvaliacao).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Auto Avaliação";
                    break;

                case "ACE":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoAsCegas).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação as Cegas";
                    break;

                case "AGE":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoGestor).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação Gestor";
                    break;

                case "FED":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoFeedback).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorFeedback).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Feedback";
                    break;

                case "AME":
                    pessoa = new AssociadosService().ObterMentor(avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoMentor).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorMentor).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação Mentor";
                    break;

                    #region OLD
                    //case "AAV":
                    //    pessoa = avaliacao.ASSOCIADOS;
                    //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao).ToString("dd/MM/yyyy");
                    //    etapaEmail = "Auto Avaliação";
                    //    break;

                    //case "ACE":
                    //    pessoa = new AssociadosService().ObterAssociado(Convert.ToInt32(projetosAssociados[0].IdGestor));
                    //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas).ToString("dd/MM/yyyy");
                    //    etapaEmail = "Avaliação as Cegas";
                    //    break;

                    //case "AGE":
                    //    pessoa = new AssociadosService().ObterAssociado(Convert.ToInt32(projetosAssociados[0].IdGestor));
                    //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor).ToString("dd/MM/yyyy");
                    //    etapaEmail = "Avaliação Gestor";
                    //    break;

                    //case "FED":
                    //    pessoa = new AssociadosService().ObterAssociado(Convert.ToInt32(projetosAssociados[0].IdGestor));
                    //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    //    etapaEmail = "Feedback";
                    //    break;

                    //case "AME":
                    //    pessoa = new AssociadosService().ObterAssociado(Convert.ToInt32(projetosAssociados[0].IdGestor));
                    //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    //    etapaEmail = "Avaliação Mentor";
                    //    break;
                    #endregion
            }


            var destinatario = new Destinatario();
            destinatario.Nome = pessoa.Nome;
            destinatario.Email = pessoa.Email;

            //xxxx
            var associadoAvaliado = new AssociadosService().ObterAssociado(avaliacao.idAssociado);
            var mentorAssociadoAvaliado = new AssociadosService().ObterAssociado(associadoAvaliado.IdAssociadoMentor);
            var gestorAssociadoAvaliado = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
            var alocacao = new ProjetosService().ObterProjetoAssociadoAtributos(avaliacao.idAssociado, avaliacao.idProjeto, (int)avaliacao.idGestor, avaliacao.idPeriodo, avaliacao.TipoAvaliacao, avaliacao.Escopo);
            var avaliador = new AssociadosService().ObterAssociado((int)alocacao.IdAvaliador);

            // MONTA O EMAIL
            var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
            _corpo = ConfiguraBody(_corpo, associadoAvaliado, avaliacao.PROJETOS, pessoa, avaliacao.PosicaoAtualFluxoAvaliacao, "", "", "", "", dataFinal, mentorAssociadoAvaliado, gestorAssociadoAvaliado,
                avaliador, alocacao);

            var mensagem = new Mensagem();
            mensagem.Titulo = "[RH Peers] Processo de Avaliação - " + etapaEmail + " do/a " + associadoAvaliado.Nome;


            mensagem.Corpo = _corpo;

            var emailService = new EmailService(configEmail, destinatario, mensagem);

            var thread = new Thread(new ThreadStart(emailService.Enviar));
            thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
            thread.IsBackground = true;
            thread.Start();
        }

        private void EnviarEmailAvancoEtapaAvaliador(AVALIACAO avaliacao)
        {
            var workflow = new WorkflowService().ObterByPeriodo(avaliacao.idPeriodo);
            var paramEmail = new EmailParametroService().ObterParametro(avaliacao.idEmpresa);
            var configEmail = new ConfigEmail();
            string etapaEmail = "";
            configEmail.From = paramEmail.RemetenteEmail;
            configEmail.SmtpServer = paramEmail.SMTPServer;
            configEmail.Porta = Convert.ToInt32(paramEmail.Porta);
            configEmail.Dominio = paramEmail.Dominio;
            configEmail.Senha = paramEmail.Password;
            configEmail.UsarSSL = Convert.ToBoolean(paramEmail.UsarSSL);
            configEmail.Remetente = paramEmail.RemetenteNome;

            ASSOCIADOS pessoa = new ASSOCIADOS();
            string dataFinal = "";

            //List<PROJETOSASSOCIADOS> projetosAssociados = new ProjetosService().ObterListaAssociados(avaliacao.idProjeto, avaliacao.idAssociado);

            var prazo = new WorkflowService().ObterPrazo(avaliacao.IdPrazo);
            switch (avaliacao.PosicaoAtualFluxoAvaliacao)
            {
                case "AAV":
                    pessoa = avaliacao.ASSOCIADOS;
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");// workflow.DiasAutoAvaliacao).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Auto Avaliação";
                    break;

                case "AEP":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoAsCegas).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação as Cegas";
                    break;

                case "ACE":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoAsCegas).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação as Cegas";
                    break;

                case "AGE":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoGestor).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação Gestor";
                    break;

                case "FED":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoFeedback).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorFeedback).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Feedback";
                    break;

                case "AME":
                    pessoa = new AssociadosService().ObterMentor(avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoMentor).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorMentor).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação Mentor";
                    break;

                #region OLD
                //case "AAV":
                //    pessoa = new AssociadosService().ObterAssociado(Convert.ToInt32(projetosAssociados[0].IdAvaliador));
                //    //dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao).ToString("dd/MM/yyyy");
                //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/mm/yyyy");
                //    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/mm/yyyy") : dataFinal;
                //    etapaEmail = "Auto Avaliação";
                //    break;

                //case "ACE":
                //    pessoa = new AssociadosService().ObterAssociado(Convert.ToInt32(projetosAssociados[0].IdAvaliador));
                //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas).ToString("dd/MM/yyyy");
                //    etapaEmail = "Avaliação as Cegas";
                //    break;

                //case "AGE":
                //    pessoa = new AssociadosService().ObterAssociado(Convert.ToInt32(projetosAssociados[0].IdAvaliador));
                //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor).ToString("dd/MM/yyyy");
                //    etapaEmail = "Avaliação Gestor";
                //    break;

                //case "FED":
                //    pessoa = new AssociadosService().ObterAssociado(Convert.ToInt32(projetosAssociados[0].IdAvaliador));
                //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                //    etapaEmail = "Feedback";
                //    break;

                //case "AME":
                //    pessoa = new AssociadosService().ObterAssociado(Convert.ToInt32(projetosAssociados[0].IdAvaliador));
                //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                //    etapaEmail = "Avaliação Mentor";
                //    break;
                    #endregion
            }

            var associadoAvaliado = new AssociadosService().ObterAssociado(avaliacao.idAssociado);
            var mentorAssociadoAvaliado = new AssociadosService().ObterAssociado(associadoAvaliado.IdAssociadoMentor);
            var gestorAssociadoAvaliado = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
            var alocacao = new ProjetosService().ObterProjetoAssociadoAtributos(avaliacao.idAssociado, avaliacao.idProjeto, (int)avaliacao.idGestor, avaliacao.idPeriodo, avaliacao.TipoAvaliacao, avaliacao.Escopo);
            var avaliador = new AssociadosService().ObterAssociado((int)alocacao.IdAvaliador);

            var destinatario = new Destinatario();
            pessoa = avaliador;
            destinatario.Nome = pessoa.Nome;
            destinatario.Email = pessoa.Email;

            // MONTA O EMAIL
            var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
            _corpo = ConfiguraBody(_corpo, associadoAvaliado, avaliacao.PROJETOS, pessoa, avaliacao.PosicaoAtualFluxoAvaliacao, "", "", "", "", dataFinal, mentorAssociadoAvaliado, gestorAssociadoAvaliado,
                avaliador, alocacao);

            var mensagem = new Mensagem();
            mensagem.Titulo = "[RH Peers] Processo de Avaliação - " + etapaEmail + " do/a " + associadoAvaliado.Nome;


            mensagem.Corpo = _corpo;

            var emailService = new EmailService(configEmail, destinatario, mensagem);

            var thread = new Thread(new ThreadStart(emailService.Enviar));
            thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
            thread.IsBackground = true;
            thread.Start();
        }

        private void EnviarEmailAvancoEtapaMentor(AVALIACAO avaliacao)
        {
            var workflow = new WorkflowService().ObterByPeriodo(avaliacao.idPeriodo);
            var paramEmail = new EmailParametroService().ObterParametro(avaliacao.idEmpresa);
            var configEmail = new ConfigEmail();
            string etapaEmail = "";
            configEmail.From = paramEmail.RemetenteEmail;
            configEmail.SmtpServer = paramEmail.SMTPServer;
            configEmail.Porta = Convert.ToInt32(paramEmail.Porta);
            configEmail.Dominio = paramEmail.Dominio;
            configEmail.Senha = paramEmail.Password;
            configEmail.UsarSSL = Convert.ToBoolean(paramEmail.UsarSSL);
            configEmail.Remetente = paramEmail.RemetenteNome;

            ASSOCIADOS pessoa = new ASSOCIADOS();
            string dataFinal = "";

            var prazo = new WorkflowService().ObterPrazo(avaliacao.IdPrazo);
            switch (avaliacao.PosicaoAtualFluxoAvaliacao)
            {
                case "AAV":
                    pessoa = avaliacao.ASSOCIADOS;
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAutoAvaliacao).ToString("dd/MM/yyyy");// workflow.DiasAutoAvaliacao).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAutoAvaliacao).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Auto Avaliação";
                    break;

                case "ACE":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoAsCegas).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoAsCegas).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação as Cegas";
                    break;

                case "AGE":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoAvaliacaoGestor).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorAvaliacaoGestor).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação Gestor";
                    break;

                case "FED":
                    pessoa = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoFeedback).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorFeedback).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Feedback";
                    break;

                case "AME":
                    pessoa = new AssociadosService().ObterMentor(avaliacao.ASSOCIADOS.IdAssociado);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoMentor).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorMentor).ToString("dd/MM/yyyy") : dataFinal;
                    etapaEmail = "Avaliação Mentor";
                    break;

                case "AFI":
                    pessoa = new AssociadosService().ObterAssociado(avaliacao.ASSOCIADOS.IdAssociadoMentor);
                    dataFinal = avaliacao.DataLiberacao.Value.AddDays(prazo.DuracaoMentor).ToString("dd/MM/yyyy");//workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    dataFinal = DateTime.Today >= Convert.ToDateTime(dataFinal, System.Globalization.CultureInfo.GetCultureInfo("pt-BR")) ? DateTime.Today.AddDays(prazo.CompensadorMentor).ToString("dd/MM/yyyy") : dataFinal; 
                    etapaEmail = "Avaliação Mentor";
                    break;

                    #region OLD
                    //case "AAV":
                    //    pessoa = new AssociadosService().ObterAssociado(avaliacao.ASSOCIADOS.IdAssociadoMentor);
                    //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao).ToString("dd/MM/yyyy");
                    //    etapaEmail = "Auto Avaliação";
                    //    break;

                    //case "ACE":
                    //    pessoa = new AssociadosService().ObterAssociado(avaliacao.ASSOCIADOS.IdAssociadoMentor);
                    //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas).ToString("dd/MM/yyyy");
                    //    etapaEmail = "Avaliação as Cegas";
                    //    break;

                    //case "AGE":
                    //    pessoa = new AssociadosService().ObterAssociado(avaliacao.ASSOCIADOS.IdAssociadoMentor);
                    //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor).ToString("dd/MM/yyyy");
                    //    etapaEmail = "Avaliação Gestor";
                    //    break;

                    //case "FED":
                    //    pessoa = new AssociadosService().ObterAssociado(avaliacao.ASSOCIADOS.IdAssociadoMentor);
                    //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    //    etapaEmail = "Feedback";
                    //    break;

                    //case "AME":
                    //    pessoa = new AssociadosService().ObterAssociado(avaliacao.ASSOCIADOS.IdAssociadoMentor);
                    //    dataFinal = avaliacao.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback).ToString("dd/MM/yyyy");
                    //    etapaEmail = "Avaliação Mentor";
                    //    break;
                    #endregion
            }


            var destinatario = new Destinatario();
            destinatario.Nome = pessoa.Nome;
            destinatario.Email = pessoa.Email;

            //xxxx
            var associadoAvaliado = new AssociadosService().ObterAssociado(avaliacao.idAssociado);
            var mentorAssociadoAvaliado = new AssociadosService().ObterAssociado(associadoAvaliado.IdAssociadoMentor);
            var gestorAssociadoAvaliado = new AssociadosService().ObterGestor(avaliacao.idProjeto, avaliacao.ASSOCIADOS.IdAssociado);
            var alocacao = new ProjetosService().ObterProjetoAssociadoAtributos(avaliacao.idAssociado, avaliacao.idProjeto, (int)avaliacao.idGestor, avaliacao.idPeriodo, avaliacao.TipoAvaliacao, avaliacao.Escopo);
            var avaliador = new AssociadosService().ObterAssociado((int)alocacao.IdAvaliador);

            // MONTA O EMAIL
            var _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailInicio);
            _corpo = ConfiguraBody(_corpo, associadoAvaliado, avaliacao.PROJETOS, pessoa, avaliacao.PosicaoAtualFluxoAvaliacao, "", "", "", "", dataFinal, mentorAssociadoAvaliado, gestorAssociadoAvaliado,
                avaliador, alocacao);

            var mensagem = new Mensagem();
            mensagem.Titulo = "[RH Peers] Processo de Avaliação - " + etapaEmail + " do/a " + associadoAvaliado.Nome;


            mensagem.Corpo = _corpo;

            var emailService = new EmailService(configEmail, destinatario, mensagem);

            var thread = new Thread(new ThreadStart(emailService.Enviar));
            thread.CurrentCulture = System.Globalization.CultureInfo.CurrentCulture;
            thread.IsBackground = true;
            thread.Start();
        }
        private string ConfiguraBody(string corpo, ASSOCIADOS associadoAvaliado, PROJETOS projeto, ASSOCIADOS associado, string etapaAvaliacao,
            string texto_inicial, string texto_principal, string info_final, string despedida, string prazoFinal, ASSOCIADOS mentorAssociadoAvaliado, ASSOCIADOS gestorAssociadoAvaliado,
            ASSOCIADOS avaliador, PROJETOSASSOCIADOS alocacao)
        {
            // Padrão Email
            var newCorpo = corpo.Replace("[INFO_INICIAL]", texto_inicial);
            newCorpo = newCorpo.Replace("[TEXTO_PRINCIPAL]", texto_principal);
            newCorpo = newCorpo.Replace("[INFO_FINAL]", info_final);
            newCorpo = newCorpo.Replace("[CUMPRIMENTOS]", despedida);

            // Dados Pessoais
            newCorpo = newCorpo.Replace("[NOME]", associado.Nome); // associadoAvaliado.Nome);
            newCorpo = newCorpo.Replace("[NOME_AVALIADO]", associadoAvaliado.Nome);
            newCorpo = newCorpo.Replace("[CARGO]", associadoAvaliado.CARGOS.Cargo);// +" - "+ associadoAvaliado.CARGOSNIVEIS.Nivel);
            newCorpo = newCorpo.Replace("[MENTOR]", mentorAssociadoAvaliado.Nome);
            newCorpo = newCorpo.Replace("[PROJETO]", projeto.Projeto);
            newCorpo = newCorpo.Replace("[AVALIADOR]", avaliador.Nome);
            newCorpo = newCorpo.Replace("[GESTOR]", gestorAssociadoAvaliado.Nome);
            newCorpo = newCorpo.Replace("[DATA_INICIO]", alocacao.DataInicio.ToString("dd/MM/yyyy"));
            newCorpo = newCorpo.Replace("[DATA_FINAL]", alocacao.DataFim?.ToString("dd/MM/yyyy"));
            newCorpo = newCorpo.Replace("[PRAZO_FINAL]", prazoFinal);

            string descricao = "";

            if (etapaAvaliacao == "AAV")
            {
                etapaAvaliacao = "Auto Avaliação";
                descricao = "Uma nova avaliação está disponível para você.";
            }
            if (etapaAvaliacao == "ACE")
            {
                etapaAvaliacao = "Avaliação as Cegas";
                descricao = "O(a) associado(a) [NOME_AVALIADO] já concluiu a etapa de autoavaliação no sistema e agora se inicia a etapa de avaliação às cegas. O prazo para finalização dessa etapa consta no cronograma do processo de avaliação enviado pelo RH por e-mail.";
            }
            if (etapaAvaliacao == "AGE")
            {
                etapaAvaliacao = "Avaliação do Gestor";
                descricao = "O(a) associado(a) [NOME_AVALIADO] já foi avaliado(a) às cegas no sistema e agora se inicia a etapa de avaliação do gestor. O prazo para finalização dessa etapa consta no cronograma do processo de avaliação enviado pelo RH por e-mail.";
            }
            if (etapaAvaliacao == "FED")
            {
                etapaAvaliacao = "Feedback com Avaliado";
                descricao = "O(a) associado(a) [NOME_AVALIADO] já foi avaliado(a) pelo gestor no sistema e agora se inicia a etapa de feedback. O avaliador também pode ser envolvido nessa etapa. O prazo para finalização dessa etapa consta no cronograma do processo de avaliação enviado pelo RH por e-mail.";
            }
            if (etapaAvaliacao == "AME")
            {
                etapaAvaliacao = "Avaliação Mentor";
                descricao = "O(a) associado(a) [NOME_AVALIADO] recebeu uma nova avaliação que você pode consultar na página de evolução do associado no sistema. Também sugerimos o preenchimento da guia do mentor com seus inputs para levar no dia do comitê, de acordo com o cronograma do processo de avaliação enviado pelo RH por e-mail.";
            }
            if (etapaAvaliacao == "AFI")
            {
                etapaAvaliacao = "Avaliação Mentor";
                descricao = "O(a) associado(a) [NOME_AVALIADO] recebeu uma nova avaliação que você pode consultar na página de evolução do associado no sistema. Também sugerimos o preenchimento da guia do mentor com seus inputs para levar no dia do comitê, de acordo com o cronograma do processo de avaliação enviado pelo RH por e-mail.";
            }

            // Mais campos
            newCorpo = newCorpo.Replace("[ETAPA_AVALIACAO]", etapaAvaliacao);
            newCorpo = newCorpo.Replace("[DESCRICAO]", descricao);
            newCorpo = newCorpo.Replace("[NOME_AVALIADO]", associadoAvaliado.Nome);


            return newCorpo;
        }

        #endregion

        #region Resultados
        public List<PALAVRASIRRELEVANTES> ObterPalavrasIrrelevantes()
        {
            DataModel context = new DataModel();
            return context.PALAVRASIRRELEVANTES.ToList();
        }
        #endregion

        #region MODO CALCULO COMPETENCIAS
        public List<MODOCALCULOCOMPETENCIAS> ObterModosCalculosCompetencias()
        {
            DataModel context = new DataModel();
            var returnItems = context.MODOCALCULOCOMPETENCIAS.ToList();
            return returnItems;
        }
        public ResultadoLider_AvDesempenho ObterResultadoLiderComoCompetencia(int idLider = -1, int idPeriodo = -1)
        {
            DataModel context = new DataModel();
            var returnResultado = new ResultadoLider_AvDesempenho();
            returnResultado.idPeriodo = idPeriodo;
            var periodoService = new PeriodoService();
            var listaPeriodos = periodoService.ListaPeriodos(1).OrderBy(x => x.IdPeriodo).Reverse().ToList();
            var getPeriodo = idPeriodo != -1 ? periodoService.ObterPeriodo(idPeriodo) : null;
            var periodoProximo = getPeriodo != null ? listaPeriodos.FirstOrDefault(x => x.ATV == 1 && x.DataInicio > getPeriodo.DataInicio) : null;


            var getAssociacoes = new List<PROJETOSASSOCIADOS>();
            Expression<Func<PROJETOSASSOCIADOS, bool>> predicateAsso = x => x.TipoAvaliacao == "lideranca" && x.ATV == 1;
            if (idLider != -1) { predicateAsso = predicateAsso.And(x => x.IdAssociado == idLider); }
            if (idPeriodo != -1) { predicateAsso = predicateAsso.And(x => x.DataInicio >= getPeriodo.DataInicio); }
            if (periodoProximo != null) { predicateAsso = predicateAsso.And(x => x.DataInicio < periodoProximo.DataInicio); }
            getAssociacoes = context.PROJETOSASSOCIADOS.Where(predicateAsso).ToList();


            var getAvaliacoesLideranca = new List<AVALIACAO>();
            var tempAvaliacoesLideranca = new List<AVALIACAO>();
            Expression<Func<AVALIACAO, bool>> predicate = x => x.TipoAvaliacao == "lideranca";
            if (idLider != -1) { predicate = predicate.And(x => x.idAssociado == idLider); }
            if (idPeriodo != -1) { predicate = predicate.And(x => x.idPeriodo == idPeriodo); }
            getAvaliacoesLideranca = context.AVALIACAO.Where(predicate).ToList();
            foreach (var item in getAvaliacoesLideranca)
            {
                var hasAssociacao = getAssociacoes.FirstOrDefault(x => x.IdAssociado == item.idAssociado && x.IdProjeto == item.idProjeto);
                if (hasAssociacao != null)
                {
                    tempAvaliacoesLideranca.Add(item);
                }
            }
            getAvaliacoesLideranca = tempAvaliacoesLideranca;

            var getRespostasLideranca = new List<AVALIACOESCOMPETENCIAS>();
            var tempRespostasLideranca = new List<AVALIACOESCOMPETENCIAS>();
            Expression<Func<AVALIACOESCOMPETENCIAS, bool>> predicateAC = x => x.TipoAvaliacao == "lideranca";
            if (idLider != -1) { predicateAC = predicateAC.And(x => x.IdAssociado == idLider); }
            if (idPeriodo != -1) { predicateAC = predicateAC.And(x => x.IdPeriodo == idPeriodo); }
            getRespostasLideranca = context.AVALIACOESCOMPETENCIAS.Where(predicateAC).ToList();
            foreach (var item in getRespostasLideranca)
            {
                var hasAssociacao = getAssociacoes.FirstOrDefault(x => x.IdAssociado == item.IdAssociado && x.IdProjeto == item.IdProjeto);
                if (hasAssociacao != null)
                {
                    tempRespostasLideranca.Add(item);
                }
            }
            getRespostasLideranca = tempRespostasLideranca;

            var getAvaliacoes = getRespostasLideranca.Select(x => x.idAvaliacao).Distinct().ToList();

            returnResultado.countAvaliacoesTotal = getAvaliacoesLideranca.Count();
            returnResultado.countAvaliacoesRespondidas = returnResultado.countAvaliacoesTotal;
            foreach (var idAvaliacao in getAvaliacoes)
            {
                var getRespostas = getRespostasLideranca.Where(x => x.idAvaliacao == idAvaliacao).ToList();
                var countNotaVazia = getRespostas.Where(x => x.IdNotaNivel1AvaliacaoGestor == 0 || x.IdNotaNivel1AvaliacaoGestor == null).Count();
                if (countNotaVazia > 0)
                {
                    returnResultado.countAvaliacoesRespondidas -= 1;
                }
            }

            var getRespondidas = getRespostasLideranca.Where(x => x.IdNotaNivel1AvaliacaoGestor != 0 && x.IdNotaNivel1AvaliacaoGestor != null).ToList();
            var listaNotas = ObterAvaliacaoCompetenciasNotas(-1);
            var notasLider = new List<decimal?>();
            foreach (var item in getRespondidas)
            {
                notasLider.Add(listaNotas.FirstOrDefault(x => x.IdNota == (int)item.IdNotaNivel1AvaliacaoGestor).Peso);
            }
            returnResultado.mediaTotal = notasLider.Average();

            if (returnResultado.countAvaliacoesTotal == 0)
            {
                returnResultado.textoMediaTotal = "Nenhuma avaliação de liderança recebida.";
            }
            else if (returnResultado.countAvaliacoesRespondidas < returnResultado.countAvaliacoesTotal)
            {
                returnResultado.textoMediaTotal = "Avaliações de liderança pendentes.";
            }
            else if (returnResultado.countAvaliacoesRespondidas < 2)
            {
                returnResultado.textoMediaTotal = "Mínimo de 2 avaliações não atingido.";
            }
            else
            {
                returnResultado.textoMediaTotal = returnResultado.mediaTotal.ToString() + " / 100,00";
            }

            return returnResultado;
        }

        public ResultadoLider_AvDesempenho ObterResultadoLiderComoCompetencia_Otimizado(List<AVALIACAO> listAvaliacao, List<AVALIACOESCOMPETENCIAS> listAvCompetencias,
            int idLider = -1, int idPeriodo = -1)
        {
            DataModel context = new DataModel();
            var returnResultado = new ResultadoLider_AvDesempenho();
            returnResultado.idPeriodo = idPeriodo;

            var getAvaliacoesLideranca = new List<AVALIACAO>();
            getAvaliacoesLideranca = listAvaliacao.Where(x => x.TipoAvaliacao == "lideranca").ToList();
            if (idLider != -1) { getAvaliacoesLideranca = getAvaliacoesLideranca.Where(x => x.idAssociado == idLider).ToList(); }
            if (idPeriodo != -1) { getAvaliacoesLideranca = getAvaliacoesLideranca.Where(x => x.idPeriodo == idPeriodo).ToList(); }

            var getRespostasLideranca = new List<AVALIACOESCOMPETENCIAS>();
            getRespostasLideranca = listAvCompetencias.Where(x => x.TipoAvaliacao == "lideranca").ToList();
            if (idLider != -1) { getRespostasLideranca = getRespostasLideranca.Where(x => x.IdAssociado == idLider).ToList(); }
            if (idPeriodo != -1) { getRespostasLideranca = getRespostasLideranca.Where(x => x.IdPeriodo == idPeriodo).ToList(); }

            var getAvaliacoes = getRespostasLideranca.Select(x => x.idAvaliacao).Distinct().ToList();

            returnResultado.countAvaliacoesTotal = getAvaliacoesLideranca.Count();
            returnResultado.countAvaliacoesRespondidas = returnResultado.countAvaliacoesTotal;
            foreach (var idAvaliacao in getAvaliacoes)
            {
                var getRespostas = getRespostasLideranca.Where(x => x.idAvaliacao == idAvaliacao).ToList();
                var countNotaVazia = getRespostas.Where(x => x.IdNotaNivel1AvaliacaoGestor == 0 || x.IdNotaNivel1AvaliacaoGestor == null).Count();
                if (countNotaVazia > 0)
                {
                    returnResultado.countAvaliacoesRespondidas -= 1;
                }
            }

            var getRespondidas = getRespostasLideranca.Where(x => x.IdNotaNivel1AvaliacaoGestor != 0 && x.IdNotaNivel1AvaliacaoGestor != null).ToList();
            var listaNotas = ObterAvaliacaoCompetenciasNotas(-1);
            var notasLider = new List<decimal?>();
            foreach (var item in getRespondidas)
            {
                notasLider.Add(listaNotas.FirstOrDefault(x => x.IdNota == (int)item.IdNotaNivel1AvaliacaoGestor).Peso);
            }
            returnResultado.mediaTotal = notasLider.Average();

            if (returnResultado.countAvaliacoesTotal == 0)
            {
                returnResultado.textoMediaTotal = "Nenhuma avaliação de liderança recebida.";
            }
            else if (returnResultado.countAvaliacoesRespondidas < returnResultado.countAvaliacoesTotal)
            {
                returnResultado.textoMediaTotal = "Avaliações de liderança pendentes.";
            }
            else if (returnResultado.countAvaliacoesRespondidas < 2)
            {
                returnResultado.textoMediaTotal = "Mínimo de 2 avaliações não atingido.";
            }
            else
            {
                returnResultado.textoMediaTotal = returnResultado.mediaTotal.ToString() + " / 100,00";
            }

            return returnResultado;
        }
        #endregion
    }
}

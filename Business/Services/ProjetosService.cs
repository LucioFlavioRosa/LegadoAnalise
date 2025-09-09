using Business.DataAccess;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using log4net;
using System.Xml.Schema;
using System.Data.SqlClient;

namespace Business.Services
{
    public class ProjetosService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ProjetosService));
        public PROJETOS ObterProjeto(int idProjeto)
        {
            DataModel context = new DataModel();
            PROJETOS projeto = new PROJETOS();
            projeto = context.PROJETOS.Where(p => p.IdProjeto == idProjeto).FirstOrDefault();
            context.Dispose();
            return projeto;
        }

        public PROJETOS ObterProjetoNome(string projetoNome)
        {
            DataModel context = new DataModel();
            PROJETOS projeto = new PROJETOS();
            projeto = context.PROJETOS.Where(p => p.Projeto == projetoNome).FirstOrDefault();
            context.Dispose();
            return projeto;
        }

        public bool InserirProjeto(PROJETOS projeto)
        {
            try
            {
                DataModel context = new DataModel();
                context.PROJETOS.Add(projeto);
                context.SaveChanges();

                context.Dispose();
            }
            catch(Exception ex) 
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        public PROJETOS ObterProjeto(string nomeProjeto, string codigoProjeto, int statusProjeto, int responsavelProjeto)
        {
            DataModel context = new DataModel();
            PROJETOS projeto = new PROJETOS();
            projeto = context.PROJETOS.Where(p => p.Projeto == nomeProjeto && p.Codigo == codigoProjeto && p.IdStatus == statusProjeto && p.IdAssociadoResponsavel == responsavelProjeto).FirstOrDefault();
            context.Dispose();
            return projeto;
        }


        public bool InserirProjetoAssociado(PROJETOSASSOCIADOS projetoAssociado)
        {
            try
            {
                DataModel context = new DataModel();
                context.PROJETOSASSOCIADOS.Add(projetoAssociado);
                context.SaveChanges();
                context.Dispose();

            }
            catch (Exception err)
            {
                //string e = err.Message;
                log.Error(err.InnerException.InnerException != null ? err.InnerException.InnerException.Message : err.Message);

                return false;
            }

            return true;
        }


        public List<PROJETOS> ObterListaProjetos()
        {
            return ObterProjetos(null);
        }

        public List<PROJETOS> ObterListaProjetosAtributos(int idGestor = -1, int ATV = 1)
        {
            Expression<Func<PROJETOS, bool>> predicate = null;
            predicate = f => f.ATV == ATV;

            if (idGestor != -1) { predicate = predicate.And(f => f.IdAssociadoGestor == idGestor); }

            return ObterProjetos(predicate);
        }

        public List<PROJETOS> ListaProjetosAtivos(ASSOCIADOS associado)
        {
            Expression<Func<PROJETOSASSOCIADOS, bool>> predicateFK = null;
            predicateFK = a => a.IdAssociado == associado.IdAssociado && a.ATV == 1;

            DataModel context = new DataModel();
            var listaIds = context.PROJETOSASSOCIADOS.Where(predicateFK).Select(s => s.IdProjeto).ToList();

            Expression<Func<PROJETOS, bool>> predicate = null;
            predicate = p => (p.IdAssociadoResponsavel == associado.IdAssociado
            || listaIds.Contains(p.IdProjeto)) && p.ATV == 1;

            return ObterProjetos(predicate);
        }

        public List<PROJETOS> ListaProjetosAtivos()
        {
            return ObterProjetos(p => p.ATV == 1);
        }

        public List<PROJETOS> ListaProjetosAtivos(PERIODOSAVALIACOES periodo)
        {
            Expression<Func<PROJETOS, bool>> predicate = null;
            predicate = f => f.ATV == 1;

            // Overlap Date
            // bool overlap = a.start < b.end && b.start < a.end;
            predicate = predicate.And(p => periodo.DataInicio <= p.DataFim && p.DataInicio <= periodo.DataFim);

            return ObterProjetos(predicate);
        }

        public List<PROJETOS> ListaProjetosAtivos(ASSOCIADOS associado, PROJETOS projeto, PROJETOSSTATUS status, PERIODOSAVALIACOES periodo, CLIENTES cliente)
        {
            Expression<Func<PROJETOSASSOCIADOS, bool>> predicateFK = f => f.ATV == 1;

            if (associado != null)
                predicateFK = predicateFK.And(f => f.IdAssociado == associado.IdAssociado);

            // Overlap Date
            // bool overlap = a.start < b.end && b.start < a.end;
            if (periodo != null)
                //predicateFK = predicateFK.And(p => periodo.DataInicio <= p.DataFim && p.DataInicio <= periodo.DataFim);
                predicateFK = predicateFK.And(p => p.DataInicio >= periodo.DataInicio || p.IdPeriodoSinalizado == periodo.IdPeriodo);

            var periodosTodos = new PeriodoService().ListaTodosPeriodos(1).Where(x => x.ATV == 1);
            periodosTodos = periodosTodos.OrderByDescending(x => x.IdPeriodo).Reverse();
            var periodoProximo = periodosTodos.FirstOrDefault(x => x.DataInicio > periodo.DataInicio);
            if (periodoProximo != null)
            {
                predicateFK = predicateFK.And(p => p.DataInicio < periodoProximo.DataInicio || p.IdPeriodoSinalizado == periodo.IdPeriodo);
            }

            DataModel context = new DataModel();
            var listaIds = context.PROJETOSASSOCIADOS.Where(predicateFK).Select(s => s.IdProjeto).ToList();

            Expression<Func<PROJETOS, bool>> predicate = p => p.ATV == 1 && listaIds.Contains(p.IdProjeto);

            if (projeto != null)
                predicate = predicate.And(p => p.IdProjeto == projeto.IdProjeto);

            if (status != null)
                predicate = predicate.And(p => p.IdStatus == status.IdStatus);

            if (cliente != null)
                predicate = predicate.And(p => p.IdCliente == cliente.IdCliente);

            return ObterProjetos(predicate);
        }

        public List<PROJETOS> ListaProjetosAtivosComoGestor(ASSOCIADOS gestor)
        {
            Expression<Func<PROJETOSASSOCIADOS, bool>> predicateFK = null;
            predicateFK = a => a.IdGestor == gestor.IdAssociado && a.ATV == 1;

            DataModel context = new DataModel();
            var listaIds = context.PROJETOSASSOCIADOS.Where(predicateFK).Select(s => s.IdProjeto).ToList();

            Expression<Func<PROJETOS, bool>> predicate = null;
            predicate = p => (p.IdAssociadoGestor == gestor.IdAssociado 
            || listaIds.Contains(p.IdProjeto)) && p.ATV == 1;

            return ObterProjetos(predicate);
        }

        public List<PROJETOS> ListaProjetosAtivosComoGestor(ASSOCIADOS gestor, PROJETOS projeto, PROJETOSSTATUS status, PERIODOSAVALIACOES periodo, CLIENTES cliente)
        {
            Expression<Func<PROJETOSASSOCIADOS, bool>> predicateFK = null;
            predicateFK = f => f.IdGestor == gestor.IdAssociado || f.IdAvaliador == gestor.IdAssociado || f.PROJETOS.IdAssociadoResponsavel == gestor.IdAssociado ||
             f.PROJETOS.IdAssociadoGestor == gestor.IdAssociado && f.ATV == 1;

            // Overlap Date
            // bool overlap = a.start < b.end && b.start < a.end;
            if (periodo != null)
                predicateFK = predicateFK.And(p => periodo.DataInicio <= p.DataFim && p.DataInicio <= periodo.DataFim);

            DataModel context = new DataModel();
            var listaIds = context.PROJETOSASSOCIADOS.Where(predicateFK).Select(s => s.IdProjeto).ToList();

            Expression<Func<PROJETOS, bool>> predicate = null;
            predicate = p => listaIds.Contains(p.IdProjeto) && p.IdEmpresa == gestor.IdEmpresa && p.ATV == 1;

            if (projeto != null)
                predicate = predicate.And(p => p.IdProjeto == projeto.IdProjeto);

            if (status != null)
                predicate = predicate.And(p => p.IdStatus == status.IdStatus);

            if (cliente != null)
                predicate = predicate.And(p => p.IdCliente == cliente.IdCliente);

            return ObterProjetos(predicate);
        }

        public List<PROJETOS> ListaProjetosAtivosComoMentor(ASSOCIADOS mentor)
        {
            DataModel context = new DataModel();
            
            // Pega a lista de Associados do Mentor passado como parâmetro
            var listaIdsAssociados = context.ASSOCIADOS.Where(a => a.IdAssociadoMentor == mentor.IdAssociado && a.ATV == 1).Select(s => s.IdAssociado).ToList();

            // Pega a lista de Projetos que possui associados da lista obtida
            var listaIdsProjetos = context.PROJETOSASSOCIADOS.Where(p => listaIdsAssociados.Contains(p.IdAssociado) && p.ATV == 1).Select(s => s.IdProjeto).ToList();

            Expression<Func<PROJETOS, bool>> predicate = null;
            predicate = p => listaIdsProjetos.Contains(p.IdProjeto) && p.ATV == 1;

            return ObterProjetos(predicate);
        }

        public List<PROJETOS> ListaProjetosAtivosComoMentor(ASSOCIADOS mentor, PROJETOS projeto, PROJETOSSTATUS status, PERIODOSAVALIACOES periodo, CLIENTES cliente, string TipoAvaliacao)
        {
            DataModel context = new DataModel();

            // Pega a lista de Associados do Mentor passado como parâmetro
            var listaIdsAssociados = context.ASSOCIADOS.Where(a => a.IdAssociadoMentor == mentor.IdAssociado && a.ATV == 1).Select(s => s.IdAssociado).ToList();

            Expression<Func<PROJETOSASSOCIADOS, bool>> predicateFK = null;
            predicateFK = p => listaIdsAssociados.Contains(p.IdAssociado) && p.ATV == 1;

            // Overlap Date
            // bool overlap = a.start < b.end && b.start < a.end;
            if (periodo != null)
                predicateFK = predicateFK.And(p => periodo.DataInicio <= p.DataFim && p.DataInicio <= periodo.DataFim);

            // Adiciona TipoAvaliacao no predicate
            predicateFK = predicateFK.And(p => p.TipoAvaliacao == TipoAvaliacao);

            // Pega a lista de Projetos que possui associados da lista obtida
            var listaIdsProjetos = context.PROJETOSASSOCIADOS.Where(predicateFK).Select(s => s.IdProjeto).ToList();

            Expression<Func<PROJETOS, bool>> predicate = null;
            predicate = p => listaIdsProjetos.Contains(p.IdProjeto) && p.IdEmpresa == mentor.IdEmpresa && p.ATV == 1;

            if (projeto != null)
                predicate = predicate.And(p => p.IdProjeto == projeto.IdProjeto);

            if (status != null)
                predicate = predicate.And(p => p.IdStatus == status.IdStatus);

            if (cliente != null)
                predicate = predicate.And(p => p.IdCliente == cliente.IdCliente);

            return ObterProjetos(predicate);
        }

        private List<PROJETOS> ObterProjetos(Expression<Func<PROJETOS, bool>> predicate)
        {
            DataModel context = new DataModel();
            if (predicate != null)
                return context.PROJETOS.Where(predicate).ToList();
            else
                return context.PROJETOS.ToList();
        }

        public List<PROJETOSASSOCIADOS> ObterListaAssociados(int idProjeto)
        {
            DataModel context = new DataModel();
            return context.PROJETOSASSOCIADOS.Where(p => p.IdProjeto == idProjeto && p.ATV == 1).ToList();
        }

        public List<PROJETOSASSOCIADOS> ObterListaAssociados(int idProjeto, int idAssociado)
        {
            DataModel context = new DataModel();
            return context.PROJETOSASSOCIADOS.Where(p => p.IdProjeto == idProjeto && p.ATV == 1).ToList();
        }

        public List<PROJETOSASSOCIADOS> ObterListaProjetosAssociadosTodos()
        {
            DataModel context = new DataModel();
            return context.PROJETOSASSOCIADOS.ToList();
        }

        public List<PROJETOSASSOCIADOS> ObterListaAssociados(bool ativos, int idProjeto, string TipoAvaliacao = "")
        {
            DataModel context = new DataModel(); 
            if (TipoAvaliacao == "")
                return context.PROJETOSASSOCIADOS.Where(p => p.ATV == (ativos ? 1 : 0) && p.IdProjeto == idProjeto).ToList();
            else
                return context.PROJETOSASSOCIADOS.Where(p => p.ATV == (ativos ? 1 : 0) && p.IdProjeto == idProjeto && p.TipoAvaliacao == TipoAvaliacao).ToList();
        }

        public List<PROJETOSASSOCIADOS> ObterListaAssociados(int idProjeto, int idAssociado, int idGestor, string TipoAvaliacao, string Escopo)
        {
            DataModel context = new DataModel();
            return context.PROJETOSASSOCIADOS.Where(p => p.IdProjeto == idProjeto && p.IdAssociado == idAssociado && p.IdGestor == idGestor && p.TipoAvaliacao == TipoAvaliacao && p.Escopo == Escopo && p.ATV == 1).ToList();
        }

        public List<PROJETOSASSOCIADOS> ObterListaAssociadosNoPeriodo(int idProjeto, int idAssociado, PERIODOSAVALIACOES periodo)
        {
            DataModel context = new DataModel();

            Expression<Func<PROJETOSASSOCIADOS, bool>> predicate = p => p.ATV == 1;
            predicate = idProjeto != -1 ? predicate.And(p => p.IdProjeto == idProjeto) : predicate;
            predicate = idAssociado != -1 ? predicate.And(p => p.IdAssociado == idAssociado) : predicate;

            var associados = context.PROJETOSASSOCIADOS.Where(predicate).ToList();

            if (periodo != null)
            {
                //associados = associados.Where(x => x.DataInicio >= periodo.DataInicio && x.DataFim <= periodo.DataFim).ToList(); // PATCHJUN2022 - IGNORAR DATA DE TÉRMINO DO PERÍODO
                associados = associados.Where(x => x.DataInicio >= periodo.DataInicio || (x.IdPeriodoSinalizado == periodo.IdPeriodo || x.IdPeriodoSinalizado == -1)).ToList();

                var proximoPeriodo = context.PERIODOSAVALIACOES.FirstOrDefault(p => p.IdPeriodo > periodo.IdPeriodo);
                if (proximoPeriodo != null)
                {
                    associados = associados.Where(x => x.DataInicio < proximoPeriodo.DataInicio).ToList();
                }
            }

            return associados;
        }

        public PROJETOSASSOCIADOS ObterProjetoAssociado(int idprojetoassociado)
        {
            DataModel context = new DataModel();
            return context.PROJETOSASSOCIADOS.FirstOrDefault(p => p.IdProjetoAssociado == idprojetoassociado);
        }

        public List<PROJETOSASSOCIADOS> ObterAvaliacoesSinalizadas()
        {
            DataModel context = new DataModel();
            return context.PROJETOSASSOCIADOS.Where(p => p.IdPeriodoSinalizado == -1).ToList();
        }
        public List<PROJETOSASSOCIADOS> ObterAlocacoesAvaliador(int idAvaliador)
        {
            DataModel context = new DataModel();
            return context.PROJETOSASSOCIADOS.Where(p => p.IdAvaliador == idAvaliador).ToList();
        }

        public PROJETOSASSOCIADOS ObterProjetoAssociadoAtributos(int IdAssociado, int IdProjeto, int IdGestor, int IdPeriodo, string TipoAvaliacao, string Escopo = "projeto")
        {
            DataModel context = new DataModel();
            DateTime dataInicio = (DateTime)new PeriodoService().ObterPeriodo(IdPeriodo).DataInicio;
            DateTime dataFim = (DateTime)new PeriodoService().ObterPeriodo(IdPeriodo).DataFim;

            // RETORNA PROJETO ASSOCIADO
            var projetosAssociadosReturn = context.PROJETOSASSOCIADOS.FirstOrDefault(p => p.IdAssociado == IdAssociado && p.IdProjeto == IdProjeto && 
                (IdGestor != -1 ? p.IdGestor == IdGestor : p.IdGestor != -1) &&
                p.DataInicio >= dataInicio && p.DataFim <= dataFim && p.TipoAvaliacao == TipoAvaliacao && p.Escopo == Escopo && p.ATV == 1);

            // SE VAZIO, RETORNA PROJETO ASSOCIADO MAIS RECENTE
            if (projetosAssociadosReturn == null)
            {
                projetosAssociadosReturn = context.PROJETOSASSOCIADOS.OrderByDescending(p => p.IdProjetoAssociado).FirstOrDefault(p => p.IdAssociado == IdAssociado && 
                    p.IdProjeto == IdProjeto &&
                    (IdGestor != -1 ? p.IdGestor == IdGestor : p.IdGestor != -1) 
                    && p.TipoAvaliacao == TipoAvaliacao &&  p.Escopo == Escopo && p.ATV == 1);
            }

            // SE VAZIO, RETORNA PROJETO ASSOCIADO DESCONSIDERANDO GESTOR
            if (projetosAssociadosReturn == null)
            {
                projetosAssociadosReturn = context.PROJETOSASSOCIADOS.OrderByDescending(p => p.IdProjetoAssociado).FirstOrDefault(p => p.IdAssociado == IdAssociado &&
                    p.IdProjeto == IdProjeto && p.TipoAvaliacao == TipoAvaliacao && p.Escopo == Escopo && p.ATV == 1);
            }

            // SE VAZIO, RETORNA PROJETO ASSOCIADO INATIVO
            if (projetosAssociadosReturn == null)
            {
                projetosAssociadosReturn = context.PROJETOSASSOCIADOS.OrderByDescending(p => p.IdProjetoAssociado).FirstOrDefault(p => p.IdAssociado == IdAssociado &&
                    p.IdProjeto == IdProjeto && p.TipoAvaliacao == TipoAvaliacao &&p.Escopo == Escopo);
            }

            return projetosAssociadosReturn;
        }

        public PROJETOSASSOCIADOS ObterProjetoAssociadoHierarquia(int IdAssociado, int IdProjeto, int IdAvaliador, int IdPeriodo, string TipoAvaliacao)
        {
            DataModel context = new DataModel();
            DateTime dataInicio = (DateTime)new PeriodoService().ObterPeriodo(IdPeriodo).DataInicio;
            DateTime dataFim = (DateTime)new PeriodoService().ObterPeriodo(IdPeriodo).DataFim;

            PROJETOSASSOCIADOS projetosAssociadosReturn = null;
            if (TipoAvaliacao == "desempenho")
            {
                // RETORNA PROJETO ASSOCIADO COM DATA REGULAR
                projetosAssociadosReturn = context.PROJETOSASSOCIADOS.FirstOrDefault(p => p.IdAssociado == IdAssociado && p.IdProjeto == IdProjeto && p.IdAvaliador == IdAvaliador &&
                    p.DataInicio >= dataInicio && p.DataFim <= dataFim && p.TipoAvaliacao == TipoAvaliacao && p.ATV == 1);

                // SE VAZIO, RETORNA PROJETO ASSOCIADO MAIS RECENTE
                if (projetosAssociadosReturn == null)
                {
                    projetosAssociadosReturn = context.PROJETOSASSOCIADOS.OrderByDescending(p => p.IdProjetoAssociado).FirstOrDefault(p => p.IdAssociado == IdAssociado &&
                        p.IdProjeto == IdProjeto && p.IdAvaliador == IdAvaliador && p.TipoAvaliacao == TipoAvaliacao && p.ATV == 1);
                }
            }
            else if (TipoAvaliacao == "lideranca")
            {
                // RETORNA PROJETO ASSOCIADO COM DATA REGULAR
                projetosAssociadosReturn = context.PROJETOSASSOCIADOS.FirstOrDefault(p => p.IdAssociado == IdAssociado && p.IdProjeto == IdProjeto && p.IdGestor == IdAvaliador &&
                    p.DataInicio >= dataInicio && p.DataFim <= dataFim && p.TipoAvaliacao == TipoAvaliacao && p.ATV == 1);

                // SE VAZIO, RETORNA PROJETO ASSOCIADO MAIS RECENTE
                if (projetosAssociadosReturn == null)
                {
                    projetosAssociadosReturn = context.PROJETOSASSOCIADOS.OrderByDescending(p => p.IdProjetoAssociado).FirstOrDefault(p => p.IdAssociado == IdAssociado &&
                        p.IdProjeto == IdProjeto && p.IdGestor == IdAvaliador && p.TipoAvaliacao == TipoAvaliacao && p.ATV == 1);
                }
            }
            

            return projetosAssociadosReturn;
        }

        public List<PROJETOSASSOCIADOS> ObterListaAssociadosComoGestor(int idProjeto, int idGestor, PERIODOSAVALIACOES periodo)
        {
            DataModel context = new DataModel();
            var projetos = context.PROJETOSASSOCIADOS.Where(p => p.ATV == 1);

            if (idGestor > 0)
            {
                projetos = projetos.Where(p => p.IdAvaliador == idGestor || p.IdGestor == idGestor || p.PROJETOS.IdAssociadoResponsavel == idGestor || p.PROJETOS.IdAssociadoGestor == idGestor);
            }

            if (idProjeto != -1)
            {
                projetos = projetos.Where(p => p.IdProjeto == idProjeto);
            }
            
            //if (periodo != null)
            //{
            //    projetos = projetos.Where(x => x.DataInicio >= periodo.DataInicio && x.DataFim <= periodo.DataFim);
            //}

            return projetos.ToList();
        }

        public List<PROJETOSASSOCIADOS> ObterListaAssociadosComoMentor(int idProjeto, int idMentor)
        {
            DataModel context = new DataModel();


            // Pega a lista de Associados do Mentor passado como parâmetro
            var listaIdsAssociados = context.ASSOCIADOS.Where(a => a.IdAssociadoMentor == idMentor && a.ATV == 1).Select(s => s.IdAssociado).ToList();

            return context.PROJETOSASSOCIADOS.Where(p => p.IdProjeto == idProjeto && listaIdsAssociados.Contains(p.IdAssociado)).ToList();
        }

        public PROJETOSASSOCIADOS ObterGestorEAvaliadorDeAssociado(int idprojeto, int idAssociado, int idProjetoAssociado = -1)
        {
            DataModel context = new DataModel();
            if (idProjetoAssociado == -1)
                return context.PROJETOSASSOCIADOS.FirstOrDefault(p => p.ATV == 1 && p.IdAssociado == idAssociado && p.IdProjeto == idprojeto);
            else
                return context.PROJETOSASSOCIADOS.FirstOrDefault(p => p.ATV ==1 && p.IdAssociado == idAssociado && p.IdProjeto == idprojeto && p.IdProjetoAssociado == idProjetoAssociado);
        }
        public PROJETOSASSOCIADOS ObterGestorEAvaliadorDeAssociadoDesempenho(int idprojeto, int idAssociado, int idProjetoAssociado = -1, string TipoAvaliacao = "desempenho")
        {
            DataModel context = new DataModel();
            if (idProjetoAssociado == -1)
                return context.PROJETOSASSOCIADOS.FirstOrDefault(p => p.ATV == 1 && p.IdAssociado == idAssociado && p.IdProjeto == idprojeto && p.TipoAvaliacao == TipoAvaliacao);
            else
                return context.PROJETOSASSOCIADOS.FirstOrDefault(p => p.ATV == 1 && p.IdAssociado == idAssociado && p.IdProjeto == idprojeto && p.TipoAvaliacao == TipoAvaliacao && p.IdProjetoAssociado == idProjetoAssociado);
        }

        public List<PROJETOSSTATUS> ListaStatusProjetos()
        {
            DataModel context = new DataModel();
            return context.PROJETOSSTATUS.ToList();
        }

        public void AlterarProjetoAssociado(PROJETOSASSOCIADOS projetoassociado)
        {
            try
            {
                DataModel context = new DataModel();
                var obj = context.PROJETOSASSOCIADOS.SingleOrDefault(x => x.IdProjetoAssociado == projetoassociado.IdProjetoAssociado);

                // JUL2022 - CASO HAJA TROCA DE AVALIADOR DURANTE AVALIAÇÃO EM ANDAMENTO, REENVIAR EMAIL DA ETAPA DE AV. AS CEGAS AO NOVO AVALIADOR
                bool novoAvaliador = obj.IdAvaliador != projetoassociado.IdAvaliador;
                bool novoGestor = obj.IdGestor != projetoassociado.IdGestor;
                int oldGestor = (int)obj.IdGestor;
                
                context.Entry(obj).CurrentValues.SetValues(projetoassociado);
                context.SaveChanges();

                // JUL2022 - CASO HAJA TROCA DE AVALIADOR/GESTOR DURANTE AVALIAÇÃO EM ANDAMENTO, REENVIAR EMAIL DA ETAPA
                if (novoAvaliador || novoGestor)
                {
                    var periodo = new PeriodoService().VerificaExistenciaPeriodo(obj.DataInicio, (DateTime)obj.DataFim, 1);
                    var avaliacao = new AvaliacoesService().ObterAvaliacaoEmail(obj.IdProjeto, obj.IdAssociado, periodo.IdPeriodo, 1, obj.TipoAvaliacao, obj.Escopo, oldGestor);

                    // JUL2022 - CASO HAJA TROCA DE GESTOR DURANTE AVALIAÇÃO EM ANDAMENTO, TROCAR O IDGESTOR DA AVALIAÇÃO EM ANDAMENTO
                    if (novoGestor)
                    {
                        new AvaliacoesService().AlterarAvaliacaoGestor(avaliacao, projetoassociado.IdGestor);
                    }

                    avaliacao = new AvaliacoesService().ObterAvaliacaoEmail(obj.IdProjeto, obj.IdAssociado, periodo.IdPeriodo, 1, obj.TipoAvaliacao, obj.Escopo, (int)obj.IdGestor);
                    new AvaliacoesService().AvancaProximaEtapaEmail(avaliacao, false);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                throw ex;
            }
        }

        public bool AlterarProjeto(PROJETOS projeto)
        {
            try
            {
                DataModel context = new DataModel();
                PROJETOS projetoAtual = new PROJETOS();
                projetoAtual = context.PROJETOS.First(a => a.IdProjeto == projeto.IdProjeto);

                if (projetoAtual != null)
                {
                    projetoAtual.IdEmpresa = projeto.IdEmpresa;
                    projetoAtual.IdCliente = projeto.IdCliente;
                    projetoAtual.IdAssociadoResponsavel = projeto.IdAssociadoResponsavel;
                    projetoAtual.IdAssociadoGestor = projeto.IdAssociadoGestor;
                    projetoAtual.IdStatus = projeto.IdStatus;
                    projetoAtual.IdTipo = projeto.IdTipo;
                    projetoAtual.IdComplexidade = projeto.IdComplexidade;
                    projetoAtual.Codigo = projeto.Codigo;
                    projetoAtual.Projeto = projeto.Projeto;
                    projetoAtual.DataInicio = projeto.DataInicio;
                    projetoAtual.DataFim = projeto.DataFim;
                    projetoAtual.USR = projeto.USR;
                    projetoAtual.ATV = projeto.ATV;
                }
                context.SaveChanges();
                context.Dispose();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        public bool ExcluirProjeto(int idProjeto)
        {
            try
            {
                DataModel context = new DataModel();
                PROJETOS projetoAtual = new PROJETOS();
                projetoAtual = context.PROJETOS.First(a => a.IdProjeto == idProjeto);

                if (projetoAtual != null)
                {
                    projetoAtual.ATV = 0;
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        public bool ExcluirProjetoAssociado(PROJETOSASSOCIADOS projeto)
        {
            try
            {
                DataModel context = new DataModel();
                PROJETOSASSOCIADOS projetoAtual = new PROJETOSASSOCIADOS();
                projetoAtual = context.PROJETOSASSOCIADOS.First(a => a.IdProjetoAssociado == projeto.IdProjetoAssociado);

                if (projetoAtual != null)
                {
                    projetoAtual.ATV = 0;
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }
    }
}
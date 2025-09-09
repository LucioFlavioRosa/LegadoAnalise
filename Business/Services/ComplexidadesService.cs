using Business.DataAccess;
using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class ComplexidadesService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ComplexidadesService));
        // COMPLEXIDADES
        public bool InserirComplexidade(PROJETOSCOMPLEXIDADES complexidade)
        {
            try
            {
                DataModel context = new DataModel();
                context.PROJETOSCOMPLEXIDADES.Add(complexidade);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        public List<PROJETOSCOMPLEXIDADES> ObterListaComplexidades()
        {
            DataModel context = new DataModel();

            List<PROJETOSCOMPLEXIDADES> listacomplexidade = new List<PROJETOSCOMPLEXIDADES>();
            listacomplexidade = context.PROJETOSCOMPLEXIDADES.ToList();
            return listacomplexidade;
        }


        public List<PROJETOSCOMPLEXIDADES> ObterListaComplexidades(bool ativos)
        {
            DataModel context = new DataModel();

            List<PROJETOSCOMPLEXIDADES> listacomplexidade = new List<PROJETOSCOMPLEXIDADES>();
            listacomplexidade = context.PROJETOSCOMPLEXIDADES.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.Complexidade).ToList();
            return listacomplexidade;
        }

        public List<ComplexidadeModel> ObterProjetosComplexidade()
        {
            DataModel context = new DataModel();

            return (from c in context.PROJETOSCOMPLEXIDADES
                    where c.ATV.HasValue && c.ATV == 1
                    select new ComplexidadeModel()
                    {
                        IdComplexidade = c.IdComplexidade,
                        Complexidade = c.Complexidade
                    }).ToList();
        }


        public PROJETOSCOMPLEXIDADES ObterComplexidade(int idComplexidade)
        {
            DataModel context = new DataModel();
            return context.PROJETOSCOMPLEXIDADES.FirstOrDefault(c => c.IdComplexidade == idComplexidade);
        }



        public bool AlterarComplexidade(PROJETOSCOMPLEXIDADES complexidade)
        {
            try
            {
                DataModel context = new DataModel();
                PROJETOSCOMPLEXIDADES complexidadeAtual = new PROJETOSCOMPLEXIDADES();
                complexidadeAtual = context.PROJETOSCOMPLEXIDADES.First(a => a.IdComplexidade == complexidade.IdComplexidade);

                if (complexidadeAtual != null)
                {
                    complexidadeAtual.Complexidade = complexidade.Complexidade;
                    complexidadeAtual.ATV = complexidade.ATV;
                    complexidadeAtual.Ponderacao = complexidade.Ponderacao;
                    complexidadeAtual.FaixaInicial = complexidade.FaixaInicial;
                    complexidadeAtual.FaixaFinal = complexidade.FaixaFinal;
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


        public bool ExcluirComplexidade(int idComplexidade)
        {
            try
            {
                DataModel context = new DataModel();
                PROJETOSCOMPLEXIDADES complexidadeAtual = new PROJETOSCOMPLEXIDADES();
                complexidadeAtual = context.PROJETOSCOMPLEXIDADES.First(a => a.IdComplexidade == idComplexidade);
                complexidadeAtual.ATV = 0;

                if (complexidadeAtual != null)
                {
                    complexidadeAtual.ATV = 0;
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


        // FATORES
        public List<FATORES> ObterFatores()
        {
            DataModel context = new DataModel();
            return context.FATORES.ToList();
        }
        public List<NOTASFATORES> ObterNotasFatores()
        {
            DataModel context = new DataModel();
            return context.NOTASFATORES.ToList();
        }
        public List<FATORESPROJETOS> ObterFatoresProjetos(int idProjeto = -1, int idFatorProjeto = -1)
        {
            DataModel context = new DataModel();
            var returnFatoresProjetos = context.FATORESPROJETOS.ToList();

            if (idProjeto != -1) { returnFatoresProjetos = returnFatoresProjetos.Where(x => x.idProjeto == idProjeto).ToList(); }
            if (idFatorProjeto != -1) { returnFatoresProjetos = returnFatoresProjetos.Where(x => x.idFatorProjeto == idFatorProjeto).ToList(); }

            return returnFatoresProjetos;
        }
        public bool GerirFatorProjeto(FATORESPROJETOS fatorProjeto)
        {
            try
            {
                DataModel context = new DataModel();
                if (fatorProjeto.idFatorProjeto == 0)
                    context.FATORESPROJETOS.Add(fatorProjeto);
                else
                    context.Entry(context.FATORESPROJETOS.FirstOrDefault(x => x.idFatorProjeto == fatorProjeto.idFatorProjeto)).CurrentValues.SetValues(fatorProjeto);

                context.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public List<FatorComplexidadeExportModel> ObterExportFatores()
        {
            var associadosService = new AssociadosService();
            var returnList = new List<FatorComplexidadeExportModel>();

            var fatoresTodos = ObterFatoresProjetos(-1, -1);
            foreach (var fator in fatoresTodos)
            {
                var addFator = new FatorComplexidadeExportModel();
                addFator.Projeto = fator.PROJETOS.Projeto;
                addFator.Gestor = associadosService.ObterAssociado(fator.PROJETOS.IdAssociadoGestor).Nome;
                addFator.Responsavel = associadosService.ObterAssociado(fator.PROJETOS.IdAssociadoResponsavel).Nome;
                addFator.Complexidade = fator.PROJETOS.PROJETOSCOMPLEXIDADES.Complexidade;
                addFator.Fator = fator.FATORES.Titulo;
                addFator.Peso = fator.FATORES.Peso.ToString();
                addFator.Nota = fator.NOTASFATORES.Valor.ToString();
                addFator.DescricaoNota = fator.NOTASFATORES.DescricaoNota;
                addFator.DescricaoFator = addFator.Nota == "1" ? fator.FATORES.DescricaoBaixa : (addFator.Nota == "3" ? fator.FATORES.DescricaoMedia : fator.FATORES.DescricaoAlta);
                addFator.DHCInput = fator.DHC;
                addFator.DHCValidacao = fator.DHCValidGestor ?? fator.DHCValidResponsavel;

                returnList.Add(addFator);
            }

            return returnList;
        }
    }
}

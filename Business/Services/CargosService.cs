using Business.DataAccess;
using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class CargosService
    {
        public bool InserirCargo(CARGOS cargos)
        {
            try
            {
                DataModel context = new DataModel();
                context.CARGOS.Add(cargos);
                context.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool SalvarCargo(CARGOS cargo)
        {
            try
            {
                DataModel context = new DataModel();
                CARGOS cargoatual = new CARGOS();
                context.CARGOS.Add(cargo);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool AlterarCargo(CARGOS cargo)
        {
            try
            {
                DataModel context = new DataModel();
                CARGOS cargoatual = new CARGOS();
                cargoatual = context.CARGOS.First(a => a.IdCargo == cargo.IdCargo);

                if (cargoatual != null)
                {
                    cargoatual.Cargo = cargo.Cargo;
                    cargoatual.ATV = cargo.ATV;
                    cargoatual.idProximoCargo = cargo.idProximoCargo;
                    cargoatual.TempoMinimoPromocao = cargo.TempoMinimoPromocao;
                    cargoatual.Funcao = cargo.Funcao;
                    cargoatual.Autonomia = cargo.Autonomia;
                    cargoatual.EscopoDeAtuacao = cargo.EscopoDeAtuacao;
                    cargoatual.NivelInterlocucao = cargo.NivelInterlocucao;
                }
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool ExcluirCargo(int idCargo)
        {
            try
            {
                DataModel context = new DataModel();
                CARGOS cargoatual = new CARGOS();
                cargoatual = context.CARGOS.First(a => a.IdCargo == idCargo);

                if (cargoatual != null)
                {
                    cargoatual.ATV = 0;
                }
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public List<CARGOS> ObterListaCargos()
        {
            DataModel context = new DataModel();

            List<CARGOS> cargoslist = new List<CARGOS>();
            cargoslist = context.CARGOS.ToList();
            return cargoslist;
        }

        public List<CARGOS> ObterListaCargos(bool ativos)
        {
            DataModel context = new DataModel();
            return context.CARGOS.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.Cargo).ToList();
        }

        public CARGOS ObterCargo(int idCargo)
        {
            DataModel context = new DataModel();
            return context.CARGOS.FirstOrDefault(c => c.IdCargo == idCargo);
        }
        public CARGOS ObterCargoPorNome(string CargoNome)
        {
            DataModel context = new DataModel();
            return context.CARGOS.FirstOrDefault(c => c.Cargo == CargoNome);
        }

        // PROMOÇÕES
        public PROMOCOES ObterUltimaPromocaoAssociado(int idAssociado)
        {
            DataModel context = new DataModel();
            return context.PROMOCOES.OrderByDescending(p => p.idPromocao).FirstOrDefault(p => p.idAssociado == idAssociado && p.ATV == true);
        }

        public List<PROMOCOES> ObterTodasPromocoesAssociado(int idAssociado)
        {
            DataModel context = new DataModel();
            return context.PROMOCOES.Where(p => p.idAssociado == idAssociado && p.ATV == true).ToList();
        }
        public List<PROMOCOES> ObterListaPromocoes()
        {
            DataModel context = new DataModel();
            return context.PROMOCOES.ToList();
        }
        public List<PROMOCOES> ObterPromocao(int idAssociado, int IdCargoAnterior, int IdCargoNovo)
        {
            DataModel context = new DataModel();
            return context.PROMOCOES.Where(p => p.idAssociado == idAssociado && p.idCargoAnterior == IdCargoAnterior && p.idCargoNovo == IdCargoNovo && p.ATV == true).ToList();
        }
        public bool AlterarPromocao(PROMOCOES promocao)
        {
            try
            {
                DataModel context = new DataModel();
                PROMOCOES promocaoAtual = new PROMOCOES();
                promocaoAtual = context.PROMOCOES.First(a => a.idPromocao == promocao.idPromocao);

                if (promocaoAtual != null)
                {
                    promocaoAtual.idAssociado = promocao.idAssociado;
                    promocaoAtual.idCargoAnterior = promocao.idCargoAnterior;
                    promocaoAtual.idCargoNovo = promocao.idCargoNovo;
                    promocaoAtual.DataPromocao = promocao.DataPromocao;
                    promocaoAtual.Comentarios = promocao.Comentarios;
                    promocaoAtual.ATV = promocao.ATV;
                }
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool AlterarPromocaoComentario(int idPromocao, string comentario)
        {
            try
            {
                DataModel context = new DataModel();
                PROMOCOES promocaoAtual = new PROMOCOES();
                promocaoAtual = context.PROMOCOES.First(a => a.idPromocao == idPromocao);

                if (promocaoAtual != null)
                {
                    promocaoAtual.Comentarios = comentario;
                }
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public bool AdicionarPromocao(PROMOCOES promocao)
        {
            try
            {
                DataModel context = new DataModel();
                context.PROMOCOES.Add(promocao);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        // CABEÇALHO DE DESCRIÇÕES DE COMPETENCIAS
        public List<DescricoesCargoModel> ObterDescricoesCompetencias (int idCargo, List<COMPETENCIAS> competencias, ASSOCIADOS associado)
        {
            CARGOS cargoAtual = ObterCargo(idCargo);
            CARGOS cargoProximo = cargoAtual.idProximoCargo != null ? ObterCargo(cargoAtual.CARGOS2.IdCargo) : null;

            List<COMPETENCIAS> listaCompetencias = competencias;// new CompetenciasService().ObterCompetenciasDeCargo(cargoAtual.IdCargo);

            List<DescricoesCargoModel> listaSubcompetencias = new List<DescricoesCargoModel>();
            int contador = 1;
            string nome_Subcompetencia = "";

            foreach (var getCompetencia in listaCompetencias)
            {
                if (associado.Vertical == null || associado.Vertical.ToLower() != "backoffice")
                {
                    nome_Subcompetencia = getCompetencia.SUBCOMPETENCIAS.SubCompetencia;
                }
                else
                {
                    nome_Subcompetencia = getCompetencia.EIXOS.Eixo;
                }
                
                if (listaSubcompetencias.Find(sub => sub.Titulo == nome_Subcompetencia) == null)
                {
                    DescricoesCargoModel addDescricaoCargoModel = new DescricoesCargoModel();
                    addDescricaoCargoModel.Titulo = nome_Subcompetencia;
                    addDescricaoCargoModel.TituloExibicao = contador.ToString() + ". " + nome_Subcompetencia.ToUpper() + " - GERAL";

                    RELACAO_CARGO_SUBCOMPETENCIA relacao = ObterRelacaoCargoSubcompetencia(cargoAtual.IdCargo, getCompetencia.IdSubCompetencia);
                    addDescricaoCargoModel.DescricaoCompetencia_Atual = relacao != null ? relacao.Descricao : "";

                    relacao = ObterRelacaoCargoSubcompetencia(cargoProximo != null ? cargoProximo.IdCargo : -1, getCompetencia.IdSubCompetencia);
                    addDescricaoCargoModel.DescricaoCompetencia_Proximo = relacao != null ? relacao.Descricao : "";

                    listaSubcompetencias.Add(addDescricaoCargoModel);

                    contador += 1;
                }
            }

            return listaSubcompetencias;
        }

        public RELACAO_CARGO_SUBCOMPETENCIA ObterRelacaoCargoSubcompetencia(int idCargo, int idSubcompetencia)
        {
            DataModel context = new DataModel();

            RELACAO_CARGO_SUBCOMPETENCIA relacao = new RELACAO_CARGO_SUBCOMPETENCIA();
            relacao = context.RELACAO_CARGO_SUBCOMPETENCIA.FirstOrDefault(rel => rel.idCargo == idCargo && rel.idSubcompetencia == idSubcompetencia);
            return relacao;
        }

        public bool AtualizarRelacaoCargoSubcompetencia(RELACAO_CARGO_SUBCOMPETENCIA relacaoCargoSubcompetencia)
        {
            try
            {
                DataModel context = new DataModel();
                RELACAO_CARGO_SUBCOMPETENCIA checkRelacaoExiste = context.RELACAO_CARGO_SUBCOMPETENCIA.FirstOrDefault(rel => 
                    rel.idCargo == relacaoCargoSubcompetencia.idCargo && 
                    rel.idSubcompetencia == relacaoCargoSubcompetencia.idSubcompetencia);

                if (checkRelacaoExiste == null)
                {
                    context.RELACAO_CARGO_SUBCOMPETENCIA.Add(relacaoCargoSubcompetencia);
                }
                else
                {
                    checkRelacaoExiste.Descricao = relacaoCargoSubcompetencia.Descricao;
                }
                context.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}

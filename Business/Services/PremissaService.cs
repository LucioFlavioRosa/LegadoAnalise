using Business.DataAccess;
using Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class PremissaService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PremissaService));
        public bool Inserir(PREMISSAS_RADAR premissa)
        {
            try
            {
                var userLogado = WebStorage.GetUsuarioLogado();

                //Padrão na Inclusão
                premissa.ATV = 1;
                premissa.DHC = DateTime.Now;
                premissa.IdEmpresa = userLogado.IdEmpresa;
                premissa.USR = userLogado.Id;

                DataModel context = new DataModel();
                context.PREMISSAS_RADAR.Add(premissa);
                return context.SaveChanges() >= 1;
            }
            catch (Exception ex) 
            { 
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message); 
                return false; 
            }
        }

        public bool Alterar(int idpremissa, PREMISSAS_RADAR premissa)
        {
            bool OK = false;

            try
            {
                using (var db = new DataModel())
                {
                    var result = db.PREMISSAS_RADAR.SingleOrDefault(c => c.IdPremissa == idpremissa);
                    if (result != null)
                    {
                        result.IdEmpresa = premissa.IdEmpresa;
                        result.IdEixo = premissa.IdEixo;                        
                        result.IdCargo = premissa.IdCargo;
                        result.IdNivel = premissa.IdNivel;
                        result.ValorBaseAutoAvaliacao = premissa.ValorBaseAutoAvaliacao;
                        result.ValorBaseAvaliacaoGestor = premissa.ValorBaseAvaliacaoGestor;
                        result.ValorRadarPeers = premissa.ValorRadarPeers;
                        result.USR = premissa.USR;
                        result.ATV = premissa.ATV;
                        result.DHC = premissa.DHC;

                        OK = db.SaveChanges() >= 1;
                    }
                }
            }
            catch (Exception ex) 
            { 
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message); 
                return false; 
            }

            return OK;
        }

        public bool Deletar(int idPremissa)
        {
            try
            {
                using (var db = new DataModel())
                {
                    var result = db.PREMISSAS_RADAR.SingleOrDefault(c => c.IdPremissa == idPremissa);
                    if (result != null)
                    {
                        db.PREMISSAS_RADAR.Remove(result);
                        return db.SaveChanges() >= 1;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }
        }

        public PREMISSAS_RADAR Obter(int idPremissa)
        {
            DataModel context = new DataModel();
            return context.PREMISSAS_RADAR.FirstOrDefault(a => a.IdPremissa == idPremissa);
        }

        public PREMISSAS_RADAR ObterPremissaPorCardo(int idcargo)
        {
            DataModel context = new DataModel();
            return context.PREMISSAS_RADAR.FirstOrDefault(a => a.IdCargo == idcargo);
        }

        public List<PREMISSAS_RADAR> ObterLista()
        {
            var userLogado = WebStorage.GetUsuarioLogado();

            DataModel context = new DataModel();
            return context.PREMISSAS_RADAR.Where(w => w.IdEmpresa == userLogado.IdEmpresa).ToList();
        }

        public List<PREMISSAS_RADAR> ObterLista(int idCargo, int idNivel)
        {
            var userLogado = WebStorage.GetUsuarioLogado();

            DataModel context = new DataModel();
            return context.PREMISSAS_RADAR.Where(a => a.IdEmpresa == userLogado.IdEmpresa &&
                a.IdCargo == idCargo && a.IdNivel == idNivel).ToList();
        }

        public bool InativarPremissa(int idpremissa)
        {
            try
            {
                DataModel context = new DataModel();                
                var premissa = context.PREMISSAS_RADAR.First(a => a.IdPremissa == idpremissa);

                if (premissa != null)
                {
                    if (premissa.ATV == 0)
                    {
                        premissa.ATV = 1;
                    }
                    else
                    {
                        premissa.ATV = 0;
                    }                    
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

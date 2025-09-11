using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class EmpresasService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EmpresasService));
        public bool InserirEmpresa(EMPRESAS empresa)
        {
            try
            {
                DataModel context = new DataModel();
                context.EMPRESAS.Add(empresa);
                context.SaveChanges();
            }
            catch (Exception ex) 
            { 
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message); 
                return false; 
            }

            return true;
        }

        public EMPRESAS ObterEmpresa(EMPRESAS empresa)
        {
            DataModel context = new DataModel();
            return context.EMPRESAS.FirstOrDefault(a => a.IdEmpresa == empresa.IdEmpresa);
        }

        public List<EMPRESAS> ObterListaEmpresas(bool ativo)
        {
            DataModel context = new DataModel();

            List<EMPRESAS> list = new List<EMPRESAS>();
            list = context.EMPRESAS.Where(a => a.ATV == (ativo ? 1 : 0)).OrderBy(a => a.Empresa).ToList();
            return list;
        }
    }
}

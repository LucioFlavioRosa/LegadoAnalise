using Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services
{
    public class ClientesService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ClientesService));
        public bool InserirCliente(CLIENTES clientes)
        {
            try
            {
                DataModel context = new DataModel();
                context.CLIENTES.Add(clientes);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        public bool AlterarCliente(CLIENTES cliente)
        {
            try
            {
                using (var db = new DataModel())
                {
                    var result = db.CLIENTES.SingleOrDefault(c => c.IdCliente == cliente.IdCliente);
                    if (result != null)
                    {
                        result.ASSOCIADOS = cliente.ASSOCIADOS;
                        result.Cliente = cliente.Cliente;
                        result.ATV = cliente.ATV;
                        result.CODIGO=cliente.CODIGO;
                        result.DHC = cliente.DHC;
                        result.Email = cliente.Email;
                        result.EMPRESAS = cliente.EMPRESAS;
                        result.GestorCliente = cliente.GestorCliente;
                        result.IdAssociacoResponsavel = cliente.IdAssociacoResponsavel;
                        result.IdAssociadoGestor = cliente.IdAssociadoGestor;
                        result.IdEmpresa = cliente.IdEmpresa;
                        result.PROJETOS = cliente.PROJETOS;
                        result.Telefones = cliente.Telefones;
                        result.USR = cliente.USR;

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.Message);
                return false;
            }

            return true;
        }

        public CLIENTES ObterCliente(int idCliente)
        {
            DataModel context = new DataModel();
            return context.CLIENTES.FirstOrDefault(a => a.IdCliente == idCliente);
        }

        public List<CLIENTES> ObterListaClientes()
        {
            DataModel context = new DataModel();
            
            List<CLIENTES> clienteslist = new List<CLIENTES>();
            clienteslist = context.CLIENTES.OrderBy(x => x.Cliente).ToList();
            return clienteslist;
        }

        public List<CLIENTES> ObterListaClientes(bool ativos)
        {
            DataModel context = new DataModel();

            List<CLIENTES> clienteslist = new List<CLIENTES>();
            clienteslist = context.CLIENTES.Where(a => a.ATV == (ativos ? 1 : 0)).OrderBy(a => a.Cliente).ToList();
            return clienteslist;
        }


        public List<CLIENTES> ListaClientesAtivos()
        {
            DataModel context = new DataModel();
            return context.CLIENTES.Where(c => c.ATV == 1).ToList();
        }

        public bool ExcluirCliente(int idCliente)
        {
            try
            {
                using (var db = new DataModel())
                {
                    var result = db.CLIENTES.SingleOrDefault(c => c.IdCliente == idCliente);
                    
                    if (result != null)
                    {
                        result.ATV = 0;

                        db.SaveChanges();
                    }
                }
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
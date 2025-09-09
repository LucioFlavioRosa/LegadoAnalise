using System.Collections.Generic;
using System.Web;

namespace Business.Util
{
    public static class WebStorage
    {
        public enum EnumTipoLista
        {
            Competencias,
            Performances,
            Emails,
            Lideranca
        }

        public static void Set(string key, string value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public static void Set(UsuarioLogado value)
        {
            HttpContext.Current.Session["UsuarioLogado"] = value;
        }

        public static void SetList<T>(EnumTipoLista key, List<T> list)
        {
            HttpContext.Current.Session["Avaliacoes_" + key.ToString()] = list;
        }

        public static string Get(string key, string defaultValue)
        {
            var retorno = HttpContext.Current.Session[key];

            if (retorno == null)
            {
                Set(key, defaultValue);
                retorno = defaultValue;
            }

            return retorno.ToString();
        }

        public static List<T> GetList<T>(EnumTipoLista key)
        {
            var retorno = HttpContext.Current.Session["Avaliacoes_" + key.ToString()];

            if (retorno != null)
                return (List<T>)retorno;
            else
                return null;
        }

        public static UsuarioLogado GetUsuarioLogado()
        {
            var retorno = HttpContext.Current.Session["UsuarioLogado"] as UsuarioLogado;

            if (retorno == null)
            {
                retorno = new UsuarioLogado { IsLogged = false };
                Set(retorno);
            }

            return retorno;
        }

        public static void Delete(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }

        public static void Delete(EnumTipoLista key)
        {
            HttpContext.Current.Session.Remove("Avaliacoes_" + key.ToString());
        }
    }
}

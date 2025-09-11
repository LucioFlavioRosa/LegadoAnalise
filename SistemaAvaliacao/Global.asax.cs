using System;
using System.Configuration;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.ApplicationInsights.Extensibility;
using SistemaAvaliacao.Helpers;

namespace SistemaAvaliacao
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Código que é executado na inicialização do aplicativo
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Configurar log4net
            log4net.Config.XmlConfigurator.Configure();

            // Configurar Application Insights
            ConfigureApplicationInsights();

            ConfigureConnectionString();
        }

        private void ConfigureApplicationInsights()
        {
            string environment = GetEnvironment(); // Identifica o ambiente (Produção ou Homologação)
            string instrumentationKey;

            if (environment == "Producao")
            {
                instrumentationKey = System.Configuration.ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY_PRODUCAO"];
            }
            else if (environment == "Homologacao")
            {
                instrumentationKey = System.Configuration.ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY_HOMOLOGACAO"];
            }
            else
            {
                throw new InvalidOperationException("Ambiente desconhecido");
            }

            TelemetryConfiguration.Active.InstrumentationKey = instrumentationKey;
        }

        private string GetEnvironment()
        {
            // Obtém o valor da variável de ambiente "ASPNET_ENVIRONMENT"
            string environment = "HLG";

            //if (string.IsNullOrEmpty(environment))
            //{
                //throw new InvalidOperationException("A variável de ambiente ASPNET_ENVIRONMENT não está configurada.");
            //}

            // Verifica se o ambiente é PRD ou HLG
            if (environment == "PRD")
            {
                return "Producao";
            }
            else if (environment == "HLG")
            {
                return "Homologacao";
            }
            else
            {
                throw new InvalidOperationException("Ambiente desconhecido");
            }
        }
        private void ConfigureConnectionString()
        {
            string database = KeyVaultHelper.GetSecretValueAsync("db-database").GetAwaiter().GetResult();
            string password = KeyVaultHelper.GetSecretValueAsync("db-password").GetAwaiter().GetResult();
            string server = KeyVaultHelper.GetSecretValueAsync("db-server").GetAwaiter().GetResult();
            string username = KeyVaultHelper.GetSecretValueAsync("db-username").GetAwaiter().GetResult();
            bool isIntegratedSecurity = false;

#if DEBUG
            database = "SistemaAvaliacao-PROD";
            server = "PEERS6GV3YY3\\SQLEXPRESS";
            username = "SA";
            isIntegratedSecurity= true; // Para desenvolvimento, use Integrated Security
#endif
            string typeOfAuth = isIntegratedSecurity ? "Integrated Security=True" : $"Password={password}";
            string providerConnectionString = $"data source={server};initial catalog={database};User ID={username};{typeOfAuth};Encrypt=False;MultipleActiveResultSets=True;App=EntityFramework";

            // Monta a connection string completa do Entity Framework
            string entityConnectionString =
                "metadata=res://*/DataAccess.DataModelEntityFramework.csdl|res://*/DataAccess.DataModelEntityFramework.ssdl|res://*/DataAccess.DataModelEntityFramework.msl;" +
                "provider=System.Data.SqlClient;" +
                $"provider connection string=\"{providerConnectionString}\"";

            // Sobrescreve a connection string do Entity Framework em tempo de execução
            var settings = ConfigurationManager.ConnectionStrings["DataModel"];
            if (settings != null)
            {
                var fi = typeof(ConfigurationElement).GetField("_bReadOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                fi.SetValue(settings, false);
                settings.ConnectionString = entityConnectionString;
            }
        }
    }
}
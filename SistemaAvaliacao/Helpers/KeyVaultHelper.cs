using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace SistemaAvaliacao.Helpers
{
	public static class KeyVaultHelper
	{
        private static readonly string keyVaultName = "azu-kv-sisaval-prd";
        private static readonly string kvUri = $"https://{keyVaultName}.vault.azure.net/";
        private static readonly string managedIdentityClientId = "b1ac368a-339c-4d81-b2ac-dad5b48512ce";


        // Este método retorna o valor do segredo
        public static async Task<string> GetSecretValueAsync(string secretName)
        {
            AzureServiceTokenProvider azureServiceTokenProvider;
            azureServiceTokenProvider = new AzureServiceTokenProvider();

            #if DEBUG
            // Local development
            azureServiceTokenProvider = new AzureServiceTokenProvider("RunAs=Developer;DeveloperTool=AzureCli");
            #else
                // Production (VM Azure)
                azureServiceTokenProvider = new AzureServiceTokenProvider($"RunAs=App;AppId={managedIdentityClientId}");
            #endif

            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret = await keyVaultClient.GetSecretAsync($"{kvUri}secrets/{secretName}").ConfigureAwait(false);
            return secret.Value;
        }
    }
}
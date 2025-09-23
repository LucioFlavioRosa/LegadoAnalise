# Documentação: Sistema de Autenticação AuthCallback

## Visão Geral

Este documento descreve a implementação do sistema de callback de autenticação OAuth2/OpenID Connect migrado do ASP.NET Web Forms para Blazor Híbrido .NET 9. O sistema processa callbacks do Azure AD e autentica usuários no sistema interno.

## Arquitetura

### Componentes Principais

1. **AuthCallbackService** (`Services/Common/Auth/AuthCallbackService.cs`)
   - Serviço central que processa todo o fluxo de callback
   - Valida estado de autenticação (CSRF protection)
   - Troca código de autorização por tokens
   - Integra com serviço de associados para validação de usuário

2. **TokenDecoder** (`Services/Common/Auth/TokenDecoder.cs`)
   - Decodifica tokens JWT ID do Azure AD
   - Extrai informações do usuário (email, nome, ID)
   - Reutilizável para outros fluxos de autenticação

3. **AuthCallback Component** (`Pages/AuthCallback.razor` + `.cs`)
   - Componente Blazor híbrido que substitui a página Web Forms
   - Processa parâmetros da URL de callback
   - Configura autenticação ASP.NET Core (Claims + Cookies)
   - Mantém compatibilidade com sessão legada

### Fluxo de Autenticação

mermaid
flowchart TD
    Start([Usuário inicia login via Azure AD])
    AzureAD[Azure AD redireciona para /auth-callback?code=...]
    Callback[AuthCallback.razor chama AuthCallbackService]
    ValidateState[Valida state para proteção CSRF]
    Token[AuthCallbackService troca code por token]
    Decode[TokenDecoder decodifica ID Token]
    BuscaUsuario[Busca usuário no banco via AssociadosService]
    ValidaUsuario{Usuário existe e ativo?}
    ConfigAuth[Configura Claims e Cookie de autenticação]
    ConfigSessao[Configura sessão para compatibilidade legada]
    Redirect[Redireciona para página principal]
    Error[Exibe erro de autenticação]

    Start --> AzureAD
    AzureAD --> Callback
    Callback --> ValidateState
    ValidateState --> Token
    Token --> Decode
    Decode --> BuscaUsuario
    BuscaUsuario --> ValidaUsuario
    ValidaUsuario -->|Sim| ConfigAuth
    ValidaUsuario -->|Não| Error
    ConfigAuth --> ConfigSessao
    ConfigSessao --> Redirect


## Configuração

### appsettings.json


{
  "Authentication": {
    "AzureAd": {
      "TenantId": "your-tenant-id",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "ADInstance": "https://login.microsoftonline.com/",
      "RedirectUri": "https://localhost:7000/auth-callback",
      "Scopes": "openid profile email"
    }
  }
}


### Program.cs - Registro de Serviços

csharp
// Auth Common Services
builder.Services.AddScoped<ITokenDecoder, TokenDecoder>();
builder.Services.AddScoped<IAuthCallbackService, AuthCallbackService>();

// Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(...)
.AddOpenIdConnect(...);


## Integração

### Dependências

- **IAssociadosService**: Para validação e busca de usuários no sistema
- **ITokenDecoder**: Para decodificação de tokens JWT
- **HttpClient**: Para comunicação com endpoints OAuth2
- **IConfiguration**: Para acesso às configurações do Azure AD

### Pontos de Extensão

1. **Outros Provedores de Autenticação**: O `ITokenDecoder` pode ser estendido para suportar outros formatos de token
2. **Validações Customizadas**: O `AuthCallbackService` pode ser estendido com validações adicionais
3. **Claims Customizados**: A configuração de Claims no code-behind pode ser personalizada

## Segurança

### Medidas Implementadas

1. **Validação de State**: Proteção contra ataques CSRF
2. **Validação de Token**: Verificação da estrutura e conteúdo dos tokens JWT
3. **Validação de Usuário**: Verificação se o usuário existe e está ativo no sistema
4. **Configuração Segura**: Segredos mantidos em configuração externa (User Secrets/Azure Key Vault)

### Recomendações

- Usar HTTPS em produção
- Configurar segredos via Azure Key Vault
- Implementar logging de eventos de autenticação
- Configurar timeouts apropriados para tokens

## Compatibilidade

### Sessão Legada

Para manter compatibilidade com código legado, o sistema configura as seguintes variáveis de sessão:

- `EMAIL`: Email do usuário
- `IDUSUARIO`: ID do associado
- `IDEMPRESA`: ID da empresa
- `IDPERFIL`: ID do perfil
- `IDCARGO`: ID do cargo
- `NOME`: Nome do usuário
- `ISAZUREAD`: Flag indicando autenticação via Azure AD

### Migração Gradual

O sistema permite migração gradual do Web Forms para Blazor, mantendo funcionalidades existentes enquanto novos recursos são desenvolvidos em Blazor.

## Troubleshooting

### Erros Comuns

1. **"Estado inválido"**: Verificar se a sessão está configurada corretamente
2. **"Usuário não encontrado"**: Verificar se o email do Azure AD corresponde ao cadastrado no sistema
3. **"Erro ao obter token"**: Verificar configurações do Azure AD (ClientId, ClientSecret, RedirectUri)

### Logs

Todos os erros são logados via `ILogger` e podem ser monitorados via Application Insights ou outros provedores de logging configurados.
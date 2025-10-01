# Fluxo de Autenticação Azure AD - Blazor (.NET 9)

## Funcionamento e Integração

Este documento detalha o funcionamento do fluxo de autenticação via Azure AD utilizando Blazor (.NET 9), descrevendo as principais etapas, integração entre serviços e componentes, e os arquivos envolvidos.

### Estrutura de Arquivos
- `Models/UsuarioLogado.cs`: Modelo do usuário autenticado, utilizado para persistir informações relevantes na sessão via claims.
- `Services/Common/UserContextService.cs`: Serviço responsável por obter os dados do usuário logado a partir dos claims do `ClaimsPrincipal`.
- `Services/Auth/AuthCallbackService.cs`: Serviço que processa o callback de autenticação, valida o token e extrai informações do usuário (implementado em etapas anteriores).
- `Pages/AuthCallback.razor`: Componente Blazor responsável por receber o callback do Azure AD e iniciar o processamento da autenticação (implementado em etapas anteriores).

### Integração
- O usuário inicia o login, é redirecionado para o Azure AD, realiza a autenticação e retorna para a aplicação via `/authentication/login-callback`.
- O componente `AuthCallback.razor` processa os parâmetros da query string e chama o serviço `AuthCallbackService` para validar o fluxo OAuth2/PKCE.
- Após validação e obtenção dos dados do usuário, o serviço `UserSessionService` cria o cookie de autenticação, persistindo os dados relevantes como claims.
- O serviço `UserContextService` é utilizado em toda a aplicação para recuperar os dados do usuário logado a partir dos claims.

## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    A[Usuário acessa /Login] --> B{Clica em 'Entrar com Azure AD'}
    B --> C[Login.razor: Gera codeVerifier, codeChallenge, state]
    C --> D[Login.razor: Armazena codeVerifier e state no Cache]
    D --> E[Login.razor: Redireciona para Azure AD com code_challenge]
    E --> F[Azure AD: Usuário faz login]
    F --> G[Azure AD: Redireciona para /authentication/login-callback com code e state]
    G --> H[AuthCallback.razor: OnInitializedAsync]
    H --> I[AuthCallback.razor: Obtém code, state da query string]
    I --> J[AuthCallback.razor: Obtém codeVerifier e sessionState do Cache]
    J --> K[AuthCallback.razor: Chama AuthCallbackService.ProcessCallbackAsync]
    K --> L[AuthCallbackService: Valida state]
    L --> M{State válido?}
    M -->|Não| N[Retorna AuthResult com erro]
    M -->|Sim| O[AuthCallbackService: Troca code por token no Azure AD]
    O --> P[AuthCallbackService: Valida id_token]
    P --> Q{Token válido?}
    Q -->|Não| N
    Q -->|Sim| R[AuthCallbackService: Extrai UserInfo do token]
    R --> S[AuthCallbackService: Retorna AuthResult com sucesso]
    S --> T[AuthCallback.razor: Obtém ASSOCIADOS do banco de dados]
    T --> U{Usuário encontrado?}
    U -->|Não| V[Redireciona para /Login com erro]
    U -->|Sim| W[AuthCallback.razor: Chama UserSessionService.CreateUserSessionAsync]
    W --> X[UserSessionService: Cria claims do usuário]
    X --> Y[UserSessionService: Cria cookie de autenticação]
    Y --> Z[AuthCallback.razor: Redireciona para /Index]
    N --> V


## Sugestões de Melhorias

1. Implementar refresh token para manter a sessão do usuário ativa por mais tempo.
2. Adicionar endpoint de logout que encerre a sessão local e no Azure AD.
3. Melhorar tratamento de erros com mensagens específicas e página customizada.
4. Integrar telemetria e logging detalhado para monitoramento do fluxo.
5. Adotar rate limiting no endpoint de callback para aumentar a segurança.
6. Utilizar IDataProtectionProvider para proteger state e codeVerifier.
7. Suportar múltiplos tenants do Azure AD se necessário.
8. Adicionar suporte a claims customizados (grupos, roles) para autorização avançada.
9. Implementar cache de metadados do Azure AD para validação eficiente dos tokens.
10. Considerar uso de Microsoft.Identity.Web.UI para simplificar componentes de login/logout.
11. Migrar para AuthenticationStateProvider do Blazor para integração nativa do estado de autenticação nos componentes.
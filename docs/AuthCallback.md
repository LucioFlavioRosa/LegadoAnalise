# Documentação do Fluxo de Autenticação OAuth 2.0 com Azure AD

## Funcionamento e Integração

O fluxo de autenticação implementado utiliza OAuth 2.0 com PKCE para autenticar usuários via Azure Active Directory. O processo é realizado de forma moderna, desacoplada e segura, utilizando componentes Blazor, serviços injetáveis e cache distribuído. A configuração sensível é gerenciada via Azure Key Vault.

### Etapas do Processo

1. **Login.razor**: O usuário clica em "Entrar com Azure AD". O sistema gera um `codeVerifier`, `codeChallenge` e um `state` aleatório. Estes dados são armazenados no cache distribuído. O usuário é redirecionado para o endpoint de autorização do Azure AD.
2. **Azure AD**: O usuário realiza o login. Após autenticação, o Azure AD redireciona para `/authentication/login-callback` com os parâmetros `code` e `state`.
3. **AuthCallback.razor**: O componente lê os parâmetros da query string, recupera o `codeVerifier` e `sessionState` do cache, e chama o serviço `AuthCallbackService` para processar o callback.
4. **AuthCallbackService**: Valida o `state`, troca o `code` por um token de acesso e ID token, valida o token e extrai as informações do usuário.
5. **AssociadosService**: Busca o usuário no banco de dados pelo email extraído do token.
6. **UserSessionService**: Cria a sessão do usuário, gerando claims e cookie de autenticação seguro.
7. **Redirecionamento**: O usuário é redirecionado para a página principal `/Index`.

### Serviços Criados/Modificados
- **AuthCallbackService**: Processa o callback OAuth, valida state, troca código por token, valida e extrai claims do token.
- **UserSessionService**: Gerencia a sessão do usuário, armazenamento de codeVerifier/state, criação de claims e cookie.
- **PkceHelper**: Gera codeVerifier e codeChallenge PKCE.
- **AssociadosService**: Busca usuário no banco de dados.

### Configuração
- Configurações do Azure AD em `appsettings.json`.
- Secrets sensíveis (ClientSecret) no Azure Key Vault.
- Cache distribuído para armazenamento temporário de dados sensíveis.

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

1. Implementar refresh token para manter o usuário autenticado por mais tempo.
2. Adicionar endpoint de logout completo, incluindo logout no Azure AD.
3. Melhorar tratamento de erros e mensagens ao usuário.
4. Integrar telemetria e logging detalhado.
5. Implementar rate limiting no endpoint de callback.
6. Usar IDataProtectionProvider para criptografar state/codeVerifier no cache.
7. Suporte a múltiplos tenants do Azure AD.
8. Implementar testes de integração do fluxo completo.
9. Mapear claims customizados do Azure AD para autorização.
10. Cache de metadados do Azure AD para validação de token.
11. Considerar uso de Microsoft.Identity.Web.UI para componentes prontos.
12. Migrar para AuthenticationStateProvider do Blazor para melhor integração com componentes.
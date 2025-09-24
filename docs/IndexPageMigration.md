# Migração da Página Inicial (Index.aspx) para Blazor Híbrido

## Visão Geral

Este documento descreve a migração completa da página inicial do sistema de avaliação interna da empresa, de ASP.NET Web Forms (Index.aspx) para Blazor Híbrido com .NET 9. A migração foi realizada seguindo os princípios de arquitetura limpa, reutilização de código e componentização.

## Arquitetura da Solução

### Serviços Criados

#### 1. UserContextService
**Localização:** `Services/Common/UserContextService.cs`

**Responsabilidades:**
- Gerenciar o contexto do usuário logado
- Abstrair acesso a sessão/cookies
- Validar sessão do usuário
- Registrar telemetria de login/logout

**Métodos Principais:**
- `GetUsuarioLogadoAsync()`: Obtém o usuário da sessão
- `SetUsuarioLogadoAsync(Associado usuario)`: Define o usuário na sessão
- `IsAuthenticated()`: Verifica se há usuário autenticado
- `ValidateUserSessionAsync()`: Valida se a sessão ainda é válida

#### 2. MenuService
**Localização:** `Services/Common/MenuService.cs`

**Responsabilidades:**
- Fornecer estrutura de menus baseada no perfil do usuário
- Gerenciar permissões de acesso aos menus
- Organizar itens por categoria (Associados, Projetos, Avaliações, Mentoria)

**Métodos Principais:**
- `GetMenuItemsAsync(userId, userProfileId)`: Obtém itens do menu administrativo
- `GetQuickAccessItemsAsync(userId, userProfileId)`: Obtém itens de acesso rápido
- `GetEvaluationMenuItemsAsync(userId, userProfileId)`: Obtém itens de avaliação
- `GetMentorshipMenuItemsAsync(userId)`: Obtém itens de mentoria

#### 3. PasswordService
**Localização:** `Services/Common/PasswordService.cs`

**Responsabilidades:**
- Validar senhas (força, confirmação, etc.)
- Alterar senhas de usuários
- Verificar se senha é padrão
- Notificar alterações de senha via telemetria

**Métodos Principais:**
- `ValidatePasswordAsync(password, confirmPassword)`: Valida nova senha
- `ChangePasswordAsync(userId, newPassword)`: Altera senha do usuário
- `RequiresPasswordChangeAsync(userId)`: Verifica se usuário precisa alterar senha

#### 4. VisibilityHelper
**Localização:** `Services/Common/VisibilityHelper.cs`

**Responsabilidades:**
- Centralizar regras de visibilidade de elementos da UI
- Definir permissões baseadas em perfil de usuário
- Fornecer métodos estáticos para verificação de visibilidade

### Componentes Blazor Criados

#### 1. Index.razor
**Localização:** `Components/Pages/Index.razor`

**Características:**
- Renderização híbrida (`@rendermode InteractiveAuto`)
- Carregamento assíncrono de dados
- Gerenciamento de estado de loading
- Integração com todos os serviços criados

#### 2. Menu.razor
**Localização:** `Components/Shared/Menu.razor`

**Características:**
- Componente reutilizável para menus administrativos
- Organização por categorias
- Filtragem baseada em visibilidade

#### 3. QuickAccess.razor
**Localização:** `Components/Shared/QuickAccess.razor`

**Características:**
- Componente para acesso rápido
- Parametrizado por perfil de usuário
- Visibilidade dinâmica baseada em permissões

#### 4. EvaluationMenu.razor
**Localização:** `Components/Shared/EvaluationMenu.razor`

**Características:**
- Menu específico para avaliações
- Separação entre itens básicos e avançados
- Controle de visibilidade por perfil

#### 5. MentorshipMenu.razor
**Localização:** `Components/Shared/MentorshipMenu.razor`

**Características:**
- Menu de mentoria
- Separação entre seções de mentor e mentorado
- Visibilidade baseada na existência de mentorados

#### 6. ChangePasswordPanel.razor
**Localização:** `Components/Shared/ChangePasswordPanel.razor`

**Características:**
- Painel para alteração de senha obrigatória
- Validação em tempo real
- Feedback visual de processamento
- Integração com PasswordService

## Fluxo de Funcionamento

mermaid
flowchart TD
    Start([Usuário acessa Página Inicial])
    Start --> CheckAuth{Usuário autenticado?}
    CheckAuth -- Não --> RedirectLogin[Redireciona para Login]
    CheckAuth -- Sim --> LoadUser[UserContextService: Carrega dados do usuário]
    LoadUser --> CheckPassword{Senha padrão?}
    CheckPassword -- Sim --> ShowChangePassword[Exibe ChangePasswordPanel]
    ShowChangePassword --> PasswordService[PasswordService: Valida e altera senha]
    PasswordService --> Success{Sucesso?}
    Success -- Não --> ShowError[Exibe erro via MessageBoxService]
    Success -- Sim --> LoadMenus[Carrega menus]
    CheckPassword -- Não --> LoadMenus
    LoadMenus --> MenuService[MenuService: Obtém menus por perfil]
    MenuService --> CheckProfile{Perfil > 2?}
    CheckProfile -- Sim --> ShowAdminMenu[Renderiza Menu administrativo]
    CheckProfile -- Não --> SkipAdminMenu[Pula menu administrativo]
    ShowAdminMenu --> ShowQuickAccess
    SkipAdminMenu --> ShowQuickAccess[Renderiza QuickAccess]
    ShowQuickAccess --> ShowEvaluations[Renderiza EvaluationMenu]
    ShowEvaluations --> CheckMentorados{Tem mentorados?}
    CheckMentorados -- Sim --> ShowMentorship[Renderiza MentorshipMenu completo]
    CheckMentorados -- Não --> ShowBasicMentorship[Renderiza MentorshipMenu básico]
    ShowMentorship --> TelemetryLog[TelemetryService: Registra acesso]
    ShowBasicMentorship --> TelemetryLog
    TelemetryLog --> End([Página carregada])


## Integração dos Serviços

### Injeção de Dependência
Todos os serviços são registrados no `Program.cs` como `Scoped`, garantindo que uma instância seja criada por requisição HTTP.

csharp
// New Common Services - Index Page Migration
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();


### Comunicação entre Componentes
- **Parent → Child**: Parâmetros (`[Parameter]`)
- **Child → Parent**: EventCallback (`OnPasswordChanged`)
- **Estado Global**: Services injetados

### Tratamento de Erros
- Todos os serviços implementam try-catch com logging via TelemetryService
- Mensagens de erro são exibidas via MessageBoxService
- Fallbacks graceful em caso de falha

## Benefícios da Migração

### Performance
- **Renderização Híbrida**: Combina Server-Side Rendering (SSR) com interatividade client-side
- **Carregamento Assíncrono**: Dados são carregados em paralelo quando possível
- **Componentização**: Reutilização de código reduz tamanho do bundle

### Manutenibilidade
- **Separação de Responsabilidades**: Lógica de negócio separada da apresentação
- **Serviços Reutilizáveis**: Código pode ser usado em outras páginas
- **Testabilidade**: Serviços podem ser testados unitariamente

### Experiência do Usuário
- **Feedback Visual**: Loading states e mensagens de erro claras
- **Responsividade**: Interface moderna e responsiva
- **Navegação Fluida**: Transições suaves entre estados

## Configurações Necessárias

### appsettings.json
As configurações existentes são suficientes. Os novos serviços utilizam:
- Connection strings para acesso ao banco
- Configurações de autenticação Azure AD
- Configurações de Application Insights para telemetria

### Dependências
Nenhuma nova dependência foi adicionada. A migração utiliza apenas:
- Microsoft.EntityFrameworkCore (já existente)
- Microsoft.ApplicationInsights (já existente)
- Blazor Server (já configurado)

## Próximos Passos

1. **Testes de Integração**: Validar funcionamento em ambiente de desenvolvimento
2. **Migração Incremental**: Aplicar padrão similar a outras páginas
3. **Otimizações**: Implementar cache quando necessário
4. **Monitoramento**: Acompanhar métricas de performance via Application Insights

## Considerações de Segurança

- **Validação de Sessão**: UserContextService valida sessões automaticamente
- **Controle de Acesso**: VisibilityHelper centraliza regras de permissão
- **Sanitização**: Inputs são validados antes do processamento
- **Telemetria**: Ações sensíveis são logadas para auditoria

## Troubleshooting

### Problemas Comuns

1. **Usuário não carrega**: Verificar se sessão está configurada corretamente
2. **Menus não aparecem**: Validar perfil do usuário e permissões
3. **Senha não altera**: Verificar conexão com banco de dados
4. **Componentes não renderizam**: Verificar se serviços estão registrados no DI

### Logs Importantes
- `UserContextService`: Login/logout de usuários
- `PasswordService`: Alterações de senha
- `MenuService`: Carregamento de menus
- Application Insights: Métricas de performance e erros
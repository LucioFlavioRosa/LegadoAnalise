# Migração da Página Inicial (Index.aspx) para Blazor Híbrido .NET 9

## Visão Geral

Este documento descreve a migração completa da página inicial do Sistema de Avaliação Interna da empresa, de ASP.NET Web Forms (Index.aspx) para Blazor Server com renderização híbrida no .NET 9.

## Arquitetura da Solução

### Serviços Criados

#### 1. UserContextService
- **Localização**: `Services/Common/UserContextService.cs`
- **Responsabilidade**: Gerenciar o contexto do usuário logado
- **Métodos principais**:
  - `GetUsuarioLogado()`: Obtém o usuário da sessão
  - `SetUsuarioLogado(Associado user)`: Define o usuário na sessão
  - `IsAuthenticated()`: Verifica se há usuário autenticado
  - `ClearUsuarioLogado()`: Remove usuário da sessão

#### 2. MenuService
- **Localização**: `Services/Common/MenuService.cs`
- **Responsabilidade**: Fornecer estrutura de menus baseada no perfil do usuário
- **Métodos principais**:
  - `GetMenuItemsAsync(int userId)`: Retorna menus administrativos
  - `GetQuickAccessItemsAsync(int userId)`: Retorna itens de acesso rápido
  - `GetEvaluationItemsAsync(int userId)`: Retorna itens de avaliação
  - `GetMentorshipItemsAsync(int userId)`: Retorna itens de mentoria

#### 3. PasswordService
- **Localização**: `Services/Common/PasswordService.cs`
- **Responsabilidade**: Gerenciar alteração e validação de senhas
- **Métodos principais**:
  - `ValidatePasswordAsync()`: Valida nova senha e confirmação
  - `ChangePasswordAsync()`: Altera senha do usuário
  - `NotifyPasswordChangeAsync()`: Registra telemetria da alteração
  - `IsDefaultPassword()`: Verifica se é senha padrão

#### 4. VisibilityHelper
- **Localização**: `Services/Common/VisibilityHelper.cs`
- **Responsabilidade**: Centralizar regras de visibilidade da UI
- **Métodos principais**:
  - `ShouldShowAdminMenu()`: Verifica se deve exibir menu admin
  - `ShouldShowPasswordChangePanel()`: Verifica se deve exibir painel de senha
  - `ShouldShowEvaluationActions()`: Verifica permissões de avaliação

### Componentes Blazor Criados

#### 1. Index.razor
- **Localização**: `Components/Pages/Index.razor`
- **Renderização**: InteractiveAuto (Híbrida)
- **Responsabilidade**: Página principal do sistema

#### 2. Menu.razor
- **Localização**: `Components/Shared/Menu.razor`
- **Responsabilidade**: Renderizar menus administrativos
- **Reutilizável**: Sim

#### 3. QuickAccess.razor
- **Localização**: `Components/Shared/QuickAccess.razor`
- **Responsabilidade**: Renderizar acesso rápido
- **Reutilizável**: Sim

#### 4. ChangePasswordPanel.razor
- **Localização**: `Components/Shared/ChangePasswordPanel.razor`
- **Responsabilidade**: Painel de alteração de senha
- **Reutilizável**: Sim

#### 5. EvaluationsPanel.razor
- **Localização**: `Components/Shared/EvaluationsPanel.razor`
- **Responsabilidade**: Painel de avaliações
- **Reutilizável**: Sim

#### 6. MentorshipPanel.razor
- **Localização**: `Components/Shared/MentorshipPanel.razor`
- **Responsabilidade**: Painel de mentoria
- **Reutilizável**: Sim

#### 7. MessageBoxHandler.razor
- **Localização**: `Components/Shared/MessageBoxHandler.razor`
- **Responsabilidade**: Exibir mensagens do sistema
- **Reutilizável**: Sim

## Integração dos Serviços

### Injeção de Dependência
Todos os serviços são registrados em `Program.cs` como `Scoped`, garantindo uma instância por requisição HTTP.

### Fluxo de Dados
1. **UserContextService** fornece dados do usuário logado
2. **MenuService** usa dados do usuário para determinar menus visíveis
3. **PasswordService** gerencia alterações de senha
4. **VisibilityHelper** determina quais elementos exibir

### Telemetria e Mensagens
- **TelemetryService**: Registra eventos de acesso e alterações
- **MessageBoxService**: Exibe mensagens de sucesso/erro

## Fluxo de Alto Nível

mermaid
flowchart TD
    Start([Usuário acessa Página Inicial])
    Start --> CheckAuth{Usuário autenticado?}
    CheckAuth -- Não --> RedirectLogin[Redireciona para Login]
    CheckAuth -- Sim --> LoadUser[UserContextService.GetUsuarioLogado]
    LoadUser --> CheckPassword{Senha padrão?}
    CheckPassword -- Sim --> ShowChangePassword[Exibe ChangePasswordPanel]
    ShowChangePassword --> ValidatePassword[PasswordService.ValidatePassword]
    ValidatePassword --> ChangePassword[PasswordService.ChangePassword]
    ChangePassword --> NotifyChange[PasswordService.NotifyPasswordChange]
    NotifyChange --> UpdateSession[UserContextService.SetUsuarioLogado]
    UpdateSession --> ReloadPage[Recarrega página]
    CheckPassword -- Não --> LoadMenus[MenuService.GetMenuItems]
    LoadMenus --> LoadQuickAccess[MenuService.GetQuickAccessItems]
    LoadQuickAccess --> LoadEvaluations[MenuService.GetEvaluationItems]
    LoadEvaluations --> LoadMentorship[MenuService.GetMentorshipItems]
    LoadMentorship --> CheckVisibility[VisibilityHelper verifica permissões]
    CheckVisibility --> RenderComponents[Renderiza componentes]
    RenderComponents --> TrackTelemetry[TelemetryService.TrackPageView]
    TrackTelemetry --> End([Página carregada])


## Benefícios da Migração

### Performance
- **Renderização Híbrida**: Combina Server-Side Rendering (SSR) para carregamento inicial rápido com interatividade client-side
- **Componentes Reutilizáveis**: Reduz duplicação de código
- **Lazy Loading**: Componentes carregados sob demanda

### Manutenibilidade
- **Separação de Responsabilidades**: Lógica de negócio em serviços, UI em componentes
- **Injeção de Dependência**: Facilita testes e manutenção
- **Código Limpo**: Eliminação do code-behind complexo

### Experiência do Usuário
- **Interatividade Melhorada**: Atualizações em tempo real sem postbacks
- **Mensagens Contextuais**: Sistema de notificações integrado
- **Navegação Fluida**: SPA-like experience

### Escalabilidade
- **Arquitetura Modular**: Fácil adição de novos recursos
- **Serviços Reutilizáveis**: Aproveitamento em outras páginas
- **Configuração Centralizada**: appsettings.json

## Próximos Passos

1. **Testes de Integração**: Validar funcionamento com dados reais
2. **Migração de Outras Páginas**: Aplicar padrões similares
3. **Otimizações**: Implementar cache e otimizações específicas
4. **Monitoramento**: Configurar alertas e métricas de performance

## Considerações de Segurança

- **Validação de Entrada**: Todas as entradas são validadas
- **Autorização**: Verificação de permissões em cada ação
- **Sessão Segura**: Configuração adequada de cookies e sessão
- **Telemetria**: Não exposição de dados sensíveis nos logs

## Configurações Necessárias

Verificar se as seguintes configurações estão presentes em `appsettings.json`:
- ConnectionStrings
- Authentication (Azure AD)
- ApplicationInsights
- Session timeout

## Troubleshooting

### Problemas Comuns
1. **Usuário não encontrado**: Verificar se o email está correto na base
2. **Sessão expirada**: Verificar configuração de timeout
3. **Permissões**: Verificar IdPerfil do usuário
4. **Componentes não carregam**: Verificar imports em _Imports.razor

### Logs Importantes
- Application Insights: Eventos de telemetria
- Console: Erros de renderização
- Network: Chamadas SignalR (Blazor Server)
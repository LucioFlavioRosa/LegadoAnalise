# Autoavaliação de Performance - Migração Web Forms para Blazor

## Visão Geral

Este documento descreve a migração da página de autoavaliação de performance (`autoavalizacao_performance.aspx`) do Web Forms para Blazor Híbrido (.NET 9). A migração mantém toda a funcionalidade original enquanto moderniza a arquitetura para um padrão mais escalável e testável.

## Arquitetura da Solução

### Componentes Principais

1. **AutoAvaliacaoPerformance.razor** - Componente principal da página
2. **PerformanceTable.razor** - Componente da tabela de performances
3. **MessageBox.razor** - Componente de notificações
4. **AutoAvaliacaoPerformanceService** - Serviço de lógica de negócio
5. **Helpers Reutilizáveis** - NotaHelper, AccordionHelper, ValidationHelper

### Serviços Utilizados

- **IAutoAvaliacaoPerformanceService** - Lógica principal de autoavaliação
- **IUserContextService** - Contexto do usuário logado
- **IMessageBoxService** - Exibição de mensagens
- **ITelemetryService** - Telemetria e monitoramento
- **INotaHelper** - Manipulação de notas e validações
- **IAccordionHelper** - Lógica de expand/collapse
- **IValidationHelper** - Validações de formulário

## Funcionalidades Implementadas

### 1. Carregamento de Dados
- Validação de parâmetros obrigatórios (Projeto, Associado, Período)
- Carregamento de informações do avaliado
- Carregamento de performances parametrizadas
- Organização por abrangência (Individual/Coletivo)

### 2. Interface de Avaliação
- Tabela responsiva com performances
- Accordions para descrições detalhadas
- Selects para notas com validação
- Textarea para considerações
- Botões de navegação e salvamento

### 3. Validações
- Validação de preenchimento obrigatório
- Controle de inputs baseado em configuração
- Validação de permissões por etapa do fluxo
- Validação de finalização

### 4. Salvamento e Fluxo
- Auto-save configurável (15 minutos)
- Salvamento manual
- Controle de etapas do fluxo de avaliação
- Finalização com validações

### 5. Navegação
- Navegação entre Competência e Performance
- Redirecionamento para finalização
- Controle de visibilidade de botões

## Configurações

### appsettings.json


{
  "AutoAvaliacao": {
    "Performance": {
      "AutoSaveIntervalMs": 900000,
      "ShowAutoSaveMessage": true,
      "EnablePerformanceValidation": true,
      "RequireAllNotasPreenchidas": true,
      "MaxObservacaoLength": 2000
    },
    "ComponenteConfig": {
      "PerformanceTable": {
        "EnableAccordions": true,
        "TruncateTextLength": 45,
        "ShowExpandButton": true,
        "ExpandButtonText": "Ver+"
      }
    }
  }
}


## Integração entre Componentes

### Fluxo de Dados
1. **Componente Principal** injeta serviços necessários
2. **AutoAvaliacaoPerformanceService** gerencia lógica de negócio
3. **Helpers** fornecem funcionalidades reutilizáveis
4. **MessageBox** exibe notificações via serviço
5. **UserContext** fornece informações do usuário

### Comunicação entre Componentes
- **Parent → Child**: Parâmetros e callbacks
- **Service → Component**: Injeção de dependência
- **Component → Service**: Chamadas de métodos
- **Event-driven**: MessageBoxService para notificações

## Benefícios da Migração

### 1. Arquitetura Moderna
- Separação clara de responsabilidades
- Injeção de dependência nativa
- Testabilidade aprimorada
- Reutilização de código

### 2. Performance
- Renderização otimizada do Blazor
- Auto-save inteligente
- Carregamento assíncrono
- Validação client-side

### 3. Manutenibilidade
- Código organizado em serviços
- Configuração centralizada
- Helpers reutilizáveis
- Documentação estruturada

### 4. Experiência do Usuário
- Interface responsiva
- Feedback visual aprimorado
- Validações em tempo real
- Navegação fluida

## Fluxo de Alto Nível

mermaid
flowchart TD
    A[Usuário acessa AutoAvaliação Performance] --> B[Validar Parâmetros]
    B --> C{Parâmetros Válidos?}
    C -->|Não| D[Exibir Erro]
    C -->|Sim| E[Carregar Dados do Avaliado]
    E --> F[Carregar Performances]
    F --> G[Organizar por Abrangência]
    G --> H[Renderizar Tabela]
    H --> I[Usuário preenche avaliação]
    I --> J{Auto-save ativo?}
    J -->|Sim| K[Salvar automaticamente]
    J -->|Não| L[Aguardar ação manual]
    K --> M[Continuar preenchimento]
    L --> M
    M --> N{Usuário clica Salvar?}
    N -->|Sim| O[Validar dados]
    N -->|Não| P{Usuário clica Finalizar?}
    O --> Q{Dados válidos?}
    Q -->|Não| R[Exibir erros]
    Q -->|Sim| S[Salvar no banco]
    S --> T[Exibir sucesso]
    P -->|Sim| U[Validar completude]
    U --> V{Tudo preenchido?}
    V -->|Não| W[Exibir aviso]
    V -->|Sim| X[Finalizar avaliação]
    X --> Y[Avançar próxima etapa]
    P -->|Não| Z{Navegar para Competência?}
    Z -->|Sim| AA[Salvar e redirecionar]
    Z -->|Não| M
    R --> M
    T --> M
    W --> M
    Y --> BB[Redirecionar para finalização]
    AA --> CC[Página de Competências]


## Estrutura de Arquivos


Peers.Moderno/
├── Components/
│   ├── AutoAvaliacao/
│   │   ├── AutoAvaliacaoPerformance.razor
│   │   └── PerformanceTable.razor
│   └── Common/
│       └── MessageBox.razor
├── Services/
│   ├── AutoAvaliacao/
│   │   ├── AutoAvaliacaoPerformanceService.cs
│   │   └── Common/
│   │       ├── NotaHelper.cs
│   │       ├── AccordionHelper.cs
│   │       └── ValidationHelper.cs
│   └── Common/
│       ├── MessageBoxService.cs
│       ├── UserContextService.cs
│       └── TelemetryService.cs
└── docs/
    └── autoavalizacao_performance.md


## Considerações Técnicas

### 1. Compatibilidade
- Mantém compatibilidade com dados existentes
- Preserva lógica de negócio original
- Suporte a sessões para migração gradual

### 2. Performance
- Renderização otimizada com `@rendermode InteractiveAuto`
- Lazy loading de componentes pesados
- Cache inteligente de dados

### 3. Segurança
- Validação server-side mantida
- Controle de permissões por perfil
- Sanitização de inputs

### 4. Monitoramento
- Telemetria integrada
- Logging estruturado
- Métricas de performance

## Próximos Passos

1. **Testes de Integração** - Validar fluxo completo
2. **Testes de Performance** - Benchmark vs versão original
3. **Migração Gradual** - Deploy em ambiente de homologação
4. **Treinamento** - Capacitar equipe na nova arquitetura
5. **Monitoramento** - Acompanhar métricas pós-deploy

## Conclusão

A migração da autoavaliação de performance para Blazor representa um marco importante na modernização do sistema. A nova arquitetura oferece melhor manutenibilidade, performance e experiência do usuário, mantendo toda a funcionalidade crítica do negócio.
# Documentação: Consolidação de Avaliação - Blazor/Services

## Visão Geral

A Consolidação de Avaliação foi migrada do Web Forms para uma arquitetura moderna baseada em Blazor, com serviços de negócio centralizados e helpers reutilizáveis. Toda a lógica de consulta, cálculo e atualização de dados foi encapsulada em serviços na pasta `Services/Consolidacao` e `Services/Consolidacao/Common`, seguindo o padrão de centralização de código reutilizável. Helpers para combos, formatação, mensagens, visibilidade, exportação, contexto de usuário e telemetria estão disponíveis em `Services/Common`.

## Estrutura de Serviços

- **IConsolidacaoService**: Interface para operações de consolidação, como obtenção de competências, performance, cálculo de notas, considerações de mentor, etc.
- **ConsolidacaoService**: Implementação concreta, usando Entity Framework Core para acesso a dados e lógica de negócio.
- **Helpers Reutilizáveis**: ComboHelper, FormatHelper, VisibilityHelper, ExportFileService, MessageBoxService, UserContextService, TelemetryService, todos em `Services/Common`.

## Integração com Componentes Blazor

Os componentes Blazor (ex: `ConsolidacaoPage.razor`, `CompetenciasTable.razor`, `PerformanceTable.razor`, `DadosRHPanel.razor`) consomem os métodos do serviço de consolidação via injeção de dependência. Os helpers reutilizáveis são usados para combos, formatação de valores, visibilidade de elementos, mensagens ao usuário e exportação de dados.

## Fluxo do Processo

mermaid
flowchart TD
    A[ConsolidacaoPage.razor] --> B[CompetenciasTable.razor]
    A --> C[PerformanceTable.razor]
    A --> D[DadosRHPanel.razor]
    B --> E[IConsolidacaoService]
    C --> E
    D --> E
    E --> F[ApplicationDbContext]
    B --> G[ComboHelper / FormatHelper]
    C --> G
    D --> G
    A --> H[MessageBoxService]
    A --> I[UserContextService]
    A --> J[TelemetryService]


## Sugestões de Melhorias

- Implementar cache para dados de combos e listas de competências/performance para melhorar performance.
- Adicionar testes automatizados para os serviços de consolidação e helpers reutilizáveis.
- Expandir os métodos de cálculo de notas para suportar regras de negócio mais complexas e parametrizáveis via configuração.
- Criar componentes Blazor genéricos para tabelas e accordions, facilitando ainda mais a reutilização.
- Integrar logs de auditoria e telemetria detalhada para rastreamento de operações críticas.

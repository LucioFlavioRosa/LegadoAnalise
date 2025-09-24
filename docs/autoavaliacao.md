# Migração da Autoavaliação - Web Forms para Blazor

## Visão Geral

Este documento descreve a migração da funcionalidade de autoavaliação do sistema legado Web Forms para a nova arquitetura Blazor Server com .NET 9. A migração seguiu o padrão de separação de responsabilidades, extraindo a lógica de negócio para serviços injetáveis reutilizáveis.

## Arquitetura

### Estrutura de Pastas


Services/
├── Avaliacoes/
│   ├── AutoAvaliacaoService.cs
│   └── Common/
│       └── IAutoAvaliacaoService.cs
Models/
├── ProjetoModel.cs
└── ProjetosAssociadosModel.cs


### Componentes Principais

#### 1. IAutoAvaliacaoService

Interface que define os contratos para:
- Carregamento de combos (projetos, clientes, períodos, status)
- Busca de avaliações filtradas
- Finalização de avaliações
- Validação de permissões

#### 2. AutoAvaliacaoService

Implementação concreta que:
- Utiliza Entity Framework Core para acesso a dados
- Integra com serviços existentes (AssociadosService, ClientesService, etc.)
- Implementa telemetria e logging
- Gerencia mensagens de feedback ao usuário

#### 3. Modelos de Dados

- **ProjetoModel**: Representa projetos com seus associados e status
- **ProjetosAssociadosModel**: Representa associados alocados em projetos
- **Modelos auxiliares**: StatusProjeto, StatusAvaliacao, PeriodoAvaliacao, etc.

## Fluxo de Funcionamento

mermaid
flowchart TD
    A[Usuário acessa Autoavaliação] --> B[Carregar Combos]
    B --> C[AutoAvaliacaoService.CarregarCombos]
    C --> D[Exibir Filtros]
    D --> E[Usuário aplica filtros]
    E --> F[BuscarAvaliacoesFiltradasAsync]
    F --> G[Consultar BD via EF Core]
    G --> H[Processar dados]
    H --> I[Retornar ProjetoModel[]]
    I --> J[Exibir lista de avaliações]
    J --> K{Usuário clica ação?}
    K -->|Iniciar/Continuar| L[Redirecionar para tela específica]
    K -->|Finalizar| M[FinalizarAvaliacaoAsync]
    M --> N[Validar dados obrigatórios]
    N --> O{Validação OK?}
    O -->|Não| P[Exibir mensagem erro]
    O -->|Sim| Q[Atualizar BD]
    Q --> R[Exibir sucesso]
    P --> J
    R --> J


## Integração com Serviços Existentes

### Dependências Injetadas

- **ApplicationDbContext**: Acesso ao banco de dados
- **IAssociadosService**: Gerenciamento de usuários
- **IClientesService**: Dados de clientes
- **IProjetosService**: Informações de projetos
- **ICargosService**: Dados de cargos
- **ITelemetryService**: Telemetria e logging
- **IMessageBoxService**: Mensagens ao usuário

### Reutilização de Código

- **ComboHelper**: Utilitários para geração de combos
- **UserContextService**: Contexto do usuário logado
- **MessageBoxService**: Sistema de mensagens padronizado

## Benefícios da Migração

1. **Separação de Responsabilidades**: Lógica de negócio isolada da UI
2. **Testabilidade**: Serviços podem ser testados independentemente
3. **Reutilização**: Lógica pode ser reutilizada em outras telas
4. **Manutenibilidade**: Código mais organizado e fácil de manter
5. **Performance**: Blazor Server oferece melhor experiência do usuário
6. **Telemetria**: Monitoramento integrado das operações

## Configuração

O serviço é registrado no `Program.cs`:

csharp
builder.Services.AddScoped<IAutoAvaliacaoService, AutoAvaliacaoService>();


## Uso em Componentes Blazor

csharp
@inject IAutoAvaliacaoService AutoAvaliacaoService

// Carregar combos
var projetos = await AutoAvaliacaoService.CarregarComboProjetosAsync(usuarioId);

// Buscar avaliações
var avaliacoes = await AutoAvaliacaoService.BuscarAvaliacoesFiltradasAsync(usuarioId, filtro);

// Finalizar avaliação
var sucesso = await AutoAvaliacaoService.FinalizarAvaliacaoAsync(idAvaliacao, usuarioId);


## Considerações Técnicas

- Todos os métodos são assíncronos para melhor performance
- Tratamento de exceções com telemetria integrada
- Validações de negócio centralizadas no serviço
- Compatibilidade com Entity Framework Core
- Suporte a injeção de dependência nativa do .NET

## Próximos Passos

1. Criar componente Blazor para a interface
2. Implementar testes unitários
3. Adicionar cache para melhor performance
4. Considerar integração com IA para sugestões inteligentes
# Módulo Complexidades - Blazor .NET 9

Este módulo implementa o cadastro e gerenciamento de complexidades de projetos utilizando Blazor Híbrido (.NET 9) e arquitetura baseada em serviços injetáveis.

## Arquitetura

### Estrutura de Pastas

Peers.Moderno/
├── Components/
│   └── Pages/
│       ├── Complexidade.razor          # Componente Blazor principal
│       └── Complexidade.razor.cs       # Code-behind do componente
├── Services/
│   └── Complexidades/
│       ├── ComplexidadesService.cs     # Implementação do serviço
│       └── Common/
│           ├── IComplexidadeService.cs # Interface do serviço
│           └── ComplexidadeDto.cs      # DTO para transporte de dados
├── Models/
│   └── ProjetoComplexidade.cs          # Modelo Entity Framework
└── docs/
    └── Complexidades.md                # Esta documentação


## Componentes Principais

### 1. ComplexidadesService
Serviço principal que implementa toda a lógica de negócio para gerenciamento de complexidades:
- **Operações CRUD**: Criar, ler, atualizar e inativar complexidades
- **Validações**: Validação de dados antes da persistência
- **Logging**: Integração com Application Insights para telemetria
- **Tratamento de Erros**: Captura e tratamento adequado de exceções

### 2. ComplexidadeDto
Objeto de transferência de dados que facilita:
- **Desacoplamento**: Separação entre modelo de dados e interface
- **Reutilização**: Pode ser usado por outros módulos
- **Validação**: Propriedades calculadas para status e validações

### 3. Componente Blazor (Complexidade.razor)
Interface de usuário moderna implementando:
- **Renderização Híbrida**: Modo `InteractiveAuto` para máxima performance
- **Data Binding**: Vinculação bidirecional de dados
- **Estado Reativo**: Atualização automática da interface
- **Validação Client-side**: Validações em tempo real

## Funcionalidades

### Cadastro de Complexidades
- Formulário para inserção de nova complexidade
- Validação de campos obrigatórios
- Cálculo automático de códigos
- Feedback visual de processamento

### Listagem e Pesquisa
- Exibição tabular de todas as complexidades
- Filtros de busca em tempo real
- Indicadores visuais de status (Ativo/Inativo)
- Paginação automática

### Edição
- Carregamento de dados existentes no formulário
- Atualização em tempo real
- Validação de integridade dos dados

### Inativação
- Inativação lógica (soft delete)
- Confirmação de ação
- Atualização automática da listagem

## Integração

### Entity Framework Core
- Mapeamento da entidade `ProjetoComplexidade`
- Configuração de tipos de dados decimais
- Relacionamentos com outras entidades

### Injeção de Dependência
- Registro automático no container DI
- Escopo `Scoped` para operações por requisição
- Interfaces para facilitar testes unitários

### Serviços Compartilhados
- **MessageBoxService**: Exibição de mensagens para o usuário
- **TelemetryService**: Logging e monitoramento
- **ApplicationDbContext**: Acesso ao banco de dados

## Configuração

### appsettings.json

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=SistemaAvaliacao;..."
  },
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=..."
  }
}


### Program.cs
csharp
// Registro do serviço
builder.Services.AddScoped<IComplexidadeService, ComplexidadesService>();


## Fluxo de Alto Nível

mermaid
flowchart TD
    A[Usuário acessa /complexidades] --> B[Complexidade.razor]
    B --> C[OnInitializedAsync]
    C --> D[ComplexidadesService.ObterListaComplexidadesAsync]
    D --> E[ApplicationDbContext]
    E --> F[SQL Server Database]
    F --> G[Retorna List<ProjetoComplexidade>]
    G --> H[Converte para List<ComplexidadeDto>]
    H --> I[Atualiza UI com dados]
    
    J[Usuário clica Cadastrar] --> K[ValidarFormulario]
    K --> L{Dados válidos?}
    L -->|Não| M[MessageBoxService.ShowWarning]
    L -->|Sim| N[ComplexidadesService.InserirComplexidadeAsync]
    N --> O[Validações de negócio]
    O --> P[Salva no banco]
    P --> Q[TelemetryService.TrackEvent]
    Q --> R[MessageBoxService.ShowSuccess]
    R --> S[Recarrega listagem]
    
    T[Usuário clica Alterar] --> U[EditarComplexidade]
    U --> V[ComplexidadesService.ObterComplexidadeAsync]
    V --> W[Carrega dados no formulário]
    W --> X[isEdicao = true]
    
    Y[Usuário clica Inativar] --> Z[InativarComplexidade]
    Z --> AA[ComplexidadesService.ExcluirComplexidadeAsync]
    AA --> BB[ATV = 0 (soft delete)]
    BB --> CC[Atualiza banco]
    CC --> DD[Recarrega listagem]


## Padrões Utilizados

### Repository Pattern (via EF Core)
- Abstração do acesso a dados
- Facilita testes unitários
- Centraliza queries complexas

### DTO Pattern
- Separação entre modelo de domínio e transporte
- Controle sobre dados expostos
- Facilita versionamento de APIs

### Service Layer Pattern
- Centralização da lógica de negócio
- Reutilização entre diferentes interfaces
- Facilita manutenção e testes

### Dependency Injection
- Baixo acoplamento entre componentes
- Facilita testes unitários
- Configuração centralizada

## Boas Práticas Implementadas

### Performance
- Queries assíncronas para não bloquear UI
- Lazy loading quando apropriado
- Paginação para grandes volumes

### Segurança
- Validação server-side obrigatória
- Sanitização de inputs
- Soft delete para auditoria

### Manutenibilidade
- Separação clara de responsabilidades
- Código autodocumentado
- Tratamento consistente de erros

### Experiência do Usuário
- Feedback visual durante operações
- Validações em tempo real
- Estados de loading claros
- Mensagens de erro amigáveis

## Extensibilidade

O módulo foi projetado para facilitar futuras extensões:

### Novos Campos
1. Adicionar propriedade no `ComplexidadeDto`
2. Atualizar modelo `ProjetoComplexidade`
3. Modificar componente Blazor
4. Executar migration do EF Core

### Novas Funcionalidades
1. Implementar método na interface `IComplexidadeService`
2. Adicionar implementação em `ComplexidadesService`
3. Criar componente UI correspondente
4. Registrar rotas se necessário

### Integração com Outros Módulos
- Interface `IComplexidadeService` pode ser injetada em outros serviços
- `ComplexidadeDto` pode ser reutilizado em APIs
- Eventos podem ser publicados para notificar outras partes do sistema

## Monitoramento e Logs

### Application Insights
- Tracking de eventos de negócio
- Monitoramento de performance
- Alertas automáticos para erros

### Logs Estruturados
- Informações de contexto em cada operação
- Correlação entre requests
- Facilita debugging em produção

## Próximos Passos

1. **Implementar Fatores de Complexidade**: Expandir para gerenciar fatores associados
2. **Relatórios**: Adicionar funcionalidades de exportação e relatórios
3. **Auditoria**: Implementar log completo de alterações
4. **API REST**: Expor funcionalidades via API para integração
5. **Testes Automatizados**: Criar suite completa de testes unitários e integração
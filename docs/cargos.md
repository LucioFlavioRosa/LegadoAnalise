# Módulo de Cargos - Documentação Técnica

## Visão Geral

O módulo de Cargos foi migrado de Web Forms para Blazor Server com renderização híbrida (.NET 9), seguindo uma arquitetura baseada em serviços e injeção de dependência. O módulo permite o gerenciamento completo de cargos da empresa, incluindo CRUD, exportação para Excel e relacionamentos hierárquicos.

## Arquitetura

### Estrutura de Pastas


Services/
├── Cargos/
│   ├── CargosService.cs          # Serviço principal de negócio
│   └── Common/
│       ├── DropdownService.cs    # Serviço para dropdowns
│       └── ExportService.cs      # Serviço de exportação
└── Common/
    └── MessageBoxService.cs      # Serviço de mensagens

Components/
└── Cargos/
    ├── Cargos.razor             # Componente principal
    └── Cargos.razor.cs          # Code-behind

Models/
├── Cargo.cs                     # Modelo principal
├── CargoExportModel.cs          # Modelo para exportação
└── DropdownItem.cs              # Modelo para dropdowns


### Princípios de Design

1. **Separação de Responsabilidades**: Lógica de negócio isolada em serviços
2. **Reutilização**: Serviços comuns organizados em pastas `Common`
3. **Injeção de Dependência**: Todos os serviços registrados no DI container
4. **Renderização Híbrida**: Componente usa `@rendermode InteractiveAuto`

## Serviços

### CargosService

**Responsabilidade**: CRUD de cargos e validações de negócio

**Métodos principais**:
- `ObterListaCargosAsync()`: Lista todos os cargos
- `ObterCargoAsync(id)`: Obtém cargo específico
- `InserirCargoAsync(cargo)`: Insere novo cargo
- `AlterarCargoAsync(cargo)`: Altera cargo existente
- `InativarCargoAsync(id)`: Inativa cargo
- `ExisteCargoComNomeAsync(nome)`: Valida duplicação

### DropdownService (Common)

**Responsabilidade**: Popular dropdowns de forma padronizada

**Métodos**:
- `ObterCargosAtivosAsync()`: Cargos ativos para dropdown
- `ObterTodosCargosAsync()`: Todos os cargos
- `ObterStatusOptionsAsync()`: Opções de status

**Reutilização**: Pode ser usado por outros módulos que precisem de dropdowns de cargos

### ExportService (Common)

**Responsabilidade**: Exportação de dados para Excel

**Métodos**:
- `ExportarCargosAsync()`: Gera arquivo Excel com dados dos cargos

**Reutilização**: Padrão pode ser aplicado para exportação de outras entidades

### MessageBoxService (Common)

**Responsabilidade**: Sistema de mensagens para feedback ao usuário

**Métodos**:
- `ShowSuccess(message)`: Mensagem de sucesso
- `ShowError(message)`: Mensagem de erro
- `ShowInfo(message)`: Mensagem informativa
- `ShowWarning(message)`: Mensagem de aviso

**Reutilização**: Usado por qualquer componente que precise de feedback

## Componente Blazor

### Cargos.razor

**Características**:
- Renderização híbrida com `@rendermode InteractiveAuto`
- Formulário reativo com validação
- Grid de dados com ações inline
- Exportação integrada
- Sistema de toast para mensagens

**Funcionalidades**:
- Cadastro e edição de cargos
- Listagem com filtros
- Inativação de registros
- Exportação para Excel
- Validação client-side e server-side

## Integração com Outros Módulos

### Como Reutilizar os Serviços

csharp
// Em outro componente/serviço
@inject Services.Cargos.Common.IDropdownService DropdownService
@inject Services.Common.IMessageBoxService MessageBoxService

// Usar dropdown de cargos
var cargos = await DropdownService.ObterCargosAtivosAsync();

// Mostrar mensagem
MessageBoxService.ShowSuccess("Operação realizada com sucesso!");


### Extensibilidade

1. **Novos Campos**: Adicionar propriedades no modelo `Cargo` e atualizar o componente
2. **Novas Validações**: Implementar no `CargosService` ou no modelo de formulário
3. **Novos Formatos de Export**: Estender o `ExportService` com novos métodos
4. **Integração com APIs**: Adicionar HttpClient nos serviços conforme necessário

## Configuração

### Registro no DI Container (Program.cs)

csharp
// Cargos Services
builder.Services.AddScoped<Services.Cargos.ICargosService, Services.Cargos.CargosService>();
builder.Services.AddScoped<Services.Cargos.Common.IDropdownService, Services.Cargos.Common.DropdownService>();
builder.Services.AddScoped<Services.Cargos.Common.IExportService, Services.Cargos.Common.ExportService>();
builder.Services.AddScoped<Services.Common.IMessageBoxService, Services.Common.MessageBoxService>();


### Entity Framework

O modelo `Cargo` está mapeado no `ApplicationDbContext` com:
- Relacionamento self-referencing para `ProximoCargo`
- Mapeamento para tabela `CARGOS`
- Navigation properties configuradas

## Fluxo de Alto Nível

mermaid
flowchart TD
    A[Usuário] --> B[Cargos.razor]
    B --> C{Ação do Usuário}
    
    C -->|Carregar Dados| D[CargosService.ObterListaCargosAsync]
    C -->|Popular Dropdown| E[DropdownService.ObterCargosAtivosAsync]
    C -->|Salvar Cargo| F[CargosService.InserirCargoAsync/AlterarCargoAsync]
    C -->|Inativar Cargo| G[CargosService.InativarCargoAsync]
    C -->|Exportar| H[ExportService.ExportarCargosAsync]
    
    D --> I[ApplicationDbContext]
    E --> I
    F --> I
    G --> I
    H --> I
    
    I --> J[(SQL Server Database)]
    
    F --> K[MessageBoxService.ShowSuccess/ShowError]
    G --> K
    H --> K
    
    K --> L[Toast Notification]
    L --> A
    
    H --> M[Download Excel File]
    M --> A
    
    style B fill:#e1f5fe
    style I fill:#f3e5f5
    style J fill:#e8f5e8
    style K fill:#fff3e0


## Benefícios da Arquitetura

1. **Testabilidade**: Serviços isolados facilitam testes unitários
2. **Manutenibilidade**: Separação clara de responsabilidades
3. **Reutilização**: Serviços comuns podem ser usados em outros módulos
4. **Performance**: Renderização híbrida otimiza experiência do usuário
5. **Escalabilidade**: Arquitetura preparada para crescimento

## Próximos Passos

1. Implementar cache para dropdowns frequentemente acessados
2. Adicionar logs estruturados usando ILogger
3. Implementar paginação para grandes volumes de dados
4. Adicionar testes unitários para os serviços
5. Considerar implementação de padrão Repository se necessário
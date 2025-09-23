# Módulo de Dimensões - Documentação Técnica

## Visão Geral

O módulo de Dimensões foi migrado de Web Forms para Blazor Híbrido (.NET 9), implementando uma arquitetura moderna baseada em serviços com separação clara de responsabilidades. O módulo permite o gerenciamento completo de dimensões de avaliação, incluindo operações CRUD e exportação para Excel.

## Arquitetura

### Componentes Principais

#### 1. Interface do Usuário (Blazor)
- **Dimensoes.razor**: Componente principal da interface
- **Dimensoes.razor.cs**: Code-behind com lógica de apresentação

#### 2. Serviços de Negócio
- **IDimensoesService**: Interface para operações CRUD
- **DimensoesService**: Implementação das regras de negócio
- **IDimensoesExportService**: Interface para exportação
- **DimensoesExportService**: Implementação da exportação Excel

#### 3. Modelos de Dados
- **Dimensao**: Entidade de domínio mapeada para EF Core
- **DimensaoExportDto**: DTO específico para exportação
- **DimensaoFormModel**: Modelo para binding de formulário

### Padrões Implementados

1. **Dependency Injection**: Todos os serviços são injetados via DI
2. **Repository Pattern**: Acesso a dados através do ApplicationDbContext
3. **DTO Pattern**: Separação entre modelos de domínio e transferência
4. **Service Layer**: Lógica de negócio centralizada em serviços
5. **Common Pattern**: Interfaces centralizadas em pastas Common

## Funcionalidades

### Operações CRUD
- **Listar**: Exibição de todas as dimensões com filtro em tempo real
- **Inserir**: Cadastro de novas dimensões com validação
- **Alterar**: Edição de dimensões existentes
- **Inativar**: Desativação lógica de dimensões

### Exportação
- Geração de arquivo Excel com todas as dimensões
- Download automático via JavaScript Interop
- Formatação profissional com cabeçalhos e auto-fit

### Validações
- Validação de campos obrigatórios
- Verificação de nomes duplicados
- Validação de tamanho de campos
- Confirmação para operações críticas

## Integração com Serviços Comuns

### MessageBoxService
csharp
// Exibição de mensagens de sucesso, erro, aviso e informação
MessageBoxService.ShowSuccess("Dimensão inserida com sucesso!");
MessageBoxService.ShowError("Erro ao salvar dimensão");


### TelemetryService
csharp
// Rastreamento de eventos e métricas
TelemetryService.TrackEvent("Dimensao_Inserida", properties);
TelemetryService.TrackException(ex, context);
TelemetryService.TrackDependency("Database", "ListarDimensoes", data, startTime, duration, success);


### ApplicationDbContext
csharp
// Acesso a dados via Entity Framework Core
var dimensoes = await _context.Dimensoes
    .OrderBy(d => d.Nome)
    .ToListAsync();


## Configuração

### Registro de Serviços (Program.cs)
csharp
// Dimensoes Services
builder.Services.AddScoped<Services.Dimensoes.Common.IDimensoesService, Services.Dimensoes.DimensoesService>();
builder.Services.AddScoped<IDimensoesExportService, DimensoesExportService>();


### Mapeamento Entity Framework
O modelo `Dimensao` está mapeado para a tabela `DIMENSOES` existente, mantendo compatibilidade com o banco de dados atual.

## Estrutura de Arquivos


Components/Pages/Dimensoes/
├── Dimensoes.razor              # Interface do usuário
└── Dimensoes.razor.cs           # Code-behind

Services/Dimensoes/
├── DimensoesService.cs          # Serviço principal
├── DimensoesExportService.cs    # Serviço de exportação
└── Common/
    ├── IDimensoesService.cs     # Interface principal
    └── IDimensoesExportService.cs # Interface de exportação

Models/
├── Dimensao.cs                  # Entidade de domínio
└── DimensaoExportDto.cs         # DTO de exportação


## Fluxo de Alto Nível

mermaid
flowchart TD
    A[Usuário acessa /dimensoes] --> B[Dimensoes.razor carregado]
    B --> C[OnInitializedAsync chamado]
    C --> D[IDimensoesService.ListarAsync]
    D --> E[ApplicationDbContext consulta DIMENSOES]
    E --> F[Lista exibida na interface]
    
    G[Usuário preenche formulário] --> H[HandleCadastrar executado]
    H --> I{Validação OK?}
    I -->|Sim| J[IDimensoesService.InserirAsync/AlterarAsync]
    I -->|Não| K[Exibir erros de validação]
    J --> L[Salvar no banco via EF Core]
    L --> M[IMessageBoxService.ShowSuccess]
    M --> N[ITelemetryService.TrackEvent]
    N --> O[Recarregar lista]
    
    P[Usuário clica Exportar] --> Q[HandleExportar executado]
    Q --> R[IDimensoesExportService.ExportarDimensoesAsync]
    R --> S[Gerar arquivo Excel com EPPlus]
    S --> T[Converter para Base64]
    T --> U[JSRuntime.InvokeVoidAsync downloadFile]
    U --> V[Download automático no browser]
    
    W[Usuário clica Inativar] --> X[Confirmação via JSRuntime]
    X --> Y{Confirmado?}
    Y -->|Sim| Z[IDimensoesService.InativarAsync]
    Y -->|Não| AA[Cancelar operação]
    Z --> BB[Atualizar status no banco]
    BB --> CC[Recarregar lista]
    
    DD[Filtro em tempo real] --> EE[FiltrarDimensoes executado]
    EE --> FF[Aplicar filtro na lista local]
    FF --> GG[StateHasChanged para atualizar UI]


## Benefícios da Migração

1. **Performance**: Blazor Híbrido com renderização otimizada
2. **Manutenibilidade**: Código organizado em serviços e componentes
3. **Testabilidade**: Interfaces permitem mock e testes unitários
4. **Reusabilidade**: Serviços podem ser reutilizados em outros módulos
5. **Monitoramento**: Integração completa com Application Insights
6. **UX Moderna**: Interface reativa com validação em tempo real
7. **Escalabilidade**: Arquitetura preparada para crescimento

## Considerações de Segurança

- Validação tanto no cliente quanto no servidor
- Sanitização de inputs para prevenir XSS
- Autenticação via Azure AD integrada
- Logs de auditoria via TelemetryService

## Próximos Passos

1. Implementar cache para melhorar performance
2. Adicionar paginação para listas grandes
3. Implementar soft delete com histórico
4. Adicionar importação via Excel
5. Criar testes unitários e de integração
# Documentação: SubCompetências - Migração Web Forms para Blazor

## Visão Geral

Este documento descreve a migração da funcionalidade de SubCompetências do sistema legado Web Forms para a nova arquitetura Blazor Server com .NET 9. A migração segue os princípios de Clean Architecture, separação de responsabilidades e reutilização de código.

## Arquitetura da Solução

### Estrutura de Pastas


Peers.Moderno/
├── Models/
│   └── SubCompetencia.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Services/
│   ├── Common/
│   │   ├── ExportFileService.cs
│   │   ├── MessageBoxService.cs
│   │   └── ComboHelper.cs
│   └── SubCompetencias/
│       ├── ISubCompetenciasService.cs
│       ├── SubCompetenciasService.cs
│       └── Common/
│           ├── ISubCompetenciasValidator.cs
│           ├── SubCompetenciasValidator.cs
│           ├── ISubCompetenciasExportService.cs
│           ├── SubCompetenciasExportService.cs
│           ├── ISubCompetenciasImportService.cs
│           └── SubCompetenciasImportService.cs
├── Components/
│   └── SubCompetencias/
│       ├── SubCompetencias.razor
│       └── SubCompetencias.razor.cs
└── docs/
    └── SubCompetencias.md


## Componentes da Arquitetura

### 1. Modelo de Dados (SubCompetencia.cs)

O modelo `SubCompetencia` representa a entidade principal com os seguintes campos:

- **IdSubCompetencia**: Identificador único
- **Nome**: Nome da subcompetência (máximo 500 caracteres)
- **ATV**: Status ativo/inativo (boolean)
- **TipoAvaliacao**: Tipo de avaliação (desempenho, liderança)
- **DHC**: Data/hora de criação
- **USR**: ID do usuário que criou/alterou
- **DataAtualizacao**: Data da última atualização

### 2. Contexto de Dados (ApplicationDbContext.cs)

Configuração do Entity Framework Core para a tabela `SUBCOMPETENCIAS`:

- Mapeamento de colunas
- Índices para performance
- Constraints e valores padrão
- Relacionamentos com outras entidades

### 3. Camada de Serviços

#### Serviço Principal (SubCompetenciasService.cs)

Implementa as operações de CRUD:

- `ListarAsync()`: Lista todas as subcompetências
- `ObterPorIdAsync(int id)`: Obtém uma subcompetência por ID
- `AdicionarAsync(SubCompetencia item)`: Adiciona nova subcompetência
- `AtualizarAsync(SubCompetencia item)`: Atualiza subcompetência existente
- `InativarAsync(int id)`: Inativa uma subcompetência
- `ExportarAsync()`: Exporta dados para Excel

#### Serviços Auxiliares (Common/)

- **SubCompetenciasValidator**: Validação de regras de negócio
- **SubCompetenciasExportService**: Exportação especializada
- **SubCompetenciasImportService**: Importação de dados

### 4. Serviços Comuns Reutilizados

- **ExportFileService**: Exportação genérica para Excel
- **MessageBoxService**: Sistema de notificações
- **ComboHelper**: Utilitários para dropdowns

## Configuração (appsettings.json)

### Seção SubCompetencias


{
  "SubCompetencias": {
    "MaxSubCompetenciaLength": 500,
    "MaxExportRecords": 50000,
    "DefaultStatus": 1,
    "DefaultTipoAvaliacao": "desempenho",
    "ValidationMessages": {
      "SubCompetenciaObrigatoria": "Preencha o campo SubCompetência",
      "SucessoInsercao": "Sub Competência Inserida com sucesso !!",
      "SucessoAlteracao": "Sub Competência alterada com sucesso !!"
    }
  }
}


## Injeção de Dependência

Serviços registrados no `Program.cs`:

csharp
// SubCompetencias Services
builder.Services.AddScoped<Services.SubCompetencias.ISubCompetenciasService, Services.SubCompetencias.SubCompetenciasService>();
builder.Services.AddScoped<Services.SubCompetencias.Common.ISubCompetenciasValidator, Services.SubCompetencias.Common.SubCompetenciasValidator>();
builder.Services.AddScoped<Services.SubCompetencias.Common.ISubCompetenciasExportService, Services.SubCompetencias.Common.SubCompetenciasExportService>();
builder.Services.AddScoped<Services.SubCompetencias.Common.ISubCompetenciasImportService, Services.SubCompetencias.Common.SubCompetenciasImportService>();


## Interface do Usuário (Blazor)

### Componente Principal (SubCompetencias.razor)

Utiliza renderização híbrida com `@rendermode InteractiveAuto`:

- Formulário de cadastro/edição
- Lista paginada com filtros
- Funcionalidade de exportação
- Integração com sistema de mensagens

### Code-Behind (SubCompetencias.razor.cs)

- Propriedades de binding
- Métodos de manipulação de eventos
- Chamadas aos serviços injetados
- Tratamento de estados e validações

## Fluxo de Alto Nível

mermaid
flowchart TD
    A[Usuário] --> B[SubCompetencias.razor]
    B --> C[SubCompetencias.razor.cs]
    C --> D[ISubCompetenciasService]
    D --> E[ApplicationDbContext]
    D --> F[ISubCompetenciasValidator]
    D --> G[ISubCompetenciasExportService]
    C --> H[IMessageBoxService]
    C --> I[ComboHelper]
    
    subgraph "Camada de Apresentação"
        A
        B
        C
    end
    
    subgraph "Camada de Serviços"
        D
        F
        G
        H
        I
    end
    
    subgraph "Camada de Dados"
        E
    end
    
    style A fill:#e1f5fe
    style B fill:#f3e5f5
    style C fill:#f3e5f5
    style D fill:#e8f5e8
    style F fill:#e8f5e8
    style G fill:#e8f5e8
    style H fill:#e8f5e8
    style I fill:#e8f5e8
    style E fill:#fff3e0


## Funcionalidades Implementadas

### CRUD Completo
- ✅ Listagem com filtros
- ✅ Cadastro de novas subcompetências
- ✅ Edição de subcompetências existentes
- ✅ Inativação/ativação
- ✅ Validações de negócio

### Exportação/Importação
- ✅ Exportação para Excel
- ✅ Formatação personalizada
- ✅ Validação de dados na importação
- ✅ Tratamento de erros

### Interface do Usuário
- ✅ Design responsivo
- ✅ Filtros dinâmicos
- ✅ Paginação
- ✅ Feedback visual
- ✅ Mensagens de sucesso/erro

## Benefícios da Migração

### Performance
- Renderização híbrida (Server + WebAssembly)
- Carregamento otimizado de dados
- Cache inteligente

### Manutenibilidade
- Separação clara de responsabilidades
- Código reutilizável
- Testes unitários facilitados
- Documentação abrangente

### Escalabilidade
- Arquitetura baseada em serviços
- Injeção de dependência
- Configuração externa
- Telemetria integrada

## Padrões Utilizados

### Repository Pattern
- Abstração da camada de dados
- Facilita testes unitários
- Permite mudança de ORM

### Service Layer Pattern
- Lógica de negócio centralizada
- Reutilização entre componentes
- Validações consistentes

### Dependency Injection
- Baixo acoplamento
- Facilita testes
- Configuração flexível

## Considerações de Segurança

- Validação de entrada em todas as camadas
- Sanitização de dados
- Controle de acesso baseado em perfis
- Auditoria de operações

## Monitoramento e Telemetria

- Application Insights integrado
- Logs estruturados
- Métricas de performance
- Rastreamento de erros

## Próximos Passos

1. Implementação de testes unitários
2. Testes de integração
3. Migração de outras funcionalidades seguindo o mesmo padrão
4. Otimizações de performance
5. Implementação de cache distribuído

## Conclusão

A migração da funcionalidade de SubCompetências estabelece um padrão sólido para futuras migrações, garantindo:

- Código limpo e manutenível
- Performance otimizada
- Experiência do usuário moderna
- Facilidade de testes e manutenção
- Documentação abrangente

Este padrão deve ser replicado para as demais funcionalidades do sistema, sempre priorizando a reutilização de código e a consistência arquitetural.
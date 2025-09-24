# Perguntas de Encerramento - Documentação Técnica

## Visão Geral

Este módulo implementa o sistema completo de gerenciamento de **Perguntas de Encerramento** utilizando **Blazor Híbrido** com **.NET 9**. O sistema permite criar, editar, listar e remover perguntas que serão utilizadas no processo de encerramento de avaliações.

## Arquitetura

### Componentes Principais

1. **Modelo de Dados**: `PerguntaEncerramento`
2. **Serviço de Negócio**: `PerguntasEncerramentoService`
3. **Interface de Usuário**: Componente Blazor `PerguntasEncerramento.razor`
4. **Persistência**: Entity Framework Core com SQL Server

### Estrutura de Arquivos


Peers.Moderno/
├── Models/
│   └── PerguntaEncerramento.cs
├── Services/
│   └── Common/
│       └── PerguntasEncerramentoService.cs
├── Components/
│   └── Pages/
│       ├── PerguntasEncerramento.razor
│       └── PerguntasEncerramento.razor.cs
├── Data/
│   └── ApplicationDbContext.cs (modificado)
└── docs/
    └── PerguntasEncerramento.md


## Funcionalidades

### CRUD Completo
- **Criar**: Adicionar novas perguntas de encerramento
- **Ler**: Listar todas as perguntas com filtros
- **Atualizar**: Editar perguntas existentes
- **Deletar**: Remover perguntas (com confirmação)

### Validações
- Código obrigatório e único
- Descrição obrigatória
- Limite de caracteres nos campos
- Verificação de duplicatas

### Interface de Usuário
- Design responsivo e moderno
- Formulário de cadastro/edição
- Tabela de listagem com ações
- Modal de confirmação para exclusão
- Indicadores de carregamento
- Mensagens de feedback

## Integração

### Injeção de Dependência

O serviço é registrado no `Program.cs`:

csharp
builder.Services.AddScoped<IPerguntasEncerramentoService, PerguntasEncerramentoService>();


### Entity Framework

A entidade é mapeada no `ApplicationDbContext`:

csharp
public DbSet<PerguntaEncerramento> PerguntasEncerramento { get; set; }


### Blazor Component

O componente utiliza:
- `@rendermode InteractiveAuto` para renderização híbrida
- Injeção de dependência para serviços
- Data binding bidirecional
- Validação de formulários

## Padrões Utilizados

### Repository Pattern
O `PerguntasEncerramentoService` atua como um repositório, encapsulando o acesso aos dados.

### Separation of Concerns
- **Model**: Estrutura de dados
- **Service**: Lógica de negócio
- **Component**: Interface de usuário
- **Context**: Acesso a dados

### Dependency Injection
Todos os serviços são injetados via DI, facilitando testes e manutenção.

## Telemetria e Monitoramento

O serviço integra com Application Insights para:
- Rastreamento de eventos (criação, edição, exclusão)
- Monitoramento de exceções
- Métricas de performance

## Reutilização

### Serviços Comuns
O serviço está localizado em `Services/Common/` para facilitar reutilização em outras partes do sistema.

### Interface Padronizada
A interface `IPerguntasEncerramentoService` permite fácil substituição e teste da implementação.

## Fluxo de Alto Nível

mermaid
flowchart TD
    A[Usuário] --> B[Blazor Component]
    B --> C{Ação do Usuário}
    
    C -->|Listar| D[PerguntasEncerramentoService.ListarAsync]
    C -->|Criar| E[PerguntasEncerramentoService.AdicionarAsync]
    C -->|Editar| F[PerguntasEncerramentoService.AtualizarAsync]
    C -->|Remover| G[PerguntasEncerramentoService.RemoverAsync]
    
    D --> H[ApplicationDbContext]
    E --> I{Validações}
    F --> I
    G --> J{Confirmação}
    
    I -->|Válido| H
    I -->|Inválido| K[Mensagem de Erro]
    
    J -->|Confirmado| H
    J -->|Cancelado| B
    
    H --> L[SQL Server Database]
    L --> M[Resposta]
    M --> N[TelemetryService]
    M --> O[MessageBoxService]
    
    N --> P[Application Insights]
    O --> Q[Feedback Visual]
    Q --> B
    
    K --> B


## Configuração de Banco de Dados

### Tabela: PERGUNTAS_ENCERRAMENTO

| Campo | Tipo | Restrições |
|-------|------|------------|
| Id | int | PK, Identity |
| Codigo | nvarchar(50) | NOT NULL, Unique |
| Descricao | nvarchar(1000) | NOT NULL |
| PossuiComentario | bit | NOT NULL |
| DataCriacao | datetime2 | NOT NULL, Default: GETUTCDATE() |
| DataAtualizacao | datetime2 | NULL |
| Ativo | bit | NOT NULL, Default: 1 |

## Extensibilidade

### Futuras Melhorias
1. **Filtros Avançados**: Implementar filtros por data, status, etc.
2. **Exportação**: Adicionar funcionalidade de exportar para Excel
3. **Importação**: Permitir importação em lote
4. **Histórico**: Rastrear alterações nas perguntas
5. **Categorização**: Agrupar perguntas por categorias

### Pontos de Extensão
- Interface `IPerguntasEncerramentoService` pode ser estendida
- Modelo `PerguntaEncerramento` pode receber novos campos
- Componente Blazor pode ser customizado

## Considerações de Performance

1. **Paginação**: Para grandes volumes, implementar paginação
2. **Cache**: Considerar cache para consultas frequentes
3. **Índices**: Criar índices apropriados no banco
4. **Lazy Loading**: Avaliar necessidade de carregamento sob demanda

## Segurança

1. **Validação**: Todas as entradas são validadas
2. **Sanitização**: Dados são tratados antes da persistência
3. **Autorização**: Integração com sistema de autenticação existente
4. **Auditoria**: Logs de todas as operações

## Testes

### Estratégia de Testes
1. **Testes Unitários**: Para o serviço e validações
2. **Testes de Integração**: Para o contexto de dados
3. **Testes de Interface**: Para o componente Blazor
4. **Testes E2E**: Para fluxos completos

### Mocks e Stubs
- `IPerguntasEncerramentoService` pode ser facilmente mockado
- `ApplicationDbContext` pode usar InMemory provider para testes

## Deployment

### Requisitos
- .NET 9 Runtime
- SQL Server (ou compatível)
- Application Insights (opcional)

### Migrations
Executar migrations do Entity Framework para criar/atualizar a tabela:

bash
dotnet ef migrations add AddPerguntasEncerramento
dotnet ef database update


## Troubleshooting

### Problemas Comuns
1. **Erro de Conexão**: Verificar connection string
2. **Tabela não existe**: Executar migrations
3. **Duplicata de código**: Validação está funcionando
4. **Performance lenta**: Verificar índices e queries

### Logs
Todos os erros são logados via `ITelemetryService` e podem ser consultados no Application Insights.

---

**Versão**: 1.0  
**Data**: 2024  
**Autor**: Sistema de Migração Peers.Moderno
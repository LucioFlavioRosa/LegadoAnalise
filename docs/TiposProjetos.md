# Tipos de Projeto - Migração Web Forms para Blazor

## Visão Geral

Este documento descreve a migração da funcionalidade de **Tipos de Projeto** de Web Forms (ASP.NET Framework) para **Blazor Híbrido** (.NET 9). A migração segue os padrões arquiteturais da aplicação, utilizando serviços injetáveis, separação de responsabilidades e componentes reutilizáveis.

## Arquitetura

### Estrutura de Pastas


Peers.Moderno/
├── Components/
│   └── Projetos/
│       ├── TiposProjetos.razor          # Componente UI principal
│       └── TiposProjetos.razor.cs       # Code-behind do componente
├── Services/
│   └── Projetos/
│       ├── Common/
│       │   └── ITiposProjetosService.cs # Interface do serviço
│       └── TiposProjetosService.cs      # Implementação do serviço
├── Models/
│   └── TipoProjeto.cs                   # Modelo de dados
└── docs/
    └── TiposProjetos.md                 # Esta documentação


### Componentes Principais

#### 1. Modelo de Dados (`TipoProjeto.cs`)
- Representa a entidade Tipo de Projeto
- Mapeamento para tabela `TIPOS_PROJETOS`
- Propriedades auxiliares para UI (StatusTexto, Ativo)

#### 2. Interface de Serviço (`ITiposProjetosService.cs`)
- Define contratos para operações CRUD
- Permite injeção de dependência e testabilidade
- Métodos: Listar, ObterPorId, Inserir, Alterar, Inativar

#### 3. Implementação do Serviço (`TiposProjetosService.cs`)
- Implementa a interface usando Entity Framework Core
- Integração com telemetria para monitoramento
- Tratamento de exceções e logging

#### 4. Componente Blazor (`TiposProjetos.razor`)
- UI moderna e responsiva
- Formulário para cadastro/edição
- Tabela para listagem com ações
- Renderização híbrida (`@rendermode InteractiveAuto`)

#### 5. Code-behind (`TiposProjetos.razor.cs`)
- Lógica de apresentação separada da UI
- Integração com serviços via injeção de dependência
- Gerenciamento de estado do componente

## Funcionalidades

### Operações Disponíveis

1. **Listagem**: Exibe todos os tipos de projeto com status
2. **Cadastro**: Permite criar novos tipos de projeto
3. **Edição**: Permite alterar tipos existentes
4. **Inativação**: Permite inativar tipos (soft delete)
5. **Validação**: Verifica duplicatas e campos obrigatórios

### Recursos da Interface

- **Formulário Reativo**: Validação em tempo real
- **Feedback Visual**: Indicadores de carregamento e processamento
- **Mensagens**: Sistema de notificações integrado
- **Responsividade**: Interface adaptável a diferentes telas
- **Acessibilidade**: Componentes com suporte a leitores de tela

## Integração

### Injeção de Dependência

O serviço é registrado em `Program.cs`:

csharp
builder.Services.AddScoped<ITiposProjetosService, TiposProjetosService>();


### Uso em Outros Componentes

Para reutilizar o serviço em outros componentes:

csharp
@inject ITiposProjetosService TiposProjetosService

// Em métodos do componente
var tipos = await TiposProjetosService.ListarAtivosAsync();


### Integração com Entity Framework

O serviço utiliza o `ApplicationDbContext` já configurado:

csharp
var tipos = await _context.TiposProjetos
    .Where(t => t.ATV == 1)
    .OrderBy(t => t.Nome)
    .ToListAsync();


## Fluxo de Funcionamento

mermaid
flowchart TD
    A["Usuário acessa /tipos-projetos"] --> B["TiposProjetos.razor carrega"]
    B --> C["OnInitializedAsync()"]
    C --> D["CarregarTiposProjetos()"]
    D --> E["ITiposProjetosService.ListarAsync()"]
    E --> F["TiposProjetosService consulta DB"]
    F --> G["ApplicationDbContext (EF Core)"]
    G --> H["SQL Server - TIPOS_PROJETOS"]
    H --> I["Retorna dados"]
    I --> J["Atualiza UI com lista"]
    
    K["Usuário preenche formulário"] --> L["SalvarTipoProjeto()"]
    L --> M["Validação de dados"]
    M --> N{"Dados válidos?"}
    N -->|Não| O["Exibe mensagem de erro"]
    N -->|Sim| P["ITiposProjetosService.InserirAsync()"]
    P --> Q["Salva no banco"]
    Q --> R["IMessageBoxService.ShowSuccess()"]
    R --> S["Recarrega lista"]
    
    T["Usuário clica Alterar"] --> U["EditarTipoProjeto()"]
    U --> V["Preenche formulário com dados"]
    V --> W["Usuário modifica e salva"]
    W --> L
    
    X["Usuário clica Inativar"] --> Y["InativarTipoProjeto()"]
    Y --> Z["ITiposProjetosService.InativarAsync()"]
    Z --> AA["Atualiza ATV = 0"]
    AA --> BB["Exibe mensagem sucesso"]
    BB --> S


## Benefícios da Migração

### Técnicos
- **Performance**: Renderização híbrida otimizada
- **Manutenibilidade**: Código mais limpo e organizado
- **Testabilidade**: Serviços injetáveis facilitam testes
- **Reutilização**: Componentes e serviços reutilizáveis
- **Monitoramento**: Integração com Application Insights

### Experiência do Usuário
- **Responsividade**: Interface moderna e adaptável
- **Feedback**: Indicadores visuais de estado
- **Validação**: Feedback imediato de erros
- **Performance**: Carregamento mais rápido

## Padrões Utilizados

1. **Repository Pattern**: Serviço encapsula acesso a dados
2. **Dependency Injection**: Inversão de controle
3. **Separation of Concerns**: UI, lógica e dados separados
4. **Component-Based**: Componentes reutilizáveis
5. **Async/Await**: Operações assíncronas

## Considerações de Segurança

- Validação de entrada no cliente e servidor
- Sanitização de dados antes de persistir
- Logs de auditoria via telemetria
- Tratamento adequado de exceções

## Próximos Passos

1. **Testes**: Implementar testes unitários e de integração
2. **Autorização**: Adicionar controle de acesso baseado em perfis
3. **Auditoria**: Implementar log de alterações
4. **Cache**: Adicionar cache para melhor performance
5. **Exportação**: Implementar exportação para Excel

## Exemplo de Uso

### Listagem de Tipos Ativos

csharp
@inject ITiposProjetosService TiposProjetosService

var tiposAtivos = await TiposProjetosService.ListarAtivosAsync();


### Criação de Novo Tipo

csharp
var novoTipo = new TipoProjeto
{
    Nome = "Desenvolvimento Web",
    ATV = 1
};

var sucesso = await TiposProjetosService.InserirAsync(novoTipo);


### Validação de Duplicata

csharp
var existe = await TiposProjetosService.ExisteNomeAsync("Nome do Tipo");
if (existe)
{
    // Tratar duplicata
}


Esta migração estabelece uma base sólida para futuras funcionalidades relacionadas a projetos, seguindo os padrões arquiteturais da aplicação e proporcionando uma experiência moderna aos usuários.
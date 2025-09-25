# Documentação: Sistema de Autoavaliação

## Visão Geral

O sistema de autoavaliação é uma funcionalidade central da plataforma Peers que permite aos associados realizarem suas próprias avaliações de desempenho e competências. O sistema foi migrado de Web Forms para Blazor Híbrido .NET 9, mantendo toda a funcionalidade original enquanto oferece melhor performance e experiência do usuário.

## Arquitetura

### Estrutura de Pastas


Services/
├── Avaliacoes/
│   ├── AutoAvaliacaoService.cs
│   ├── EnvioAvaliacoesService.cs
│   ├── EvolucaoAssociadoService.cs
│   └── Common/
│       ├── IAutoAvaliacaoService.cs
│       ├── IEnvioAvaliacoesService.cs
│       ├── IEvolucaoAssociadoService.cs
│       ├── EmailUtils.cs
│       ├── WorkflowUtils.cs
│       └── ComboHelperAvaliacao.cs
├── Common/
│   ├── UserContextService.cs
│   ├── MessageBoxService.cs
│   ├── ComboHelper.cs
│   └── TelemetryService.cs
└── Associados/
    └── AssociadosService.cs


### Componentes Principais

#### 1. AutoAvaliacaoService

**Localização:** `Services/Avaliacoes/AutoAvaliacaoService.cs`

**Responsabilidades:**
- Carregar combos de projetos, clientes, períodos e status
- Filtrar avaliações baseado nos critérios selecionados
- Processar finalização de avaliações
- Gerenciar workflow de avaliação

**Dependências:**
- `IAssociadosService`: Para obter dados de associados
- `IUserContextService`: Para contexto do usuário logado
- `ITelemetryService`: Para logging e métricas
- `IMessageBoxService`: Para exibição de mensagens

#### 2. Serviços Reutilizáveis (Common)

**ComboHelper:** Utilitários para geração de combos padronizados
**EmailUtils:** Utilitários para processamento de emails de avaliação
**WorkflowUtils:** Utilitários para gerenciamento de fluxo de avaliação

### Modelos de Dados

#### ProjetoModel
csharp
public class ProjetoModel
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string DataInicio { get; set; }
    public string DataTermino { get; set; }
    public Associado Gestor { get; set; }
    public Associado Responsavel { get; set; }
    public Cliente Cliente { get; set; }
    public ProjetoStatus Status { get; set; }
    public List<ProjetosAssociadosModel> Associados { get; set; }
}


#### ProjetosAssociadosModel
csharp
public class ProjetosAssociadosModel
{
    public int Id { get; set; }
    public Projeto Projeto { get; set; }
    public Associado Associado { get; set; }
    public Associado Gestor { get; set; }
    public Associado Avaliador { get; set; }
    public string DataInicio { get; set; }
    public string DataTermino { get; set; }
    public string TipoAvaliacao { get; set; }
    public string Escopo { get; set; }
    public string Status { get; set; }
    public string Etapa { get; set; }
    public string RotuloBotao { get; set; }
    public bool ExibirBotaoFinalizar { get; set; }
    public bool AvaliacaoLiberada { get; set; }
}


## Integração dos Serviços

### Injeção de Dependência

Todos os serviços são registrados no `Program.cs`:

csharp
// AutoAvaliacao Services
builder.Services.AddScoped<IAutoAvaliacaoService, AutoAvaliacaoService>();

// AI Services for AutoAvaliacao
builder.Services.AddScoped<IAvaliacaoIAService, AvaliacaoIAService>();

// Avaliacoes Services
builder.Services.AddScoped<IEnvioAvaliacoesService, EnvioAvaliacoesService>();
builder.Services.AddScoped<IEvolucaoAssociadoService, EvolucaoAssociadoService>();

// Avaliacoes Common Services
builder.Services.AddScoped<IEmailUtils, EmailUtils>();
builder.Services.AddScoped<IWorkflowUtils, WorkflowUtils>();


### Configurações

As configurações específicas da autoavaliação estão centralizadas no `appsettings.json`:


"AutoAvaliacao": {
  "MaxExportRecords": 50000,
  "MaxImportRecords": 10000,
  "DefaultTipoAvaliacao": "desempenho",
  "DefaultEscopo": "projeto",
  "EnableFinalizacaoAutomatica": true,
  "RequireAllCompetenciasPreenchidas": true,
  "RequireAllPerformancesPreenchidas": true,
  "ValidationMessages": {
    "CompetenciasNaoPreenchidas": "É obrigatório digitar todas as notas da Avaliação Competência antes de finalizar a Avaliação",
    "PerformancesNaoPreenchidas": "É obrigatório digitar todas as notas da Avaliação Performance antes de finalizar a Avaliação"
  },
  "EtapasAvaliacao": {
    "NaoIniciada": "Não Iniciada",
    "AutoAvaliacao": "Em Auto-avaliação",
    "AvaliacaoAsCegas": "Em Av. às Cegas",
    "AvaliacaoGestor": "Em Av. Gestor",
    "Feedback": "Em Feedback",
    "AvaliacaoMentor": "Em Cons. Mentor",
    "Finalizada": "Finalizada"
  }
}


## Fluxo de Funcionamento

### Processo de Autoavaliação

mermaid
flowchart TD
    A[Usuário acessa Autoavaliação] --> B[UserContextService obtém usuário logado]
    B --> C[AutoAvaliacaoService carrega combos]
    C --> D[Exibe filtros: Projeto, Cliente, Período, Status]
    D --> E[Usuário seleciona filtros]
    E --> F[Clica em Filtrar]
    F --> G[AutoAvaliacaoService busca avaliações]
    G --> H{Avaliações encontradas?}
    H -->|Sim| I[Exibe lista de projetos e associados]
    H -->|Não| J[Exibe mensagem: Nenhuma avaliação encontrada]
    I --> K[Usuário visualiza avaliações]
    K --> L{Ação do usuário}
    L -->|Iniciar/Continuar| M[Redireciona para tela de competências]
    L -->|Ver Avaliação| N[Exibe avaliação em modo leitura]
    L -->|Finalizar| O[Valida preenchimento]
    O --> P{Validação OK?}
    P -->|Não| Q[Exibe mensagem de erro]
    P -->|Sim| R[Finaliza avaliação]
    R --> S[Avança etapa do workflow]
    S --> T[Redireciona para comentários]
    T --> U[Retorna para lista atualizada]
    Q --> K
    J --> D
    M --> V[Tela de Competências]
    N --> K
    U --> K


### Fluxo de Dados

mermaid
flowchart LR
    A[Blazor Component] --> B[AutoAvaliacaoService]
    B --> C[AssociadosService]
    B --> D[ProjetosService]
    B --> E[ClientesService]
    B --> F[PeriodoService]
    B --> G[StatusService]
    B --> H[AvaliacoesService]
    
    C --> I[Database]
    D --> I
    E --> I
    F --> I
    G --> I
    H --> I
    
    B --> J[UserContextService]
    B --> K[MessageBoxService]
    B --> L[TelemetryService]
    
    J --> M[Session]
    K --> N[UI Messages]
    L --> O[Application Insights]


### Estados da Avaliação

mermaid
stateDiagram-v2
    [*] --> NaoIniciada
    NaoIniciada --> AutoAvaliacao: Iniciar
    AutoAvaliacao --> AvaliacaoAsCegas: Finalizar Auto
    AvaliacaoAsCegas --> AvaliacaoGestor: Finalizar Cegas
    AvaliacaoGestor --> Feedback: Finalizar Gestor
    Feedback --> AvaliacaoMentor: Finalizar Feedback
    AvaliacaoMentor --> Finalizada: Finalizar Mentor
    Finalizada --> [*]
    
    AutoAvaliacao --> AutoAvaliacao: Salvar Parcial
    AvaliacaoAsCegas --> AvaliacaoAsCegas: Salvar Parcial


## Principais Funcionalidades

### 1. Carregamento de Combos
- **Projetos:** Lista projetos ativos do usuário logado
- **Clientes:** Lista todos os clientes ativos
- **Períodos:** Lista períodos de avaliação da empresa
- **Status:** Lista status de avaliação disponíveis

### 2. Filtragem de Avaliações
- Filtro por projeto específico ou todos
- Filtro por cliente específico ou todos
- Filtro por período específico ou todos
- Filtro por status específico ou todos

### 3. Exibição de Resultados
- Lista hierárquica: Projetos > Associados
- Informações do projeto: código, nome, cliente, status, datas, responsável
- Informações do associado: foto, nome, cargo, período, gestor, avaliador, mentor, etapa
- Botões de ação contextuais baseados no status da avaliação

### 4. Ações Disponíveis
- **Iniciar Avaliação:** Para avaliações não iniciadas
- **Continuar Avaliação:** Para avaliações em andamento
- **Ver Avaliação:** Para avaliações concluídas
- **Finalizar Avaliação:** Para avaliações preenchidas mas não finalizadas

### 5. Validações
- Verificação de preenchimento de todas as competências
- Verificação de preenchimento de todas as performances
- Validação de existência de competências parametrizadas
- Validação de existência de performances parametrizadas

## Tratamento de Erros

O sistema implementa tratamento robusto de erros:

1. **Validações de Negócio:** Mensagens específicas para cada tipo de erro
2. **Logging:** Todos os erros são registrados via TelemetryService
3. **Fallbacks:** Valores padrão para situações de erro
4. **Mensagens de Usuário:** Feedback claro através do MessageBoxService

## Performance e Otimizações

### Caching
- Combos são carregados sob demanda
- Contexto do usuário é mantido em sessão
- Configurações são carregadas uma vez na inicialização

### Lazy Loading
- Avaliações são carregadas apenas quando filtros são aplicados
- Dados de associados são carregados conforme necessário

### Telemetria
- Métricas de uso são coletadas via Application Insights
- Eventos de negócio são rastreados para análise
- Exceções são automaticamente reportadas

## Segurança

### Autorização
- Usuários só veem projetos aos quais estão associados
- Validação de permissões em cada operação
- Contexto de usuário validado a cada requisição

### Validação de Dados
- Sanitização de inputs
- Validação de tipos e formatos
- Proteção contra SQL injection via Entity Framework

## Extensibilidade

### Integração com IA
O sistema está preparado para futuras integrações com IA:
- Interface `IAvaliacaoIAService` já definida
- Configuração para habilitação de recursos de IA
- Estrutura para sugestões inteligentes de avaliação

### Novos Tipos de Avaliação
- Arquitetura flexível para novos tipos além de "desempenho"
- Configuração via appsettings.json
- Extensão fácil de validações e regras de negócio

## Monitoramento

### Métricas Coletadas
- Tempo de carregamento de combos
- Número de avaliações por usuário
- Taxa de finalização de avaliações
- Erros e exceções

### Dashboards
- Application Insights para métricas técnicas
- Logs estruturados para análise de negócio
- Alertas automáticos para erros críticos

## Migração e Compatibilidade

### Compatibilidade com Sistema Legado
- Mantém mesma estrutura de dados
- Preserva URLs e parâmetros existentes
- Suporte a redirecionamentos do sistema antigo

### Processo de Migração
1. Implementação dos serviços mantendo compatibilidade
2. Criação do componente Blazor com mesma funcionalidade
3. Testes de regressão para garantir paridade
4. Migração gradual com rollback disponível

## Conclusão

O sistema de autoavaliação migrado oferece:
- **Melhor Performance:** Blazor Híbrido com renderização otimizada
- **Manutenibilidade:** Separação clara de responsabilidades
- **Reutilização:** Serviços compartilhados entre módulos
- **Extensibilidade:** Arquitetura preparada para futuras evoluções
- **Monitoramento:** Telemetria completa para análise e otimização

A arquitetura modular e o uso de padrões estabelecidos garantem que o sistema seja robusto, escalável e fácil de manter, atendendo às necessidades atuais e futuras da plataforma Peers.
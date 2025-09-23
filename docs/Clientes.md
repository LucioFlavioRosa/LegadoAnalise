# Documentação - Módulo de Clientes

## Visão Geral

O módulo de Clientes foi migrado de Web Forms para Blazor Server com renderização híbrida (InteractiveAuto), seguindo os princípios de arquitetura limpa e injeção de dependência. O módulo permite o cadastro, edição, listagem e inativação de clientes do sistema.

## Arquitetura

### Estrutura de Pastas


Peers.Moderno/
├── Components/
│   ├── Pages/
│   │   ├── Clientes.razor              # Página principal
│   │   └── Clientes.razor.cs           # Code-behind da página
│   └── Shared/
│       ├── ClienteForm.razor           # Componente de formulário
│       ├── ClienteForm.razor.cs        # Code-behind do formulário
│       ├── ClienteList.razor           # Componente de listagem
│       └── ClienteList.razor.cs        # Code-behind da listagem
├── Services/
│   └── Clientes/
│       ├── ClientesService.cs          # Serviço principal
│       └── Common/
│           ├── IClientesService.cs     # Interface do serviço
│           └── ClientesDto.cs          # DTOs de transferência
├── Models/
│   └── Cliente.cs                      # Entidade do banco
└── docs/
    └── Clientes.md                     # Esta documentação


### Componentes

#### 1. Clientes.razor (Página Principal)
- **Responsabilidade**: Orquestrar a interação entre os componentes filhos
- **Renderização**: InteractiveAuto (híbrida)
- **Funcionalidades**:
  - Coordenar operações CRUD
  - Gerenciar estado da aplicação
  - Controlar navegação e feedback

#### 2. ClienteForm.razor (Formulário Reutilizável)
- **Responsabilidade**: Capturar dados do cliente
- **Funcionalidades**:
  - Validação de formulário
  - Modo inserção/edição
  - Carregamento de dropdowns (sócios)
  - Feedback visual (loading states)

#### 3. ClienteList.razor (Lista Reutilizável)
- **Responsabilidade**: Exibir lista de clientes
- **Funcionalidades**:
  - Listagem paginada
  - Ações de alteração e inativação
  - Estados visuais (ativo/inativo)
  - Loading states

### Serviços

#### ClientesService
- **Interface**: IClientesService
- **Responsabilidades**:
  - Operações CRUD com banco de dados
  - Validações de negócio
  - Integração com MessageBoxService
  - Tratamento de exceções

#### Métodos Principais:
- `ObterListaClientesAsync()`: Lista todos os clientes
- `ObterClienteAsync(int id)`: Busca cliente específico
- `InserirClienteAsync(ClienteDto)`: Insere novo cliente
- `AlterarClienteAsync(ClienteDto)`: Atualiza cliente existente
- `ExcluirClienteAsync(int id)`: Inativa cliente
- `ObterSociosAsync()`: Lista sócios para dropdown
- `ValidarClienteAsync(ClienteDto)`: Validações de negócio

### DTOs (Data Transfer Objects)

#### ClienteDto
- Representa um cliente completo com todas as propriedades
- Inclui propriedades calculadas (StatusTexto)
- Usado para transferência entre camadas

#### ClienteFormDto
- DTO específico para o formulário
- Campos simplificados para entrada de dados
- Mapeamento automático para ClienteDto

#### SocioDto
- Representa sócios para dropdown
- Propriedades mínimas (Id, Nome)

## Integração com Outros Módulos

### MessageBoxService
- Utilizado para feedback ao usuário
- Mensagens de sucesso, erro, aviso e informação
- Centralizado em Services/Common

### ApplicationDbContext
- Entity Framework Core para acesso a dados
- Configuração da entidade Cliente
- Relacionamento com Associados (sócios)

### Injeção de Dependência
- Todos os serviços registrados no Program.cs
- Interfaces para facilitar testes e manutenção
- Escopo Scoped para serviços de negócio

## Fluxo de Alto Nível

mermaid
flowchart TD
    A[Usuário acessa /clientes] --> B[Clientes.razor]
    B --> C[Carrega ClienteForm]
    B --> D[Carrega ClienteList]
    
    C --> E[ClientesService.ObterSociosAsync]
    D --> F[ClientesService.ObterListaClientesAsync]
    
    E --> G[ApplicationDbContext]
    F --> G
    
    G --> H[(Banco de Dados)]
    
    I[Usuário preenche formulário] --> J[ClienteForm.OnSubmitAsync]
    J --> K{Novo ou Edição?}
    
    K -->|Novo| L[ClientesService.InserirClienteAsync]
    K -->|Edição| M[ClientesService.AlterarClienteAsync]
    
    L --> N[Validações]
    M --> N
    
    N -->|Válido| O[Salva no Banco]
    N -->|Inválido| P[MessageBoxService.ShowWarning]
    
    O --> Q[MessageBoxService.ShowSuccess]
    Q --> R[Atualiza Lista]
    P --> S[Exibe Mensagem]
    
    T[Usuário clica Alterar] --> U[ClienteList.OnAlterar]
    U --> V[Carrega dados no formulário]
    
    W[Usuário clica Inativar] --> X[Confirmação JavaScript]
    X -->|Sim| Y[ClientesService.ExcluirClienteAsync]
    X -->|Não| Z[Cancela operação]
    
    Y --> AA[Atualiza status no banco]
    AA --> BB[Atualiza lista]


## Principais Melhorias da Migração

### 1. Separação de Responsabilidades
- Lógica de negócio extraída para serviços
- UI dividida em componentes reutilizáveis
- DTOs para transferência de dados

### 2. Reutilização de Código
- Componentes podem ser usados em outras telas
- Serviços injetáveis e testáveis
- Interface para abstração

### 3. Performance
- Renderização híbrida (Server + WebAssembly)
- Loading states para melhor UX
- Operações assíncronas

### 4. Manutenibilidade
- Código mais limpo e organizado
- Fácil adição de novos recursos
- Testes unitários facilitados

### 5. Experiência do Usuário
- Interface mais responsiva
- Feedback visual aprimorado
- Confirmações de ações críticas

## Configuração e Deploy

### Dependências
- .NET 9.0
- Entity Framework Core
- Blazor Server
- SQL Server

### Configuração
- String de conexão em appsettings.json
- Serviços registrados em Program.cs
- Migrações do Entity Framework

### Segurança
- Validações server-side
- Sanitização de inputs
- Confirmações para ações críticas

## Próximos Passos

1. **Testes Automatizados**: Implementar testes unitários e de integração
2. **Auditoria**: Adicionar logs de auditoria para operações CRUD
3. **Filtros Avançados**: Implementar busca e filtros na listagem
4. **Exportação**: Adicionar funcionalidade de exportar para Excel
5. **Validações Avançadas**: Implementar validações customizadas
6. **Cache**: Implementar cache para melhorar performance
7. **Paginação**: Adicionar paginação na listagem
8. **Ordenação**: Permitir ordenação por colunas

## Considerações Técnicas

### Performance
- Queries otimizadas com Include() apenas quando necessário
- Projeções com Select() para reduzir dados transferidos
- Operações assíncronas para não bloquear UI

### Escalabilidade
- Arquitetura preparada para crescimento
- Serviços desacoplados
- Fácil adição de novos recursos

### Manutenção
- Código bem documentado
- Padrões consistentes
- Separação clara de responsabilidades
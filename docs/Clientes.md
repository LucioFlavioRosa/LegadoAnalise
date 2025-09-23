# Módulo de Clientes - Documentação Técnica

## Visão Geral

O módulo de Clientes foi migrado de Web Forms para Blazor Híbrido (.NET 9), implementando uma arquitetura moderna baseada em componentes, serviços injetáveis e separação de responsabilidades.

## Arquitetura

### Componentes Principais

- **Components/Clientes/Clientes.razor**: Componente principal da UI
- **Components/Clientes/Clientes.razor.cs**: Code-behind com lógica de apresentação
- **Services/Clientes/ClienteService.cs**: Serviço de negócio para operações CRUD
- **Services/Clientes/Common/**: Serviços e DTOs reutilizáveis
- **Models/Cliente.cs**: Entidade de dados mapeada para Entity Framework

### Padrões Implementados

1. **Dependency Injection**: Todos os serviços são injetados via DI
2. **Repository Pattern**: Acesso a dados através do ApplicationDbContext
3. **DTO Pattern**: Transferência de dados entre camadas usando DTOs
4. **Service Layer**: Lógica de negócio centralizada em serviços
5. **Component-Based UI**: Interface moderna com Blazor

## Funcionalidades

### Operações CRUD

- **Criar**: Inserção de novos clientes com validação
- **Ler**: Listagem e consulta de clientes
- **Atualizar**: Edição de dados existentes
- **Deletar**: Inativação lógica (soft delete)

### Validações

- Nome do cliente (obrigatório, máx. 200 caracteres)
- E-mail (obrigatório, formato válido, máx. 100 caracteres)
- Telefone (obrigatório, máx. 20 caracteres)
- Gestor do cliente (obrigatório, máx. 100 caracteres)
- Sócio responsável (obrigatório, deve existir)

### Recursos Adicionais

- Feedback visual com mensagens de sucesso/erro
- Confirmação para operações de exclusão
- Loading states durante processamento
- Formulário responsivo com Bootstrap

## Integração

### Dependências

csharp
// Serviços injetados no componente
[Inject] private IClienteService ClienteService { get; set; }
[Inject] private IDropdownService DropdownService { get; set; }
[Inject] private IMessageBoxService MessageBoxService { get; set; }
[Inject] private IJSRuntime JSRuntime { get; set; }


### Configuração no Program.cs

csharp
// Registro dos serviços
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<Services.Clientes.Common.IDropdownService, Services.Clientes.Common.DropdownService>();


### Roteamento

O componente é acessível através da rota `/clientes` definida na diretiva `@page`.

## Fluxo de Alto Nível

mermaid
flowchart TD
    A[Usuário acessa /clientes] --> B[Clientes.razor]
    B --> C[OnInitializedAsync]
    C --> D[CarregarDados]
    D --> E[ClienteService.ObterClientesAsync]
    D --> F[DropdownService.ObterSociosAsync]
    E --> G[ApplicationDbContext]
    F --> G
    G --> H[Dados carregados]
    H --> I[UI renderizada]
    
    J[Usuário preenche formulário] --> K[CadastrarCliente]
    K --> L{Cliente.Id > 0?}
    L -->|Sim| M[ClienteService.AlterarClienteAsync]
    L -->|Não| N[ClienteService.InserirClienteAsync]
    M --> O[ApplicationDbContext.SaveChanges]
    N --> O
    O --> P[MessageBoxService.ExibirMensagem]
    P --> Q[CarregarClientes]
    Q --> R[UI atualizada]
    
    S[Usuário clica Inativar] --> T[Confirmação JS]
    T --> U[ClienteService.ExcluirClienteAsync]
    U --> V[Soft Delete no banco]
    V --> W[Feedback e atualização]


## Estrutura de Dados

### Entidade Cliente

csharp
public class Cliente
{
    public int IdCliente { get; set; }
    public string Cliente1 { get; set; } // Nome do cliente
    public string Email { get; set; }
    public string Telefones { get; set; }
    public string GestorCliente { get; set; }
    public int IdAssociadoResponsavel { get; set; }
    public int ATV { get; set; } // Status ativo/inativo
    public DateTime DHC { get; set; } // Data/hora criação
    public int USR { get; set; } // Usuário que criou
    public int IdEmpresa { get; set; }
}


### DTO ClienteDto

csharp
public class ClienteDto
{
    public int Id { get; set; }
    public string Cliente { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string GestorCliente { get; set; }
    public int IdAssociadoResponsavel { get; set; }
    public bool Ativo { get; set; }
}


## Melhorias Futuras

1. **Paginação**: Implementar paginação para grandes volumes de dados
2. **Filtros**: Adicionar filtros avançados na listagem
3. **Exportação**: Funcionalidade para exportar dados para Excel/PDF
4. **Auditoria**: Log detalhado de alterações
5. **Validações Avançadas**: Validação de CNPJ, telefone com máscara
6. **Upload de Arquivos**: Anexar documentos aos clientes
7. **Histórico**: Rastreamento de mudanças ao longo do tempo

## Considerações de Performance

- Uso de `async/await` em todas as operações de banco
- Carregamento assíncrono de dados na inicialização
- Queries otimizadas com projeções (Select)
- Soft delete para manter integridade referencial
- Renderização híbrida para melhor performance inicial

## Testes

Para implementação futura:
- Testes unitários dos serviços
- Testes de integração com banco de dados
- Testes de componente Blazor
- Testes de validação de formulário

## Manutenção

- Logs estruturados com Application Insights
- Tratamento de exceções centralizado
- Validações tanto no cliente quanto no servidor
- Documentação atualizada a cada mudança significativa
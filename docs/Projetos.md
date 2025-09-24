# Documentação - Sistema de Gestão de Projetos

## Visão Geral

O sistema de gestão de projetos é responsável pelo cadastro, edição, listagem e gerenciamento de projetos e suas alocações de associados. Este módulo faz parte da migração do sistema legado Web Forms para Blazor Server (.NET 9) seguindo padrões modernos de arquitetura.

## Arquitetura

### Estrutura de Pastas


Services/
  Projetos/
    ProjetosService.cs              # Serviço principal de negócio
    Common/
      ProjetosComboHelper.cs        # Utilitários para combos
      ProjetosValidationHelper.cs   # Validações específicas
      ProjetosExportHelper.cs       # Exportação de dados
  Common/
    MessageBoxService.cs            # Mensagens (reutilizado)
    TelemetryService.cs            # Telemetria (reutilizado)
    UserContextService.cs          # Contexto do usuário (reutilizado)
    ComboHelper.cs                 # Utilitários genéricos de combo (expandido)

Components/
  Projetos/
    ProjetoForm.razor              # Formulário de cadastro/edição
    ProjetosList.razor             # Listagem de projetos
    AssociadosProjeto.razor        # Gestão de associados no projeto
    Modals/
      ResponsavelModal.razor       # Modal para cadastro de responsável
      GestorModal.razor           # Modal para cadastro de gestor
      AssociadosModal.razor       # Modal para adicionar associados

Models/
  Projeto.cs                     # Modelo de domínio do projeto
  AssociadoProjeto.cs           # Modelo de alocação
  TipoProjeto.cs                # Tipos de projeto
  ProjetoComplexidade.cs        # Complexidades


### Serviços Reutilizados

#### MessageBoxService
- **Localização**: `Services/Common/MessageBoxService.cs`
- **Propósito**: Centralizar exibição de mensagens de sucesso, erro, aviso e informação
- **Uso**: Injetado nos serviços de negócio para feedback ao usuário

csharp
// Exemplo de uso
public class ProjetosService
{
    private readonly IMessageBoxService _messageBoxService;
    
    public async Task<bool> InserirProjetoAsync(Projeto projeto)
    {
        try
        {
            // Lógica de inserção
            _messageBoxService.ShowSuccess("Projeto cadastrado com sucesso");
            return true;
        }
        catch (Exception ex)
        {
            _messageBoxService.ShowError("Erro ao cadastrar projeto");
            return false;
        }
    }
}


#### TelemetryService
- **Localização**: `Services/Common/TelemetryService.cs`
- **Propósito**: Rastreamento de eventos, exceções e métricas para observabilidade
- **Uso**: Monitoramento de operações críticas e debugging

csharp
// Exemplo de uso
public async Task<List<Projeto>> ObterProjetosAsync()
{
    var startTime = DateTimeOffset.UtcNow;
    try
    {
        var projetos = await _context.Projetos.ToListAsync();
        
        _telemetryService.TrackEvent("ProjetosListLoaded", new Dictionary<string, string>
        {
            { "Count", projetos.Count.ToString() }
        });
        
        return projetos;
    }
    catch (Exception ex)
    {
        _telemetryService.TrackException(ex, new Dictionary<string, string>
        {
            { "Method", "ObterProjetosAsync" },
            { "Component", "ProjetosService" }
        });
        throw;
    }
    finally
    {
        var duration = DateTimeOffset.UtcNow - startTime;
        _telemetryService.TrackDependency("Database", "ObterProjetos", "SELECT", startTime, duration, true);
    }
}


#### UserContextService
- **Localização**: `Services/Common/UserContextService.cs`
- **Propósito**: Gerenciar contexto do usuário logado, permissões e sessão
- **Uso**: Controle de acesso e auditoria

csharp
// Exemplo de uso
public async Task<bool> PodeEditarProjetoAsync(int projetoId)
{
    var usuario = await _userContextService.GetUsuarioLogadoAsync();
    if (usuario == null) return false;
    
    // Verificar permissões baseadas no perfil
    if (usuario.IdPerfil < 3) return false;
    
    // Verificar se é responsável ou gestor do projeto
    var projeto = await ObterProjetoAsync(projetoId);
    return projeto?.IdAssociadoResponsavel == usuario.Id || 
           projeto?.IdAssociadoGestor == usuario.Id;
}


#### ComboHelper (Expandido)
- **Localização**: `Services/Common/ComboHelper.cs`
- **Propósito**: Utilitários genéricos para criação e manipulação de combos
- **Expansões**: Métodos específicos para projetos e criação dinâmica de combos

csharp
// Novos métodos adicionados
var statusItems = ComboHelper.GetStatusProjetoItems();
var comboFromList = ComboHelper.CreateComboFromList(
    clientes, 
    c => c.IdCliente.ToString(), 
    c => c.Nome
);


## Configurações

### appsettings.json - Seção Projetos


{
  "Projetos": {
    "MaxProjetoLength": 500,
    "MaxCodigoLength": 100,
    "EnableValidation": true,
    "RequireDataInicio": true,
    "RequireDataTermino": true,
    "MaxAssociadosPorProjeto": 50,
    "ValidationMessages": {
      "CodigoObrigatorio": "Código do projeto é obrigatório",
      "ProjetoObrigatorio": "Nome do projeto é obrigatório",
      "SucessoInsercao": "Projeto cadastrado com sucesso"
    },
    "Features": {
      "EnableAuditLog": true,
      "EnableNotifications": true
    }
  }
}


## Fluxo de Alto Nível

mermaid
flowchart TD
    Start([Usuário Acessa Sistema]) --> Auth{Usuário Autenticado?}
    Auth -->|Não| Login[Página de Login]
    Auth -->|Sim| Context[UserContextService: Obter Contexto]
    
    Context --> Permission{Tem Permissão?}
    Permission -->|Não| AccessDenied[Acesso Negado]
    Permission -->|Sim| MainPage[Página Principal de Projetos]
    
    MainPage --> Action{Ação do Usuário}
    
    Action -->|Listar| ListFlow[Fluxo de Listagem]
    Action -->|Cadastrar| CreateFlow[Fluxo de Cadastro]
    Action -->|Editar| EditFlow[Fluxo de Edição]
    Action -->|Gerenciar Associados| AssocFlow[Fluxo de Associados]
    
    ListFlow --> ProjetosService1[ProjetosService: ObterProjetosAsync]
    ProjetosService1 --> DB1[(ApplicationDbContext)]
    DB1 --> Telemetry1[TelemetryService: Track Event]
    Telemetry1 --> ProjetosList[ProjetosList.razor]
    
    CreateFlow --> ProjetoForm1[ProjetoForm.razor]
    ProjetoForm1 --> ComboHelper1[ComboHelper: Popular Combos]
    ComboHelper1 --> Validation1{Validação OK?}
    Validation1 -->|Não| MessageBox1[MessageBoxService: Show Error]
    Validation1 -->|Sim| ProjetosService2[ProjetosService: InserirProjetoAsync]
    ProjetosService2 --> DB2[(ApplicationDbContext)]
    DB2 --> MessageBox2[MessageBoxService: Show Success]
    MessageBox2 --> Telemetry2[TelemetryService: Track Success]
    
    EditFlow --> LoadProject[Carregar Projeto Existente]
    LoadProject --> ProjetoForm2[ProjetoForm.razor: Modo Edição]
    ProjetoForm2 --> Validation2{Validação OK?}
    Validation2 -->|Não| MessageBox3[MessageBoxService: Show Error]
    Validation2 -->|Sim| ProjetosService3[ProjetosService: AtualizarProjetoAsync]
    ProjetosService3 --> DB3[(ApplicationDbContext)]
    DB3 --> MessageBox4[MessageBoxService: Show Success]
    
    AssocFlow --> AssociadosModal[AssociadosModal.razor]
    AssociadosModal --> ProjetosService4[ProjetosService: GerenciarAssociadosAsync]
    ProjetosService4 --> DB4[(ApplicationDbContext)]
    DB4 --> AssociadosProjeto[AssociadosProjeto.razor: Atualizar Lista]
    
    MessageBox1 --> MainPage
    MessageBox2 --> MainPage
    MessageBox3 --> MainPage
    MessageBox4 --> MainPage
    AssociadosProjeto --> MainPage
    ProjetosList --> MainPage
    
    Login --> Auth
    AccessDenied --> End([Fim])
    MainPage --> End


## Padrões de Implementação

### 1. Injeção de Dependência

Todos os serviços devem ser registrados no `Program.cs`:

csharp
// Projetos Services
builder.Services.AddScoped<IProjetosService, ProjetosService>();
builder.Services.AddScoped<IProjetosComboHelper, ProjetosComboHelper>();

// Reutilização de serviços comuns
builder.Services.AddScoped<IMessageBoxService, MessageBoxService>();
builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();


### 2. Tratamento de Erros

csharp
public async Task<OperationResult<Projeto>> InserirProjetoAsync(Projeto projeto)
{
    try
    {
        // Validações
        var validationResult = await ValidarProjetoAsync(projeto);
        if (!validationResult.IsValid)
        {
            _messageBoxService.ShowError(validationResult.ErrorMessage);
            return OperationResult<Projeto>.Error(validationResult.ErrorMessage);
        }
        
        // Inserção
        _context.Projetos.Add(projeto);
        await _context.SaveChangesAsync();
        
        _messageBoxService.ShowSuccess("Projeto cadastrado com sucesso");
        _telemetryService.TrackEvent("ProjetoInserido", new Dictionary<string, string>
        {
            { "ProjetoId", projeto.Id.ToString() },
            { "UserId", _userContext.GetUsuarioLogado()?.Id.ToString() ?? "Unknown" }
        });
        
        return OperationResult<Projeto>.Success(projeto);
    }
    catch (Exception ex)
    {
        _telemetryService.TrackException(ex);
        _messageBoxService.ShowError("Erro interno ao cadastrar projeto");
        return OperationResult<Projeto>.Error("Erro interno");
    }
}


### 3. Validações

csharp
public async Task<ValidationResult> ValidarProjetoAsync(Projeto projeto)
{
    var config = _configuration.GetSection("Projetos");
    var messages = config.GetSection("ValidationMessages");
    
    if (string.IsNullOrEmpty(projeto.Codigo))
        return ValidationResult.Error(messages["CodigoObrigatorio"]);
        
    if (string.IsNullOrEmpty(projeto.Nome))
        return ValidationResult.Error(messages["ProjetoObrigatorio"]);
        
    if (projeto.DataInicio > projeto.DataFim)
        return ValidationResult.Error(messages["DataInicioMaiorQueTermino"]);
        
    return ValidationResult.Success();
}


## Componentes Blazor

### ProjetoForm.razor

razor
@page "/projetos/novo"
@page "/projetos/editar/{id:int}"
@inject IProjetosService ProjetosService
@inject IProjetosComboHelper ComboHelper
@inject IMessageBoxService MessageBox
@rendermode InteractiveServer

<EditForm Model="@projeto" OnValidSubmit="@SalvarProjeto">
    <DataAnnotationsValidator />
    <ValidationSummary />
    
    <!-- Campos do formulário -->
    <InputText @bind-Value="projeto.Codigo" class="form-control" />
    <InputText @bind-Value="projeto.Nome" class="form-control" />
    
    <!-- Combos populados via ComboHelper -->
    <InputSelect @bind-Value="projeto.IdCliente" class="form-control">
        @foreach (var item in clientesCombo)
        {
            <option value="@item.Value">@item.Text</option>
        }
    </InputSelect>
    
    <button type="submit" class="btn btn-primary">Salvar</button>
</EditForm>

@code {
    [Parameter] public int? Id { get; set; }
    
    private Projeto projeto = new();
    private List<ComboItem> clientesCombo = new();
    
    protected override async Task OnInitializedAsync()
    {
        clientesCombo = await ComboHelper.GetClientesComboAsync();
        
        if (Id.HasValue)
        {
            projeto = await ProjetosService.ObterProjetoAsync(Id.Value) ?? new();
        }
    }
    
    private async Task SalvarProjeto()
    {
        if (Id.HasValue)
        {
            await ProjetosService.AtualizarProjetoAsync(projeto);
        }
        else
        {
            await ProjetosService.InserirProjetoAsync(projeto);
        }
    }
}


## Integração com Sistema Legado

### Migração Incremental

1. **Fase 1**: Manter Web Forms funcionando enquanto desenvolve Blazor
2. **Fase 2**: Redirecionar URLs específicas para componentes Blazor
3. **Fase 3**: Migração completa e remoção do código legado

### Compatibilidade de Dados

- Entity Framework Core mapeia para as mesmas tabelas do sistema legado
- Nomes de colunas mantidos para compatibilidade
- Triggers e procedures existentes continuam funcionando

## Monitoramento e Observabilidade

### Application Insights

- Eventos customizados para operações de negócio
- Rastreamento de exceções com contexto
- Métricas de performance de consultas
- Dashboards específicos para módulo de projetos

### Logs Estruturados


{
  "Logging": {
    "LogLevel": {
      "Peers.Moderno.Services.Projetos": "Debug"
    }
  }
}


## Testes

### Testes Unitários

- Serviços de negócio isolados com mocks
- Validações de regras de negócio
- Utilitários e helpers

### Testes de Integração

- Fluxos completos de cadastro/edição
- Integração com banco de dados
- Testes de permissões e segurança

## Considerações de Performance

### Otimizações

- Lazy loading para listas grandes
- Paginação server-side
- Cache de combos frequentemente acessados
- Queries otimizadas com Entity Framework

### Escalabilidade

- Serviços stateless para suporte a múltiplas instâncias
- Connection pooling configurado
- Async/await em todas as operações I/O

## Segurança

### Controle de Acesso

- Validação de permissões em todos os endpoints
- Auditoria de operações críticas
- Sanitização de inputs
- Proteção contra SQL Injection via Entity Framework

### Dados Sensíveis

- Configurações sensíveis em User Secrets/Azure Key Vault
- Logs sem informações pessoais
- Criptografia de dados em trânsito e repouso

## Roadmap

### Próximas Funcionalidades

- [ ] Workflow de aprovação de projetos
- [ ] Integração com sistemas externos
- [ ] Relatórios avançados
- [ ] API REST para integrações
- [ ] Mobile app com Blazor Hybrid

### Melhorias Técnicas

- [ ] Implementação de CQRS
- [ ] Event Sourcing para auditoria
- [ ] GraphQL endpoint
- [ ] Containerização com Docker
- [ ] CI/CD pipeline completo
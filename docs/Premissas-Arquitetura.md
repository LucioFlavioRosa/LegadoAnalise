# Arquitetura - Sistema de Premissas

## Diagrama de Alto Nível

```
mermaid
flowchart TD
    A[Usuário] --> B[Premissas.razor]
    B --> C[Premissas.razor.cs]
    C --> D[IPremissasService]
    D --> E[PremissasService]
    E --> F[ApplicationDbContext]
    F --> G[(SQL Server Database)]
    
    H[Program.cs] --> I[DI Container]
    I --> D
    I --> F
    
    J[Models] --> E
    J --> F
    
    K[appsettings.json] --> F
    
    subgraph "Presentation Layer"
        B
        C
    end
    
    subgraph "Business Layer"
        D
        E
    end
    
    subgraph "Data Layer"
        F
        J
    end
    
    subgraph "Infrastructure"
        G
        H
        I
        K
    end
    
    style A fill:#e1f5fe
    style B fill:#f3e5f5
    style C fill:#f3e5f5
    style D fill:#e8f5e8
    style E fill:#e8f5e8
    style F fill:#fff3e0
    style J fill:#fff3e0
    style G fill:#ffebee
    style H fill:#f1f8e9
    style I fill:#f1f8e9
    style K fill:#f1f8e9
```

## Fluxo de Operações
```
mermaid
sequenceDiagram
    participant U as Usuário
    participant UI as Premissas.razor
    participant CB as Code-Behind
    participant S as PremissasService
    participant DB as ApplicationDbContext
    participant SQL as SQL Server
    
    U->>UI: Acessa /premissas
    UI->>CB: OnInitializedAsync()
    CB->>S: ObterListaAsync()
    S->>DB: PremissasRadar.Include(...)
    DB->>SQL: SELECT com JOINs
    SQL-->>DB: Dados das premissas
    DB-->>S: List<PremissasRadar>
    S-->>CB: Lista de premissas
    CB->>UI: StateHasChanged()
    UI-->>U: Exibe tabela preenchida
    
    U->>UI: Preenche formulário
    U->>UI: Clica "Cadastrar"
    UI->>CB: SalvarPremissa()
    CB->>CB: ValidarCampos()
    CB->>S: InserirAsync(premissa)
    S->>DB: Add(premissa)
    DB->>SQL: INSERT INTO PREMISSAS_RADAR
    SQL-->>DB: Confirmação
    DB-->>S: SaveChanges() = true
    S-->>CB: true
    CB->>CB: ExibirMensagem("Sucesso")
    CB->>S: ObterListaAsync()
    S-->>CB: Lista atualizada
    CB->>UI: StateHasChanged()
    UI-->>U: Tabela atualizada + mensagem
```

## Componentes da Arquitetura

### Camada de Apresentação
- **Premissas.razor:** Interface do usuário com componentes Blazor
- **Premissas.razor.cs:** Lógica de apresentação e gerenciamento de estado

### Camada de Negócio
- **IPremissasService:** Contrato de serviço
- **PremissasService:** Implementação da lógica de negócio

### Camada de Dados
- **ApplicationDbContext:** Contexto do Entity Framework
- **Models:** Entidades de domínio (PremissasRadar, CargoNivel, etc.)

### Infraestrutura
- **Program.cs:** Configuração de DI e pipeline
- **appsettings.json:** Configurações da aplicação
- **SQL Server:** Banco de dados relacional

## Padrões Utilizados

1. **Dependency Injection:** Inversão de controle para baixo acoplamento
2. **Repository Pattern:** Implícito via Entity Framework DbContext
3. **Service Layer:** Encapsulamento da lógica de negócio
4. **Component-Based Architecture:** Componentes Blazor reutilizáveis
5. **Async/Await Pattern:** Operações assíncronas para melhor performance

## Benefícios da Arquitetura

- **Testabilidade:** Interfaces bem definidas facilitam testes unitários
- **Manutenibilidade:** Separação clara de responsabilidades
- **Escalabilidade:** Estrutura preparada para crescimento
- **Performance:** Renderização híbrida e operações assíncronas
- **Flexibilidade:** Fácil extensão e modificação de funcionalidades

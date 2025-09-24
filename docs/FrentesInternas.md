# Documentação - Módulo Frentes Internas

## Visão Geral

O módulo de Frentes Internas foi migrado do Web Forms para Blazor Híbrido (.NET 9), mantendo todas as funcionalidades originais com melhorias na arquitetura, performance e manutenibilidade.

## Arquitetura

### Componentes Blazor

O módulo é composto por componentes modulares que seguem o padrão de responsabilidade única:

- **FrentesInternas.razor**: Componente principal que orquestra todos os subcomponentes
- **EditorFrenteInterna.razor**: Gerencia criação e edição de frentes internas
- **LideresFrenteInterna.razor**: Gerencia líderes das frentes internas
- **ParticipantesFrenteInterna.razor**: Gerencia participantes das frentes internas
- **ListaFrentesInternas.razor**: Exibe listagem com ações de editar/inativar
- **AvaliarFrenteInterna.razor**: Interface para avaliação de associados
- **ExportarFrentesInternas.razor**: Funcionalidade de exportação para Excel

### Serviços

#### Serviços Principais
- **IFrentesInternasService**: Interface principal para operações de negócio
- **FrentesInternasService**: Implementação das regras de negócio

#### Serviços Auxiliares (Common)
- **IStatusHelper**: Helper para conversão de status (Ativo/Inativo)
- **IExportHelper**: Helper para exportação de dados para Excel

### Modelos de Dados

- **FrenteInternaModel**: Modelo principal da frente interna
- **LiderFrenteInternaModel**: Modelo para líderes
- **ParticipanteFrenteInternaModel**: Modelo para participantes

## Funcionalidades

### 1. Gerenciamento de Frentes Internas
- Criação de novas frentes internas
- Edição de frentes existentes
- Inativação de frentes
- Listagem com filtros

### 2. Gerenciamento de Líderes
- Adição de líderes às frentes
- Inativação de líderes
- Visualização de líderes ativos/inativos

### 3. Gerenciamento de Participantes
- Adição de participantes às frentes
- Inativação de participantes
- Visualização de participantes ativos/inativos

### 4. Avaliação
- Interface para avaliação de associados
- Controle de permissões baseado em liderança
- Redirecionamento para módulo de avaliação

### 5. Exportação
- Exportação de avaliações para Excel
- Controle de permissões baseado em perfil
- Download automático via JavaScript

## Integração com Serviços Comuns

### MessageBoxService
Todos os componentes utilizam o serviço centralizado de mensagens para:
- Mensagens de sucesso
- Mensagens de erro
- Mensagens de aviso
- Mensagens informativas

### TelemetryService
Integração com Application Insights para:
- Rastreamento de eventos
- Rastreamento de exceções
- Métricas de performance
- Análise de uso

## Configuração

### appsettings.json


"FrentesInternas": {
  "MaxFrenteInternaLength": 500,
  "MaxExportRecords": 50000,
  "MaxImportRecords": 10000,
  "MaxFileSizeMB": 10,
  "AllowedFileExtensions": [".xlsx", ".xls"],
  "ExportTempPath": "temp/exports",
  "ImportTempPath": "temp/imports",
  "EnableValidation": true,
  "EnableCompression": false,
  "CacheExpirationMinutes": 30,
  "DefaultStatus": 1,
  "EnableAutoRedirect": true,
  "MinPerfilEdicao": 3,
  "EnableLiderPermissions": true
}


### Injeção de Dependência

csharp
// FrentesInternas Services
builder.Services.AddScoped<IFrentesInternasService, FrentesInternasService>();

// FrentesInternas Common Services
builder.Services.AddScoped<IStatusHelper, StatusHelper>();
builder.Services.AddScoped<IExportHelper, ExportHelper>();


## Roteamento

- **URL Principal**: `/frentes-internas`
- **Avaliação**: `/avaliacao-frentes-internas`

## Permissões

### Edição
- Usuários com perfil >= 3 podem criar/editar frentes internas
- Usuários com perfil < 3 têm acesso somente leitura

### Avaliação
- Apenas líderes de frentes internas podem avaliar
- Verificação automática de liderança

### Exportação
- Apenas usuários com perfil >= 3 podem exportar
- Controle de permissões no componente

## Fluxo de Alto Nível

mermaid
flowchart TD
    UI[Blazor: FrentesInternas.razor]
    subgraph Subcomponentes
      Editor[EditorFrenteInterna.razor]
      Lideres[LideresFrenteInterna.razor]
      Participantes[ParticipantesFrenteInterna.razor]
      Lista[ListaFrentesInternas.razor]
      Avaliar[AvaliarFrenteInterna.razor]
      Exportar[ExportarFrentesInternas.razor]
    end
    UI --> Editor
    UI --> Lideres
    UI --> Participantes
    UI --> Lista
    UI --> Avaliar
    UI --> Exportar
    Editor -->|Usa| FrentesInternasService
    Lideres -->|Usa| FrentesInternasService
    Participantes -->|Usa| FrentesInternasService
    Lista -->|Usa| FrentesInternasService
    Avaliar -->|Usa| FrentesInternasService
    Exportar -->|Usa| ExportHelper
    FrentesInternasService -->|Usa| ApplicationDbContext
    FrentesInternasService -->|Usa| StatusHelper
    FrentesInternasService -->|Usa| MessageBoxService
    FrentesInternasService -->|Usa| TelemetryService


## Benefícios da Migração

### Performance
- Renderização híbrida com `@rendermode InteractiveAuto`
- Otimização automática entre Server e WebAssembly
- Carregamento assíncrono de dados

### Manutenibilidade
- Separação clara de responsabilidades
- Componentes reutilizáveis
- Serviços injetáveis e testáveis
- Configuração centralizada

### Experiência do Usuário
- Interface mais responsiva
- Feedback visual melhorado
- Validação em tempo real
- Mensagens de status centralizadas

### Observabilidade
- Telemetria integrada
- Rastreamento de eventos
- Monitoramento de performance
- Análise de exceções

## Próximos Passos

1. **Implementação dos Serviços**: Criar as implementações dos serviços de negócio
2. **Modelos de Dados**: Definir os modelos específicos para o módulo
3. **Testes**: Implementar testes unitários e de integração
4. **Autenticação**: Integrar com o sistema de autenticação
5. **Validação**: Implementar validações de negócio
6. **Cache**: Implementar estratégias de cache para performance

## Considerações Técnicas

### Compatibilidade
- Mantém compatibilidade com dados existentes
- Migração incremental sem impacto nos usuários
- Preserva todas as funcionalidades originais

### Segurança
- Validação de entrada em todos os formulários
- Controle de permissões baseado em perfil
- Sanitização de dados para exportação

### Escalabilidade
- Arquitetura preparada para crescimento
- Serviços desacoplados
- Configuração flexível
- Suporte a cache distribuído
# Sistema de Avaliação - Documentação Técnica

## Visão Geral da Solução

Este é um sistema de avaliação interna desenvolvido em ASP.NET Core 9 com Blazor Server, utilizando Entity Framework Core para persistência de dados e Azure Application Insights para observabilidade.

## Estrutura da Solução

### Organização de Pastas

```text
Peers.Moderno/
├── Components/           # Componentes Blazor reutilizáveis
├── Data/                # Contexto do Entity Framework
├── Models/              # Modelos de domínio
├── Pages/               # Páginas Razor/Blazor
├── Services/            # Serviços de negócio e utilitários
│   ├── Common/          # Serviços reutilizáveis (Telemetria, etc)
│   └── Business/        # Serviços específicos de negócio
├── wwwroot/             # Arquivos estáticos
└── docs/                # Documentação técnica
```

### Padrão de Reutilização de Código

#### Services/Common
Todos os utilitários e integrações compartilhadas devem ser implementados nesta pasta:

- **TelemetryService**: Observabilidade e telemetria
- **Futuros serviços**: Cache, Email, Validação, etc.

#### Services/Business
Serviços específicos do domínio de avaliação:

- **CompetenciasService**: Gestão de competências
- **AvaliacoesService**: Processamento de avaliações
- **ExportFileService**: Exportação de relatórios

## Tecnologias Utilizadas

- **.NET 9**: Framework principal
- **Blazor Server**: Interface de usuário interativa
- **Entity Framework Core**: ORM para acesso a dados
- **SQL Server**: Banco de dados
- **Azure Application Insights**: Observabilidade
- **EPPlus**: Geração de arquivos Excel

## Padrões de Desenvolvimento

### Injeção de Dependência
Todos os serviços são registrados no `Program.cs` e injetados via DI:

csharp
// Common Services
builder.Services.AddScoped<ITelemetryService, TelemetryService>();

// Business Services
builder.Services.AddScoped<ICompetenciasService, CompetenciasService>();


### Configuração
Todas as configurações são centralizadas no `appsettings.json`:

- **ConnectionStrings**: Strings de conexão com banco de dados
- **ApplicationInsights**: Configuração de telemetria
- **Logging**: Configuração de logs

## Documentação por Módulo

- [Observabilidade](observabilidade.md): Application Insights e TelemetryService

## Processo de Migração Incremental

Este sistema está sendo migrado incrementalmente. Cada etapa segue o padrão:

1. **Análise**: Identificação de funcionalidades a migrar
2. **Implementação**: Criação/modificação de código seguindo os padrões
3. **Documentação**: Atualização da documentação técnica
4. **Reutilização**: Aproveitamento de código já criado

### Próximas Etapas Planejadas

1. Migração de páginas de cadastro (Competências, Cargos, Eixos)
2. Implementação de fluxos de avaliação
3. Sistema de relatórios e dashboards
4. Integração com sistemas externos

## Como Contribuir

1. Siga os padrões de organização de pastas estabelecidos
2. Reutilize serviços da pasta `Services/Common` sempre que possível
3. Documente novas funcionalidades na pasta `docs/`
4. Utilize o `TelemetryService` para rastreamento de eventos importantes
5. Mantenha a consistência com os padrões de código existentes

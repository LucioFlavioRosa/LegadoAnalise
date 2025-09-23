# Documentação - Módulo de Consolidação de Avaliações

## Visão Geral

O módulo de Consolidação de Avaliações foi migrado do Web Forms (.NET Framework) para Blazor (.NET 9) utilizando renderização híbrida (InteractiveAuto) para máxima performance e responsividade. O módulo permite a visualização, exportação e importação de dados relacionados às avaliações de desempenho dos associados.

## Arquitetura

### Estrutura de Pastas


Services/
├── Consolidacao/
│   ├── ConsolidacaoService.cs          # Serviço principal de domínio
│   └── Common/
│       ├── ExportFileService.cs        # Serviço reutilizável de exportação
│       └── ImportFileService.cs        # Serviço reutilizável de importação
├── Common/
│   ├── MessageBoxService.cs            # Serviço de mensagens (reutilizável)
│   └── TelemetryService.cs             # Serviço de telemetria (reutilizável)
Models/
├── ConsideracoesMentorModel.cs         # Modelo para considerações do mentor
├── PerformanceNotasModelExport.cs      # Modelo para exportação de notas
└── ConsolidacaoItem.cs                 # Modelo para itens de consolidação
Components/
└── Pages/
    ├── Consolidacao.razor              # Componente Blazor principal
    └── Consolidacao.razor.cs           # Code-behind do componente


### Princípios Arquiteturais

1. **Separação de Responsabilidades**: Lógica de negócio extraída para serviços injetáveis
2. **Reutilização**: Serviços comuns organizados em pastas `Common`
3. **Injeção de Dependência**: Todos os serviços registrados no container DI
4. **Async/Await**: Operações assíncronas para melhor performance
5. **Telemetria**: Monitoramento integrado com Application Insights

## Componentes Principais

### 1. ConsolidacaoService

**Responsabilidade**: Lógica de negócio principal para consolidação de avaliações.

**Métodos Principais**:
- `ObterListConsolidacaoAsync()`: Busca avaliações com filtros
- `CalculaNotaPerformanceComiteAsync()`: Calcula notas do comitê

**Dependências**:
- `ApplicationDbContext`: Acesso a dados
- `IAssociadosService`: Serviços de associados
- `ITelemetryService`: Telemetria

### 2. ExportFileService (Common)

**Responsabilidade**: Geração de arquivos Excel para exportação.

**Métodos Principais**:
- `GenerateExcelConsideracoesMentorAsync()`: Exporta considerações do mentor
- `GenerateExcelPerformanceNotasAsync()`: Exporta notas de performance

**Tecnologia**: EPPlus para manipulação de Excel

### 3. ImportFileService (Common)

**Responsabilidade**: Processamento de arquivos Excel para importação.

**Métodos Principais**:
- `ImportConsideracoesMentorAsync()`: Importa considerações do mentor
- `ImportPerformanceNotasAsync()`: Importa notas de performance

**Características**:
- Validação de dados
- Tratamento de erros por linha
- Relatório de importação com estatísticas

### 4. Componente Blazor Consolidacao

**Responsabilidade**: Interface do usuário e coordenação de ações.

**Características**:
- Renderização híbrida (`@rendermode InteractiveAuto`)
- Binding reativo para filtros
- Estados de loading para operações assíncronas
- Upload de arquivos com validação

## Integração com Outros Módulos

### Serviços Reutilizados

1. **MessageBoxService**: Feedback ao usuário (Success, Error, Info, Warning)
2. **TelemetryService**: Rastreamento de eventos e exceções
3. **AssociadosService**: Dados de associados
4. **ApplicationDbContext**: Acesso ao banco de dados

### Configurações

As configurações estão centralizadas em `appsettings.json` na seção `Consolidacao`:


{
  "Consolidacao": {
    "MaxExportRecords": 50000,
    "MaxImportRecords": 10000,
    "MaxFileSizeMB": 10,
    "AllowedFileExtensions": [".xlsx", ".xls"],
    "EnableValidation": true,
    "DefaultTipoAvaliacao": "desempenho",
    "DefaultEscopo": "projeto"
  }
}


## Fluxo de Funcionamento

### Busca de Avaliações
1. Usuário seleciona filtros (Período, Projeto, Associado)
2. Componente chama `ConsolidacaoService.ObterListConsolidacaoAsync()`
3. Serviço consulta banco via Entity Framework
4. Resultados são exibidos na tabela reativa

### Exportação
1. Usuário seleciona filtros de exportação
2. Componente chama `ExportFileService.GenerateExcel...()`
3. Serviço gera arquivo Excel usando EPPlus
4. Arquivo é baixado via JavaScript

### Importação
1. Usuário seleciona arquivo Excel
2. Componente chama `ImportFileService.Import...()`
3. Serviço processa arquivo com validação
4. Resultados são salvos no banco
5. Relatório de importação é exibido

## Benefícios da Migração

### Performance
- Renderização híbrida Blazor (Server + WebAssembly)
- Operações assíncronas
- Caching automático de componentes

### Manutenibilidade
- Separação clara de responsabilidades
- Serviços reutilizáveis
- Injeção de dependência
- Código testável

### Experiência do Usuário
- Interface reativa
- Estados de loading
- Feedback imediato
- Upload de arquivos moderno

### Monitoramento
- Telemetria integrada
- Rastreamento de performance
- Logs estruturados

## Próximos Passos

1. **Implementar Testes**: Testes unitários para serviços
2. **Cache**: Implementar cache para consultas frequentes
3. **Validação Avançada**: Regras de negócio mais complexas
4. **Relatórios**: Dashboards de consolidação
5. **API**: Endpoints REST para integração externa

## Considerações Técnicas

### Dependências NuGet
- `EPPlus`: Manipulação de Excel
- `Microsoft.EntityFrameworkCore`: ORM
- `Microsoft.ApplicationInsights`: Telemetria

### Compatibilidade
- .NET 9
- Blazor Server/WebAssembly híbrido
- SQL Server
- Navegadores modernos

### Segurança
- Autenticação Azure AD
- Validação de upload de arquivos
- Sanitização de dados
- Logs de auditoria
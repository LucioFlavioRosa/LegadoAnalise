# Fluxo de Alto Nível - Módulo de Consolidação

## Diagrama Mermaid

mermaid
flowchart TD
    A[Usuário acessa página Consolidação] --> B[Componente Blazor Consolidação]
    B --> C[Injeta serviços de negócio]
    C --> D[ConsolidacaoService]
    C --> E[ExportFileService]
    C --> F[ImportFileService]
    C --> G[MessageBoxService]
    
    B --> H[Carrega dados iniciais]
    H --> I[Períodos, Projetos, Associados]
    
    B --> J[Usuário aplica filtros]
    J --> K[Buscar Avaliações]
    K --> D
    D --> L[ApplicationDbContext]
    L --> M[Consulta SQL Server]
    M --> N[Retorna dados]
    N --> O[Exibe resultados na tabela]
    
    B --> P[Usuário exporta dados]
    P --> Q[Seleciona filtros de exportação]
    Q --> E
    E --> R[Gera arquivo Excel com EPPlus]
    R --> S[Download via JavaScript]
    
    B --> T[Usuário importa dados]
    T --> U[Seleciona arquivo Excel]
    U --> F
    F --> V[Processa arquivo com validação]
    V --> W[Salva dados válidos no banco]
    W --> X[Exibe relatório de importação]
    
    D --> Y[TelemetryService]
    E --> Y
    F --> Y
    Y --> Z[Application Insights]
    
    O --> G
    S --> G
    X --> G
    G --> AA[Feedback ao usuário]
    
    style A fill:#e1f5fe
    style B fill:#f3e5f5
    style D fill:#e8f5e8
    style E fill:#e8f5e8
    style F fill:#e8f5e8
    style G fill:#fff3e0
    style Y fill:#fff3e0
    style L fill:#fce4ec
    style Z fill:#f1f8e9


## Descrição dos Fluxos

### 1. Fluxo de Inicialização
- Usuário acessa a página `/consolidacao`
- Componente Blazor é renderizado com modo InteractiveAuto
- Serviços são injetados via DI
- Dados iniciais são carregados (períodos, projetos, associados)

### 2. Fluxo de Busca de Avaliações
- Usuário seleciona filtros (período, projeto, associado)
- Clica em "Listar Avaliações"
- ConsolidacaoService consulta o banco via Entity Framework
- Resultados são exibidos na tabela reativa
- Telemetria registra a operação

### 3. Fluxo de Exportação
- Usuário seleciona filtros específicos para exportação
- Clica em "Exportar"
- ExportFileService gera arquivo Excel usando EPPlus
- Arquivo é convertido para Base64
- JavaScript faz download do arquivo
- MessageBoxService exibe confirmação

### 4. Fluxo de Importação
- Usuário seleciona arquivo Excel
- ImportFileService processa o arquivo
- Dados são validados linha por linha
- Registros válidos são salvos no banco
- Relatório de importação é gerado
- MessageBoxService exibe resultado

### 5. Fluxo de Monitoramento
- Todas as operações geram eventos de telemetria
- TelemetryService envia dados para Application Insights
- Logs estruturados são gerados
- Métricas de performance são coletadas

## Pontos de Integração

### Serviços Externos
- **SQL Server**: Persistência de dados
- **Application Insights**: Telemetria e monitoramento
- **Azure AD**: Autenticação (implícita)

### Serviços Internos
- **AssociadosService**: Dados de associados
- **PeriodosService**: Dados de períodos
- **ProjetosService**: Dados de projetos
- **MessageBoxService**: Feedback ao usuário

### Tecnologias Utilizadas
- **Blazor Server/WebAssembly**: Interface híbrida
- **Entity Framework Core**: ORM
- **EPPlus**: Manipulação de Excel
- **JavaScript Interop**: Download de arquivos

## Estados da Aplicação

### Estados de Loading
- `isLoading`: Busca de avaliações
- `isExporting`: Exportação de dados
- `isImporting`: Importação de dados
- `isExportingNotas`: Exportação de notas
- `isImportingNotas`: Importação de notas

### Estados de Dados
- `consolidacaoItems`: Lista de avaliações
- `periodos`: Lista de períodos disponíveis
- `projetos`: Lista de projetos disponíveis
- `associados`: Lista de associados disponíveis

## Tratamento de Erros

### Níveis de Tratamento
1. **Componente**: Try-catch em métodos de evento
2. **Serviços**: Try-catch com telemetria
3. **Importação**: Validação por linha com relatório
4. **Global**: Exception handler do Blazor

### Tipos de Erro
- **Validação**: Dados inválidos ou ausentes
- **Negócio**: Regras de negócio violadas
- **Técnico**: Falhas de sistema ou rede
- **Usuário**: Ações inválidas ou não permitidas

## Performance

### Otimizações Implementadas
- Renderização híbrida Blazor
- Operações assíncronas
- Lazy loading de dados
- Validação client-side
- Compressão de arquivos (opcional)

### Métricas Monitoradas
- Tempo de resposta das consultas
- Tamanho dos arquivos exportados/importados
- Taxa de sucesso das operações
- Uso de memória durante processamento

## Segurança

### Medidas Implementadas
- Autenticação obrigatória
- Validação de tipos de arquivo
- Limite de tamanho de upload
- Sanitização de dados importados
- Logs de auditoria

### Configurações de Segurança
- Tamanho máximo: 10MB
- Extensões permitidas: .xlsx, .xls
- Timeout de operações: 30 segundos
- Validação de esquema Excel
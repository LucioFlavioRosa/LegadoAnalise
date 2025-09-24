# Módulo Performance - Documentação Técnica

## Visão Geral

O módulo Performance foi modernizado seguindo os padrões estabelecidos no projeto Peers.Moderno, com foco na reutilização de código e separação de responsabilidades. Os utilitários comuns foram criados na pasta `Services/Performance/Common/` para facilitar a manutenção e permitir reutilização em outros módulos.

## Arquitetura

### Estrutura de Pastas


Services/Performance/Common/
├── PerformanceImportExportUtil.cs    # Utilitário para import/export Excel
├── PerformanceValidationUtil.cs      # Validações de regras de negócio
└── PerformanceComboHelper.cs         # Helper para combos e dropdowns


### Componentes Principais

#### 1. PerformanceImportExportUtil
- **Responsabilidade**: Gerenciar importação e exportação de planilhas Excel
- **Tecnologia**: EPPlus para manipulação de arquivos Excel
- **Funcionalidades**:
  - Exportação de performances para Excel com formatação
  - Importação com validação de estrutura
  - Parsing de dados com tratamento de erros
  - Integração com telemetria

#### 2. PerformanceValidationUtil
- **Responsabilidade**: Centralizar todas as validações de regras de negócio
- **Funcionalidades**:
  - Validação de campos obrigatórios
  - Validação de regras de negócio (tamanhos, formatos)
  - Validação de consistência entre inputs e notas padrão
  - Validações específicas para inserção, alteração e exclusão

#### 3. PerformanceComboHelper
- **Responsabilidade**: Gerenciar dados para combos e dropdowns
- **Funcionalidades**:
  - Carregamento de cargos
  - Opções de status (Ativo/Inativo)
  - Opções de abrangência (Individual/Coletivo)
  - Carregamento de notas padrão
  - Formatação de textos para exibição

## Fluxo de Alto Nível

mermaid
flowchart TD
    UI[Performance UI Component]
    
    UI -->|Import/Export| ImportExportUtil[PerformanceImportExportUtil]
    UI -->|Validation| ValidationUtil[PerformanceValidationUtil]
    UI -->|Combos/Dropdowns| ComboHelper[PerformanceComboHelper]
    
    ImportExportUtil -->|Excel Processing| EPPlus[EPPlus Library]
    ImportExportUtil -->|Validation| ValidationUtil
    ImportExportUtil -->|Telemetry| TelemetryService[ITelemetryService]
    
    ValidationUtil -->|Business Rules| BusinessLogic[Business Rules Engine]
    ValidationUtil -->|Combo Validation| ComboHelper
    ValidationUtil -->|Telemetry| TelemetryService
    
    ComboHelper -->|Cargos| CargosService[ICargosService]
    ComboHelper -->|Notas| AvaliacoesService[IAvaliacoesService]
    ComboHelper -->|Telemetry| TelemetryService
    
    ImportExportUtil -->|Data Models| Models[Performance Models]
    ValidationUtil -->|Data Models| Models
    ComboHelper -->|Data Models| Models
    
    Models -->|Entity Framework| Database[(Database)]


## Integração

### Injeção de Dependência

Os serviços devem ser registrados no `Program.cs`:

csharp
// Performance Common Services
builder.Services.AddScoped<IPerformanceImportExportUtil, PerformanceImportExportUtil>();
builder.Services.AddScoped<IPerformanceValidationUtil, PerformanceValidationUtil>();
builder.Services.AddScoped<IPerformanceComboHelper, PerformanceComboHelper>();


### Uso nos Componentes

csharp
public class PerformanceComponent : ComponentBase
{
    [Inject] private IPerformanceImportExportUtil ImportExportUtil { get; set; }
    [Inject] private IPerformanceValidationUtil ValidationUtil { get; set; }
    [Inject] private IPerformanceComboHelper ComboHelper { get; set; }
    
    // Implementação do componente
}


## Reutilização

### Padrões de Reutilização Implementados

1. **Validação Centralizada**: Todas as validações ficam no `PerformanceValidationUtil`, permitindo reutilização em diferentes fluxos (inserção, alteração, importação)

2. **Import/Export Padronizado**: O `PerformanceImportExportUtil` segue o mesmo padrão usado em outros módulos, facilitando manutenção

3. **Combos Centralizados**: O `PerformanceComboHelper` centraliza toda a lógica de carregamento de dados para dropdowns

4. **Modelos de Dados**: Uso de modelos específicos para import/export que podem ser reutilizados

### Extensibilidade

- **Novos Tipos de Validação**: Facilmente adicionáveis no `PerformanceValidationUtil`
- **Novos Formatos de Export**: Extensível no `PerformanceImportExportUtil`
- **Novos Combos**: Facilmente adicionáveis no `PerformanceComboHelper`

## Segurança

### Validação de Dados
- Validação de estrutura de arquivos Excel
- Sanitização de dados de entrada
- Validação de tipos de dados
- Tratamento de exceções com telemetria

### Telemetria
- Rastreamento de operações de import/export
- Logging de erros de validação
- Métricas de performance

## Configuração

### Constantes e Configurações

csharp
public static class PerformanceComboConstants
{
    public static class DefaultValues
    {
        public const int DefaultStatus = Status.Ativo;
        public const string DefaultAbrangencia = Abrangencia.Individual;
        public const bool DefaultInputAutoAvaliacao = Input.Habilitado;
    }
}


### Limites e Validações

- **MAX_PERFORMANCE_LENGTH**: 500 caracteres
- **MAX_DESCRICAO_LENGTH**: 2000 caracteres
- Validação de abrangência: Individual ou Coletivo
- Validação de status: 0 (Inativo) ou 1 (Ativo)

## Tratamento de Erros

### Estratégias Implementadas

1. **Validação Preventiva**: Validações antes de operações críticas
2. **Tratamento de Exceções**: Try-catch com logging via telemetria
3. **Mensagens Amigáveis**: Retorno de mensagens claras para o usuário
4. **Rollback Automático**: Em caso de erro durante importação

### Tipos de Erro

- **Erros de Validação**: Campos obrigatórios, formatos inválidos
- **Erros de Negócio**: Inconsistências de dados, regras violadas
- **Erros de Sistema**: Problemas de acesso a dados, falhas de rede
- **Erros de Arquivo**: Estrutura inválida, formato não suportado

## Performance

### Otimizações Implementadas

1. **Carregamento Assíncrono**: Operações de I/O são assíncronas
2. **Cache de Combos**: Dados de combos podem ser cacheados
3. **Validação em Lote**: Importações validam múltiplos registros eficientemente
4. **Streaming de Arquivos**: Processamento de arquivos grandes sem carregar tudo na memória

### Métricas

- Tempo de processamento de importação/exportação
- Número de registros processados
- Taxa de erro por operação
- Uso de memória durante processamento

## Manutenção

### Pontos de Atenção

1. **Dependências**: EPPlus, Entity Framework, serviços de telemetria
2. **Versionamento**: Compatibilidade com versões anteriores dos arquivos Excel
3. **Testes**: Cobertura de testes para todos os cenários de validação
4. **Documentação**: Manter documentação atualizada com mudanças

### Evolução Futura

- Suporte a outros formatos de arquivo (CSV, JSON)
- Validações mais sofisticadas com IA
- Cache distribuído para ambientes de alta disponibilidade
- Processamento assíncrono para importações grandes
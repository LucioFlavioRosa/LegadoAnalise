# Competências - Arquitetura e Integração

Este documento descreve a arquitetura dos serviços e componentes relacionados ao cadastro, edição, importação/exportação e listagem de competências no sistema modernizado.

## Visão Geral

O módulo de Competências foi migrado de Web Forms (.NET Framework) para Blazor Server (.NET 9), seguindo os princípios de arquitetura limpa, separação de responsabilidades e reutilização de código.

## Estrutura de Pastas


Services/
├── Competencias/
│   ├── CompetenciasService.cs              # Serviço principal de negócio
│   ├── ICompetenciasService.cs             # Interface do serviço principal
│   └── Common/
│       ├── ICompetenciasValidator.cs       # Interface de validação
│       ├── CompetenciasValidator.cs        # Implementação das validações
│       └── CompetenciasImportExportUtil.cs # Utilitários de import/export
│
Components/
├── Competencias/
│   ├── CompetenciasPage.razor              # Página principal
│   ├── CompetenciasForm.razor              # Formulário de cadastro/edição
│   ├── CompetenciasList.razor              # Listagem e filtros
│   └── CompetenciasImportExport.razor      # Import/Export de arquivos
│
docs/
└── Competencias.md                         # Esta documentação


## Fluxo de Alto Nível

mermaid
flowchart TD
    A[CompetenciasPage.razor] --> B[CompetenciasForm.razor]
    A --> C[CompetenciasList.razor]
    A --> D[CompetenciasImportExport.razor]
    
    B --> E[ICompetenciasService]
    C --> E
    D --> F[ICompetenciasImportExportUtil]
    
    E --> G[ICompetenciasValidator]
    E --> H[(ApplicationDbContext)]
    F --> H
    G --> H
    
    I[appsettings.json] --> G
    I --> E
    I --> F
    
    J[Program.cs - DI Container] --> E
    J --> G
    J --> F
    
    subgraph "Validações"
        G --> K[Validação de Campos Obrigatórios]
        G --> L[Validação de Regras de Negócio]
        G --> M[Validação de Integridade Referencial]
        G --> N[Validação de Configurações]
    end
    
    subgraph "Tipos de Avaliação"
        O[Desempenho] --> P[Cargo + Eixo + SubCompetência + Dimensão]
        Q[Liderança] --> R[Cargo 96 + Eixo + Título + Escopo]
    end


## Componentes Principais

### 1. CompetenciasService

**Responsabilidade:** Orquestrar todas as operações de negócio relacionadas a competências.

**Principais Métodos:**
- `CadastrarAsync(Competencia competencia)` - Cadastra nova competência
- `AlterarAsync(Competencia competencia)` - Altera competência existente
- `ExcluirAsync(int idCompetencia)` - Inativa competência
- `ListarAsync(bool incluirInativas = false)` - Lista competências
- `ObterPorIdAsync(int idCompetencia)` - Obtém competência por ID
- `ImportarAsync(Stream arquivo)` - Importa competências de Excel
- `ExportarAsync(bool incluirInativas = false)` - Exporta competências para Excel
- `AgregarAsync(int idCompetencia, int idPeriodo)` - Agrega competência às avaliações existentes

### 2. CompetenciasValidator

**Responsabilidade:** Centralizar todas as validações de regras de negócio.

**Principais Validações:**
- **Campos Obrigatórios:** Cargo, Eixo, SubCompetência, Dimensão, Detalhamento, etc.
- **Regras de Desempenho:** Validações específicas para avaliações de desempenho
- **Regras de Liderança:** Validações específicas para avaliações de liderança (Cargo 96, Dimensão 10)
- **Integridade Referencial:** Verificação de existência de registros relacionados
- **Configurações:** Validação de notas padrão, modos de cálculo, configurações de visibilidade
- **Duplicatas:** Prevenção de competências duplicadas

### 3. CompetenciasImportExportUtil

**Responsabilidade:** Manipular arquivos Excel para importação e exportação.

**Funcionalidades:**
- Leitura de arquivos Excel (.xlsx, .xls)
- Validação de estrutura de arquivo
- Mapeamento de dados para modelos
- Geração de arquivos Excel para exportação
- Tratamento de erros de importação
- Relatórios de importação (inseridos, alterados, com erro)

## Configurações (appsettings.json)

### Seção Competencias


{
  "Competencias": {
    "MaxExportRecords": 10000,           // Máximo de registros para exportação
    "MaxImportRecords": 5000,            // Máximo de registros para importação
    "MaxDetalheLength": 2000,            // Tamanho máximo do detalhamento
    "MaxCompetenciaLength": 500,         // Tamanho máximo da competência
    "MaxPalavrasChaveLength": 500,       // Tamanho máximo das palavras-chave
    "MaxDescricaoRelacaoLength": 1000,   // Tamanho máximo da descrição da relação
    "ExportTempPath": "temp/exports",    // Caminho temporário para exports
    "ImportTempPath": "temp/imports",    // Caminho temporário para imports
    "AllowedFileExtensions": [".xlsx", ".xls"], // Extensões permitidas
    "MaxFileSizeMB": 10,                 // Tamanho máximo do arquivo em MB
    "EnableAggregation": true,           // Habilita funcionalidade de agregação
    "DefaultNotaPadraoNivel1": 1,        // Nota padrão nível 1
    "DefaultNotaPadraoNivel2": 1,        // Nota padrão nível 2
    "DefaultModoCalculo": 1,             // Modo de cálculo padrão
    "ValidTiposAvaliacao": ["desempenho", "lideranca"], // Tipos válidos
    "ValidEscoposLideranca": ["projeto", "líder", "backoffice"], // Escopos válidos
    "CargoLideranca": 96,                // ID do cargo para liderança
    "DimensaoLideranca": 10,             // ID da dimensão para liderança
    "EnableAutoPreenchimento": true,     // Habilita auto preenchimento
    "RequireAllConfiguracoesVisibilidade": false, // Requer todas as configs de visibilidade
    "EnableImportValidation": true,      // Habilita validação na importação
    "EnableExportCompression": false,    // Habilita compressão na exportação
    "CacheExpirationMinutes": 30         // Tempo de expiração do cache
  }
}


## Tipos de Avaliação

### 1. Avaliação de Desempenho

**Características:**
- Utiliza qualquer cargo (exceto 96)
- Requer seleção de Eixo, SubCompetência e Dimensão (exceto 10)
- Permite configuração de palavras-chave
- Suporta configurações de auto preenchimento e visibilidade
- Permite agregação às avaliações existentes

**Campos Específicos:**
- Cargo (dropdown)
- Eixo (dropdown)
- SubCompetência (dropdown)
- Dimensão (dropdown)
- Detalhamento Nível Atual (rich text)
- Nível Atual (texto)
- Palavras-chave (textarea)
- Resumo da subcompetência pro cargo (auto-preenchido)
- Configurações de input e visibilidade
- Notas padrão para níveis 1 e 2
- Modo de cálculo

### 2. Avaliação de Liderança

**Características:**
- Utiliza exclusivamente o cargo 96
- Utiliza exclusivamente a dimensão 10
- Escopo limitado a: projeto, líder, backoffice
- Não permite agregação
- Não possui configurações de auto preenchimento

**Campos Específicos:**
- Escopo (dropdown: Por projeto, Por líder, backoffice)
- Pilar (dropdown de eixos de liderança)
- Título (dropdown de subcompetências)
- Detalhamento (texto simples)

## Funcionalidades Principais

### 1. Cadastro e Edição

- Formulário dinâmico que se adapta ao tipo de avaliação selecionado
- Validação em tempo real dos campos obrigatórios
- Auto preenchimento do resumo da subcompetência baseado na relação cargo-subcompetência
- Configurações avançadas de auto preenchimento e visibilidade
- Suporte a rich text para detalhamentos

### 2. Listagem e Filtros

- Tabela responsiva com paginação
- Filtro global que busca em qualquer coluna
- Ações de editar e inativar por registro
- Indicação visual do status (Ativo/Inativo)
- Ordenação por colunas

### 3. Importação e Exportação

**Exportação:**
- Gera arquivo Excel com todas as competências
- Inclui códigos de referência para facilitar importação
- Opção de incluir registros inativos
- Validação de limites de exportação

**Importação:**
- Suporte a arquivos .xlsx e .xls
- Validação de estrutura do arquivo
- Validação de dados e integridade referencial
- Relatório detalhado de importação
- Suporte a inserção e alteração em lote
- Tratamento de erros com feedback específico

### 4. Agregação

- Funcionalidade para adicionar competências às avaliações existentes
- Disponível apenas para competências de desempenho
- Aplica a competência a todos os associados do cargo no período atual
- Utiliza notas padrão configuradas
- Relatório de quantas competências e associados foram afetados

## Regras de Negócio

### Validações Gerais

1. **Campos Obrigatórios:** Todos os campos marcados como obrigatórios devem ser preenchidos
2. **Unicidade:** Não é permitido criar competências duplicadas (mesma combinação de cargo, eixo, subcompetência, dimensão, tipo e escopo)
3. **Integridade Referencial:** Todos os IDs referenciados devem existir e estar ativos
4. **Limites de Tamanho:** Textos não podem exceder os limites configurados

### Validações de Desempenho

1. **Cargo:** Não pode ser o cargo 96 (reservado para liderança)
2. **Dimensão:** Não pode ser a dimensão 10 (reservada para liderança)
3. **Palavras-chave:** Opcional, mas limitada em tamanho
4. **Configurações:** Pelo menos um nível deve estar habilitado para input e visibilidade

### Validações de Liderança

1. **Cargo:** Deve ser obrigatoriamente o cargo 96
2. **Dimensão:** Deve ser obrigatoriamente a dimensão 10
3. **Escopo:** Deve ser um dos valores válidos (projeto, líder, backoffice)
4. **Detalhamento:** Campo único que serve para todos os níveis

### Validações de Importação

1. **Estrutura do Arquivo:** Deve manter a ordem e quantidade de colunas do template
2. **Dados Obrigatórios:** Todos os campos obrigatórios devem estar preenchidos
3. **Códigos de Referência:** IDs de cargo, eixo, subcompetência e dimensão devem existir
4. **Limites:** Não pode exceder o limite máximo de registros para importação

## Integração com Outros Módulos

### Cargos
- Utiliza o serviço de cargos para popular dropdowns
- Mantém relação cargo-subcompetência
- Valida existência e status ativo dos cargos

### Eixos
- Diferencia eixos de desempenho e liderança
- Valida existência e status ativo dos eixos

### SubCompetências
- Utilizada tanto para desempenho quanto liderança (como "título")
- Mantém relação com cargos para auto preenchimento

### Dimensões
- Filtra dimensões por tipo de avaliação
- Reserva dimensão 10 para liderança

### Avaliações
- Integra com o módulo de avaliações para agregação
- Utiliza notas padrão configuradas
- Respeita períodos ativos

## Padrões de Desenvolvimento

### Injeção de Dependência

Todos os serviços são registrados no `Program.cs` e injetados via construtor:

csharp
// Registro
builder.Services.AddScoped<ICompetenciasValidator, CompetenciasValidator>();

// Uso
public class CompetenciasService
{
    private readonly ICompetenciasValidator _validator;
    
    public CompetenciasService(ICompetenciasValidator validator)
    {
        _validator = validator;
    }
}


### Validação

Todas as operações passam por validação antes da persistência:

csharp
public async Task<bool> CadastrarAsync(Competencia competencia)
{
    var validationResult = _validator.ValidateForCreate(competencia);
    if (!validationResult.IsValid)
    {
        throw new ValidationException(string.Join("; ", validationResult.Errors));
    }
    
    // Prosseguir com a persistência
}


### Configuração

Todas as configurações são centralizadas no `appsettings.json` e acessadas via `IConfiguration`:

csharp
var maxLength = _configuration.GetValue<int>("Competencias:MaxDetalheLength", 2000);


### Tratamento de Erros

Erros são tratados de forma consistente com mensagens específicas:

csharp
try
{
    await _competenciasService.CadastrarAsync(competencia);
    _messageBox.ShowSuccess("Competência cadastrada com sucesso!");
}
catch (ValidationException ex)
{
    _messageBox.ShowError(ex.Message);
}
catch (Exception ex)
{
    _messageBox.ShowError("Erro interno. Tente novamente.");
    _logger.LogError(ex, "Erro ao cadastrar competência");
}


## Considerações de Performance

1. **Cache:** Implementar cache para dropdowns que não mudam frequentemente
2. **Paginação:** Implementar paginação server-side para listas grandes
3. **Lazy Loading:** Carregar dados relacionados apenas quando necessário
4. **Índices:** Garantir índices adequados no banco de dados
5. **Compressão:** Considerar compressão para arquivos de exportação grandes

## Testes

### Testes Unitários
- Validadores devem ter cobertura completa
- Serviços devem ser testados com mocks
- Utilitários devem ser testados isoladamente

### Testes de Integração
- Fluxos completos de cadastro, edição e exclusão
- Importação e exportação com arquivos reais
- Agregação com dados de avaliação

### Testes de UI
- Navegação entre componentes
- Validação de formulários
- Feedback visual adequado

## Roadmap

### Próximas Funcionalidades
1. **Histórico de Alterações:** Auditoria de mudanças nas competências
2. **Aprovação de Competências:** Workflow de aprovação para novas competências
3. **Templates Personalizados:** Permitir templates customizados de importação
4. **API REST:** Expor funcionalidades via API para integração externa
5. **Relatórios Avançados:** Dashboards e relatórios analíticos

### Melhorias Técnicas
1. **Cache Distribuído:** Implementar Redis para cache
2. **Background Jobs:** Processamento assíncrono para importações grandes
3. **Versionamento:** Controle de versão das competências
4. **Backup Automático:** Backup automático antes de importações
5. **Monitoramento:** Métricas e alertas para operações críticas

## Conclusão

A migração do módulo de Competências para Blazor Server (.NET 9) trouxe benefícios significativos em termos de manutenibilidade, testabilidade e reutilização de código. A arquitetura implementada segue as melhores práticas de desenvolvimento .NET moderno e está preparada para futuras expansões e melhorias.
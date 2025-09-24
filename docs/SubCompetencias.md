# SubCompetências - Migração para Blazor

## Visão Geral

Este documento descreve a migração da funcionalidade de SubCompetências do Web Forms para Blazor, incluindo a criação de serviços reutilizáveis e a centralização da lógica de negócio.

## Arquitetura

### Serviços Implementados

#### ISubCompetenciasService
- **Localização**: `Services/SubCompetencias/`
- **Responsabilidade**: Gerenciar todas as operações CRUD de SubCompetências
- **Funcionalidades**:
  - Listagem de SubCompetências (todas e apenas ativas)
  - Inserção, alteração e exclusão
  - Inativação de registros
  - Busca por nome
  - Validação de duplicatas

#### IExportFileService
- **Localização**: `Services/Common/`
- **Responsabilidade**: Exportação de dados para Excel
- **Funcionalidades**:
  - Exportação genérica para qualquer tipo de dados
  - Exportação com mapeamento customizado de colunas
  - Formatação automática de valores (datas, booleanos)
  - Geração de nomes de arquivo com timestamp

#### IMessageBoxService
- **Localização**: `Services/Common/`
- **Responsabilidade**: Exibição de mensagens para o usuário
- **Funcionalidades**:
  - Mensagens de sucesso, erro, informação e aviso
  - Sistema de eventos para comunicação com a UI

### Modelos

#### SubCompetencia
- **Localização**: `Models/`
- **Mapeamento**: Tabela `SUBCOMPETENCIAS`
- **Propriedades**:
  - `IdSubCompetencia`: Chave primária
  - `Nome`: Nome da subcompetência
  - `TipoAvaliacao`: Tipo de avaliação (opcional)
  - `Ativo`: Status ativo/inativo
  - `DHC`: Data/hora de criação
  - `USR`: ID do usuário que criou/alterou

#### SubcompetenciaModelExport
- **Localização**: `Models/`
- **Responsabilidade**: Modelo específico para exportação
- **Características**:
  - Propriedades com atributos `DisplayName`
  - Formatação de status como texto
  - Otimizado para geração de Excel

## Integração

### Injeção de Dependência

Os serviços são registrados no `Program.cs`:

csharp
builder.Services.AddScoped<Services.SubCompetencias.ISubCompetenciasService, Services.SubCompetencias.SubCompetenciasService>();
builder.Services.AddScoped<IExportFileService, ExportFileService>();
builder.Services.AddScoped<IMessageBoxService, MessageBoxService>();


### Uso em Componentes Blazor

csharp
@inject ISubCompetenciasService SubCompetenciasService
@inject IExportFileService ExportService
@inject IMessageBoxService MessageBox

// Exemplo de uso
var subCompetencias = await SubCompetenciasService.ListarSubCompetenciasAsync();
var excelData = await ExportService.GenerateExcelSubCompetenciasAsync("SubCompetencias", subCompetencias);
MessageBox.ShowSuccess("Operação realizada com sucesso!");


## Fluxo de Alto Nível

mermaid
flowchart TD
    A[Usuário acessa SubCompetencias.razor] --> B{Ação do Usuário}
    
    B -->|Listar| C[SubCompetenciasService.ListarSubCompetenciasAsync]
    C --> D[Renderiza Tabela]
    
    B -->|Cadastrar/Editar| E[Validação de Dados]
    E -->|Válido| F[SubCompetenciasService.InserirOuAlterarAsync]
    E -->|Inválido| G[MessageBoxService.ShowError]
    
    F -->|Sucesso| H[MessageBoxService.ShowSuccess]
    F -->|Erro| I[MessageBoxService.ShowError]
    
    B -->|Inativar| J[SubCompetenciasService.InativarAsync]
    J -->|Sucesso| K[MessageBoxService.ShowSuccess]
    J -->|Erro| L[MessageBoxService.ShowError]
    
    B -->|Exportar| M[ExportFileService.GenerateExcelSubCompetenciasAsync]
    M --> N[Download do Arquivo Excel]
    
    D -->|Alterar| E
    D -->|Inativar| J
    D -->|Exportar| M
    
    H --> O[Recarrega Lista]
    K --> O
    O --> D


## Benefícios da Migração

1. **Separação de Responsabilidades**: Lógica de negócio separada da UI
2. **Reutilização**: Serviços podem ser utilizados em outros componentes
3. **Testabilidade**: Serviços podem ser testados independentemente
4. **Manutenibilidade**: Código mais organizado e fácil de manter
5. **Performance**: Entity Framework Core com otimizações (AsNoTracking)
6. **Telemetria**: Rastreamento de eventos e exceções
7. **Configuração**: Uso do appsettings.json para configurações

## Próximos Passos

1. Criar o componente Blazor `SubCompetencias.razor`
2. Implementar o code-behind `SubCompetencias.razor.cs`
3. Configurar roteamento
4. Implementar testes unitários
5. Validar migração com dados reais

## Considerações Técnicas

- **Entity Framework**: Utiliza AsNoTracking para consultas de leitura
- **Telemetria**: Application Insights para monitoramento
- **Tratamento de Exceções**: Logs estruturados e mensagens amigáveis
- **Validação**: Validação tanto no cliente quanto no servidor
- **Exportação**: EPPlus para geração de arquivos Excel
- **Sessão**: Compatibilidade mantida para migração gradual
# Módulo de Associados - Documentação Técnica

## Visão Geral

O módulo de Associados foi migrado do Web Forms para Blazor Híbrido .NET 9, mantendo todas as funcionalidades originais com melhorias na arquitetura, performance e manutenibilidade.

## Arquitetura

### Componentes Blazor

O módulo está organizado em componentes especializados:

- **AssociadosCadastro.razor**: Componente principal para cadastro e edição de associados
- **AssociadosLista.razor**: Listagem com filtros e ações de gerenciamento
- **HistoricoPromocoes.razor**: Histórico de promoções com edição inline
- **ImportExportAssociados.razor**: Operações de importação/exportação Excel

### Serviços

#### Serviço Principal
- **AssociadosService**: CRUD completo de associados

#### Serviços Common (Reutilizáveis)
- **FotoService**: Manipulação de fotos (upload, conversão base64, validação)
- **ExcelService**: Importação/exportação de dados Excel
- **DropdownService**: População de combos/dropdowns
- **PromocaoService**: Lógica de promoções e histórico

### Modelos de Dados

- **Associado**: Entidade principal com relacionamentos
- **Promocao**: Histórico de promoções
- **Perfil**: Perfis de acesso
- **Vertical**: Verticais de negócio
- **DTOs**: AssociadoListaItem, PromocaoHistorico, DropdownItem

## Funcionalidades

### 1. Cadastro de Associados
- Formulário reativo com validação
- Upload de fotos com preview
- Seleção de mentor, cargo, perfil e vertical
- Checkbox para promoção automática

### 2. Listagem e Gerenciamento
- Filtro dinâmico por múltiplos campos
- Ações de editar e inativar
- Carregamento assíncrono

### 3. Histórico de Promoções
- Visualização cronológica
- Edição inline de comentários
- Atualização em tempo real

### 4. Importação/Exportação
- Export para Excel com formatação
- Import com validação e relatório de resultados
- Suporte a inserção e atualização

## Como Usar

### Injeção de Dependência

csharp
// No Program.cs - já configurado
builder.Services.AddScoped<AssociadosService>();
builder.Services.AddScoped<FotoService>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<DropdownService>();
builder.Services.AddScoped<PromocaoService>();


### Uso em Componentes

csharp
@inject AssociadosService AssociadosService
@inject FotoService FotoService

// Exemplo de uso
var associados = await AssociadosService.ObterAssociadosListaAsync();
var fotoBase64 = await FotoService.ProcessarFotoAsync(file);


### Reutilização dos Serviços Common

csharp
// Em outros módulos
@inject DropdownService DropdownService
@inject ExcelService ExcelService

// Obter dados para dropdowns
var cargos = await DropdownService.ObterCargosAsync();

// Exportar dados
var (bytes, fileName) = await ExcelService.ExportarAssociadosAsync();


## Configuração

### String de Conexão

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=SistemaAvaliacao;..."
  }
}


### Mapeamento de Tabelas
As entidades mapeiam para as tabelas existentes:
- ASSOCIADOS
- PROMOCOES
- PERFIS
- VERTICAIS
- CARGOS

## Melhorias Implementadas

1. **Performance**: Blazor Híbrido com renderização automática
2. **Reutilização**: Serviços Common para uso em outros módulos
3. **Manutenibilidade**: Separação clara de responsabilidades
4. **Testabilidade**: Injeção de dependência e interfaces
5. **UX**: Interface reativa com feedback visual
6. **Segurança**: Validação de arquivos e dados

## Extensibilidade

### Adicionando Novos Campos
1. Atualizar modelo `Associado`
2. Modificar componente `AssociadosCadastro`
3. Ajustar serviços conforme necessário

### Reutilizando Serviços
Os serviços na pasta `Common` podem ser injetados em qualquer componente:

csharp
// Em um novo módulo
@inject FotoService FotoService
@inject ExcelService ExcelService


## Fluxo de Alto Nível

mermaid
flowchart TD
    UI["Blazor Components<br/>(AssociadosCadastro, Lista, Historico, ImportExport)"] -->|Injeta| SVC["Services/Associados/*Service.cs"]
    SVC -->|Reutiliza| COMMON["Services/Associados/Common/*Service.cs"]
    SVC -->|Acessa| DB[("DbContext<br/>(Entity Framework Core)")]
    COMMON -->|Pode ser usado por outros módulos| OTHERS["Outros Serviços/Componentes"]
    UI -->|Comunica| JS["JS Interop (site.js)"]
    
    subgraph "Serviços Common Reutilizáveis"
        FOTO["FotoService<br/>(Upload, Base64)"]
        EXCEL["ExcelService<br/>(Import/Export)"]
        DROP["DropdownService<br/>(Combos)"]
        PROMO["PromocaoService<br/>(Histórico)"]
    end
    
    COMMON --> FOTO
    COMMON --> EXCEL
    COMMON --> DROP
    COMMON --> PROMO
    
    subgraph "Modelos de Dados"
        ASSOC["Associado"]
        PROM["Promocao"]
        PERF["Perfil"]
        VERT["Vertical"]
    end
    
    DB --> ASSOC
    DB --> PROM
    DB --> PERF
    DB --> VERT


## Considerações de Segurança

- Validação de tipos de arquivo no upload
- Sanitização de dados de entrada
- Uso de User Secrets para strings de conexão
- Validação de permissões (a implementar)

## Próximos Passos

1. Implementar autenticação e autorização
2. Adicionar logs estruturados
3. Implementar cache para dropdowns
4. Adicionar testes unitários
5. Considerar integração com IA para sugestões de promoção
# Competências - Estrutura Migrada (.NET 9 Blazor)

## Visão Geral
Este documento descreve a estrutura dos arquivos e componentes migrados do legado ASP.NET Web Forms para Blazor Híbrido .NET 9. A lógica de negócio foi movida para serviços injetáveis, e a UI foi reescrita em componentes Blazor para máxima responsividade.

## Arquitetura da Solução

### Modelos (Models/)
- **Competencia.cs:** Modelo principal representando uma competência com todas suas propriedades e configurações
- **RelacaoCargoSubcompetencia.cs:** Modelo para relacionar cargos com subcompetências
- **Cargo.cs, Eixo.cs, SubCompetencia.cs, Dimensao.cs:** Modelos de apoio para os dropdowns
- **AvaliacaoCompetenciaNota.cs, ModoCalculoCompetencia.cs:** Modelos para configurações de avaliação
- **CompetenciaExportModel.cs:** Modelo específico para exportação Excel

### Serviços (Services/)
- **CompetenciasService:** Serviço principal para CRUD de competências
- **CargosService:** Gerencia cargos e relações cargo-subcompetência
- **EixosService, SubCompetenciasService, DimensoesService:** Serviços para entidades de apoio
- **AvaliacoesService:** Gerencia notas e modos de cálculo
- **ExportFileService:** Responsável pela geração de arquivos Excel

### Componentes Blazor (Pages/ e Components/)
- **Competencias.razor:** Página principal de cadastro/gestão
- **CompetenciaFormDesempenho.razor:** Formulário específico para competências de desempenho
- **CompetenciaFormLideranca.razor:** Formulário específico para competências de liderança
- **CompetenciasList.razor:** Lista de competências com filtros e ações

### Configuração e Infraestrutura
- **ApplicationDbContext.cs:** Context do Entity Framework com mapeamento das entidades
- **Program.cs:** Configuração da aplicação e injeção de dependência
- **appsettings.json:** Configurações da aplicação incluindo connection strings

## Principais Melhorias Implementadas

1. **Arquitetura Moderna:** Migração de Web Forms para Blazor Server com renderização híbrida
2. **Injeção de Dependência:** Todos os serviços são injetáveis e testáveis
3. **Programação Assíncrona:** Todos os métodos de acesso a dados são assíncronos
4. **Entity Framework Core:** Substituição do acesso a dados legado por EF Core 9
5. **Componentização:** UI dividida em componentes reutilizáveis e especializados
6. **Configuração Moderna:** Migração de Web.config para appsettings.json

## Funcionalidades Implementadas

- ✅ Cadastro e edição de competências (desempenho e liderança)
- ✅ Listagem de competências com status ativo/inativo
- ✅ Configurações avançadas de auto preenchimento
- ✅ Exportação para Excel
- ✅ Gerenciamento de relações cargo-subcompetência
- ⚠️ Importação de Excel (em desenvolvimento)
- ⚠️ Funcionalidade de agregação (em desenvolvimento)

## Tecnologias Utilizadas

- **.NET 9:** Framework principal
- **Blazor Server:** Para renderização híbrida e interatividade
- **Entity Framework Core 9:** ORM para acesso a dados
- **EPPlus:** Geração de arquivos Excel
- **Bootstrap:** Framework CSS para UI responsiva

## Diagrama de Alto Nível

mermaid
flowchart TD
    A[Competencias.razor] --> B[CompetenciaFormDesempenho.razor]
    A --> C[CompetenciaFormLideranca.razor]
    A --> D[CompetenciasList.razor]
    A --> E[CompetenciasService]
    E --> F[ApplicationDbContext]
    E --> G[Entity Framework Core]
    G --> H[SQL Server Database]
    A --> I[CargosService]
    A --> J[EixosService]
    A --> K[SubCompetenciasService]
    A --> L[DimensoesService]
    A --> M[AvaliacoesService]
    A --> N[ExportFileService]
    I --> F
    J --> F
    K --> F
    L --> F
    M --> F
    N --> O[EPPlus Excel Generation]
    P[Program.cs] --> Q[Dependency Injection Container]
    Q --> E
    Q --> I
    Q --> J
    Q --> K
    Q --> L
    Q --> M
    Q --> N
    R[appsettings.json] --> S[Configuration]
    S --> F


## Como Executar

1. Configure a string de conexão no `appsettings.json`
2. Execute as migrações do Entity Framework: `dotnet ef database update`
3. Execute a aplicação: `dotnet run`
4. Acesse `/competencias` para a página de gestão de competências

## Próximos Passos

1. Implementar a funcionalidade de importação de Excel
2. Implementar a funcionalidade de agregação de competências
3. Adicionar validações mais robustas nos formulários
4. Implementar testes automatizados
5. Adicionar logging estruturado
6. Implementar cache para melhor performance
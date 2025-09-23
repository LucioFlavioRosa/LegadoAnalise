# Competências - Arquitetura e Integração

Este documento descreve a arquitetura dos serviços e componentes relacionados ao cadastro, edição, importação/exportação e listagem de competências no sistema modernizado.

## Estrutura de Pastas

- `Services/Competencias/` - Serviços de negócio específicos de competências
- `Services/Competencias/Common/` - Utilitários, interfaces, validadores e DTOs reutilizáveis
- `Components/Competencias/` - Componentes Blazor para UI de competências (a serem criados)
- `docs/` - Documentação e diagramas

## Arquitetura dos Serviços

### Serviços Principais

#### CompetenciasService
Serviço principal que centraliza toda a lógica de negócio relacionada a competências:
- Cadastro e alteração de competências
- Listagem e busca de competências
- Exclusão lógica (inativação)
- Integração com importação/exportação
- Agregação de competências a avaliações existentes

#### ICompetenciasService
Interface que define os contratos do serviço principal, facilitando:
- Injeção de dependência
- Testes unitários
- Substituição de implementações

### Serviços de Apoio (Common)

#### CompetenciasValidator
Responsável pela validação de regras de negócio:
- Validação de campos obrigatórios por tipo de avaliação
- Validação de dados de importação
- Validação de consistência de dados

#### CompetenciasImportExportUtil
Utilitário para manipulação de arquivos Excel:
- Exportação de competências para Excel
- Importação de competências via Excel
- Validação de estrutura de arquivos
- Mapeamento de dados entre Excel e entidades

### DTOs (Data Transfer Objects)

#### CompetenciaDto
DTO principal para transferência de dados de competências entre camadas:
- Contém todos os campos necessários para cadastro/edição
- Inclui configurações de auto preenchimento
- Suporta tanto avaliações de desempenho quanto liderança

#### CompetenciaExportDto
DTO específico para exportação:
- Inclui dados relacionados (nomes de cargo, eixo, etc.)
- Otimizado para geração de relatórios Excel

#### CompetenciaImportDto
DTO para importação via Excel:
- Contém apenas campos essenciais para importação
- Validação simplificada para processamento em lote

#### ImportResultDto
DTO para resultado de importação:
- Estatísticas detalhadas do processo
- Lista de erros encontrados
- Mensagem resumo para feedback ao usuário

## Fluxo de Alto Nível

mermaid
flowchart TD
    UI[Blazor Page: CompetenciasPage.razor]
    sub1[CompetenciasForm.razor]
    sub2[CompetenciasList.razor]
    sub3[CompetenciasImportExport.razor]
    service[CompetenciasService]
    validator[CompetenciasValidator]
    util[CompetenciasImportExportUtil]
    db[(ApplicationDbContext)]
    cargos[CargosService]
    avaliacoes[AvaliacoesService]

    UI --> sub1
    UI --> sub2
    UI --> sub3
    sub1 --> service
    sub2 --> service
    sub3 --> util
    service --> validator
    service --> util
    service --> cargos
    service --> avaliacoes
    service --> db
    util --> db
    validator --> db


## Integração dos Serviços

### Cadastro e Edição
1. O componente de formulário injeta `ICompetenciasService`
2. Os dados são validados via `ICompetenciasValidator`
3. Para competências de desempenho, atualiza relação cargo-subcompetência
4. Persiste dados via `ApplicationDbContext`

### Listagem e Busca
1. O componente de listagem injeta `ICompetenciasService`
2. Busca competências com dados relacionados (Include)
3. Suporte a filtros por status ativo/inativo
4. Operações de inativação via serviço

### Importação/Exportação
1. O componente de import/export utiliza `CompetenciasImportExportUtil`
2. Exportação gera arquivo Excel com dados completos
3. Importação processa arquivo Excel com validação linha por linha
4. Resultado detalhado com estatísticas e erros

### Agregação de Competências
1. Funcionalidade para adicionar competência a avaliações existentes
2. Integração com `IAvaliacoesService`
3. Busca avaliações do período atual para o cargo da competência
4. Adiciona competência apenas onde não existe

## Validações Implementadas

### Competências de Desempenho
- Cargo obrigatório
- Eixo obrigatório
- Sub Competência obrigatória
- Dimensão obrigatória
- Detalhamento do Nível Atual obrigatório
- Nível Atual obrigatório

### Competências de Liderança
- Pilar (Eixo) obrigatório
- Título (Sub Competência) obrigatório
- Escopo obrigatório
- Detalhamento obrigatório

### Importação
- Validação de IDs de entidades relacionadas
- Validação de campos obrigatórios
- Verificação de duplicatas
- Tratamento de erros por linha

## Configurações e Dependências

### Injeção de Dependência
Todos os serviços são registrados no `Program.cs`:
csharp
builder.Services.AddScoped<Services.Competencias.ICompetenciasService, Services.Competencias.CompetenciasService>();
builder.Services.AddScoped<Services.Competencias.Common.ICompetenciasValidator, Services.Competencias.Common.CompetenciasValidator>();
builder.Services.AddScoped<Services.Competencias.Common.CompetenciasImportExportUtil>();


### Dependências Externas
- Entity Framework Core para acesso a dados
- EPPlus para manipulação de arquivos Excel
- Serviços de Cargos para relações cargo-subcompetência
- Serviços de Avaliações para agregação

## Padrões Utilizados

### Repository Pattern
O `ApplicationDbContext` atua como repository, centralizando acesso a dados.

### Service Layer Pattern
Lógica de negócio centralizada em serviços específicos.

### DTO Pattern
Separação clara entre modelos de domínio e objetos de transferência.

### Dependency Injection
Todos os serviços utilizam injeção de dependência para baixo acoplamento.

### Validation Pattern
Validações centralizadas em validadores específicos.

## Próximos Passos

1. **Criação dos Componentes Blazor**: Implementar os componentes de UI para substituir as páginas Web Forms
2. **Testes Unitários**: Criar testes para validadores e serviços
3. **Otimizações de Performance**: Implementar cache e otimizações de consulta
4. **Logs e Monitoramento**: Adicionar logging detalhado para auditoria
5. **Documentação de API**: Documentar endpoints se necessário para integração

## Considerações de Migração

Esta implementação mantém compatibilidade com a estrutura de dados existente, facilitando a migração incremental do sistema Web Forms para Blazor. Os serviços podem ser utilizados tanto por componentes Blazor quanto por páginas Web Forms durante o período de transição.
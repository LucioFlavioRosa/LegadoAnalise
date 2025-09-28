# Cadastro e Listagem de Períodos de Avaliação

## Visão Geral

Esta funcionalidade permite o cadastro, edição e listagem de períodos de avaliação, além de exibir avaliações sinalizadas para o próximo período. A implementação foi migrada de Web Forms para Blazor, utilizando serviços e helpers reutilizáveis, centralizados na arquitetura moderna do projeto.

## Estrutura e Integração

- **Serviços criados:**
  - `PeriodoService`: Lida com operações CRUD de períodos.
  - `PeriodoValidator`: Valida regras de negócio do formulário de períodos.
  - `PeriodoFormatHelper`: Formata datas e status de períodos.
  - `IAvaliacoesSinalizadasService`/`AvaliacoesSinalizadasService`: Obtém e atualiza avaliações sinalizadas para o próximo período.
- **Componentes:**
  - `PeriodosAvaliacao.razor`/`.razor.cs`: Componente Blazor para UI e lógica de cadastro/listagem.
- **Reutilização:**
  - Utiliza `ComboHelper` e `FormatHelper` para combos e formatação.
  - Mensagens exibidas via `MessageBoxService`.
- **Banco de Dados:**
  - Utiliza `ApplicationDbContext` para acesso a entidades `PERIODOSAVALIACOES`, `AssociadosProjetos`, etc.

## Fluxo do Processo

mermaid
flowchart TD
    PeriodosAvaliacao[PeriodosAvaliacao.razor]
    PeriodoService[PeriodoService]
    PeriodoValidator[PeriodoValidator]
    PeriodoFormatHelper[PeriodoFormatHelper]
    AvaliacoesSinalizadasService[AvaliacoesSinalizadasService]
    ComboHelper[ComboHelper]
    MessageBoxService[MessageBoxService]
    ApplicationDbContext[ApplicationDbContext]

    PeriodosAvaliacao -- Carrega combos --> ComboHelper
    PeriodosAvaliacao -- Carrega períodos --> PeriodoService
    PeriodosAvaliacao -- Carrega avaliações sinalizadas --> AvaliacoesSinalizadasService
    PeriodosAvaliacao -- Submete formulário --> PeriodoValidator
    PeriodosAvaliacao -- Submete formulário --> PeriodoService
    PeriodosAvaliacao -- Exibe mensagens --> MessageBoxService
    PeriodosAvaliacao -- Formata datas/status --> PeriodoFormatHelper
    PeriodoService -- Usa --> ApplicationDbContext
    AvaliacoesSinalizadasService -- Usa --> ApplicationDbContext


## Sugestões de Melhorias Futuras

- Implementar paginação e filtros na listagem de períodos e avaliações sinalizadas.
- Adicionar logs de auditoria para operações de cadastro/edição.
- Permitir exportação dos períodos e avaliações para Excel.
- Tornar os combos de empresa dinâmicos conforme perfil do usuário.
- Adicionar testes automatizados para serviços e helpers.
- Implementar notificações em tempo real ao cadastrar novo período.
- Internacionalizar mensagens e labels para multi-idioma.

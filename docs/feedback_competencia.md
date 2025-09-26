# Documentação: Feedback de Competências (Blazor)

## Visão Geral

Este módulo implementa o fluxo de feedback de competências migrado de Web Forms para Blazor, centralizando regras de negócio em serviços reutilizáveis e compondo a interface em componentes reativos. O componente principal é `FeedbackCompetencia.razor`, que consome o serviço `FeedbackService` e helpers de validação e formatação.

## Estrutura de Integração

- **Componentes:**
  - `Components/Feedback/FeedbackCompetencia.razor`: Interface principal do feedback de competências.
- **Serviços:**
  - `Services/Feedback/FeedbackService.cs`: Lógica de negócio para obtenção, validação e salvamento de avaliações.
  - `Services/Feedback/Common/FeedbackValidationHelper.cs`: Regras de validação reutilizáveis.
  - `Services/Feedback/Common/FeedbackComboHelper.cs`: Utilitários para combos e selects.
  - `Services/Feedback/Common/FeedbackFormatHelper.cs`: Métodos de formatação de textos, notas, percentuais.
- **Configuração:**
  - `appsettings.json`: Parâmetros de negócio, textos e limites para o fluxo de feedback.
- **DbContext:**
  - `Data/ApplicationDbContext.cs`: Persistência das entidades relacionadas a avaliações, competências, cargos, associados, etc.

## Fluxo do Processo

mermaid
flowchart TD
    Start([Usuário acessa Feedback de Competências])
    Start --> LoadPage[Carrega FeedbackCompetencia.razor]
    LoadPage --> ServiceCall[Chama FeedbackService.CarregarFeedbackCompetenciaAsync]
    ServiceCall --> DbContext[Consulta dados via ApplicationDbContext]
    DbContext --> ViewModel[Preenche FeedbackCompetenciaViewModel]
    ViewModel --> RenderUI[Renderiza UI com dados e combos]
    RenderUI --> UserInput[Usuário preenche feedbacks e notas]
    UserInput --> Validacao[Validação via FeedbackValidationHelper]
    Validacao --> SalvarBtn[Usuário clica em Salvar]
    SalvarBtn --> ServiceSave[Chama FeedbackService.SalvarFeedbackCompetenciaAsync]
    ServiceSave --> DbContextSave[Persiste alterações via ApplicationDbContext]
    DbContextSave --> MessageBox[Exibe mensagem de sucesso/erro]
    MessageBox --> End([Fim do fluxo])


## Sugestões de Melhorias

- Implementar autosave periódico do feedback, conforme configuração em `appsettings.json`.
- Adicionar exportação dos feedbacks em PDF/Excel.
- Integrar análise de sentimentos de feedbacks utilizando IA.
- Permitir comentários anônimos para feedbacks.
- Adicionar trilha de auditoria para alterações de feedback.
- Melhorar acessibilidade do componente para leitores de tela.
- Implementar testes automatizados para regras de validação e fluxo de salvamento.

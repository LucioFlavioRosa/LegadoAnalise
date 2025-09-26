# Documentação: Feedback de Competências (Blazor)

## Visão Geral

Este módulo implementa o fluxo de Feedback de Competências, migrado do Web Forms para Blazor híbrido .NET 9. A lógica de negócio foi extraída para serviços injetáveis, e a interface foi recriada como componentes Blazor modulares. Toda a configuração e mensagens estão centralizadas em `appsettings.json`.

## Estrutura de Pastas

- `Pages/Feedback/FeedbackCompetencia.razor`: Página principal do fluxo de feedback de competências.
- `Pages/Feedback/Components/CompetenciaTable.razor`: Componente reutilizável para renderização da tabela de competências.
- `Services/Feedback/FeedbackService.cs`: Serviço de regras de negócio do feedback (injeção).
- `Services/Feedback/Common/FeedbackComboHelper.cs`: Helper para combos e selects reutilizáveis.
- `Services/Feedback/Common/FeedbackValidationHelper.cs`: Helper para validações de negócio do feedback.
- `Services/Common/MessageBoxService.cs`: Serviço centralizado para mensagens ao usuário.
- `Services/Common/TelemetryService.cs`: Serviço para rastreamento de eventos e erros.
- `appsettings.json`: Configurações e mensagens do fluxo.
- `docs/FeedbackCompetencia.md`: Esta documentação.

## Integração e Funcionamento

1. **Carregamento:**
   - Ao acessar `/feedback/competencia`, o componente Blazor injeta os serviços necessários e carrega a lista de competências via `FeedbackService`.
   - O componente `CompetenciaTable` recebe a lista de competências e renderiza a tabela, com suporte a edição inline, accordions e botões de ação.

2. **Validação e Salvamento:**
   - Ao clicar em "Salvar Avaliação" ou "Finalizar Avaliação", o componente chama os métodos de validação do `FeedbackValidationHelper`.
   - Se a validação for bem-sucedida, o serviço `FeedbackService` persiste os dados.
   - Mensagens de sucesso, erro ou validação são exibidas via `MessageBoxService`.
   - Eventos e erros são rastreados via `TelemetryService`.

3. **Configuração:**
   - Todas as mensagens, textos e parâmetros de validação estão em `appsettings.json` (seção `AutoAvaliacao:FeedbackCompetencia`).

## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    Start([Usuário acessa /feedback/competencia])
    Start --> LoadPage[Carregamento inicial]
    LoadPage --> |Chama| FeedbackService
    FeedbackService --> |Retorna| Competencias
    LoadPage --> RenderTable[Renderiza CompetenciaTable]
    RenderTable --> UserAction[Usuário edita ou preenche]
    UserAction --> SaveBtn[Usuário clica em Salvar/Finalizar]
    SaveBtn --> Validate[Chama FeedbackValidationHelper]
    Validate --> |Validação OK| Persist[Chama FeedbackService para salvar/finalizar]
    Validate --> |Validação Falha| ShowMsg[Exibe mensagem via MessageBoxService]
    Persist --> ShowMsgSuccess[Exibe sucesso via MessageBoxService]
    Persist --> Telemetry[Registra evento via TelemetryService]
    ShowMsgSuccess --> End([Fim])
    ShowMsg --> End
    Telemetry --> End


## Sugestões de Melhorias

- Implementar testes automatizados de integração para o fluxo Blazor.
- Adicionar suporte a internacionalização (i18n) nas mensagens do fluxo.
- Permitir edição em lote das competências para maior produtividade.
- Integrar sugestões automáticas de feedback via IA (quando habilitado).
- Melhorar acessibilidade dos componentes para usuários com necessidades especiais.
- Adicionar logs detalhados para auditoria de alterações no feedback.

---

Para dúvidas ou contribuições, consulte o time de arquitetura ou abra um pull request.

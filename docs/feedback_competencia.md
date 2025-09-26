# Documentação: Feedback de Competências (Migração Web Forms para Blazor)

## Visão Geral

Esta documentação descreve a arquitetura, funcionamento e integração dos serviços e componentes envolvidos na migração da página de Feedback de Competências do sistema legado Web Forms para o novo modelo Blazor Híbrido .NET 9. O objetivo é garantir manutenibilidade, reutilização e clareza de fluxo.

## Estrutura de Serviços e Componentes

- **Services/Common/ComboHelper.cs**: Centraliza métodos para geração de combos reutilizáveis (tipos de avaliação, escopos, status, notas, abrangências, etc.).
- **Services/Common/FormatHelper.cs**: Centraliza métodos de formatação de percentuais, decimais, moedas e outros formatos usados em tabelas e gráficos.
- **Services/Common/MessageBoxService.cs**: Serviço para exibição de mensagens (sucesso, erro, info, warning) com eventos para integração com UI Blazor.
- **Services/Common/TelemetryService.cs**: Serviço para telemetria e rastreamento de eventos, exceções e métricas, pronto para integração cloud-native.
- **Services/Feedback/FeedbackService.cs**: Serviço de domínio para encapsular a lógica de negócio do feedback de competências, incluindo carregamento e salvamento de feedbacks.
- **Services/Feedback/Common/FeedbackValidator.cs**: Serviço reutilizável para validação de regras de negócio do feedback de competências.

## Integração e Uso

- Os serviços comuns são registrados no DI container (ver Program.cs) e podem ser injetados em qualquer componente ou serviço.
- O FeedbackService utiliza o FeedbackValidator para garantir as regras de negócio ao salvar feedbacks.
- O MessageBoxService permite exibir mensagens para o usuário de forma desacoplada da UI.
- O TelemetryService pode ser usado para rastrear eventos de uso e exceções para monitoramento e diagnóstico.
- O ComboHelper e FormatHelper são utilitários estáticos para uso em componentes e serviços.

## Fluxo do Processo

mermaid
flowchart TD
    Start([Início]) --> PaginaFeedback[FeedbackCompetencia.razor]
    PaginaFeedback -->|Carrega combos| ComboHelper
    PaginaFeedback -->|Carrega competências| FeedbackService
    FeedbackService -->|Valida regras| FeedbackValidator
    PaginaFeedback -->|Exibe mensagens| MessageBoxService
    PaginaFeedback -->|Salva feedback| FeedbackService
    PaginaFeedback -->|Rastreia eventos| TelemetryService
    FeedbackService -->|Consulta dados| ApplicationDbContext
    PaginaFeedback -->|Formata dados| FormatHelper
    PaginaFeedback -->|Exibe combos| ComboHelper


## Sugestões de Melhorias Futuras

- Integrar IA para sugerir feedbacks automáticos baseados em padrões históricos.
- Implementar testes automatizados para os serviços de domínio e validadores.
- Expandir o FeedbackValidator para suportar regras configuráveis por cargo ou tipo de avaliação.
- Criar componentes Blazor reutilizáveis para tabelas de competências e gráficos radar.
- Adicionar logs detalhados de auditoria para cada alteração de feedback.
- Internacionalizar mensagens e labels para múltiplos idiomas.

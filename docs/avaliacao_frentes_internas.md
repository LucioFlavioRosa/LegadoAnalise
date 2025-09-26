# Avaliação de Frentes Internas - Blazor Híbrido (.NET 9)

## Visão Geral

Esta documentação descreve a arquitetura, integração e fluxo dos componentes e serviços responsáveis pela tela de Avaliação de Frentes Internas, migrada de Web Forms para Blazor Híbrido. O objetivo é garantir reuso, desacoplamento e facilidade de manutenção, centralizando lógica de negócio em serviços e compondo a interface em componentes Blazor.

## Estrutura dos Componentes

- **AvaliacaoFrentesInternas.razor**: Componente principal da página. Orquestra o carregamento de dados, integração com serviços, e composição dos subcomponentes.
- **PeriodoTabs.razor**: Renderiza as abas de períodos disponíveis para avaliação. Permite seleção de período.
- **AvaliacaoCard.razor**: Exibe cada alocação interna (frente) e lista os avaliados daquela alocação.
- **AvaliadoItem.razor**: Renderiza os controles de nota, comentário e validação para cada avaliado.

## Serviços e Helpers Utilizados

- **IFrentesInternasService**: Serviço de negócio para carregar períodos, avaliações, atualizar notas, comentários e validações.
- **ComboHelper**: Fornece listas para dropdowns de notas e status.
- **UserContextService**: Obtém o usuário logado para carregar dados personalizados.
- **MessageBoxService**: Exibe mensagens de sucesso, erro e informação para o usuário.
- **TelemetryService**: Rastreia eventos e exceções para observabilidade.
- **FormatHelper, VisibilityHelper, MenuService, FooterLinksService**: Utilizados conforme necessidade para formatação, visibilidade e navegação.

## Integração e Fluxo de Dados

- Ao acessar a página, o componente principal carrega o usuário logado e busca os períodos e avaliações disponíveis via `IFrentesInternasService`.
- Os períodos são exibidos em abas por `PeriodoTabs.razor`. Ao selecionar um período, são carregadas as alocações e avaliados daquele período.
- Cada alocação é exibida como um card por `AvaliacaoCard.razor`, que lista os avaliados usando `AvaliadoItem.razor`.
- Alterações de nota, comentário ou validação disparam eventos que chamam métodos do serviço para persistência e exibem mensagens ao usuário.
- Todos os serviços comuns são injetados via DI, promovendo reuso e desacoplamento.

## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    Start([Usuário acessa /frentesinternas])
    Start --> LoadUser["Obtém usuário logado<br/>(UserContextService)"]
    LoadUser --> LoadPeriodos["Carrega períodos e avaliações<br/>(IFrentesInternasService)"]
    LoadPeriodos --> RenderTabs["Renderiza abas de períodos<br/>(PeriodoTabs.razor)"]
    RenderTabs --> SelectPeriodo["Usuário seleciona período"]
    SelectPeriodo --> LoadAlocacoes["Carrega alocações e avaliados<br/>(IFrentesInternasService)"]
    LoadAlocacoes --> RenderCards["Renderiza cards de alocação<br/>(AvaliacaoCard.razor)"]
    RenderCards --> RenderAvaliados["Renderiza avaliados<br/>(AvaliadoItem.razor)"]
    RenderAvaliados --> Interacao["Usuário altera nota/comentário/validação"]
    Interacao --> AtualizaServico["Atualiza via IFrentesInternasService"]
    AtualizaServico --> Feedback["Exibe mensagem (MessageBoxService)"]
    Feedback --> Telemetria["Rastreia evento (TelemetryService)"]


## Exemplos de Uso

- Para adicionar a tela ao menu, basta adicionar um link para `/frentesinternas`.
- Os componentes podem ser reutilizados em outros contextos de avaliação interna, bastando fornecer os modelos adequados.

## Observações

- Todos os helpers e combos são centralizados em `Services/Common` para reuso.
- O fluxo de autenticação, mensagens e telemetria é padronizado em toda a aplicação.
- Para customizações, utilize os serviços e helpers já existentes, evitando duplicação de código.

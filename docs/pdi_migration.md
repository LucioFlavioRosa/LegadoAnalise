# Migração do Preenchimento PDI: Web Forms para Blazor (.NET 9)

## Visão Geral

Este documento detalha a arquitetura, integração e fluxo do novo módulo de Preenchimento do PDI migrado do Web Forms para Blazor Web App (.NET 9), seguindo as melhores práticas de desacoplamento, reutilização e centralização de serviços.

## Arquitetura e Integração

- **Serviços de Negócio:** Toda a lógica de carregamento, atualização e validação do PDI foi extraída para o serviço `IPDIService`/`PDIService` (em `Services/PDI`), facilitando a reutilização e testabilidade.
- **Helpers Comuns:** O helper `PDIModelHelper` (em `Services/PDI/Common`) centraliza o mapeamento de entidades do banco para modelos de exibição (DTOs), promovendo padronização.
- **Modelos DTO:** Os modelos de dados para períodos, colunas, respostas e pills estão em `Services/PDI/Common/Models`.
- **DbContext:** O `ApplicationDbContext` foi atualizado para mapear as entidades `PDI_RESPOSTAS`, `PDI_QUESTOES` e `PERIODOSAVALIACOES`.
- **Componente Blazor:** A interface do usuário foi migrada para o componente `Pages/PDI/AvaliacaoPDI.razor`, utilizando data binding, navegação por pills e integração direta com os serviços via DI.
- **Serviços Comuns:** Serviços como `IUserContextService`, `IMessageBoxService` e `ITelemetryService` são utilizados para contexto de usuário, mensagens e telemetria.

## Fluxo do Processo

mermaid
flowchart TD
    Start([Usuário acessa /pdi/avaliacao])
    Start --> Auth{Usuário autenticado?}
    Auth -- Não --> ErrorMsg[Exibe erro de autenticação]
    Auth -- Sim --> LoadData[Chama PDIService.GetPdiPeriodosAsync/GetPdiPillsAsync]
    LoadData --> RenderUI[Renderiza UI com Pills e Periodos]
    RenderUI --> EditResp[Usuário edita resposta]
    EditResp --> AtualizaResp[PDIService.AtualizarRespostaAsync]
    AtualizaResp --> SuccessMsg[Exibe mensagem de sucesso]
    RenderUI --> NavegaPills[Usuário navega entre períodos]
    NavegaPills --> RenderUI


## Sugestões de Melhorias Futuras

- Implementar cache de dados para reduzir queries repetidas ao banco.
- Adicionar validação de campos no lado do cliente para respostas do PDI.
- Permitir edição em lote de respostas.
- Adicionar testes automatizados para o serviço PDIService.
- Melhorar a experiência de navegação entre períodos (ex: animações, loading incremental).
- Internacionalização dos textos e labels.
- Expor endpoints API para integração externa (mobile, BI).

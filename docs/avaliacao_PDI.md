# Documentação: Migração e Funcionamento - Página de Preenchimento PDI

## Visão Geral

A página de Preenchimento PDI foi migrada de Web Forms para Blazor (.NET 9), utilizando arquitetura de serviços reutilizáveis e componentes modulares. Toda a lógica de negócio foi extraída para um serviço injetável (`IPDIService`), facilitando manutenção, testes e reutilização. O componente Blazor `AvaliacaoPDI.razor` implementa a interface de preenchimento, navegação por períodos e edição de respostas.

## Estrutura dos Serviços e Componentes

- **Services/PDI/Common/IPDIService.cs**: Interface de contrato para operações de negócio do PDI (listar períodos, colunas, respostas, atualizar resposta, validar respostas).
- **Services/PDI/PDIService.cs**: Implementação da interface, utilizando o ApplicationDbContext para acesso a dados.
- **Services/PDI/Common/Models/**: Modelos de transporte de dados (`PDIPeriodoModel`, `PDIColunaModel`, `PDIRespostaModel`) otimizados para binding em Blazor.
- **Pages/AvaliacaoPDI.razor**: Componente Blazor responsável pela interface do PDI, navegação por períodos, exibição de colunas e edição de respostas.
- **Program.cs**: Registro do serviço `IPDIService` para injeção de dependência.
- **Data/ApplicationDbContext.cs**: Garantida a existência dos DbSets e relacionamentos para entidades do PDI.

## Fluxo de Funcionamento

mermaid
flowchart TD
    Start[Usuário acessa /avaliacao-pdi] --> CarregaPeriodos[Blazor: OnInitializedAsync carrega períodos via IPDIService]
    CarregaPeriodos -->|Para cada período| CarregaColunas[Blazor: Carrega colunas e respostas via IPDIService]
    CarregaColunas --> ExibeUI[Blazor: Exibe pills, colunas e respostas]
    ExibeUI -->|Usuário edita resposta| AtualizaResposta[Blazor: Chama AtualizarRespostaAsync do serviço]
    AtualizaResposta --> MostraMensagem[Blazor: Mostra mensagem de sucesso via IMessageBoxService]
    ExibeUI -->|Usuário troca de período| CarregaColunas


## Integração

- O componente Blazor injeta `IPDIService` e `IMessageBoxService`.
- Toda a lógica de negócio e acesso a dados é feita via o serviço, mantendo a UI desacoplada.
- Os modelos de domínio são otimizados para binding e navegação.
- O ApplicationDbContext centraliza o acesso às entidades do PDI.

## Sugestões de Melhorias

- Implementar cache para dados de períodos e questões para otimizar performance.
- Integrar SignalR para atualização em tempo real das respostas.
- Permitir edição inline com auto-save periódico.
- Adicionar testes automatizados para o serviço de negócio.
- Integrar sugestões automáticas de resposta utilizando IA.
- Modularizar ainda mais a UI criando componentes filhos para pills, colunas e respostas.
- Permitir customização de estilos e responsividade avançada para dispositivos móveis.

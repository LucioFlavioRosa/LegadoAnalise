# Avaliação Gestor Performance - Documentação Técnica

## Visão Geral

Este documento descreve a arquitetura, integração e fluxo do processo de Avaliação de Performance do Gestor, migrado do Web Forms para Blazor Híbrido .NET 9, com foco em reutilização de serviços e helpers centralizados em `Services/Common`.

### Estrutura de Serviços e Helpers

- **Data/ApplicationDbContext.cs**: Responsável pelo mapeamento das entidades de domínio, incluindo todas as tabelas e relacionamentos necessários para avaliações de performance, garantindo compatibilidade total com o legado.
- **Services/Common/ComboHelper.cs**: Centraliza métodos para geração de combos reutilizáveis (notas, status, abrangências, etc.), facilitando a manutenção e padronização dos selects em múltiplos componentes.
- **Services/Common/MessageBoxService.cs**: Serviço singleton para exibição de mensagens de sucesso, erro, aviso e informação, substituindo o antigo MessageBoxHandler.ascx.
- **Services/Common/FormatHelper.cs**: Centraliza funções utilitárias para formatação de notas, percentuais, valores e datas, garantindo padronização visual na UI.

## Integração dos Serviços

- Todos os serviços são registrados no DI container em `Program.cs`.
- Os componentes Blazor consomem os serviços via injeção (`[Inject]` ou `@inject`).
- O `ComboHelper` é utilizado para popular combos de notas e status em todas as telas de avaliação.
- O `MessageBoxService` é utilizado para exibir mensagens reativas ao usuário, substituindo controles antigos.
- O `FormatHelper` é utilizado em bindings e templates para exibir valores formatados.

## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    Start[Início - Página Avaliação Gestor Performance]
    LoadData[Carregamento de Dados via IAvaliacoesGestorService]
    ShowForm[Renderização do Formulário Blazor]
    ComboNotas[Combo de Notas via ComboHelper]
    MsgBox[Exibição de Mensagens via MessageBoxService]
    Format[Formatação de Valores via FormatHelper]
    Save[Salvar/Finalizar Avaliação]
    End[Fim]

    Start --> LoadData
    LoadData --> ShowForm
    ShowForm --> ComboNotas
    ShowForm --> MsgBox
    ShowForm --> Format
    ShowForm --> Save
    Save --> MsgBox
    Save --> End


## Sugestões de Melhorias Futuras

- **Automação de Testes:** Implementar testes automatizados para os serviços e helpers, garantindo maior robustez e cobertura.
- **Integração com IA:** Utilizar IA para sugerir feedbacks automáticos e análise de desempenho.
- **Otimização de Performance:** Avaliar uso de AOT e otimizações específicas para grandes volumes de dados.
- **Componentização Avançada:** Quebrar grandes formulários em componentes menores e reutilizáveis.
- **Internacionalização:** Preparar os helpers e mensagens para múltiplos idiomas.
- **Monitoramento Avançado:** Expandir o uso de Application Insights para rastrear eventos de uso detalhados.

---

Este documento deve ser atualizado a cada evolução relevante do fluxo de avaliação ou dos serviços comuns.
# Avaliação Gestor às Cegas - Fluxo Blazor (.NET 9)

## Visão Geral

Este documento descreve o funcionamento, integração e fluxo do processo de Avaliação do Gestor às Cegas, migrado de Web Forms para Blazor Híbrido (.NET 9). O fluxo foi refatorado para utilizar serviços e helpers reutilizáveis, centralizados em `Services/AvaliacoesGestor/Common`, e componentes Blazor para a interface.

## Estrutura e Integração

- **Serviços:**
  - `IAvaliacoesGestorService` / `AvaliacoesGestorService`: expõem métodos para carregar filtros (projetos, clientes, períodos, status), buscar avaliações, finalizar avaliações, etc.
  - `IAvaliacoesGestorHelper` / `AvaliacoesGestorHelper`: centralizam validações, manipulação de etapas, status e lógicas auxiliares.
  - Serviços comuns como `ComboHelper` e `MessageBoxService` são reutilizados para combos e mensagens.
- **Componentes Blazor:**
  - `Components/AvaliacoesGestor/AvaliacaoGestorCegas.razor`: componente principal da tela, renderiza filtros, tabela de avaliações e botões de ação.
  - `AvaliacaoGestorCegas.razor.cs`: code-behind para manipulação de eventos e ligação com os serviços.
- **Configuração:**
  - Todas as mensagens, etapas, status e limites são parametrizados em `appsettings.json` (bloco `AvaliacoesGestor`).
- **Injeção de Dependência:**
  - Todos os serviços e helpers são registrados em `Program.cs` para uso via DI.

## Fluxo de Processo (Mermaid)

```mermaid
flowchart TD
    Start([Início]) --> Filtros[Renderiza Filtros: Projetos, Clientes, Períodos, Status];
    Filtros --> BtnFiltrar[Usuário clica em "Filtrar"];
    BtnFiltrar --> CarregaAvaliacoes[Chama AvaliacoesGestorService.BuscarAvaliacoes];
    CarregaAvaliacoes --> RenderTabela[Renderiza Tabela de Projetos e Associados];
    RenderTabela --> BtnAcao[Usuário interage: Iniciar/Continuar/Ver/Finalizar Avaliação];
    
    BtnAcao -->|Finalizar| Finalizacao[Chama AvaliacoesGestorService.FinalizarAvaliacao];
    Finalizacao --> MessageBox[Exibe mensagem (sucesso/erro)];
    MessageBox --> AtualizaTabela[Atualiza tabela após ação];
    AtualizaTabela --> RenderTabela;
    
    BtnAcao -->|Visualizar| RenderTabela;
    BtnAcao -->|Iniciar/Continuar| RenderTabela;
    
    RenderTabela --> End([Fim]);
```

## Sugestões de Melhorias Futuras

- **Integração com IA:** Análise automática das avaliações para identificar padrões, inconsistências ou oportunidades de feedback personalizado.
- **Otimização de Queries:** Revisar e otimizar queries para grandes volumes de dados, usando projeções e carregamento assíncrono.
- **Melhorias UX:**
  - Adicionar loading spinners e feedback visual durante operações longas.
  - Implementar paginação e filtros avançados na tabela de avaliações.
  - Permitir exportação direta dos resultados filtrados para Excel/CSV.
- **Testes Automatizados:** Implementar testes de integração e UI para garantir robustez do fluxo.
- **Acessibilidade:** Garantir que todos os elementos da UI sejam acessíveis (teclado, leitores de tela).
- **Observabilidade:** Integrar logs detalhados de ações do usuário e erros para facilitar troubleshooting.

---

**Última atualização:** Junho/2024

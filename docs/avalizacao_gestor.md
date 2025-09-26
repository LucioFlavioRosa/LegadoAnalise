# Documentação: Avaliação do Gestor (Blazor)

## Visão Geral

Este módulo implementa a funcionalidade de Avaliação do Gestor, migrada do Web Forms para Blazor (.NET 9), centralizando toda a lógica de negócio em serviços injetáveis e utilizando componentes reutilizáveis para combos, tabelas e mensagens. O componente principal é `AvaliacoesGestor.razor`.

## Componentes e Serviços Envolvidos

- **Components/AvaliacoesGestor/AvaliacoesGestor.razor**: Interface principal da página de avaliações do gestor, com filtros, tabelas e ações.
- **Components/AvaliacoesGestor/AvaliacoesGestor.razor.cs**: Code-behind com lógica de carregamento, busca, finalização e liberação.
- **Services/AvaliacoesGestor/AvaliacoesGestorService.cs**: Serviço de negócio centralizando toda a lógica da avaliação do gestor.
- **Services/AvaliacoesGestor/Common/AvaliacoesGestorHelper.cs**: Helper para combos, status, etapas e validações específicas.
- **Services/Common/ComboHelper.cs, FormatHelper.cs, MessageBoxService.cs**: Helpers e serviços reutilizáveis para combos, formatação e mensagens.

## Integração e Fluxo de Funcionamento

O componente Blazor injeta os serviços necessários e, ao inicializar, carrega os combos de filtro e busca as avaliações do gestor e de liderados. O usuário pode filtrar, finalizar avaliações ou liberar visualização para o líder. Todas as ações são processadas via métodos assíncronos dos serviços, e mensagens de sucesso/erro são exibidas via MessageBoxService.

### Diagrama de Fluxo (Mermaid)

mermaid
flowchart TD
    Start([Início]) --> LoadCombos[Carregar Combos de Filtro]
    LoadCombos --> BuscarAvaliacoes[Buscar Avaliações do Gestor]
    BuscarAvaliacoes --> MostrarTabela[Exibir Tabela de Projetos/Avaliações]
    MostrarTabela -->|Filtrar| BuscarAvaliacoes
    MostrarTabela -->|Finalizar Avaliação| FinalizarAvaliacao[Chamar Service: FinalizarAvaliacaoAsync]
    MostrarTabela -->|Liberar Líder| LiberarLider[Chamar Service: LiberarLiderAsync]
    FinalizarAvaliacao --> BuscarAvaliacoes
    LiberarLider --> BuscarAvaliacoes
    BuscarAvaliacoes --> MostrarTabela


## Sugestões de Melhorias Futuras

- Implementar paginação e ordenação avançada nas tabelas.
- Adicionar exportação de relatórios em Excel diretamente da interface.
- Integrar com IA para análise automatizada das avaliações e sugestões de desenvolvimento.
- Adicionar testes automatizados para garantir a robustez do fluxo.
- Melhorar a experiência mobile com responsividade aprimorada.
- Permitir customização dinâmica dos filtros e colunas exibidas.

## Observações

- Toda a lógica de negócio foi extraída para serviços injetáveis, facilitando manutenção e testes.
- Os helpers comuns foram reutilizados ao máximo, seguindo a arquitetura modular proposta.
- O fluxo de permissões e visibilidade segue as regras do sistema original.
- A documentação será atualizada conforme evolução do módulo.

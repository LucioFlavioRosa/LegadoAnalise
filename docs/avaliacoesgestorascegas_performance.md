# Avaliação às Cegas - Performance (Blazor)

## Visão Geral

Este módulo implementa a funcionalidade de avaliação de performance às cegas, migrada do Web Forms para Blazor, utilizando arquitetura moderna baseada em serviços e componentes reutilizáveis. O fluxo contempla o carregamento dos dados do cabeçalho (associado, projeto, período, gestor), exibição da lista de performances agrupadas por abrangência, seleção de notas, considerações do avaliado e ações de salvar/finalizar avaliação.

## Integração dos Códigos

- **Serviço de Negócio:** Toda a lógica de negócio da avaliação de performance está centralizada no serviço `IAvaliacaoGestorasCegasPerformanceService` (Services/AvaliacoesGestorasCegas), que expõe métodos para obtenção de dados, validação e persistência.
- **Componente de Tabela:** O componente `Components/Common/PerformanceTable.razor` é reutilizável e parametrizável, recebendo a lista de performances e callbacks para alterações de nota e observação.
- **Configuração Centralizada:** Textos, estilos e mensagens são parametrizados via `appsettings.json`, facilitando manutenção e customização.
- **Mensagens ao Usuário:** O serviço `IMessageBoxService` é utilizado para feedback visual ao usuário em todas as ações críticas.
- **Registro de Serviços:** Todos os serviços necessários são registrados no `Program.cs` para injeção de dependência.

## Fluxo do Processo

mermaid
flowchart TD
    Start[Início - Navegação para Avaliação às Cegas Performance]
    LoadCabecalho[Carregar Dados do Cabeçalho (Projeto, Associado, Período, Gestor)]
    LoadPerformances[Carregar Lista de Performances]
    RenderTable[Renderizar PerformanceTable.razor]
    UserEdit[Usuário seleciona notas e preenche considerações]
    BtnSalvar[Usuário clica em 'Salvar Avaliação']
    BtnFinalizar[Usuário clica em 'Finalizar Avaliação']
    Validar[Validação dos dados]
    Salvar[Persistir dados via IAvaliacaoGestorasCegasPerformanceService]
    Feedback[Mostrar mensagem (sucesso/erro) via IMessageBoxService]
    Navegar[Redirecionar para próxima etapa ou página de resumo]
    End[Fim]

    Start --> LoadCabecalho --> LoadPerformances --> RenderTable --> UserEdit
    UserEdit --> BtnSalvar
    UserEdit --> BtnFinalizar
    BtnSalvar --> Validar --> Salvar --> Feedback --> RenderTable
    BtnFinalizar --> Validar --> Salvar --> Feedback --> Navegar --> End


## Sugestões de Melhorias

- **Validação em Tempo Real:** Implementar validação reativa dos campos de nota e considerações para evitar erros na submissão.
- **AutoSave:** Utilizar o parâmetro de auto-save do appsettings.json para salvar automaticamente as respostas do usuário em intervalos regulares.
- **Acessibilidade:** Garantir que todos os elementos da tabela e botões estejam acessíveis via teclado e leitores de tela.
- **Testes de Usabilidade:** Realizar testes com usuários para validar a experiência na navegação entre performances e uso dos accordions.
- **Internacionalização:** Permitir tradução dos textos da interface e mensagens para múltiplos idiomas.
- **Logs de Auditoria:** Integrar logs detalhados de ações do usuário para rastreabilidade e compliance.

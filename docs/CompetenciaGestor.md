# Avaliação de Competências do Gestor - Blazor

## Visão Geral

Este módulo implementa a avaliação de competências do gestor, migrando o fluxo do Web Forms para um componente Blazor moderno, com centralização de regras de negócio em serviços e helpers reutilizáveis. Toda a lógica de manipulação, validação e persistência das avaliações foi extraída para serviços injetáveis, promovendo reuso, testabilidade e manutenção facilitada.

## Estrutura dos Arquivos

- `Services/AvaliacoesGestor/AvaliacoesGestorService.cs`: Serviço principal de orquestração da avaliação de competências do gestor.
- `Services/AvaliacoesGestor/Common/AvaliacoesGestorHelper.cs`: Helper com regras de negócio, mapeamento e validação das competências.
- `Components/AvaliacoesGestor/CompetenciaGestor.razor`: Componente Blazor da interface de avaliação.
- `Components/AvaliacoesGestor/CompetenciaGestor.razor.cs`: Code-behind do componente, com integração aos serviços e lógica de UI.

## Funcionamento e Integração

1. O componente Blazor é acessado via rota `/avaliacoesgestor/competencias/{IdProjeto}/{IdAssociado}/{IdPeriodo}/{IdGestor}`.
2. Ao inicializar, o componente injeta e utiliza o `IAvaliacoesGestorService` para carregar todos os dados necessários da avaliação, incluindo projeto, associado, gestor, período e competências.
3. As competências são exibidas em uma tabela editável, com combos para notas e campos para considerações.
4. O usuário pode salvar ou finalizar a avaliação. Ambas as ações validam os dados via serviço antes de persistir.
5. Toda a lógica de validação, consistência de notas e regras de negócio está centralizada no helper reutilizável.
6. Mensagens ao usuário são exibidas via `IMessageBoxService`.

## Fluxo do Processo

mermaid
flowchart TD
    Start([Início]) --> Pagina[CompetenciaGestor.razor]
    Pagina -->|OnInitializedAsync| SVC[AvaliacoesGestorService]
    SVC -->|Carregar dados| DB[(ApplicationDbContext)]
    Pagina -->|Exibe tabela| Usuario
    Usuario -->|Edita e clica Salvar| Pagina
    Pagina -->|SalvarAvaliacao| SVC
    SVC -->|Validar e Persistir| DB
    Pagina -->|Exibe mensagem| MessageBoxService
    Usuario -->|Clica Finalizar| Pagina
    Pagina -->|FinalizarAvaliacao| SVC
    SVC -->|Valida, Persiste e Finaliza| DB
    Pagina -->|Redireciona| End([Fim])


## Sugestões de Melhorias

- Implementar paginação e filtros na tabela de competências para avaliações com grande volume.
- Adicionar logs de auditoria para rastrear alterações e finalizações.
- Permitir comentários por competência e anexos de evidências.
- Internacionalização dos textos e mensagens.
- Testes automatizados de integração e UI.
- Melhorar acessibilidade (ARIA, navegação por teclado).
- Implementar auto-save incremental para evitar perda de dados.

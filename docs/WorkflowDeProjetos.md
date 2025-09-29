# Workflow de Projetos - Blazor Híbrido (.NET 9)

## Visão Geral

Este documento detalha a arquitetura, funcionamento e integração do novo fluxo de Workflow de Projetos migrado de Web Forms para Blazor Híbrido (.NET 9). O objetivo é modernizar a interface e centralizar a lógica de negócio em serviços reutilizáveis, promovendo desacoplamento, testabilidade e facilidade de manutenção.

## Estrutura de Componentes e Serviços

- **Pages/WorkflowDeProjetos.razor**: Componente Blazor responsável pela interface do formulário e da tabela de workflows.
- **Services/Workflow/IWorkflowService.cs**: Interface para operações CRUD de workflows.
- **Services/Workflow/WorkflowService.cs**: Implementação do serviço de workflows, utilizando o ApplicationDbContext.
- **Services/Workflow/IWorkflowComboHelper.cs**: Interface para helper de combos relacionados a workflow.
- **Services/Workflow/WorkflowComboHelper.cs**: Implementação do helper para carregar combos de períodos.
- **Services/Common/IMessageBoxService.cs**: Serviço reutilizável para exibição de mensagens e avisos.
- **Data/ApplicationDbContext.cs**: Contexto EF Core, expõe DbSets para WORKFLOW e PERIODOSAVALIACOES.

## Funcionamento e Integração

1. **Carregamento Inicial**: Ao acessar a página WorkflowDeProjetos, o componente Blazor utiliza o `IWorkflowComboHelper` para carregar os períodos disponíveis e exibe o formulário para cadastro/edição de workflows.
2. **CRUD de Workflow**: As operações de inclusão, alteração, exclusão e listagem são realizadas através do `IWorkflowService`, que manipula os dados via Entity Framework Core.
3. **Mensagens**: O serviço `IMessageBoxService` é utilizado para exibir avisos, erros e confirmações ao usuário, garantindo uma experiência consistente.
4. **Atualização da UI**: Após cada operação, a lista de workflows é recarregada e o formulário é limpo ou preenchido conforme a ação (inclusão ou edição).
5. **Validação**: Toda validação de campos obrigatórios, datas e valores é realizada antes do envio ao serviço, com feedback imediato ao usuário.

## Diagrama de Fluxo (Mermaid)

mermaid
flowchart TD
    Start([Acesso à página WorkflowDeProjetos])
    Start --> LoadCombos["Carrega períodos via IWorkflowComboHelper"]
    LoadCombos --> ShowForm["Exibe formulário e tabela de workflows"]
    ShowForm --> UserAction{Usuário escolhe ação}
    UserAction -- Cadastrar/Salvar --> ValidateForm["Valida campos"]
    ValidateForm -- OK --> CallService["Chama IWorkflowService para salvar"]
    CallService --> ShowMessage["Exibe mensagem via IMessageBoxService"]
    ShowMessage --> RefreshList["Recarrega lista de workflows"]
    RefreshList --> ShowForm
    UserAction -- Alterar --> FillForm["Preenche formulário para edição"]
    FillForm --> ShowForm
    UserAction -- Deletar --> CallDelete["Chama IWorkflowService para deletar"]
    CallDelete --> ShowMessage
    ShowMessage --> RefreshList


## Sugestões de Melhorias Futuras

- Implementar paginação e filtros avançados na tabela de workflows para melhor escalabilidade.
- Adicionar logs de auditoria para rastrear alterações em workflows.
- Permitir exportação dos dados de workflow para Excel ou PDF.
- Integrar permissões baseadas em perfil de usuário para restringir ações de cadastro/edição/exclusão.
- Adicionar testes automatizados para os serviços de workflow e helpers.
- Evoluir a UI para utilizar componentes visuais mais ricos (ex: MudBlazor, Radzen).
- Implementar cache para combos de períodos, reduzindo consultas repetidas ao banco.

## Observações

- Toda a lógica de negócio foi extraída do code-behind, promovendo reutilização e desacoplamento.
- Os serviços seguem o padrão de injeção de dependência do ASP.NET Core.
- O fluxo de mensagens foi centralizado para garantir padronização na experiência do usuário.
- O ApplicationDbContext já expõe os DbSets necessários para o funcionamento do fluxo.

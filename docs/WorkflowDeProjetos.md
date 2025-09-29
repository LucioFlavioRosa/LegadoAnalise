# Workflow de Projetos - Blazor Híbrido (.NET 9)

## Visão Geral

O componente `WorkflowDeProjetos` foi migrado do modelo Web Forms para Blazor Server/Híbrido, modernizando a experiência do usuário e centralizando a lógica de negócio em serviços reutilizáveis. Toda a interface e regras de negócio foram extraídas para os serviços `IWorkflowService`, `IWorkflowComboHelper` e `IMessageBoxService`, promovendo desacoplamento, testabilidade e reuso.

## Estrutura e Integração

- **Componente:** `Pages/WorkflowDeProjetos.razor` e `Pages/WorkflowDeProjetos.razor.cs`
- **Serviços:**
  - `Services/Workflow/IWorkflowService.cs` e `WorkflowService.cs`: CRUD de workflows
  - `Services/Workflow/IWorkflowComboHelper.cs` e `WorkflowComboHelper.cs`: Carregamento de combos de períodos
  - `Services/Common/IMessageBoxService.cs`: Exibição de mensagens e avisos
- **Modelos:**
  - `WorkflowDto`: DTO para transferência de dados do workflow
  - `ComboItem`: Modelo para itens de combo reutilizável
- **DbContext:**
  - `ApplicationDbContext`: expõe `Workflows` e `PeriodosAvaliacoes` como `DbSet`

## Funcionamento do Componente

- Carrega a lista de períodos via `WorkflowComboHelper` e workflows via `WorkflowService` ao inicializar.
- Permite cadastrar, editar e excluir workflows, validando todos os campos obrigatórios.
- Exibe mensagens de sucesso, erro ou aviso usando o serviço de mensagens reutilizável.
- Atualiza a lista e limpa os campos após operações.
- O botão "Cadastrar" alterna para "Salvar" ao editar.

## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    Start([Usuário acessa /workflowdeprojetos])
    Start --> LoadCombos[Carrega combos de períodos via WorkflowComboHelper]
    Start --> LoadWorkflows[Carrega lista de workflows via WorkflowService]
    LoadCombos --> Form[Formulário de cadastro/edição]
    LoadWorkflows --> Table[Tabela de workflows]
    Form -- Submete --> Validacao[Validação dos campos]
    Validacao -- OK --> SalvaWorkflow[Chama WorkflowService para inserir/alterar]
    SalvaWorkflow --> AtualizaLista[Atualiza lista de workflows]
    AtualizaLista --> ExibeMensagem[Exibe mensagem via MessageBoxService]
    Table -- Editar --> Form
    Table -- Deletar --> DeletaWorkflow[Chama WorkflowService para deletar]
    DeletaWorkflow --> AtualizaLista
    AtualizaLista --> ExibeMensagem


## Sugestões de Melhorias Futuras

- Implementar paginação e filtros na tabela de workflows para melhor escalabilidade.
- Adicionar confirmação modal antes de excluir um workflow.
- Permitir edição inline dos prazos diretamente na tabela.
- Adicionar logs de auditoria para operações de CRUD.
- Internacionalizar mensagens e labels para múltiplos idiomas.
- Testes automatizados de UI e integração dos serviços.

## Observações

- Toda a lógica de negócio foi extraída dos code-behind antigos e centralizada em serviços reutilizáveis.
- O componente segue o padrão de injeção de dependência e data binding do Blazor.
- O serviço de mensagens é compartilhado com outros fluxos do sistema.
- O fluxo de dados e regras de validação foi mantido fiel ao sistema legado, garantindo continuidade operacional.

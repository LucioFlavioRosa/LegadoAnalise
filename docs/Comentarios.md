# Documentação: Migração da Página de Comentários para Blazor (.NET 9)

## Visão Geral

Esta documentação descreve a estrutura, funcionamento e integração dos serviços e componentes envolvidos na migração da página de comentários do sistema de avaliação interna da empresa, migrando de Web Forms para Blazor Híbrido (.NET 9).

A solução foi desenhada para garantir alta reutilização, desacoplamento e centralização das regras de negócio, seguindo o padrão de organização em `Services/Comentarios` e `Services/Comentarios/Common`.

## Estrutura dos Arquivos Envolvidos

- **Data/ApplicationDbContext.cs**: Contexto EF Core, agora com o DbSet<COMENTARIOS> e mapeamento da entidade COMENTARIOS.
- **Services/Comentarios/Common/IComentariosService.cs**: Interface para o serviço de comentários (definida em etapa anterior).
- **Services/Comentarios/ComentariosService.cs**: Implementação do serviço de comentários (definida em etapa anterior).
- **Services/Comentarios/Common/ComentariosHelper.cs**: Métodos utilitários para validação e manipulação de comentários (definido em etapa anterior).
- **Program.cs**: Registro dos serviços de comentários no DI.
- **Components/Pages/Comentarios.razor**: Componente Blazor da tela de comentários (definido em etapa anterior).
- **Components/Pages/Comentarios.razor.cs**: Code-behind do componente (definido em etapa anterior).

## Funcionamento e Integração

1. O componente `Comentarios.razor` utiliza injeção de dependência para acessar o `IComentariosService`, responsável por obter, criar e atualizar comentários do usuário logado para o período vigente.
2. O serviço utiliza o `ApplicationDbContext` para persistência e consulta dos dados na tabela `COMENTARIOS`.
3. O helper `ComentariosHelper` centraliza regras de negócio reutilizáveis, como validação de existência do comentário para o usuário/período.
4. O serviço de mensagens `IMessageBoxService` é utilizado para feedback ao usuário na interface.
5. Todos os serviços são registrados no DI em `Program.cs`, garantindo ciclo de vida adequado e integração transparente com os componentes Blazor.

## Fluxo do Processo

mermaid
flowchart TD
    A[Usuario acessa Comentarios.razor] --> B{OnInitializedAsync}
    B --> C[Obter usuário logado]
    C --> D[Chamar IComentariosService.ObterComentarioAsync]
    D --> E[ApplicationDbContext.Consultar COMENTARIOS]
    E --> F[Exibir comentário na UI]
    F --> G[Usuário edita e clica em Salvar]
    G --> H[Chamar IComentariosService.CriarOuAtualizarComentarioAsync]
    H --> I[ApplicationDbContext.Cria/Atualiza COMENTARIOS]
    I --> J[Exibir mensagem via IMessageBoxService]
    J --> K[Redirecionar ou atualizar tela]


## Sugestões de Melhorias Futuras

- Implementar testes automatizados para os serviços e componentes Blazor.
- Adicionar logs de auditoria para operações de criação/atualização de comentários.
- Permitir edição de comentários de períodos anteriores, com controle de permissão.
- Adicionar paginação e histórico de comentários por usuário.
- Melhorar a experiência do usuário com feedbacks em tempo real (ex: auto-save).
- Internacionalização das mensagens e textos da interface.
- Adicionar integração com notificações (e-mail, push) para avisar sobre comentários salvos.

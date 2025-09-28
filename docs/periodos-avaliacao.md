# Cadastro e Listagem de Períodos de Avaliação

## Visão Geral

Esta documentação descreve a arquitetura, funcionamento e integração dos serviços responsáveis pelo cadastro, edição, listagem e atualização de períodos de avaliação no sistema, migrados do Web Forms para Blazor Híbrido (.NET 9). O objetivo é centralizar a lógica de negócio, validação e formatação em serviços reutilizáveis, promovendo manutenção facilitada e reutilização em outros fluxos.

## Estrutura de Serviços e Helpers

- **Services/Periodos/PeriodoService.cs**: Serviço principal para manipulação de períodos (listar, inserir, alterar, atualizar avaliações sinalizadas).
- **Services/Periodos/Common/PeriodoValidator.cs**: Helper para validação de formulários e regras de negócio dos períodos.
- **Services/Periodos/Common/PeriodoFormatHelper.cs**: Helper para formatação de status e datas dos períodos.
- **Services/Periodos/Common/IAvaliacoesSinalizadasService.cs & AvaliacoesSinalizadasService.cs**: Interface e implementação para obtenção e atualização de avaliações sinalizadas para o próximo período.

Todos os serviços são registrados no DI em `Program.cs` e podem ser injetados em componentes Blazor ou outros serviços.

## Integração com a Aplicação

- O componente Blazor `PeriodosAvaliacao.razor` utiliza os serviços acima para exibir, cadastrar e editar períodos.
- A validação dos campos do formulário é feita via `PeriodoValidator`.
- A atualização das avaliações sinalizadas para o novo período é feita de forma centralizada, facilitando a rastreabilidade e manutenção.
- O helper de formatação permite padronizar a exibição de status e datas em toda a aplicação.
- O acesso ao banco de dados é realizado via `ApplicationDbContext`, já preparado para as entidades de períodos e avaliações.

## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    PeriodosAvaliacaoPage[PeriodosAvaliacao.razor]
    PeriodosAvaliacaoPage -- injeção --> PeriodoService
    PeriodosAvaliacaoPage -- injeção --> PeriodoValidator
    PeriodosAvaliacaoPage -- injeção --> PeriodoFormatHelper
    PeriodosAvaliacaoPage -- injeção --> AvaliacoesSinalizadasService
    PeriodoService -- usa --> ApplicationDbContext
    AvaliacoesSinalizadasService -- usa --> ApplicationDbContext
    PeriodoValidator -- valida --> PeriodoService
    PeriodoFormatHelper -- formata --> PeriodosAvaliacaoPage
    ApplicationDbContext -- entidades --> PERIODOSAVALIACOES
    ApplicationDbContext -- entidades --> AssociadosProjetos


## Sugestões de Melhorias Futuras

- **Internacionalização**: Centralizar textos de validação e mensagens em arquivos de recursos para facilitar tradução.
- **Testes Automatizados**: Implementar testes unitários para os serviços e helpers de períodos.
- **Paginação e Filtros**: Adicionar paginação e filtros avançados na listagem de períodos para melhor performance e usabilidade.
- **Logs e Auditoria**: Integrar logs detalhados e trilha de auditoria para operações críticas de cadastro e alteração de períodos.
- **Permissões Granulares**: Refinar regras de permissão para cadastro/edição de períodos conforme perfil do usuário.
- **Notificações**: Integrar notificações automáticas para usuários impactados quando um novo período é cadastrado.
- **API Pública**: Expor endpoints REST para integração com outros sistemas de RH ou BI.
- **Validações Assíncronas**: Utilizar validações assíncronas para checagem de conflitos de datas/períodos em grandes volumes.

---

Para dúvidas ou contribuições, consulte o time de arquitetura ou abra uma issue no repositório.

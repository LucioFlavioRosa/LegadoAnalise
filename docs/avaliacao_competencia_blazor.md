# Documentação: Avaliação de Competências em Blazor (.NET 9)

## Visão Geral

Esta documentação descreve a arquitetura, funcionamento, integração e fluxo do novo módulo de Avaliação de Competências migrado de Web Forms para Blazor Híbrido (.NET 9). O objetivo é modernizar a experiência do usuário, centralizar a lógica de negócio em serviços reutilizáveis e garantir manutenibilidade e extensibilidade do sistema.

## Estrutura de Componentes e Serviços

- **Componentes Blazor**: Interface da avaliação de competências, com tabelas, accordions, combos e botões, implementados em `Components/AvaliacaoCompetencia/AvaliacaoCompetencia.razor` e seu code-behind.
- **Serviços de Negócio**: Toda a lógica de carregamento, validação e persistência de avaliações foi extraída para `Services/Competencias/CompetenciasService.cs`.
- **Helpers Reutilizáveis**: Funções utilitárias (ex: truncamento de texto, validação de notas, carregamento de combos) estão em `Services/Competencias/Common/CompetenciaHelper.cs`.
- **Helpers Comuns**: Combos, mensagens e formatação são centralizados em `Services/Common/ComboHelper.cs`, `Services/Common/MessageBoxService.cs` e `Services/Common/FormatHelper.cs`.
- **Persistência**: O acesso ao banco é feito via Entity Framework Core, com as entidades mapeadas em `Data/ApplicationDbContext.cs`.

## Funcionamento e Integração

1. **Carregamento Inicial**
   - O componente Blazor é carregado e injeta os serviços necessários (`CompetenciasService`, `CompetenciaHelper`, `ComboHelper`, `MessageBoxService`).
   - Os parâmetros de contexto (projeto, associado, período, tipo de avaliação, escopo, gestor) são obtidos da URL, sessão ou contexto de navegação.
   - O serviço de competências carrega os dados do projeto, associado, período e cliente, além das competências parametrizadas.
2. **Renderização da Tabela de Competências**
   - As competências são exibidas em uma tabela responsiva, com accordions para detalhamento e botões para expandir/collapse.
   - Os combos de notas são preenchidos usando o `ComboHelper`.
   - O truncamento de textos longos é feito pelo `CompetenciaHelper`.
3. **Interação do Usuário**
   - O usuário seleciona notas e preenche considerações.
   - A validação é feita em tempo real (ex: nota do próximo nível não pode ser maior que a do nível atual, regras de "Não se aplica").
   - Mensagens de feedback são exibidas via `MessageBoxService`.
4. **Salvamento e Finalização**
   - Ao clicar em "Salvar Avaliação" ou "Finalizar", o serviço de competências valida e persiste os dados.
   - O status da avaliação é atualizado conforme o fluxo.
   - O usuário é redirecionado para a próxima etapa ou recebe confirmação de sucesso.

## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    Start([Início]) --> PaginaAvaliacaoCompetencia["AvaliacaoCompetencia.razor"]
    PaginaAvaliacaoCompetencia -->|Carrega contexto| CompetenciasService
    CompetenciasService -->|Busca dados| ApplicationDbContext
    PaginaAvaliacaoCompetencia -->|Renderiza tabela| TabelaCompetencias["Tabela de Competências"]
    TabelaCompetencias -->|Usa combos| ComboHelper
    TabelaCompetencias -->|Trunca textos| CompetenciaHelper
    PaginaAvaliacaoCompetencia -->|Exibe mensagens| MessageBoxService
    PaginaAvaliacaoCompetencia -->|Salva/Finaliza| CompetenciasService
    CompetenciasService -->|Persiste| ApplicationDbContext
    PaginaAvaliacaoCompetencia -->|Redireciona| ProximaEtapa["Próxima Página"]
    ProximaEtapa --> End([Fim])


## Integração com Outros Módulos

- **ComboHelper**: Reutilizado para todos os combos de notas, tipos de avaliação e escopos.
- **MessageBoxService**: Centraliza mensagens de feedback para o usuário.
- **FormatHelper**: Utilizado para formatação de números, percentuais e textos exibidos.
- **ApplicationDbContext**: Todas as operações de leitura e escrita usam o contexto EF Core já existente.

## Sugestões de Melhorias Futuras

- Implementar testes automatizados para os serviços e helpers de competências.
- Modularizar ainda mais os subcomponentes da tabela (ex: linha de competência, combo de nota, textarea de considerações).
- Adicionar suporte a internacionalização (i18n) para textos e mensagens.
- Melhorar a experiência mobile com responsividade avançada e navegação adaptativa.
- Utilizar SignalR para feedback em tempo real e colaboração simultânea.
- Adicionar logs de auditoria detalhados para todas as ações de avaliação.
- Permitir customização de regras de validação via configuração.
- Integrar com notificações push para lembrar o usuário de avaliações pendentes.

---

**Esta documentação deve ser mantida e atualizada a cada evolução do módulo de avaliação de competências.**

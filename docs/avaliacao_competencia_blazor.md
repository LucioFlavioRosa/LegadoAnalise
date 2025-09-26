# Avaliação às Cegas por Competência - Blazor Híbrido (.NET 9)

## Visão Geral

Esta documentação descreve a arquitetura, funcionamento e integração do novo componente Blazor para Avaliação às Cegas por Competência, migrado do Web Forms para Blazor híbrido em .NET 9. O objetivo é modernizar a experiência do usuário, centralizar regras de negócio em serviços reutilizáveis e facilitar a manutenção e evolução do sistema.

## Estrutura e Integração

- **Componente Blazor:** `Components/Avaliacoes/AvaliacaoCompetencia.razor`
- **Serviço de Domínio:** `Services/Avaliacoes/AvaliacaoCompetenciaService.cs` (registrado no DI)
- **Helpers Reutilizáveis:** `Services/Common/CompetenciasHelper.cs`, `Services/Common/ComboHelper.cs`
- **Configuração:** `appsettings.json` (mensagens, parâmetros de UI, opções de negócio)
- **Registro de Serviços:** `Program.cs`
- **Modelos de Domínio:** `Peers.Moderno.Models` e `Data/ApplicationDbContext.cs`

O componente consome o serviço de domínio para carregar, validar e salvar avaliações, utilizando helpers comuns para lógica de negócio e combos. Toda a configuração é centralizada em `appsettings.json`.

## Fluxo do Processo

mermaid
flowchart TD
    Start[Usuário acessa /avaliacao-competencia] --> LoadViewModel[Blazor chama AvaliacaoCompetenciaService.ObterAvaliacaoCompetenciaViewModelAsync]
    LoadViewModel --> RenderizaTela[Renderização da tela com dados de contexto]
    RenderizaTela --> InteracaoUsuario[Usuário preenche notas e considerações]
    InteracaoUsuario --> Salvar[Usuário clica em "Salvar Avaliação"]
    Salvar --> Validacao[Chama AvaliacaoCompetenciaService.SalvarAvaliacaoCompetenciaAsync]
    Validacao -->|Sucesso| MensagemSucesso[Exibe mensagem de sucesso]
    Validacao -->|Erro| MensagemErro[Exibe mensagem de erro]
    MensagemSucesso --> Fim[Fim]
    MensagemErro --> Fim


## Funcionamento

1. O componente é renderizado automaticamente ao acessar `/avaliacao-competencia`.
2. Os dados da avaliação, competências, notas e contexto são carregados pelo serviço de domínio.
3. O usuário pode preencher notas e considerações, conforme regras de negócio centralizadas.
4. Ao salvar, o serviço valida as regras (ex: notas obrigatórias, consistência entre níveis) e persiste os dados.
5. Mensagens de sucesso ou erro são exibidas via serviço de MessageBox reutilizável.
6. Toda a configuração de textos, opções e parâmetros é lida de `appsettings.json`.

## Sugestões de Melhorias Futuras

- Implementar testes automatizados de integração para o serviço e componente.
- Adicionar suporte a auto-save (salvamento automático) com feedback visual ao usuário.
- Integrar com IA para sugestões automáticas de notas e considerações.
- Melhorar acessibilidade e responsividade da interface.
- Permitir exportação dos dados de avaliação diretamente da tela.
- Centralizar ainda mais as mensagens e textos para facilitar internacionalização.
- Disponibilizar histórico de alterações das avaliações para auditoria.

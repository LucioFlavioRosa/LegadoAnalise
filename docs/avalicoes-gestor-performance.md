# Avaliação Gestor Performance - Arquitetura, Integração e Fluxo

## Visão Geral

Este módulo implementa a lógica de avaliação de performance do gestor, migrando o legado Web Forms para uma arquitetura moderna baseada em Blazor e serviços reutilizáveis. Toda a lógica de negócio foi extraída para serviços e helpers injetáveis, facilitando manutenção, testes e reutilização em outras telas do sistema.

## Estrutura dos Serviços

- **Services/AvaliacoesGestor/Common/IAvaliacoesGestorService.cs**: Interface que define os contratos para carregamento, salvamento e validação das avaliações de performance do gestor.
- **Services/AvaliacoesGestor/Common/AvaliacoesGestorService.cs**: Implementação concreta da interface, centralizando toda a lógica de negócio e integração com o ApplicationDbContext.
- **Services/AvaliacoesGestor/Common/AvaliacoesGestorHelper.cs**: Helper utilitário para truncagem de textos, validação de preenchimento e manipulação de listas de performances.
- **Services/Common/ComboHelper.cs**: Centraliza métodos para geração de combos reutilizáveis em toda a aplicação.
- **Services/Common/MessageBoxService.cs**: Serviço para exibição padronizada de mensagens de sucesso, erro, aviso e informação.
- **Services/Common/FormatHelper.cs**: Funções utilitárias para formatação de notas, percentuais, valores e datas.

## Integração com o Blazor

Os serviços e helpers acima são injetados nos componentes Blazor responsáveis pela interface da avaliação de performance do gestor. O componente principal (exemplo: `Pages/AvaliacoesGestorPerformance.razor`) consome os métodos do serviço para carregar dados, salvar avaliações e validar inputs. Toda a configuração é lida de `appsettings.json`.

## Fluxo do Processo

mermaid
flowchart TD
    Start[Início: Página Avaliação Gestor Performance] --> CarregaDados[Chama IAvaliacoesGestorService.ObterDadosAvaliacaoGestorPerformanceAsync]
    CarregaDados --> RenderizaUI[Renderiza grid de performances e dados do avaliado]
    RenderizaUI --> UsuarioEdita[Usuário preenche notas e observações]
    UsuarioEdita --> Salva[Usuário clica em Salvar ou Finalizar]
    Salva --> ChamaSalvar[Chama IAvaliacoesGestorService.SalvarAvaliacoesGestorPerformanceAsync]
    ChamaSalvar --> MessageBox[Exibe mensagem via MessageBoxService]
    MessageBox --> Fim[Fim]


## Sugestões de Melhorias Futuras

- Implementar caching para carregamento de listas de performances e combos, reduzindo consultas ao banco.
- Integrar sugestões automáticas de feedback usando IA (Azure OpenAI ou similar).
- Adicionar testes automatizados para serviços e helpers.
- Otimizar queries para grandes volumes de dados.
- Melhorar a experiência do usuário com autosave visual e feedback em tempo real.
- Internacionalização dos textos e mensagens para múltiplos idiomas.

# Documentação: Avaliação de Competências do Gestor (Blazor .NET 9)

## Visão Geral

Este documento descreve o funcionamento, integração e fluxo do processo de avaliação de competências do gestor, migrado de Web Forms para Blazor Híbrido (.NET 9). A solução foi modernizada para centralizar regras de negócio, validações e helpers em serviços reutilizáveis, com configuração dinâmica via appsettings.json.

## Estrutura de Integração

- **Componentes Blazor**: UI da avaliação de competências do gestor implementada em `Components/AvaliacoesGestor/CompetenciaGestor.razor` e seu code-behind.
- **Serviços de Domínio**: Toda lógica de negócio, validação e manipulação de competências foi extraída para:
  - `Services/AvaliacoesGestor/AvaliacoesGestorService.cs`: Orquestra obtenção, validação e persistência das avaliações.
  - `Services/AvaliacoesGestor/Common/AvaliacoesGestorHelper.cs`: Helpers para fluxo, etapas e cálculos.
  - `Services/Common/CompetenciaHelper.cs`: Manipulação de competências, truncamento de texto, regras de negócio.
  - `Services/Common/ValidationHelper.cs`: Validações comuns e de negócio.
  - `Services/Common/ComboHelper.cs`: Opções de combos (notas, tipos, escopos).
  - `Services/Common/MessageBoxService.cs`: Exibição padronizada de mensagens.
- **Configuração**: Mensagens, parâmetros e regras de negócio centralizados em `appsettings.json`.
- **Persistência**: Entidades e contexto garantidos em `Data/ApplicationDbContext.cs`.

## Funcionamento

1. **Renderização**: O componente Blazor é carregado, buscando dados de contexto (projeto, associado, período, gestor, tipo de avaliação, escopo) via serviços.
2. **Carregamento de Competências**: O serviço de avaliações do gestor obtém as competências e avaliações do banco, monta os modelos de exibição e aplica regras de negócio (ex: títulos, truncamento, visibilidade de campos).
3. **Validação**: Ao salvar ou finalizar, o componente utiliza os helpers de validação para garantir consistência das notas, obrigatoriedade de preenchimento, regras de negócio (ex: nota do próximo nível não pode ser maior que a do atual, obrigatoriedade de pelo menos uma nota mensurável por pilar/subcompetência).
4. **Persistência**: O serviço de avaliações salva as alterações no banco, atualizando etapas do fluxo e status das avaliações.
5. **Mensagens**: Todas as mensagens de sucesso, erro ou alerta são exibidas via MessageBoxService, com textos parametrizados do appsettings.json.
6. **Configuração Dinâmica**: Parâmetros de negócio, limites e textos podem ser ajustados no appsettings.json sem recompilar o sistema.

## Fluxo do Processo

mermaid
flowchart TD
    Start([Início]) --> PaginaAvalGestor["Página: CompetenciaGestor.razor"]
    PaginaAvalGestor -->|Carrega contexto e dados| AvaliacoesGestorService
    AvaliacoesGestorService -->|Busca competências, avaliações e regras| ApplicationDbContext
    PaginaAvalGestor -->|Renderiza UI, combos, accordions| UIHelpers
    PaginaAvalGestor -->|Validações| ValidationHelper
    PaginaAvalGestor -->|Exibe mensagens| MessageBoxService
    PaginaAvalGestor -->|Salva/Finaliza| AvaliacoesGestorService
    AvaliacoesGestorService -->|Atualiza banco| ApplicationDbContext
    PaginaAvalGestor -->|Configuração dinâmica| AppSettingsJson
    AppSettingsJson -.->|Mensagens, limites, ids padrão| PaginaAvalGestor
    ApplicationDbContext -.->|Persistência| BancoDados[(SQL Server)]
    subgraph Serviços Comuns
        UIHelpers["CompetenciaHelper, ComboHelper"]
        ValidationHelper
        MessageBoxService
    end
    subgraph Configuração
        AppSettingsJson["appsettings.json"]
    end
    subgraph Persistência
        ApplicationDbContext
        BancoDados
    end


## Sugestões de Melhorias Futuras

- **Internacionalização (i18n)**: Centralizar textos e mensagens para permitir fácil tradução e suporte a múltiplos idiomas.
- **Testes Automatizados**: Implementar testes unitários e de integração para os serviços e helpers.
- **Feedback em Tempo Real**: Utilizar SignalR para feedback instantâneo de alterações e status de avaliação.
- **Aprimoramento de UI/UX**: Adicionar loading spinners, animações de accordions e feedback visual mais rico.
- **Auditoria Detalhada**: Logar todas as ações relevantes para rastreabilidade e compliance.
- **Permissões Granulares**: Expandir regras de permissão e visibilidade conforme perfil do usuário.
- **Integração com IA**: Explorar sugestões automáticas de notas ou feedbacks baseadas em IA, conforme já previsto na arquitetura.
- **Cache Otimizado**: Utilizar cache distribuído para melhorar performance em grandes volumes.
- **Customização Avançada via Configuração**: Permitir ajustes de regras de negócio e layout diretamente via appsettings.json ou painel administrativo.

---

**Esta documentação deve ser atualizada a cada evolução relevante do fluxo de avaliação de competências do gestor.**

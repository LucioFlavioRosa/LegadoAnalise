# Avaliação Gestor Performance (Migração Web Forms → Blazor Híbrido)

## Visão Geral

Este documento detalha a arquitetura, funcionamento e integração do novo fluxo de Avaliação de Performance do Gestor, migrado de Web Forms para Blazor Híbrido (.NET 9), centralizando lógica de negócio em serviços reutilizáveis e promovendo manutenibilidade, testabilidade e performance.

## Componentes e Serviços Envolvidos

- **Pages/AvaliacoesGestorPerformance.razor**: Componente Blazor responsável pela interface da avaliação de performance do gestor.
- **Services/AvaliacoesGestor/Common/IAvaliacoesGestorService.cs**: Interface para operações de negócio da avaliação de performance do gestor.
- **Services/AvaliacoesGestor/Common/AvaliacoesGestorHelper.cs**: Helper com funções utilitárias para manipulação e validação de dados.
- **Services/Common/ComboHelper.cs**: Métodos para carregamento de combos reutilizáveis (notas, abrangências, status).
- **Services/Common/MessageBoxService.cs**: Serviço para exibição padronizada de mensagens na UI.
- **Services/Common/FormatHelper.cs**: Funções para formatação de valores, datas e percentuais.
- **appsettings.json**: Centraliza todas as configurações de negócio, mensagens e parâmetros do fluxo.
- **Program.cs**: Registro dos serviços IAvaliacoesGestorService e AvaliacoesGestorHelper no container de DI.
- **Data/ApplicationDbContext.cs**: Mapeamento das entidades de Avaliação de Performance, garantindo persistência e integridade dos dados.

## Fluxo do Processo

mermaid
flowchart TD
    Start([Início]) --> Pagina[AvaliacoesGestorPerformance.razor]
    Pagina -->|Carrega dados| SvcAvaliacoesGestor[IAvaliacoesGestorService]
    Pagina -->|Carrega combos| ComboHelper[ComboHelper]
    Pagina -->|Formata valores| FormatHelper[FormatHelper]
    Pagina -->|Exibe mensagens| MessageBoxService[MessageBoxService]
    Pagina -->|Salva/Finaliza| SvcAvaliacoesGestor
    SvcAvaliacoesGestor -->|Acessa dados| DbContext[ApplicationDbContext]
    SvcAvaliacoesGestor -->|Valida/Trunca| AvaliacoesGestorHelper[AvaliacoesGestorHelper]
    Pagina -->|Lê configuração| AppSettings[appsettings.json]
    End([Fim])


### Descrição do Fluxo
1. O componente Blazor é iniciado e lê as configurações do fluxo no `appsettings.json`.
2. Dados da avaliação, combos de notas e abrangências são carregados via IAvaliacoesGestorService e ComboHelper.
3. O usuário interage com o formulário, que utiliza FormatHelper para exibir valores e MessageBoxService para feedback.
4. Ao salvar/finalizar, a lógica de negócio é executada via IAvaliacoesGestorService, que utiliza AvaliacoesGestorHelper para validações e ApplicationDbContext para persistência.

## Integração e Reutilização
- **Serviços e helpers** são injetados via DI, promovendo reutilização e desacoplamento.
- **Configurações** centralizadas em `appsettings.json` permitem ajustes sem recompilar o código.
- **ComboHelper, MessageBoxService e FormatHelper** são utilizados em múltiplos fluxos de avaliação, evitando duplicidade.

## Sugestões de Melhorias Futuras
- Implementar testes automatizados para os serviços e helpers.
- Integrar sugestões automáticas de feedback utilizando IA (Azure OpenAI ou similar).
- Otimizar o carregamento de dados com caching e lazy loading.
- Adicionar logs de auditoria detalhados para rastreabilidade.
- Permitir customização de combos e mensagens via painel administrativo.
- Melhorar acessibilidade e responsividade da interface.
- Implementar notificações em tempo real para status de avaliação.

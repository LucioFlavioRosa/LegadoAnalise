# Documentação: Consolidação de Avaliação - Serviços e Integração

## Visão Geral

A Consolidação de Avaliação foi migrada de Web Forms para uma arquitetura moderna baseada em Blazor e serviços reutilizáveis. Toda a lógica de negócio foi centralizada no serviço `ConsolidacaoService`, exposto pela interface `IConsolidacaoService` na pasta `Services/Consolidacao/Common`. Os modelos de dados são compatíveis com o Entity Framework Core e a integração com o banco de dados ocorre via `ApplicationDbContext`.

## Estrutura dos Serviços

- **IConsolidacaoService**: Interface que define todos os métodos necessários para a consolidação de avaliação, incluindo obtenção de competências, performance, cálculo de notas, carregamento e salvamento de considerações do mentor e geração de dados para gráficos radar.
- **ConsolidacaoService**: Implementação concreta da interface, utilizando o `ApplicationDbContext` para acesso a dados e serviços comuns para telemetria e mensagens.
- **Modelos DTO**: Estruturas como `ResultadoCompetenciaModel`, `ResultadoPerfomanceModel` e `ConsideracoesMentorDto` padronizam a comunicação entre camadas.

## Integração com Componentes Blazor

Os métodos do serviço podem ser injetados diretamente em componentes Blazor, facilitando a obtenção e atualização dos dados da consolidação. A lógica de apresentação (UI) deve consumir apenas os métodos da interface, promovendo desacoplamento e reutilização.

## Fluxo de Processo (Mermaid)

mermaid
flowchart TD
    Start([Início])
    A[Usuário acessa página de Consolidação]
    B[Blazor injeta IConsolidacaoService]
    C[Obter Competências e Performance]
    D[Renderizar Tabelas e Accordions]
    E[Usuário edita notas/comentários]
    F[Salvar Considerações Mentor]
    G[Calcular Notas Comitê]
    H[Gerar Radar JSON para gráficos]
    I[Fim]

    Start --> A --> B --> C --> D --> E
    E --> F
    E --> G
    C --> H
    F --> I
    G --> I
    H --> I


## Sugestões de Melhorias Futuras

- Implementar cache para dados de consolidação para melhorar performance em grandes volumes.
- Adicionar testes automatizados para todos os métodos de serviço.
- Expandir o serviço para suportar múltiplos tipos de avaliação (ex: liderança, feedback).
- Criar eventos de domínio para notificação em tempo real de alterações.
- Integrar com serviços de exportação para geração de relatórios em PDF/Excel diretamente da UI.
- Melhorar tratamento de erros e mensagens amigáveis ao usuário.
- Modularizar ainda mais os DTOs para facilitar manutenção e evolução.

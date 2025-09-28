# Documentação Técnica: Resultado de Avaliação de Liderança

## Visão Geral

Esta documentação descreve a arquitetura, funcionamento e integração dos serviços criados para a tela de Resultado de Avaliação de Liderança migrada de Web Forms para Blazor. O objetivo é centralizar a lógica de negócio e utilitários em serviços reutilizáveis, facilitando a manutenção, testes e evolução do sistema.

## Estrutura dos Serviços

- **Services/ResultadoLideranca/ResultadoLiderancaService.cs**: Serviço principal responsável por fornecer filtros, montar o painel de resultados, gerar dados para gráficos e acionar notificações para líderes. Expõe métodos assíncronos para consumo por componentes Blazor e outras partes do sistema.
- **Services/ResultadoLideranca/Common/ResultadoLiderancaHelper.cs**: Helper estático com funções utilitárias para formatação, cálculo de médias, truncamento de texto, extração de palavras-chave e montagem de dados de gráficos e tabelas.

## Integração

- O serviço `IResultadoLiderancaService` deve ser registrado no DI container e injetado nos componentes Blazor que implementam a tela de resultado de liderança.
- O helper pode ser utilizado diretamente por outros serviços ou componentes para reaproveitar lógica de formatação e cálculo.
- Os modelos de dados definidos nos arquivos podem ser compartilhados entre backend e frontend para tipagem forte e consistência.

## Fluxo de Uso (Mermaid)

mermaid
flowchart TD
    subgraph UI
        RL[ResultadoLideranca.razor]
        Filtros[Componentes de Filtros]
        Graficos[Componentes de Gráficos]
        Tabelas[Componentes de Tabelas]
    end
    RL --> |"Injeta IResultadoLiderancaService"| SVC[ResultadoLiderancaService]
    SVC --> |"Usa"| HLPR[ResultadoLiderancaHelper]
    SVC --> |"Consulta"| DB[(ApplicationDbContext)]
    SVC --> |"Retorna modelos de resultado"| RL
    Filtros --> RL
    Graficos --> RL
    Tabelas --> RL


## Sugestões de Melhorias Futuras

- Implementar cache para os resultados dos filtros e painéis, reduzindo consultas repetidas ao banco de dados.
- Integrar serviço real de envio de e-mails para notificações de líderes, utilizando um provider configurável.
- Padronizar todos os modelos de dados em uma biblioteca compartilhada para uso em frontend e backend.
- Adicionar testes automatizados para os métodos do serviço e helper, garantindo cobertura e qualidade.
- Evoluir o helper para suportar internacionalização (i18n) e diferentes formatos de exibição.
- Integrar logs estruturados para rastreabilidade de uso dos serviços.

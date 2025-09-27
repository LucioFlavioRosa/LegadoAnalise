# Documentação: Avaliação de Performance do Mentor

## Visão Geral

Este módulo centraliza a lógica de apresentação e manipulação de dados da avaliação de performance do mentor, migrando funcionalidades do Web Forms para Blazor Híbrido. Utiliza serviços reutilizáveis em `Services/Common` para garantir desacoplamento e facilidade de manutenção, especialmente para accordions, truncamento de texto e exibição de mensagens.

### Componentes e Serviços Envolvidos
- **AccordionHelper** (`Services/Common/AccordionHelper.cs`): Centraliza a lógica de truncamento de texto, controle de accordions e estilização de linhas ocultas.
- **MessageBoxService** (`Services/Common/MessageBoxService.cs`): Gerencia exibição de mensagens de sucesso, erro, informação e aviso, com cobertura para cenários customizados.
- **MentorPerformance.razor**: Componente Blazor que consome os serviços acima para renderizar a tabela de performance, accordions e mensagens ao usuário.

## Funcionamento e Integração

- O componente de avaliação injeta `AccordionHelper` para truncar descrições longas e controlar o estado dos accordions, garantindo responsividade e clareza na UI.
- Mensagens de validação, erro ou sucesso são exibidas via `MessageBoxService`, que dispara eventos para serem capturados por componentes visuais.
- Toda lógica de UI que pode ser reaproveitada (ex: truncamento, accordions, exibição de mensagens) está centralizada em `Services/Common`, seguindo o padrão do projeto.

## Diagrama de Fluxo (Mermaid)

mermaid
flowchart TD
    A[Page: MentorPerformance.razor] -->|Carrega dados| B(PerformanceMentorService)
    B --> C[AccordionHelper]
    B --> D[MessageBoxService]
    C --> E[Renderiza accordions/tabela]
    D --> F[Exibe mensagens]
    A --> E
    A --> F


## Sugestões de Melhorias

- **Internacionalização:** Centralizar textos exibidos em mensagens e botões para facilitar tradução e adaptação regional.
- **Customização de Accordions:** Permitir configuração dinâmica do tamanho de truncamento e textos de expansão via appsettings ou parâmetros de componente.
- **Testes de Usabilidade:** Adicionar métricas de uso dos accordions e mensagens para identificar pontos de melhoria na experiência do usuário.
- **Integração com IA:** Futuramente, integrar análise automática de feedbacks e sugestões de performance via serviços de IA.
- **Exportação de Relatórios:** Permitir exportação dos dados de performance diretamente da interface Blazor, aproveitando os serviços de exportação já existentes.

## Observações

- Toda lógica criada está desacoplada e pronta para reuso em outros componentes do sistema.
- O fluxo de dados e regras de negócio permanece íntegro, conforme exigido pelo plano de migração.

# Documentação: Avaliação de Competência do Mentor (Blazor Híbrido)

## Visão Geral
Esta documentação cobre a implementação dos componentes Blazor `ResultadosAccordion.razor` e `DadosRHAccordion.razor`, que fazem parte do fluxo de avaliação de competência do mentor. Os componentes foram desenhados para modularizar a interface, facilitar a manutenção e garantir máxima reutilização de código e serviços, seguindo o padrão de centralização em `Services/Common`.

### Componentes Criados
- **ResultadosAccordion.razor**: Renderiza os resultados do semestre, consolidado e por projeto, exibindo tabelas de competências, performance e soma de resultados.
- **DadosRHAccordion.razor**: Exibe informações sensíveis do RH (ação comitê, salários, pontos fortes/fracos, regimes de contratação), com lógica de visibilidade baseada na liberação do RH.

### Integração de Serviços
Ambos os componentes utilizam o serviço `IAvaliacaoMentorCompetenciaService` para buscar dados do backend, e helpers para formatação e truncamento de textos. O serviço é registrado via DI em `Program.cs` e utiliza os modelos e entidades mapeados no `ApplicationDbContext`.

## Fluxo do Processo
mermaid
flowchart TD
    A[avalizacao_mentor_competencia.razor] --> B[ResultadosAccordion.razor]
    A --> C[DadosRHAccordion.razor]
    B --> D[IAvaliacaoMentorCompetenciaService]
    C --> D
    D --> E[ApplicationDbContext]
    D --> F[AvaliacaoMentorCompetenciaHelper]


## Funcionamento
- O componente principal (`avalizacao_mentor_competencia.razor`) injeta e utiliza os componentes filhos para renderizar as seções de resultados e dados RH.
- Cada componente filho faz chamadas assíncronas ao serviço de negócio para buscar os dados necessários e renderizar as tabelas e informações.
- Helpers são utilizados para formatação de percentuais, truncamento de texto e validação de dados.
- A visibilidade dos dados RH é controlada pela propriedade `LiberadoRH` do modelo retornado.

## Sugestões de Melhorias Futuras
- **Adicionar testes automatizados para os serviços e componentes.**
- **Implementar loading e feedback visual aprimorado para operações assíncronas.**
- **Centralizar ainda mais as validações e formatações em helpers reutilizáveis.**
- **Permitir edição inline dos dados do RH e resultados, com validação em tempo real.**
- **Expandir o uso de gráficos interativos para visualização dos resultados.**
- **Internacionalização dos textos e labels para suporte multilíngue.**
- **Documentar exemplos de uso dos serviços e componentes para onboarding de novos desenvolvedores.**

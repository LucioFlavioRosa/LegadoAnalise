# Documentação: Avaliação de Competência do Mentor

## Visão Geral

Esta documentação detalha o funcionamento, integração e fluxo da nova implementação da página de Avaliação de Competência do Mentor, migrada de Web Forms para Blazor Híbrido (.NET 9). O objetivo é centralizar a lógica de negócio em serviços reutilizáveis, modularizar a interface em componentes Blazor e garantir máxima compatibilidade com o banco de dados existente, seguindo os padrões de arquitetura definidos para o projeto.

## Estrutura de Componentes e Serviços

- **Serviços**
  - `Services/AvaliacaoMentorCompetencia/AvaliacaoMentorCompetenciaService.cs`: Serviço principal que encapsula toda a lógica de obtenção, cálculo e persistência das avaliações de competência do mentor.
  - `Services/AvaliacaoMentorCompetencia/Common/IAvaliacaoMentorCompetenciaService.cs`: Interface para o serviço principal, permitindo fácil substituição e mock em testes.
  - `Services/AvaliacaoMentorCompetencia/Common/AvaliacaoMentorCompetenciaHelper.cs`: Classe helper para funções utilitárias como formatação de percentuais, truncamento de texto e validação de inputs.

- **Componentes Blazor**
  - `Components/AvaliacaoMentorCompetencia/AvaliacaoMentorCompetencia.razor`: Componente principal da página, utilizando diretiva `@rendermode InteractiveAuto`.
  - `Components/AvaliacaoMentorCompetencia/CompetenciasAccordion.razor`: Renderiza a tabela de competências, com suporte a expand/collapse, vinculação de notas e observações.
  - `Components/AvaliacaoMentorCompetencia/ResultadosAccordion.razor`: Renderiza os resultados do semestre, consolidado e por projeto, com suporte a gráficos.
  - `Components/AvaliacaoMentorCompetencia/ConsideracoesMentorAccordion.razor`: Seção de considerações do mentor, incluindo formulários editáveis, dropdowns, checkboxes e botão de salvar.
  - `Components/AvaliacaoMentorCompetencia/DadosRHAccordion.razor`: Exibe informações sensíveis do RH, com lógica de visibilidade.

- **Banco de Dados**
  - O `ApplicationDbContext` foi revisado para garantir que todas as entidades envolvidas na avaliação de competência do mentor estejam corretamente mapeadas, sem remoção de funcionalidades existentes.

- **Configuração**
  - O arquivo `appsettings.json` foi revisado para garantir que todas as configurações necessárias para limites, validações e recursos estejam presentes e documentadas, incluindo seções específicas para Avaliação de Competência do Mentor, FeedbackPerformance, AvaliacoesGestor, Mentoria, entre outros.

## Fluxo de Processo (Mermaid)

mermaid
flowchart TD
    A[avalizacao_mentor_competencia.razor] --> B[CompetenciasAccordion.razor]
    A --> C[ResultadosAccordion.razor]
    A --> D[ConsideracoesMentorAccordion.razor]
    A --> E[DadosRHAccordion.razor]
    A --> F[Services/AvaliacaoMentorCompetencia/AvaliacaoMentorCompetenciaService]
    F --> G[Services/AvaliacaoMentorCompetencia/Common/IAvaliacaoMentorCompetenciaService]
    F --> H[Services/AvaliacaoMentorCompetencia/Common/AvaliacaoMentorCompetenciaHelper]
    F --> I[Data/ApplicationDbContext]
    A --> J[appsettings.json]


## Integração dos Códigos

- Os componentes Blazor consomem os serviços via injeção de dependência, permitindo desacoplamento da lógica de negócio da interface.
- O serviço principal (`AvaliacaoMentorCompetenciaService`) interage com o `ApplicationDbContext` para obter e persistir dados relacionados à avaliação de competências, considerações do mentor, radar de competências e resultados do semestre.
- Helpers centralizam funções utilitárias para formatação, validação e manipulação de dados, promovendo reutilização em múltiplos componentes e serviços.
- As configurações do sistema, como limites de exportação/importação, mensagens de validação e parâmetros de negócio, são mantidas em `appsettings.json` para fácil ajuste e governança.

## Sugestões de Melhorias Futuras

1. **Automatizar Testes de Integração:** Implementar testes automatizados para garantir o correto funcionamento dos fluxos de avaliação, persistência e exibição dos dados.
2. **Internacionalização:** Adicionar suporte multilíngue para as mensagens e labels exibidas na interface, utilizando recursos do Blazor.
3. **Melhorar Performance de UI:** Avaliar o uso de virtualização em tabelas e listas para otimizar a renderização de grandes volumes de dados.
4. **Integração com AI:** Expandir o uso dos recursos de AI para sugestões automáticas de considerações do mentor e análise de competências.
5. **Auditoria e Log:** Implementar logs detalhados de ações do usuário e alterações nas avaliações para fins de auditoria e conformidade.
6. **Melhor UX para Mobile:** Adaptar os componentes para melhor experiência em dispositivos móveis, utilizando responsividade avançada.
7. **Documentação Técnica Dinâmica:** Utilizar ferramentas de documentação dinâmica para manter o manual sempre atualizado conforme evolução do código.

## Referências

- [Documentação Oficial Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Mermaid Diagrams](https://mermaid-js.github.io/mermaid/#/)

---

**Esta documentação deve ser revisada e expandida conforme novas funcionalidades forem migradas ou criadas.**

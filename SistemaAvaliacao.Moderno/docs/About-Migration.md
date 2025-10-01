# Migração da Página About – SistemaAvaliacao.Moderno (.NET 9 Blazor)

## Visão Geral

Esta documentação detalha o processo de migração da página informativa About do sistema de avaliação interna, originalmente implementada em ASP.NET Web Forms, para o novo paradigma Blazor Web App (.NET 9) com renderização híbrida (InteractiveAuto). O objetivo foi modernizar a interface, promover reutilização de componentes e garantir escalabilidade futura.

## Arquitetura de Componentes

- **MainLayout.razor**: Layout principal, substitui o antigo Site.Master. Aplica estrutura base (header, main, footer) a todas as páginas.
- **PageHeader.razor**: Componente reutilizável para títulos e subtítulos de páginas, utilizado em About e outras futuras páginas.
- **About.razor**: Página About migrada, utiliza PageHeader e segue o novo padrão de roteamento Blazor.

## Fluxo de Renderização

mermaid
flowchart TD
    A[Usuário acessa /about] --> B[Blazor Router]
    B --> C[About.razor carregado]
    C --> D[MainLayout.razor aplicado]
    D --> E[PageHeader.razor renderizado]
    E --> F[Conteúdo estático renderizado]
    F --> G[Página exibida ao usuário]
    style C fill:#e1f5ff
    style E fill:#ffe1f5


## Componentes Reutilizáveis Criados

- **PageHeader.razor**
  - **Propósito:** Exibir título e subtítulo em páginas informativas.
  - **Parâmetros:** `Title` (string), `Subtitle` (string)
  - **Exemplo de Uso:** `<PageHeader Title="About" Subtitle="Your application description page." />`
  - **Localização:** `Components/Shared/PageHeader.razor`

## Integração dos Códigos

- O arquivo `Program.cs` foi configurado para habilitar renderização híbrida.
- O CSS global foi migrado para `wwwroot/css/site.css`, garantindo visual moderno e responsivo.
- O componente `About.razor` utiliza o layout e o cabeçalho reutilizável, promovendo consistência e facilidade de manutenção.

## Sugestões de Melhorias

1. **Internacionalização (i18n):** Implementar suporte a múltiplos idiomas usando `IStringLocalizer`.
2. **Conteúdo Dinâmico:** Permitir que o conteúdo da página About seja carregado de um CMS ou banco de dados via serviço injetável.
3. **Acessibilidade:** Adicionar atributos ARIA e garantir navegação por teclado para conformidade com WCAG 2.1.
4. **SEO:** Utilizar `HeadContent` para meta tags dinâmicas e otimização de busca.
5. **Analytics:** Integrar rastreamento de página via JavaScript Interop para monitoramento de uso.

---

Esta documentação deve ser consultada por toda a equipe envolvida na migração e manutenção do sistema, servindo como referência para futuras páginas e componentes.
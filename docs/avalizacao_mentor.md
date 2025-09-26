# Documentação: Migração e Estrutura da Página de Avaliação Mentor

## Visão Geral

Esta documentação detalha a migração da página de avaliação de mentor do Web Forms para Blazor .NET 9, destacando a extração da lógica de negócio para serviços reutilizáveis e helpers, conforme o novo padrão arquitetural. O objetivo é garantir máxima reutilização, desacoplamento e facilidade de manutenção, centralizando funcionalidades em `Services/Mentoria` e `Services/Mentoria/Common`.

## Estrutura dos Serviços

- **MentoriaService**: Serviço principal para carregamento de combos, mentorados e projetos de mentoria. Exposto via interface `IMentoriaService` para injeção e reutilização.
- **MentoriaHelper**: Helper de métodos auxiliares para manipulação de dados de mentorados, projetos e avaliações, como obtenção de fotos e construção de modelos de exibição.
- **ComboHelperMentoria**: Helper estático especializado para carregamento de combos (Projetos, Clientes, Periodos, Status) no contexto de mentoria, reutilizando lógica do ComboHelper comum.

## Integração e Uso

Os serviços são registrados no DI (`Program.cs`) e podem ser injetados em componentes Blazor ou outros serviços. O fluxo de dados segue o padrão:

1. O componente Blazor solicita dados ao `IMentoriaService`.
2. O serviço utiliza os helpers para construir os modelos de exibição e combos.
3. Os dados são retornados ao componente para renderização.

## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    AvaliacaoMentorRazor[Pages/AvaliacaoMentor.razor]
    MentoriaService[Services/Mentoria/MentoriaService]
    MentoriaHelper[Services/Mentoria/Common/MentoriaHelper]
    ComboHelperMentoria[Services/Mentoria/Common/ComboHelperMentoria]
    ApplicationDbContext[Data/ApplicationDbContext]

    AvaliacaoMentorRazor -->|Solicita dados| MentoriaService
    MentoriaService -->|Carrega combos| ComboHelperMentoria
    MentoriaService -->|Carrega mentorados/projetos| MentoriaHelper
    MentoriaService -->|Acessa dados| ApplicationDbContext
    ComboHelperMentoria -->|Consulta| ApplicationDbContext
    MentoriaHelper -->|Consulta| ApplicationDbContext


## Sugestões de Melhorias Futuras

- Implementar cache para combos e listas, reduzindo consultas repetidas ao banco.
- Adicionar testes automatizados para os métodos dos serviços e helpers.
- Evoluir o MentoriaHelper para suportar validações de regras de negócio específicas.
- Permitir customização dinâmica dos combos via configuração em `appsettings.json`.
- Integrar notificações automáticas para mentorados e mentores via serviço de mensageria.
- Utilizar IA para recomendações de mentoria e análise de desempenho dos mentorados.

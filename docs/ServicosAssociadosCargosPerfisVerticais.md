# Documentação dos Serviços: Associados, Cargos, Perfis e Verticais

## Visão Geral

Esta documentação cobre os serviços criados para manipulação das entidades Associados, Cargos, Perfis e Verticais, essenciais para o funcionamento da página de Associados no sistema migrado para Blazor .NET 9. Os serviços seguem o padrão de injeção de dependência e são totalmente assíncronos, garantindo alta performance e desacoplamento da camada de apresentação.

## Integração dos Serviços

- **IAssociadosService / AssociadosService**: CRUD de associados, busca por e-mail, inativação, obtenção do último associado.
- **ICargosService / CargosService**: Consulta de cargos, histórico de promoções, adição/edição de promoções e comentários.
- **IPerfisService / PerfisService**: Consulta de perfis de acesso.
- **IVerticalService / VerticalService**: Consulta de verticais de negócio.

Todos os serviços recebem o `ApplicationDbContext` via injeção de dependência e podem ser registrados no container do ASP.NET Core.

## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    A[AssociadosService] -->|CRUD| B[ASSOCIADOS]
    B --> C[CARGOS]
    B --> D[PERFIS]
    B --> E[VERTICAL]
    B --> F[EMPRESAS]
    B --> G[FOTOSASSOCIADOS]
    B --> H[PROMOCOES]
    H --> I[CargosService]
    I --> J[CARGOS]
    I --> K[PROMOCOES]
    L[PerfisService] --> D
    M[VerticalService] --> E
    subgraph UI
        N[Associados.razor]
        O[DataTableComponent]
        P[FileUploadComponent]
        Q[MessageBox]
    end
    N -->|Injeta| A
    N -->|Injeta| I
    N -->|Injeta| L
    N -->|Injeta| M
    N --> O
    N --> P
    N --> Q


## Funcionamento

- Os componentes Blazor injetam os serviços para realizar operações sobre as entidades.
- O `AssociadosService` é responsável por todas as operações relacionadas ao cadastro, edição, consulta e inativação de associados.
- O `CargosService` gerencia os cargos e o histórico de promoções, incluindo edição de comentários.
- O `PerfisService` e o `VerticalService` fornecem os dados dos combos de perfis e verticais.
- Todas as operações são feitas via métodos assíncronos, garantindo responsividade e escalabilidade.

## Sugestões de Melhorias

- Implementar cache para os dados dos combos (cargos, perfis, verticais, mentores) usando IMemoryCache.
- Adicionar paginação e busca avançada nos métodos de listagem para performance com grandes volumes de dados.
- Implementar auditoria de alterações nos métodos dos serviços.
- Permitir upload de múltiplas fotos por associado.
- Adicionar validação de e-mail duplicado em tempo real.
- Implementar notificações por e-mail para promoções.
- Adicionar dashboard com estatísticas agregadas dos associados.
- Avaliar uso de IA para sugestões de cargos e análise de promoções.

## Referências
- [Documentação oficial ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Blazor Components](https://docs.microsoft.com/aspnet/core/blazor/components/)

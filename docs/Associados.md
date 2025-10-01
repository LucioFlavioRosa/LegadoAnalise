# Página de Associados - Documentação Técnica

## Visão Geral
A página de Associados permite o cadastro, edição, inativação, visualização e exportação/importação de associados, integrando dados de cargos, perfis, verticais e empresas. Os modelos de dados foram criados para suportar todos os relacionamentos necessários e facilitar a integração com o Entity Framework Core.

## Modelos de Dados
- **ASSOCIADOS**: Representa o usuário associado, com propriedades para identificação, dados pessoais, relacionamentos e status.
- **CARGOS**: Representa cargos disponíveis na empresa, relacionados aos associados e promoções.
- **PERFIS**: Define perfis de acesso dos associados.
- **VERTICAL**: Segmenta os associados por área de atuação (vertical).

## Integração
Os modelos foram criados para serem utilizados diretamente com o Entity Framework Core, permitindo o uso de navegação entre entidades e facilitando queries complexas. Os relacionamentos (um-para-muitos, muitos-para-um) são definidos por propriedades de navegação e coleções.

## Fluxo do Processo
```mermaid
flowchart TD
    A[Usuário acessa /associados] --> B{Página Associados.razor}
    B --> C[OnInitializedAsync]
    C --> D[Carregar Cargos]
    C --> E[Carregar Perfis]
    C --> F[Carregar Mentores]
    C --> G[Carregar Verticais]
    C --> H[Carregar Lista de Associados]
    D --> I[CARGOS]
    E --> J[PERFIS]
    F --> K[ASSOCIADOS]
    G --> L[VERTICAL]
    H --> K
    I --> M[Preencher Combo Cargos]
    J --> N[Preencher Combo Perfis]
    K --> O[Preencher Combo Mentores]
    L --> P[Preencher Combo Verticais]
    K --> Q[Exibir Grid de Associados]
    Q --> R{Usuário clica em Alterar}
    R --> S[EditarAssociado]
    S --> T[Carregar dados do associado]
    T --> U[Preencher formulário]
    U --> V[Carregar Histórico de Promoções]
    V --> W[CARGOS]
    W --> X[Exibir Grid de Promoções]
    Q --> Y{Usuário clica em Inativar}
    Y --> Z[InativarAssociado]
    Z --> AA[ASSOCIADOS]
    AA --> AB[Atualizar Grid]
    U --> AC{Usuário clica em Salvar}
    AC --> AD[SalvarAssociado]
    AD --> AE{Validar dados}
    AE -- Válido --> AF{Novo ou Edição?}
    AF -- Novo --> AG[ASSOCIADOS]
    AF -- Edição --> AH[ASSOCIADOS]
    AG --> AI[Salvar Foto]
    AH --> AI
    AI --> AJ[FOTOSASSOCIADOS]
    AJ --> AK{Promoção marcada?}
    AK -- Sim --> AL[CARGOS]
    AK -- Não --> AM[Exibir mensagem de sucesso]
    AL --> AM
    AM --> AN[Limpar formulário]
    AN --> AB
    AE -- Inválido --> AO[Exibir mensagem de erro]
```

## Sugestões de Melhorias
- Implementar cache para os combos de cargos, perfis, mentores e verticais.
- Adicionar paginação e filtros avançados na grid de associados.
- Incluir validação de e-mail duplicado em tempo real.
- Permitir upload de múltiplas fotos por associado.
- Adicionar auditoria de alterações e notificações por e-mail.
- Criar dashboard com estatísticas dos associados.

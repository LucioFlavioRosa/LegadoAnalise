# Página de Associados - Blazor (.NET 9)

## Visão Geral

A página de Associados permite o cadastro, edição, inativação, visualização e importação/exportação de associados e promoções, além do upload de fotos e histórico de promoções. Sua implementação foi migrada de Web Forms para Blazor, utilizando componentes reutilizáveis e arquitetura moderna.

## Arquitetura

- **Componentes Reutilizáveis:**
  - MessageBox (exibição de mensagens)
  - FileUploadComponent (upload de arquivos)
  - DataTableComponent (exibição de grids)
- **Serviços:**
  - AssociadosService, CargosService, PerfisService, VerticalService, FotosAssociadosService, ExportFileService, ImportFileService
- **Modelos:**
  - ASSOCIADOS, CARGOS, PERFIS, VERTICAL, FOTOSASSOCIADOS, PROMOCOES
- **DbContext:**
  - ApplicationDbContext (Entity Framework Core)

## Fluxo de Dados

mermaid
flowchart TD
    A[Usuário acessa /associados] --> B{Página Associados.razor}
    B --> C[OnInitializedAsync]
    C --> D[Carregar Cargos]
    C --> E[Carregar Perfis]
    C --> F[Carregar Mentores]
    C --> G[Carregar Verticais]
    C --> H[Carregar Lista de Associados]
    D --> I[CargosService.ObterListaCargosAsync]
    E --> J[PerfisService.ObterListaPerfisAsync]
    F --> K[AssociadosService.ObterAssociadosAsync]
    G --> L[VerticalService.ListarVerticaisAsync]
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
    V --> W[CargosService.ObterPromocoesAssociadoAsync]
    W --> X[Exibir Grid de Promoções]
    Q --> Y{Usuário clica em Inativar}
    Y --> Z[InativarAssociado]
    Z --> AA[AssociadosService.InativarAssociadoAsync]
    AA --> AB[Atualizar Grid]
    U --> AC{Usuário clica em Salvar}
    AC --> AD[SalvarAssociado]
    AD --> AE{Validar dados}
    AE -- Válido --> AF{Novo ou Edição?}
    AF -- Novo --> AG[AssociadosService.InserirAssociadoAsync]
    AF -- Edição --> AH[AssociadosService.AlterarAssociadoAsync]
    AG --> AI[Salvar Foto]
    AH --> AI
    AI --> AJ[FotosService.AdicionarFotoAsync ou AtualizarFotoAsync]
    AJ --> AK{Promoção marcada?}
    AK -- Sim --> AL[CargosService.AdicionarPromocaoAsync]
    AK -- Não --> AM[Exibir mensagem de sucesso]
    AL --> AM
    AM --> AN[Limpar formulário]
    AN --> AB
    AE -- Inválido --> AO[Exibir mensagem de erro]
    B --> AP{Usuário clica em Exportar Associados}
    AP --> AQ[ExportarAssociados]
    AQ --> AR[ExportService.ExportarAssociadosExcelAsync]
    AR --> AS[Download do arquivo]
    B --> AT{Usuário clica em Importar Associados}
    AT --> AU[Selecionar arquivo]
    AU --> AV[ImportarAssociados]
    AV --> AW[ImportService.ImportarAssociadosExcelAsync]
    AW --> AX[Exibir mensagem de resultado]
    AX --> AB
    B --> AY{Usuário clica em Exportar Promoções}
    AY --> AZ[ExportarPromocoes]
    AZ --> BA[ExportService.ExportarPromocoesExcelAsync]
    BA --> AS
    B --> BB{Usuário clica em Importar Promoções}
    BB --> BC[Selecionar arquivo]
    BC --> BD[ImportarPromocoes]
    BD --> BE[ImportService.ImportarPromocoesExcelAsync]
    BE --> AX


## Modelos de Dados

- **ASSOCIADOS:** IdAssociado, Nome, Email, Senha, IdCargo, IdPerfil, IdAssociadoMentor, IdEmpresa, IdVertical, DataAdmissao, Vertical, FotoNome, ATV, IdStatus, IdNivel, USR, DHC
- **CARGOS:** IdCargo, Cargo, ATV, DHC, USR
- **PERFIS:** IdPerfil, Perfil, ATV, DHC, USR
- **VERTICAL:** IdVertical, Descricao, ATV, DHC, USR
- **FOTOSASSOCIADOS:** IdFoto, IdAssociado, AssociadoFoto, NomeFoto, Imagem, DHC, ATV
- **PROMOCOES:** idPromocao, idAssociado, idCargoAnterior, idCargoNovo, DataPromocao, Comentarios, ATV, DHC

## Serviços

- **AssociadosService:** CRUD de associados, busca por e-mail, obtenção do último associado
- **CargosService:** Listagem de cargos, promoções, alteração de comentários
- **PerfisService:** Listagem de perfis
- **VerticalService:** Listagem de verticais
- **FotosAssociadosService:** CRUD de fotos dos associados
- **ExportFileService:** Exportação de associados e promoções para Excel
- **ImportFileService:** Importação de associados e promoções a partir de Excel

## Componentes Blazor

- **MessageBox:** Exibe mensagens de sucesso, erro, alerta ou informação
- **FileUploadComponent:** Permite upload de arquivos (Excel, imagens)
- **DataTableComponent:** Exibe listas de dados com ações de editar e inativar

## Integração com Banco de Dados

- Utiliza Entity Framework Core
- ApplicationDbContext centraliza o acesso às tabelas
- Relacionamentos configurados via Fluent API

## Sugestões de Melhorias

- Implementar cache para combos (cargos, perfis, mentores, verticais)
- Adicionar paginação e busca avançada nos grids
- Implementar auditoria de alterações
- Permitir upload de múltiplas fotos
- Validação de e-mail duplicado em tempo real
- Notificações por e-mail para promoções
- Dashboard de estatísticas de associados
- Testes automatizados para serviços e componentes
- Autenticação e autorização com ASP.NET Core Identity
- Monitoramento com Application Insights
- CI/CD automatizado
- Integração com IA para sugestões de cargos e análise de promoções

# Documentação Técnica - Página de Associados (Blazor .NET 9)

## Visão Geral
A página de Associados permite o cadastro, edição, inativação, upload de fotos, histórico de promoções, exportação/importação de dados e integração com múltiplas entidades (Cargos, Perfis, Mentores, Verticais, Empresas). Todo o fluxo foi migrado para Blazor, utilizando serviços injetáveis e componentes reutilizáveis.

## Arquitetura
- **Modelos:** ASSOCIADOS, CARGOS, PERFIS, VERTICAL, EMPRESAS, FOTOSASSOCIADOS, PROMOCOES
- **Serviços:** AssociadosService, CargosService, PerfisService, VerticalService, FotosAssociadosService, ExportFileService, ImportFileService
- **Componentes Blazor:** MessageBox, FileUploadComponent, DataTableComponent, Associados.razor
- **DbContext:** ApplicationDbContext

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


## Integração dos Códigos
- Todos os serviços são registrados via injeção de dependência no Program.cs.
- Os componentes Blazor consomem os serviços via @inject.
- O ApplicationDbContext centraliza o acesso ao banco de dados.
- O serviço de exportação usa EPPlus para gerar arquivos Excel.
- O serviço de importação lê arquivos Excel e executa inserção/atualização conforme a existência do registro.
- O serviço de fotos armazena imagens em Base64 no banco.

## Sugestões de Melhorias
- Implementar cache para combos de dados estáticos (Cargos, Perfis, Verticais, Mentores) usando IMemoryCache.
- Adicionar paginação e busca avançada nos grids.
- Implementar auditoria de alterações em associados e promoções.
- Permitir upload de múltiplas fotos/documentos por associado.
- Adicionar validação de e-mail duplicado em tempo real.
- Notificações automáticas por e-mail para promoções.
- Dashboard com estatísticas de associados e promoções.
- Testes automatizados para serviços e componentes.
- Segurança avançada: autenticação, autorização e uso de Key Vault.
- Monitoramento com Application Insights e logging estruturado.
- Avaliar uso de IA para sugestões de cargos e análise de promoções.
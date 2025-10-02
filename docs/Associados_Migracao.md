# Documentação da Migração: Serviços de Domínio - Associados

## Visão Geral

Esta etapa da migração do sistema de avaliação interna da empresa contempla a criação dos serviços de domínio responsáveis por toda a lógica de negócio relacionada a Associados, Cargos, Perfis, Verticais, Fotos de Associados e Exportação de Arquivos. Estes serviços seguem o padrão de injeção de dependência e são totalmente reutilizáveis em toda a aplicação Blazor .NET 9.

### Serviços Criados

- **IAssociadosService / AssociadosService**: CRUD de associados, busca por e-mail, busca do último associado.
- **ICargosService / CargosService**: Listagem de cargos, promoções, alteração de comentários de promoções.
- **IPerfisService / PerfisService**: Listagem de perfis.
- **IVerticalService / VerticalService**: Listagem de verticais.
- **IFotosAssociadosService / FotosAssociadosService**: CRUD de fotos dos associados.
- **IExportFileService / ExportFileService**: Geração de arquivos Excel genérica para qualquer modelo.

Todos os serviços utilizam o ApplicationDbContext (Entity Framework Core) para acesso ao banco de dados e são assíncronos, promovendo performance e escalabilidade.

## Integração dos Serviços

- Os serviços são registrados no container de dependências em `Program.cs`.
- Os componentes Blazor injetam os serviços necessários via `@inject` ou construtor.
- O ApplicationDbContext é compartilhado entre os serviços, evitando duplicidade de código e promovendo reuso.
- O serviço de exportação é genérico e pode ser utilizado para qualquer entidade do domínio.

## Fluxo do Processo (Mermaid)

mermaid
graph TD
    A[Componente Blazor requisita operação] --> B{Serviço de domínio chamado}
    B --> C1[AssociadosService]
    B --> C2[CargosService]
    B --> C3[PerfisService]
    B --> C4[VerticalService]
    B --> C5[FotosAssociadosService]
    B --> C6[ExportFileService]
    C1 --> D[ApplicationDbContext]
    C2 --> D
    C3 --> D
    C4 --> D
    C5 --> D
    C6 --> E[EPPlus]
    D --> F[Banco de Dados SQL]
    E --> G[Arquivo Excel gerado]
    F --> H[Dados retornados para serviço]
    G --> I[Arquivo enviado ao usuário]
    H --> J[Serviço retorna resultado ao componente]
    J --> K[Componente atualiza UI]


### Estrutura de Páginas Relacionadas

- `Components/Pages/Associados.razor` (principal)
    - Consome todos os serviços acima para CRUD, histórico, import/export, upload de fotos
- `Components/Pages/Associados_CadastroSection.razor`
- `Components/Pages/Associados_HistoricoSection.razor`
- `Components/Pages/Associados_ImportExportSection.razor`
- `Components/Pages/Associados_ListaSection.razor`

## Sugestões de Melhorias

- Implementar cache para dados que mudam pouco (ex: cargos, perfis, verticais)
- Adicionar logging estruturado em todos os métodos dos serviços
- Adicionar tratamento de exceções centralizado e políticas de retry para operações críticas
- Implementar testes unitários e mocks para os serviços
- Considerar uso de DTOs para desacoplar entidades do banco da camada de apresentação
- Adicionar validação extra de dados antes de persistir no banco
- Permitir upload de fotos diretamente para blob storage (Azure, AWS) para maior escalabilidade
- Otimizar consultas com projeções (Select) ao invés de carregar entidades completas quando não necessário
- Implementar versionamento de arquivos exportados
- Adicionar suporte a internacionalização nas mensagens de erro dos serviços

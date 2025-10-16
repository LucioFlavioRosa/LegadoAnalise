# Guia de Migração: Sistema de Avaliação - Modernização para Blazor Web App (.NET 9)

## Sumário
- [Introdução](#introducao)
- [Visão Geral da Arquitetura](#visao-geral-da-arquitetura)
- [Mapeamento de Funcionalidades](#mapeamento-de-funcionalidades)
- [Principais Decisões Arquiteturais](#principais-decisoes-arquiteturais)
- [Breaking Changes](#breaking-changes)
- [Guia de Rollback](#guia-de-rollback)
- [Considerações Finais](#consideracoes-finais)

---

## Introdução
Este documento descreve o processo completo de migração do sistema legado ASP.NET Web Forms para a nova solução baseada em Blazor Web App (.NET 9), detalhando decisões técnicas, mapeamento de funcionalidades, mudanças críticas e orientações para rollback.

## Visão Geral da Arquitetura
- **Frontend:** Blazor Web App (.NET 9), modo interativo Auto (Server/WebAssembly).
- **Backend:** ASP.NET Core 9, Entity Framework Core 9, arquitetura orientada a serviços.
- **Banco de Dados:** SQL Server, migrations via EF Core.
- **Autenticação:** ASP.NET Core Identity ou Azure AD B2C (conforme requisitos).
- **Monitoramento:** Azure Application Insights.
- **CI/CD:** GitHub Actions.

## Mapeamento de Funcionalidades
| Funcionalidade Legada | Novo Blazor Web App |
|----------------------|---------------------|
| Cadastro/Edição de Cargos | Página `Cargos.razor` com formulário validado e integração com serviço |
| Lista de Cargos | Componente `CargosList.razor` usando QuickGrid, busca e ações |
| Mensagens de Sucesso/Erro | Componente `MessageBox.razor` com toasts |
| Exportação para Excel | Serviço `ExportService` com download via JS Interop |
| Controle de Acesso | `[Authorize]` via ASP.NET Core Identity/Azure AD |
| Layout e Navegação | `MainLayout.razor` e `Breadcrumb.razor` |
| Validação de Dados | DataAnnotations + FluentValidation |
| Testes Unitários e de UI | xUnit, Moq, bUnit |

## Principais Decisões Arquiteturais
- **Migração para Blazor Web App:** Permite UI moderna, interatividade e reuso de código C#.
- **Entity Framework Core:** Facilita migrations, seed de dados e integrações modernas.
- **Componentização:** Separação clara de responsabilidades e reuso de UI.
- **Remoção de dependências legadas:** jQuery, DataTables e Web Forms substituídos por Blazor e funcionalidades nativas.
- **Monitoramento e Logging:** Azure Application Insights e Serilog para rastreabilidade e diagnóstico.

## Breaking Changes
- **URLs e Rotas:** As rotas das páginas mudaram para o padrão Blazor (`/cargos` em vez de `.aspx`).
- **Autenticação:** Integração com Identity/Azure AD pode exigir redefinição de usuários e permissões.
- **Exportação:** Download de arquivos Excel agora é feito via JS Interop, não mais postback.
- **Validação:** Validações agora são feitas no client e no server, com feedback instantâneo.
- **Layout:** Estrutura visual e navegação modernizadas, removendo master pages e user controls ASPX.

## Guia de Rollback
1. **Backup:** Antes da migração, realizar backup completo do banco de dados e do código legado.
2. **Rollback de Banco:** Se necessário, restaurar backup do banco de dados anterior à migration inicial do EF Core.
3. **Rollback de Código:** Restaurar branch/tag do repositório referente à versão Web Forms.
4. **Ambiente:** Remover/atualizar variáveis de ambiente de conexão e monitoramento para apontar ao sistema legado.
5. **DNS/URL:** Reverter apontamento de DNS ou slots de deploy para a aplicação anterior.

## Considerações Finais
- Recomenda-se validar todos os fluxos críticos após a migração, especialmente integrações externas e permissões.
- O monitoramento via Application Insights deve ser acompanhado nos primeiros dias para identificar possíveis anomalias.
- Consulte a documentação técnica dos componentes Blazor e do Entity Framework para customizações avançadas.

---

**Dúvidas ou problemas?** Consulte o time de arquitetura ou abra um chamado no repositório do projeto.

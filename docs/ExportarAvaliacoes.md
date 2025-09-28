# Exportação de Avaliações - Documentação Técnica

## Visão Geral

Esta documentação descreve o funcionamento, integração e fluxo da funcionalidade de exportação de avaliações do sistema Peers.Moderno, migrada de Web Forms para Blazor Híbrido (.NET 9). O processo foi modularizado e segue as melhores práticas de reutilização e separação de responsabilidades, centralizando a lógica em serviços e helpers reutilizáveis.

A exportação permite que usuários filtrem avaliações por período, projeto e associado, visualizem uma prévia dos dados e exportem os resultados completos em formato Excel (.xlsx).

---

## Estrutura dos Componentes e Serviços

### Componentes Blazor
- **Components/Pages/ExportarAvaliacoes.razor**: Página principal da exportação, com UI para filtros, listagem e botão de exportação.
- **Components/Pages/ExportarAvaliacoes.razor.cs**: Code-behind com lógica de binding, busca e exportação.
- **Components/Shared/ExportarAvaliacoesTable.razor**: Componente reutilizável para renderização da tabela de avaliações.
- **Components/Shared/ComboSelect.razor**: Componente genérico para combos, utiliza ComboHelper.

### Serviços e Helpers
- **Services/AvaliacoesExportacao/ExportarAvaliacoesService.cs**: Serviço principal para obtenção e filtragem dos dados de avaliações.
- **Services/AvaliacoesExportacao/Common/ExportarAvaliacoesHelper.cs**: Métodos utilitários para formatação, validação e cálculos.
- **Services/AvaliacoesExportacao/Common/ExportarAvaliacoesExcelService.cs**: Geração do arquivo Excel via EPPlus.
- **Services/Common/ComboHelper.cs**: Preenchimento dos combos de filtros.
- **Services/Common/FormatHelper.cs**: Formatação de percentuais, decimais e truncamento de texto.
- **Services/Common/TelemetryService.cs**: Telemetria para rastreamento de eventos e erros.
- **Services/Common/MessageBoxService.cs**: Exibição de mensagens ao usuário.

### Acesso a Dados
- **Data/ApplicationDbContext.cs**: DbSets necessários para avaliações, projetos, associados, períodos, etc.

---

## Integração dos Códigos

1. **UI Blazor**: O usuário acessa a página ExportarAvaliacoes.razor, onde pode selecionar filtros (período, projeto, associado) usando ComboSelect, que obtém os dados via ComboHelper.
2. **Busca de Dados**: Ao clicar em "Listar Avaliações", o método do code-behind chama ExportarAvaliacoesService para buscar e filtrar os dados conforme os parâmetros.
3. **Prévia dos Dados**: Os dados retornados são exibidos no componente ExportarAvaliacoesTable.razor, utilizando FormatHelper para apresentação.
4. **Exportação**: Ao clicar em "Exportar", o ExportarAvaliacoesExcelService é chamado para gerar o arquivo Excel, utilizando os dados filtrados e formatados pelo ExportarAvaliacoesHelper.
5. **Mensagens e Telemetria**: MessageBoxService exibe feedback ao usuário (sucesso/erro) e TelemetryService registra eventos de exportação e falhas.
6. **Download**: O arquivo Excel é enviado ao navegador para download.

---

## Fluxo do Processo (Mermaid)

```mermaid
flowchart TD
    Start([Usuário acessa Exportar Avaliações])
    Filtros[Seleciona filtros (Período, Projeto, Associado)]
    Busca[Busca avaliações via ExportarAvaliacoesService]
    Previa[Exibe prévia na tabela ExportarAvaliacoesTable]
    Exportar[Usuário clica em Exportar]
    GerarExcel[Geração do Excel via ExportarAvaliacoesExcelService]
    Download[Arquivo Excel enviado para download]
    Mensagem[Exibe mensagem com MessageBoxService]
    Telemetria[Registra evento com TelemetryService]

    Start --> Filtros
    Filtros --> Busca
    Busca --> Previa
    Previa --> Exportar
    Exportar --> GerarExcel
    GerarExcel --> Download
    GerarExcel --> Mensagem
    GerarExcel --> Telemetria
```

### Estrutura de Páginas e Serviços
- **ExportarAvaliacoes.razor** (UI) → **ComboHelper** (combos)
- **ExportarAvaliacoes.razor.cs** (lógica) → **ExportarAvaliacoesService** (dados)
- **ExportarAvaliacoesTable.razor** (tabela) → **FormatHelper** (formatação)
- **ExportarAvaliacoesExcelService** (exportação)
- **MessageBoxService** (mensagens)
- **TelemetryService** (telemetria)

---

## Sugestões de Melhorias Futuras

- **Paginação e Filtros Avançados**: Implementar paginação na prévia e filtros adicionais (status, tipo de avaliação, etc.).
- **Exportação Assíncrona**: Para grandes volumes, permitir exportação em background e notificação por e-mail.
- **Customização de Colunas**: Permitir ao usuário escolher quais colunas exportar.
- **Histórico de Exportações**: Registrar e exibir histórico de exportações realizadas por usuário.
- **Aprimoramento da Telemetria**: Detalhar eventos de erro e sucesso para melhor rastreabilidade.
- **Internacionalização**: Suporte a múltiplos idiomas na interface e nos arquivos exportados.
- **Testes Automatizados**: Implementar testes de integração para garantir a robustez da exportação.
- **Aprimorar UX**: Adicionar loading indicators e feedback visual durante a exportação.

---

## Observações
- Toda a lógica de negócio e acesso a dados foi extraída para serviços reutilizáveis, facilitando manutenção e extensibilidade.
- O fluxo foi desenhado para ser facilmente adaptável a outras áreas do sistema que necessitem de exportação de dados.
- A documentação será mantida nesta pasta e deve ser atualizada a cada evolução da funcionalidade.

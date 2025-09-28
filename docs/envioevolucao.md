# Documentação Técnica: Envio de Evolução - Serviços Comuns

## Visão Geral

Esta documentação cobre a centralização e modernização dos serviços comuns utilizados no processo de Envio de Evolução, migrando lógicas utilitárias para a pasta `Services/Common`. O objetivo é garantir máxima reutilização, manutenção facilitada e padronização em toda a aplicação.

## Serviços e Helpers Implementados

### 1. ComboHelper
- Centraliza métodos para carregar combos de períodos, associados, verticais, status, tipos de avaliação, escopos, notas de competência/performance, abrangências, etc.
- Permite consultas assíncronas ao banco de dados via EF (ApplicationDbContext).
- Retorna listas de ComboItem reutilizáveis em qualquer componente.

### 2. MessageBoxService
- Serviço singleton para exibição de mensagens de sucesso, erro, informação e alerta.
- Utiliza eventos para notificar componentes Blazor.
- Padroniza a experiência de mensagens em toda a aplicação.

### 3. EmailService
- Serviço injetável para envio de emails parametrizados.
- Lê configurações do `appsettings.json` (seção Email).
- Permite envio assíncrono, com suporte a HTML, CC e BCC.
- Facilita manutenção e centraliza lógica de envio de email.

### 4. FormatHelper
- Helper estático para formatação de percentuais, decimais, moedas e truncamento de texto.
- Garante consistência visual e reaproveitamento em toda a aplicação.

## Integração e Uso

- Todos os serviços estão disponíveis via DI (Dependency Injection) e podem ser utilizados em componentes, páginas e outros serviços.
- Exemplo de injeção em um componente Blazor:
  csharp
  @inject Services.Common.ComboHelper ComboHelper
  @inject Peers.Moderno.Services.Common.IMessageBoxService MessageBoxService
  @inject Services.Common.IEmailService EmailService
  @inject Peers.Moderno.Services.Common.FormatHelper FormatHelper
  
- O ComboHelper pode ser usado para popular combos de dropdowns de períodos, associados, verticais, etc.
- O MessageBoxService pode ser usado para exibir mensagens de feedback ao usuário.
- O EmailService pode ser utilizado para enviar emails de notificação, evolução, etc.
- O FormatHelper pode ser usado para formatar valores exibidos na UI.

## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    A[EnvioEvolucao.razor] -->|Carrega combos| B[ComboHelper]
    A -->|Exibe mensagens| C[MessageBoxService]
    A -->|Envia emails| D[EmailService]
    A -->|Formata dados| E[FormatHelper]
    B -->|Consulta dados| F[ApplicationDbContext]
    D -->|Lê config| G[appsettings.json]


## Sugestões de Melhorias Futuras

- Implementar cache para combos que não mudam com frequência, reduzindo queries ao banco.
- Adicionar suporte a templates de email dinâmicos e multilíngue no EmailService.
- Permitir customização de temas e estilos no MessageBoxService.
- Expandir o FormatHelper para suportar internacionalização e formatação de datas customizadas.
- Criar testes automatizados para todos os helpers e serviços comuns.

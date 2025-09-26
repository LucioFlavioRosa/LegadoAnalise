# Documentação: Avaliação de Competências do Gestor - Blazor/Serviços

## Visão Geral

Este módulo implementa a lógica centralizada e reutilizável para a avaliação de competências do gestor, migrando regras de negócio, validações e helpers para serviços modernos e desacoplados, prontos para uso em componentes Blazor e outras camadas da aplicação.

### Serviços/Helpers Criados ou Atualizados

- **CompetenciaHelper**: Centraliza regras de truncamento de texto, validação e consistência de notas, regras de negócio de pilar.
- **ValidationHelper**: Centraliza validações genéricas e integra com CompetenciaHelper para regras de negócio específicas.
- **MessageBoxService**: Serviço de mensagens aprimorado, agora com suporte a título e delay customizáveis.
- **ComboHelper**: Centraliza geração de combos de notas, tipos de avaliação, escopos e outros, com suporte a pesos e seleção padrão.

## Integração e Fluxo de Uso

- Os componentes Blazor de avaliação de competências injetam os serviços/Helpers.
- O ComboHelper fornece as listas de opções para os combos de notas, tipos e escopos.
- O ValidationHelper e CompetenciaHelper são usados para validação de inputs, consistência de notas e regras de negócio antes de salvar ou finalizar a avaliação.
- O MessageBoxService exibe mensagens de feedback ao usuário, com título e tempo customizáveis.

## Exemplo de Uso (Blazor)

csharp
@inject IMessageBoxService MessageBoxService
@code {
    void Salvar()
    {
        if (!ValidationHelper.ValidarNotasObrigatorias(notaNivel1, notaNivel2))
        {
            MessageBoxService.ShowError("Selecione uma nota para cada nível.", "Erro", 5000);
            return;
        }
        // ...
    }
}


## Fluxo do Processo (Mermaid)

mermaid
flowchart TD
    Start([Início]) --> PaginaAvalGestor["Página: Avaliação Gestor (Blazor)"]
    PaginaAvalGestor -->|Obtém combos| ComboHelper
    PaginaAvalGestor -->|Valida inputs| ValidationHelper
    PaginaAvalGestor -->|Regras de negócio| CompetenciaHelper
    PaginaAvalGestor -->|Exibe mensagens| MessageBoxService
    PaginaAvalGestor -->|Salva dados| BackendAPI
    BackendAPI -->|Persistência| ApplicationDbContext
    PaginaAvalGestor --> End([Fim])


## Sugestões de Melhorias Futuras

- Implementar testes unitários para todos os helpers e serviços.
- Internacionalizar mensagens do MessageBoxService.
- Permitir configuração dinâmica dos combos de notas via banco de dados.
- Expandir o CompetenciaHelper para suportar regras de negócio parametrizáveis por tipo de avaliação.
- Integrar validações assíncronas para cenários de múltiplos usuários.
- Documentar exemplos de integração com componentes Blazor prontos.

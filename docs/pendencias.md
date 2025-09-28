# Documentação Técnica: Fluxo de Pendências (Blazor Híbrido)

## Visão Geral

Esta documentação descreve a arquitetura, funcionamento e integração do novo fluxo de "Pendências" migrado de Web Forms para Blazor Híbrido (.NET 9). O fluxo foi modernizado para desacoplar a lógica de negócio da interface, promover reuso de código e facilitar manutenção futura.

## Estrutura de Componentes e Serviços

- **PendenciasService** (`Services/Pendencias/Common/PendenciasService.cs`): Serviço central de negócio, responsável por obter a lista de pendências do usuário logado. Recebe via DI os serviços necessários para acessar dados de associados, avaliações, períodos, fotos, etc.
- **ProjetoPendenciaModel** (`Services/Pendencias/Common/ProjetoPendenciaModel.cs`): DTO para transportar dados de cada pendência para a UI de forma desacoplada.
- **Pendencias.razor / Pendencias.razor.cs** (`Pages/Pendencias.razor`): Componente Blazor responsável pela exibição da tabela de pendências. Consome o serviço de negócio e utiliza o MessageBoxService para mensagens ao usuário.
- **MessageBoxService** (`Services/Common/MessageBoxService.cs`): Serviço reutilizável para exibição de mensagens de feedback ao usuário.

## Integração com o Projeto

- O serviço `PendenciasService` é registrado no DI container em `Program.cs`:
  csharp
  builder.Services.AddScoped<IPendenciasService, PendenciasService>();
  
- O componente Blazor `Pendencias.razor` injeta o serviço e carrega os dados no ciclo de vida `OnInitializedAsync`.
- O modelo `ProjetoPendenciaModel` é utilizado para facilitar o binding e desacoplar a UI da estrutura de dados original.
- O serviço de mensagens é reutilizado para manter a experiência de feedback ao usuário.

## Fluxo do Processo (Mermaid)

```mermaid
flowchart TD
    A[Usuário acessa Pendencias.razor] --> B{OnInitializedAsync}
    B --> C[PendenciasService.ObterPendenciasAsync(userId)]
    C --> D[Serviços de Dados: Associados, Avaliacoes, Periodos, Fotos]
    D --> E[Lista de ProjetoPendenciaModel]
    E --> F[Renderização da Tabela de Pendências]
    F --> G[MessageBoxService para feedback]
```

## Sugestões de Melhorias Futuras

- Implementar paginação e filtros avançados na tabela de pendências para melhor escalabilidade e usabilidade.
- Adicionar integração com IA para priorização automática de pendências ou sugestões inteligentes ao usuário.
- Permitir exportação dos dados de pendências para formatos como Excel ou PDF.
- Adicionar testes automatizados para o serviço de negócio e para o componente Blazor.
- Melhorar a experiência de usuário com indicadores de carregamento e mensagens de erro mais detalhadas.
- Internacionalização do componente para suportar múltiplos idiomas.

---

**Observação:**
A arquitetura adotada segue o padrão de centralização de serviços reutilizáveis, facilitando a manutenção e evolução do sistema. O fluxo foi cuidadosamente migrado para garantir compatibilidade e integridade das funcionalidades existentes.

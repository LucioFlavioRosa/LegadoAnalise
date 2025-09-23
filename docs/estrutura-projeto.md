# Estrutura do Projeto - Sistema de Avaliação Moderno

## Visão Geral

O projeto foi estruturado seguindo os princípios de **Domain-Driven Design (DDD)** e **Clean Architecture**, organizando o código por domínios de negócio com pastas `Common` para componentes e serviços reutilizáveis.

## Estrutura de Pastas


SistemaAvaliacao.Moderno/
├── Components/
│   ├── Layout/
│   └── Pages/
├── CadastroProjetos/
│   ├── Pages/
│   ├── Models/
│   ├── Services/
│   └── Common/
│       ├── Components/
│       └── Services/
├── wwwroot/
├── appsettings.json
└── Program.cs


## Convenções de Organização

### Domínios de Negócio
Cada domínio (ex: `CadastroProjetos`) possui sua própria pasta com:
- **Pages/**: Componentes Blazor de página específicos do domínio
- **Models/**: Classes de modelo de dados (POCOs)
- **Services/**: Serviços de negócio específicos do domínio
- **Common/**: Código reutilizável dentro do domínio

### Pasta Common
A pasta `Common` dentro de cada domínio contém:
- **Components/**: Componentes Blazor reutilizáveis
- **Services/**: Serviços que podem ser compartilhados

## Tecnologias Utilizadas

- **.NET 9**: Framework principal
- **Blazor Web App**: Modo híbrido (Server + WebAssembly)
- **Entity Framework Core 9**: ORM para acesso a dados
- **Bootstrap**: Framework CSS para UI responsiva

## Fluxo de Alto Nível

mermaid
flowchart TD
    Start([Usuário acessa aplicação])
    Start --> Router[Router Blazor]
    Router --> Layout[MainLayout]
    Layout --> NavMenu[Menu de Navegação]
    Layout --> PageContent[Conteúdo da Página]
    PageContent --> Domain[Domínio Específico]
    Domain --> Components[Componentes Common]
    Domain --> Services[Serviços de Negócio]
    Services --> Database[(Banco de Dados)]
    Components --> UI[Interface do Usuário]
    UI --> End([Resposta ao Usuário])

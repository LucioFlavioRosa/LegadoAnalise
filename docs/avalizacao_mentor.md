# Documentação Técnica: Página de Avaliação Mentor

## 1. Visão Geral

Esta documentação detalha a arquitetura, funcionamento e integração dos serviços e componentes envolvidos na migração da página de Avaliação de Mentor do sistema de avaliação interna da empresa para Blazor Híbrido .NET 9. O objetivo é garantir desacoplamento, reutilização e escalabilidade, mantendo todas as funcionalidades atuais.

## 2. Estrutura de Serviços e Componentes

- **Services/Mentoria/MentoriaService.cs**: Centraliza a lógica de negócio para filtros, carregamento de mentorados, projetos e avaliações.
- **Services/Mentoria/Common/MentoriaHelper.cs**: Métodos auxiliares para manipulação e formatação de dados de mentoria.
- **Services/Mentoria/Common/ComboHelperMentoria.cs**: Métodos especializados para combos de mentoria, reutilizando o máximo possível de Services/Common/ComboHelper.cs.
- **Pages/AvaliacaoMentor.razor**: Componente Blazor que representa a interface da página de avaliação de mentor, dividida em subcomponentes reutilizáveis.
- **Configuração**: Todas as configurações de mentoria estão centralizadas na seção "Mentoria" do appsettings.json.
- **Injeção de Dependência**: Todos os serviços de mentoria e helpers são registrados no Program.cs e disponíveis para injeção.

## 3. Integração e Fluxo de Dados

Os componentes Blazor consomem os serviços de mentoria via DI. O carregamento dos combos, mentorados e avaliações é realizado de forma assíncrona, respeitando os filtros selecionados pelo usuário. Helpers garantem formatação e validação dos dados antes da exibição.

### Diagrama de Fluxo (Mermaid)
```mermaid
flowchart TD
    %% 1. Primeiro, defina os subgrupos e todos os nós dentro deles
    subgraph UI
        A[Pages/AvaliacaoMentor.razor];
    end

    subgraph Services
        B[Services/Mentoria/MentoriaService];
        C[Services/Mentoria/Common/MentoriaHelper];
        D[Services/Mentoria/Common/ComboHelperMentoria];
        F[Services/Common/ComboHelper];
    end

    subgraph Infra
        E[Data/ApplicationDbContext];
        G["appsettings.json (Mentoria)"];
    end

    %% 2. Depois, defina todas as conexões entre os nós
    A --> B;
    A --> C;
    A --> D;
    B --> E;
    D --> F;
    A --> G;
```

## 4. Sugestões de Melhorias Futuras

- Implementar cache para combos e mentorados, reduzindo chamadas ao banco e melhorando performance.
- Adicionar testes automatizados para os serviços e helpers.
- Evoluir o fluxo de mentoria para permitir recomendações baseadas em IA, utilizando o Azure OpenAI.
- Permitir customização de colunas e filtros na UI via configuração.
- Internacionalização dos textos e mensagens para suporte a múltiplos idiomas.
- Logging detalhado de ações do usuário para auditoria e análise de uso.

## 5. Referências de Configuração

Todas as configurações relacionadas à mentoria podem ser ajustadas na seção `Mentoria` do arquivo `appsettings.json`, incluindo limites, filtros padrão, mensagens de validação e estilos de tabela.

## 6. Observações

- A arquitetura foi desenhada para máxima reutilização e desacoplamento, facilitando futuras evoluções.
- O fluxo de dados é totalmente assíncrono e reativo, garantindo performance e experiência do usuário.
- Todas as funcionalidades originais do sistema foram mantidas e validadas.

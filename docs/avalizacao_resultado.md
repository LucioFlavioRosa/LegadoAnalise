# Documentação - Página de Resultado de Avaliação (Blazor)

## Visão Geral

Esta documentação descreve a arquitetura, funcionamento e integração dos componentes criados para a página de resultado de avaliação migrada de Web Forms para Blazor. Os componentes ConsideracoesMentor.razor, ModalRadar.razor e ModalComplexidade.razor são parte fundamental da nova estrutura, promovendo modularidade, reutilização e integração fluida com os serviços de domínio.

## Componentes Criados

### 1. ConsideracoesMentor.razor
- Exibe as considerações do mentor para o associado avaliado.
- Integra dados de associado, cargo, projetos envolvidos e status de promoção.
- Utiliza serviços reutilizáveis para obter dados e helpers para cálculos de tempo e elegibilidade.
- Interface somente leitura, focada em exibir informações consolidadas.

### 2. ModalRadar.razor
- Modal para exibição de gráfico radar (ex: competências, desempenho).
- Integra com JSInterop para renderização de gráficos via Chart.js.
- Recebe dados e parâmetros de exibição do componente pai.

### 3. ModalComplexidade.razor
- Modal para edição da complexidade de um projeto.
- Carrega opções de complexidade de forma dinâmica via serviço de combos reutilizável.
- Permite salvar alterações e notifica o componente pai via callback.

## Integração dos Componentes

- Todos os componentes utilizam injeção de dependência para acessar serviços centralizados em Services/Common e Services/Resultados.
- O fluxo de dados é reativo: ao abrir um modal ou exibir considerações, os dados são carregados sob demanda, garantindo performance e atualização.
- Mensagens de sucesso, erro ou aviso são exibidas via MessageBoxService, desacoplado da UI.

## Fluxo de Páginas/Componentes (Mermaid)

mermaid
flowchart TD
    ResultadoAvaliacao[ResultadoAvaliacao.razor]
    CardAssociado[CardAssociado.razor]
    TabelaProjetos[TabelaProjetos.razor]
    TabsResultado[TabsResultado.razor]
    ConsideracoesMentor[ConsideracoesMentor.razor]
    ModalRadar[ModalRadar.razor]
    ModalComplexidade[ModalComplexidade.razor]

    ResultadoAvaliacao --> CardAssociado
    ResultadoAvaliacao --> TabelaProjetos
    ResultadoAvaliacao --> TabsResultado
    TabsResultado --> ConsideracoesMentor
    TabelaProjetos --> ModalRadar
    TabelaProjetos --> ModalComplexidade


## Sugestões de Melhorias Futuras

- Implementar edição inline das considerações do mentor, com validação e salvamento assíncrono.
- Adicionar testes de integração para os componentes Blazor.
- Melhorar a acessibilidade dos modais e componentes, garantindo navegação via teclado.
- Centralizar ainda mais os helpers de formatação e combos para facilitar a manutenção.
- Expandir o ModalRadar para suportar múltiplos tipos de gráficos (linha, barra, etc) conforme necessidade do negócio.
- Adicionar logs de telemetria mais detalhados para rastrear interações dos usuários nestes componentes.

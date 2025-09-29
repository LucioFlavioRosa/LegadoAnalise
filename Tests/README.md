# Documentação dos Testes Unitários

## Visão Geral

Esta documentação descreve os testes unitários implementados para garantir a qualidade e robustez do sistema de avaliações Peers Moderno.

## Estrutura dos Testes

### ResultadoService Tests

O `ResultadoService` é responsável por gerenciar operações relacionadas aos resultados de avaliações, incluindo listagem, exportação e liberação de resultados.

#### Teste: ListarResultadosAsync_QuandoDbFalha_DeveRetornarListaVaziaEChamarShowError

Este teste garante que o método `ListarResultadosAsync` do `ResultadoService` trata corretamente falhas ao acessar o banco de dados, retornando uma lista vazia e exibindo uma mensagem de erro ao usuário.

##### Objetivo
- Simular uma exceção ao consultar o banco de dados
- Verificar que:
  - O retorno é uma lista vazia
  - O método `ShowError` do `IMessageBoxService` é chamado
  - O método `TrackException` do `ITelemetryService` é chamado

##### Diagrama de Fluxo

mermaid
flowchart TD
    A[Início do método ListarResultadosAsync] --> B{Acesso ao banco de dados}
    B -- Sucesso --> C[Retorna lista de resultados]
    B -- Exceção --> D[Chama TrackException no TelemetryService]
    D --> E[Chama ShowError no MessageBoxService]
    E --> F[Retorna lista vazia]


##### Passos do Teste
1. Configurar um mock do `ApplicationDbContext` para lançar uma exceção ao acessar `ResultadoProjetos`
2. Invocar o método `ListarResultadosAsync`
3. Verificar que o resultado é uma lista vazia
4. Verificar que `ShowError` foi chamado
5. Verificar que `TrackException` foi chamado

##### Importância
Este teste é **crítico** para garantir a robustez do sistema, pois protege a aplicação contra falhas inesperadas no banco de dados, evitando que exceções não tratadas impactem a experiência do usuário e garantindo rastreabilidade de erros via telemetria.

#### Teste: ExportarResultadosLiderancaAsync_QuandoExportFileFalha_DeveRetornarArrayVazioEChamarShowError

Este teste verifica o comportamento do método de exportação quando o serviço de exportação falha.

##### Diagrama de Fluxo

mermaid
flowchart TD
    A[Início ExportarResultadosLiderancaAsync] --> B[Buscar dados do banco]
    B --> C[Chamar serviço de exportação]
    C -- Sucesso --> D[Retornar bytes do arquivo]
    C -- Exceção --> E[Chama TrackException]
    E --> F[Chama ShowError]
    F --> G[Retorna array vazio]


#### Teste: LiberarLiderancaAsync_QuandoPeriodoNaoExiste_DeveRetornarFalse

Testa o cenário onde o período solicitado para liberação não existe no banco de dados.

##### Diagrama de Fluxo

mermaid
flowchart TD
    A[Início LiberarLiderancaAsync] --> B[Buscar período no banco]
    B -- Período encontrado --> C[Atualizar flag de liberação]
    C --> D[Salvar alterações]
    D --> E[Retornar true]
    B -- Período não encontrado --> F[Retornar false]


### ComboHelper Tests

O `ComboHelper` fornece métodos utilitários para criação de listas de opções para componentes de interface.

#### Teste: GetNotasCompetenciaItems_DeveRetornarItensEsperados_QuandoIncludeSelecionarTrueOuFalse

Verifica se o método retorna a estrutura correta de itens baseado no parâmetro `includeSelecionar`.

##### Diagrama de Fluxo

mermaid
flowchart TD
    A[GetNotasCompetenciaItems chamado] --> B{includeSelecionar?}
    B -- true --> C[Adicionar item [Selecionar]]
    B -- false --> D[Pular item [Selecionar]]
    C --> E[Adicionar itens 1-5]
    D --> E
    E --> F[Retornar lista completa]


#### Teste: GetSelectedText_DeveRetornarSelecionadoOuSelecionarPadrao

Testa a lógica de busca de texto baseado no valor selecionado.

##### Diagrama de Fluxo

mermaid
flowchart TD
    A[GetSelectedText chamado] --> B{Value é nulo/vazio?}
    B -- Sim --> C[Retornar [Selecionar]]
    B -- Não --> D[Buscar item na lista]
    D --> E{Item encontrado?}
    E -- Sim --> F[Retornar texto do item]
    E -- Não --> C


### MessageBoxService Tests

O `MessageBoxService` gerencia a exibição de mensagens para o usuário.

#### Teste: ShowSuccess_DeveDispararEventoComTipoCorreto

Verifica se o evento é disparado corretamente com os parâmetros adequados.

##### Diagrama de Fluxo

mermaid
flowchart TD
    A[ShowSuccess chamado] --> B[Criar MessageBoxEventArgs]
    B --> C[Disparar evento OnMessageReceived]
    C --> D[Subscribers recebem evento]


### TelemetryService Tests

O `TelemetryService` é responsável pelo rastreamento de eventos e métricas da aplicação.

#### Teste: TrackEvent_DevePassarParametrosCorretamente

Verifica se os parâmetros são passados corretamente para o cliente de telemetria.

##### Diagrama de Fluxo

mermaid
flowchart TD
    A[TrackEvent chamado] --> B[Validar parâmetros]
    B --> C[Chamar TelemetryClient.TrackEvent]
    C --> D[Evento registrado no Application Insights]


## Estratégias de Teste

### Mocking
- Utilizamos **Moq** para criar mocks de dependências externas
- Todos os testes são isolados, sem dependência de banco de dados real
- Mocks garantem controle total sobre o comportamento das dependências

### Cobertura de Cenários
- **Cenários de Sucesso**: Verificam o comportamento esperado em condições normais
- **Cenários de Falha**: Testam o tratamento de exceções e erros
- **Casos Extremos**: Validam comportamento com entradas nulas, vazias ou inválidas

### Padrões de Nomenclatura
- `MetodoTestado_Condicao_ComportamentoEsperado`
- Exemplo: `ListarResultadosAsync_QuandoDbFalha_DeveRetornarListaVaziaEChamarShowError`

## Execução dos Testes

bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~ResultadoServiceTests"


## Métricas de Qualidade

- **Cobertura de Código**: Objetivo de 90%+ para classes críticas
- **Tempo de Execução**: Todos os testes devem executar em < 100ms
- **Isolamento**: Nenhum teste deve depender de recursos externos
- **Determinismo**: Todos os testes devem produzir resultados consistentes

## Benefícios dos Testes Implementados

1. **Detecção Precoce de Bugs**: Identificação de problemas antes da produção
2. **Refatoração Segura**: Confiança para modificar código existente
3. **Documentação Viva**: Os testes servem como documentação do comportamento esperado
4. **Qualidade do Código**: Força a criação de código mais modular e testável
5. **Rastreabilidade**: Telemetria adequada para monitoramento em produção
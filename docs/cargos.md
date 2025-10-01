# Documentação da Página de Cargos

## Introdução
A funcionalidade de Cargos permite o cadastro, edição, inativação e exportação de cargos dentro do sistema de avaliação interna. Os dados de cargos são estruturados nos modelos `CARGOS` e `CargosModelExport`.

## Modelos de Dados

### CARGOS
- **IdCargo**: Identificador do cargo (int)
- **Cargo**: Nome do cargo (string)
- **idProximoCargo**: ID do próximo cargo (int?)
- **TempoMinimoPromocao**: Tempo mínimo para promoção, em meses (int)
- **Funcao**: Descrição da função (string)
- **Autonomia**: Descrição da autonomia (string)
- **EscopoDeAtuacao**: Descrição do escopo de atuação (string)
- **NivelInterlocucao**: Nível de interlocução principal no cliente (string)
- **ATV**: Status do cargo (1 = Ativo, 0 = Inativo) (int)
- **DHC**: Data/Hora de criação (DateTime)

### CargosModelExport
- **IdCargo**: Identificador do cargo (int)
- **Cargo**: Nome do cargo (string)
- **IdProximoCargo**: ID do próximo cargo (int)
- **ProximoCargo**: Nome do próximo cargo (string)
- **TempoMinimoPromocao**: Tempo mínimo para promoção, em meses (int)
- **Funcao**: Descrição da função (string)
- **Autonomia**: Descrição da autonomia (string)
- **EscopoDeAtuacao**: Descrição do escopo de atuação (string)
- **NivelInterlocucao**: Nível de interlocução principal no cliente (string)
- **ATV**: Status do cargo (1 = Ativo, 0 = Inativo) (int)

## Integração e Fluxo

A seguir, o fluxo geral de páginas e processos relacionados à funcionalidade de cargos:

mermaid
flowchart TD
    A[Usuário acessa /cargos] --> B{OnInitializedAsync}
    B --> C[CargosService.ObterListaCargos]
    C --> D[Renderiza lista de cargos na DataTable]
    D --> E{Usuário clica em Alterar?}
    E -- Sim --> F[AlterarCargo idCargo]
    F --> G[CargosService.ObterCargo idCargo]
    G --> H[Preenche formulário com dados do cargo]
    H --> I[Usuário edita e clica em Salvar]
    E -- Não --> J{Usuário preenche formulário e clica em Salvar?}
    J -- Sim --> I
    I --> K{CargoAtual.IdCargo == 0?}
    K -- Sim --> L[CargosService.InserirCargo]
    K -- Não --> M[CargosService.AlterarCargo]
    L --> N[Exibe mensagem de sucesso]
    M --> N
    N --> O[Recarrega lista de cargos]
    O --> D
    J -- Não --> P{Usuário clica em Inativar?}
    P -- Sim --> Q[InativarCargo idCargo]
    Q --> R[CargosService.ExcluirCargo idCargo]
    R --> S[Exibe mensagem de sucesso]
    S --> O
    P -- Não --> T{Usuário clica em Exportar?}
    T -- Sim --> U[ExportarCargos]
    U --> V[CargosService.ObterListaCargos]
    V --> W[Mapeia para CargosModelExport]
    W --> X[ExportFileService.GenerateExcel]
    X --> Y[Download do arquivo Excel]
    T -- Não --> D


## Sugestões de Melhorias
- Implementar validação de campos obrigatórios e regras de negócio no front-end e back-end.
- Adicionar paginação server-side e busca avançada na tabela de cargos.
- Implementar testes unitários para os serviços de cargos.
- Utilizar cache para listas de cargos frequentemente acessadas.
- Implementar auditoria de alterações em cargos.
- Melhorar feedback visual ao usuário durante operações assíncronas (ex: loading spinner).
- Adicionar confirmação de exclusão/inativação via modal.

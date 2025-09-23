# Premissas - Blazor Híbrido (.NET 9)

## Visão Geral
Este documento explica a migração da tela de premissas do sistema legado ASP.NET Web Forms para um componente Blazor Híbrido, detalhando os serviços, modelos e fluxo de dados.

## Componentes Criados

### Modelos de Dados
- **PremissasRadar.cs:** Representa a entidade principal da tabela PREMISSAS_RADAR
- **CargoNivel.cs:** Representa os níveis de cargos da tabela CARGOSNIVEIS

### Serviços
- **IPremissasService.cs:** Interface que define os contratos para operações de CRUD
- **PremissasService.cs:** Implementação do serviço de negócio, responsável por:
  - CRUD completo de premissas
  - Carregamento de combos (Eixos, Cargos, Níveis)
  - Operações de ativação/inativação

### Interface do Usuário
- **Premissas.razor:** Componente principal da UI com formulários e tabela dinâmica
- **Premissas.razor.cs:** Code-behind com lógica de apresentação e gerenciamento de estado

## Funcionalidades Implementadas

### Cadastro e Edição
- Formulário reativo com validação em tempo real
- Combos dinâmicos carregados do banco de dados
- Modo de edição inline com cancelamento
- Feedback visual durante operações assíncronas

### Listagem
- Tabela responsiva com dados em tempo real
- Indicadores visuais de status (Ativo/Inativo)
- Ações contextuais (Alterar, Ativar/Inativar)
- Loading states durante carregamento

### Experiência do Usuário
- Mensagens de feedback automáticas
- Renderização híbrida para máxima performance
- Interface responsiva e acessível
- Validação de campos obrigatórios

## Fluxo de Dados

1. **Inicialização:** Usuário acessa a página `/premissas`
2. **Carregamento:** Combos e lista de premissas são carregados em paralelo
3. **Interação:** Usuário preenche formulário ou interage com a tabela
4. **Validação:** Campos são validados antes do envio
5. **Persistência:** Dados são salvos via Entity Framework Core
6. **Atualização:** Interface é atualizada automaticamente
7. **Feedback:** Mensagens informam o resultado das operações

## Melhorias Técnicas

### Performance
- Carregamento assíncrono e paralelo de dados
- Renderização híbrida (Server + WebAssembly)
- Estados de loading para melhor UX

### Manutenibilidade
- Separação clara entre camadas (UI, Service, Data)
- Injeção de dependência para testabilidade
- Código limpo e bem estruturado

### Escalabilidade
- Padrão Repository implícito via Entity Framework
- Interfaces bem definidas para extensibilidade
- Configuração centralizada em appsettings.json

## Configuração

O serviço foi registrado no `Program.cs` para injeção de dependência:

csharp
builder.Services.AddScoped<IPremissasService, PremissasService>();


A string de conexão está configurada em `appsettings.json` e o DbContext foi atualizado com as novas entidades.

## Próximos Passos

1. Implementar autenticação e autorização
2. Adicionar logs estruturados
3. Implementar cache para combos
4. Adicionar testes unitários e de integração
5. Implementar paginação para grandes volumes de dados
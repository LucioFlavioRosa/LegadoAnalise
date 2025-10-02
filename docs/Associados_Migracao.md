# Documentação da Migração: Página de Associados para Blazor (.NET 9)

## Visão Geral

Esta documentação descreve a estrutura, funcionamento e integração dos arquivos criados para a migração da página de Associados do sistema legado para a nova arquitetura Blazor Web App (.NET 9). Foram criados os modelos POCO das entidades do domínio e o contexto de dados com Entity Framework Core, base para toda a lógica de negócio da aplicação.

## Estrutura dos Arquivos

- **Peers.Moderno.csproj**: Projeto principal Blazor Web App (.NET 9), com dependências para Entity Framework Core, manipulação de imagens e Excel.
- **Models/**: Contém as classes POCO que representam as entidades do domínio (ASSOCIADOS, CARGOS, PERFIS, VERTICAL, EMPRESAS, PROMOCOES, FOTOSASSOCIADOS).
- **Data/ApplicationDbContext.cs**: Contexto de dados do Entity Framework Core, com DbSets para todas as entidades e mapeamentos de relacionamento.

## Integração e Funcionamento

- As entidades do domínio são mapeadas diretamente para as tabelas do banco de dados.
- O ApplicationDbContext gerencia o acesso aos dados e os relacionamentos entre as entidades.
- Os serviços de negócio (a serem implementados nos próximos passos) irão injetar o ApplicationDbContext para realizar operações de CRUD e consultas.

## Fluxo do Processo (Mermaid)

mermaid
graph TD
    subgraph Estrutura de Dados
        A[ASSOCIADOS]
        B[CARGOS]
        C[PERFIS]
        D[VERTICAL]
        E[EMPRESAS]
        F[PROMOCOES]
        G[FOTOSASSOCIADOS]
    end
    A -- IdCargo --> B
    A -- IdPerfil --> C
    A -- IdVertical --> D
    A -- IdEmpresa --> E
    A -- IdAssociadoMentor --> A
    F -- idAssociado --> A
    F -- idCargoAnterior --> B
    F -- idCargoNovo --> B
    G -- IdAssociado --> A


## Estrutura de Páginas/Componentes Envolvidas

- Models/ASSOCIADOS.cs
- Models/CARGOS.cs
- Models/PERFIS.cs
- Models/VERTICAL.cs
- Models/EMPRESAS.cs
- Models/PROMOCOES.cs
- Models/FOTOSASSOCIADOS.cs
- Data/ApplicationDbContext.cs

## Sugestões de Melhorias

- Adicionar Data Annotations mais detalhadas para validação (ex: tamanho máximo de campos, expressões regulares para e-mail, etc.).
- Implementar métodos auxiliares de navegação para facilitar queries complexas.
- Utilizar Value Objects para campos compostos ou que demandam lógica de validação específica.
- Adicionar comentários de documentação para cada propriedade (quando apropriado).
- Configurar índices e constraints adicionais no OnModelCreating para garantir integridade referencial e performance.
- Considerar o uso de migrations automáticas para facilitar o versionamento do banco de dados.
- Avaliar o uso de Soft Delete (campo de exclusão lógica) para entidades críticas.
- Implementar logging e tratamento de exceções centralizado nas operações de dados.
- Planejar testes de integração para garantir a correta configuração dos relacionamentos.

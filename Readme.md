# Sistema de Avaliação - FIOP

Este repositório contém o código-fonte do sistema de avaliação para a plataforma FIOP, desenvolvido para permitir a gestão de avaliações de usuários, incluindo o login com Azure AD e notificações por e-mail.

## Estrutura do Projeto

Abaixo está a descrição dos principais diretórios e arquivos do projeto:

- **Business/**: Contém a lógica de negócios da aplicação, com classes e métodos que implementam as regras de negócio específicas para o sistema de avaliação.
  
- **LoginAzureAD/**: Módulo responsável pela integração com o Azure Active Directory, utilizado para autenticação dos usuários no sistema.

- **MonitorEmail/**: Contém funcionalidades para envio de e-mails e monitoramento de notificações, usadas para manter os usuários informados sobre eventos importantes.

- **Properties/**: Diretório com configurações da aplicação, incluindo parâmetros e constantes usadas em diversas partes do sistema.

- **SisAval.Upgrades/**: Área destinada para futuras atualizações e melhorias incrementais da aplicação, permitindo fácil controle de versões e alterações.

- **SistemaAvaliacao/**: Diretório principal contendo os arquivos centrais do sistema de avaliação, incluindo o código que integra todas as funcionalidades da aplicação.

- **Content/**: Recursos estáticos, como CSS e outros arquivos de design usados na interface da aplicação.

- **dist/**: Pasta de distribuição para arquivos compilados e preparados para o ambiente de produção.

- **Scripts/**: Scripts utilitários que auxiliam em tarefas de configuração, inicialização ou manutenção do sistema.

- **azure-pipelines.yml**: Configuração do Azure Pipelines para CI/CD, facilitando o processo de integração e entrega contínua.

- **package.json** e **package-lock.json**: Arquivos de configuração do Node.js, listando as dependências da aplicação e versões específicas.

## Tecnologias Utilizadas

- **ASP.NET**: Framework de desenvolvimento para a criação da aplicação web.
- **Azure Active Directory**: Serviço para autenticação e autorização de usuários.
- **Azure DevOps**: Plataforma de CI/CD e controle de versão.
- **Node.js**: Utilizado para gerenciamento de pacotes e algumas dependências front-end.

## Contribuição

Para contribuir com este projeto:

1. Faça um fork do repositório.
2. Crie uma branch para suas alterações (`git checkout -b feature/MinhaFeature`).
3. Commit suas mudanças (`git commit -am 'Adicionando Minha Feature'`).
4. Push para a branch (`git push origin feature/MinhaFeature`).
5. Abra um Pull Request.
using Business.DataAccess;
using Business.Services;
using System;
using Tria.Framework.Domain.Service;

namespace MonitorEmail
{
    class Program
    {
        private enum EnumModeloEmail
        {
            DiasSemAlteracao,
            DiasParaTermino
        }

        static void Main(string[] args)
        {
            ChecaEtapaAutoAvaliacao();
            ChecaEtapaAsCegas();
            ChecaEtapaGestor();
            ChecaEtapaFeedback();
        }

        private static void ChecaEtapaAutoAvaliacao()
        {
            var avaliacaoService = new AvaliacoesService();
            var periodoAtual = new PeriodoService().ObterPeriodoAtual();
            var workflow = new WorkflowService().ObterByPeriodo(periodoAtual.IdPeriodo);
            var avaliacoesEmail = avaliacaoService.ObterAvaliacoesEmail(periodoAtual.IdEmpresa.Value, periodoAtual.IdPeriodo);

            foreach (var avaliacaoEmail in avaliacoesEmail)
            {
                // Só envia se a avaliação estiver Liberada
                var avaliacoes = avaliacaoService.ObterAvaliacoesCompetencias(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo);

                foreach (var avaliacao in avaliacoes)
                {
                    // Avaliações sem Alteração há mais de 2 dias
                    var ultimaAlteracao = DateTime.Now - avaliacao.DHCAutoAvaliacao;
                    var dataFimAvaliacao = avaliacao.DataHoraInicio.AddDays(workflow.DiasAutoAvaliacao);
                    var prazoParaFimAvaliacao = dataFimAvaliacao - DateTime.Now;

                    if (ultimaAlteracao.TotalDays >= workflow.DiasAlertaSemAlteracao)
                    {
                        // Envia Alerta sobre x dias sem alteração na avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasSemAlteracao, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }

                    if (prazoParaFimAvaliacao.TotalDays == 3)
                    {
                        // Envia Alerta faltando 3 dias para o fim da avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasParaTermino, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }

                    if (prazoParaFimAvaliacao.TotalDays == 1)
                    {
                        // Envia Alerta faltando 3 dias para o fim da avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasParaTermino, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }

                    // Indica temporariamenta como Inativo (sem salvar no database) apenas para checar no final do 
                    // Foreach se encontrou todas as avaliações. Isso é importante pois pode existir uma avaliação que já foi
                    // gerada e enviada, mas se o Associado sequer abriu para iniciar dentro de 2 dias, não criou registro na 
                    // tabela AvaliacaoCompetencia, mas o email de alerta deve ser enviado normalmente
                    avaliacaoEmail.Liberado = false;
                }
            }

            // Verifica se restou alguma avaliacaoEmail sem enviar 
            avaliacoesEmail = avaliacoesEmail.FindAll(a => a.Liberado = true);
            foreach (var avaliacaoEmail in avaliacoesEmail)
            {
                // Avaliações sem Alteração há mais de 2 dias
                var ultimaAlteracao = DateTime.Now - avaliacaoEmail.DataLiberacao.Value;
                var dataFimAvaliacao = avaliacaoEmail.DataLiberacao.Value.AddDays(workflow.DiasAutoAvaliacao);
                var prazoParaFimAvaliacao = dataFimAvaliacao - DateTime.Now;

                //// TERMINAR DAQUI PRA BAIXO
                ///

                if (ultimaAlteracao.TotalDays >= workflow.DiasAlertaSemAlteracao)
                {
                    // Envia Alerta sobre x dias sem alteração na avaliação
                    GerarEnviar(avaliacaoEmail.ASSOCIADOS, avaliacaoEmail.PROJETOS, EnumModeloEmail.DiasSemAlteracao, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacaoEmail.PosicaoAtualFluxoAvaliacao);
                }

                if (prazoParaFimAvaliacao.TotalDays == 3)
                {
                    // Envia Alerta faltando 3 dias para o fim da avaliação
                    GerarEnviar(avaliacaoEmail.ASSOCIADOS, avaliacaoEmail.PROJETOS, EnumModeloEmail.DiasParaTermino, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacaoEmail.PosicaoAtualFluxoAvaliacao);
                }

                if (prazoParaFimAvaliacao.TotalDays == 1)
                {
                    // Envia Alerta faltando 3 dias para o fim da avaliação
                    GerarEnviar(avaliacaoEmail.ASSOCIADOS, avaliacaoEmail.PROJETOS, EnumModeloEmail.DiasParaTermino, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacaoEmail.PosicaoAtualFluxoAvaliacao);
                }
            }
        }

        private static void ChecaEtapaAsCegas()
        {
            var avaliacaoService = new AvaliacoesService();
            var periodoAtual = new PeriodoService().ObterPeriodoAtual();
            var workflow = new WorkflowService().ObterByPeriodo(periodoAtual.IdPeriodo);
            var avaliacoesEmail = avaliacaoService.ObterAvaliacoesEmail(periodoAtual.IdEmpresa.Value, periodoAtual.IdPeriodo);

            foreach (var avaliacaoEmail in avaliacoesEmail)
            {
                // Só envia se a avaliação estiver Liberada
                var avaliacoes = avaliacaoService.ObterAvaliacoesCompetencias(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo);
                foreach (var avaliacao in avaliacoes)
                {
                    // Avaliações sem Alteração há mais de 2 dias
                    var ultimaAlteracao = DateTime.Now - avaliacao.DHCAvaliacaoCegas;
                    var dataFimAvaliacao = avaliacao.DataHoraInicio.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas);
                    var prazoParaFimAvaliacao = dataFimAvaliacao - DateTime.Now;

                    if (ultimaAlteracao?.TotalDays >= workflow.DiasAlertaSemAlteracao)
                    {
                        // Envia Alerta sobre x dias sem alteração na avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasSemAlteracao, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }

                    if (prazoParaFimAvaliacao.TotalDays == 3)
                    {
                        // Envia Alerta faltando 3 dias para o fim da avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasParaTermino, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }

                    if (prazoParaFimAvaliacao.TotalDays == 1)
                    {
                        // Envia Alerta faltando 3 dias para o fim da avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasParaTermino, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }
                }
            }
        }

        private static void ChecaEtapaGestor()
        {
            var avaliacaoService = new AvaliacoesService();
            var periodoAtual = new PeriodoService().ObterPeriodoAtual();
            var workflow = new WorkflowService().ObterByPeriodo(periodoAtual.IdPeriodo);
            var avaliacoesEmail = avaliacaoService.ObterAvaliacoesEmail(periodoAtual.IdEmpresa.Value, periodoAtual.IdPeriodo);

            foreach (var avaliacaoEmail in avaliacoesEmail)
            {
                // Só envia se a avaliação estiver Liberada
                var avaliacoes = avaliacaoService.ObterAvaliacoesCompetencias(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo);

                foreach (var avaliacao in avaliacoes)
                {
                    // Avaliações sem Alteração há mais de 2 dias
                    var ultimaAlteracao = DateTime.Now - avaliacao.DHCAvaliacaoCegas;
                    var dataFimAvaliacao = avaliacao.DataHoraInicio.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor);
                    var prazoParaFimAvaliacao = dataFimAvaliacao - DateTime.Now;

                    if (ultimaAlteracao?.TotalDays >= workflow.DiasAlertaSemAlteracao)
                    {
                        // Envia Alerta sobre x dias sem alteração na avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasSemAlteracao, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }

                    if (prazoParaFimAvaliacao.TotalDays == 3)
                    {
                        // Envia Alerta faltando 3 dias para o fim da avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasParaTermino, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }

                    if (prazoParaFimAvaliacao.TotalDays == 1)
                    {
                        // Envia Alerta faltando 3 dias para o fim da avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasParaTermino, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }
                }
            }
        }

        private static void ChecaEtapaFeedback()
        {
            var avaliacaoService = new AvaliacoesService();
            var periodoAtual = new PeriodoService().ObterPeriodoAtual();
            var workflow = new WorkflowService().ObterByPeriodo(periodoAtual.IdPeriodo);
            var avaliacoesEmail = avaliacaoService.ObterAvaliacoesEmail(periodoAtual.IdEmpresa.Value, periodoAtual.IdPeriodo);

            foreach (var avaliacaoEmail in avaliacoesEmail)
            {
                // Só envia se a avaliação estiver Liberada
                var avaliacoes = avaliacaoService.ObterAvaliacoesCompetencias(avaliacaoEmail.idAssociado, avaliacaoEmail.idProjeto, avaliacaoEmail.idPeriodo);

                foreach (var avaliacao in avaliacoes)
                {
                    // Avaliações sem Alteração há mais de 2 dias
                    var ultimaAlteracao = DateTime.Now - avaliacao.DHCAvaliacaoCegas;
                    var dataFimAvaliacao = avaliacao.DataHoraInicio.AddDays(workflow.DiasAutoAvaliacao + workflow.DiasAvaliacaoCegas + workflow.DiasAvaliacaoGestor + workflow.DiasFeedback);
                    var prazoParaFimAvaliacao = dataFimAvaliacao - DateTime.Now;

                    if (ultimaAlteracao?.TotalDays >= workflow.DiasAlertaSemAlteracao)
                    {
                        // Envia Alerta sobre x dias sem alteração na avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasSemAlteracao, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }

                    if (prazoParaFimAvaliacao.TotalDays == 3)
                    {
                        // Envia Alerta faltando 3 dias para o fim da avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasParaTermino, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }

                    if (prazoParaFimAvaliacao.TotalDays == 1)
                    {
                        // Envia Alerta faltando 3 dias para o fim da avaliação
                        GerarEnviar(avaliacao.ASSOCIADOS, avaliacao.PROJETOS, EnumModeloEmail.DiasParaTermino, dataFimAvaliacao.ToString("dd/MM/yyyy"), avaliacao.PosicaoAtualFluxoAvaliacao);
                    }
                }
            }
        }

        private static void GerarEnviar(ASSOCIADOS associado, PROJETOS projeto, EnumModeloEmail modeloEmail, string prazoFinal, string etapaAtual)
        {
            var paramEmail = new EmailParametroService().ObterParametro(projeto.IdEmpresa);
            var configEmail = new ConfigEmail();
            configEmail.From = paramEmail.RemetenteEmail;
            configEmail.SmtpServer = paramEmail.SMTPServer;
            configEmail.Porta = Convert.ToInt32(paramEmail.Porta);
            configEmail.Dominio = paramEmail.Dominio;
            configEmail.Senha = paramEmail.Password;
            configEmail.UsarSSL = Convert.ToBoolean(paramEmail.UsarSSL);
            configEmail.Remetente = paramEmail.RemetenteNome;

            var destinatario = new Destinatario();
            destinatario.Nome = associado.Nome;
            destinatario.Email = associado.Email;


            // MONTA O EMAIL
            var _corpo = "";

            switch (modeloEmail)
            {
                case EnumModeloEmail.DiasSemAlteracao:
                    _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailDiasSemAlteracao);
                    break;
                case EnumModeloEmail.DiasParaTermino:
                    _corpo = new EmailUtils().CarregaMascara(paramEmail.ModeloEmailDiasParaTermino);
                    break;
            }

            _corpo = ConfiguraBody(_corpo, projeto, associado, "", "", "", "", prazoFinal, etapaAtual);

            var mensagem = new Mensagem();
            mensagem.Titulo = "Atualização de Status da Avaliação";
            mensagem.Corpo = _corpo;

            new EmailService(configEmail, destinatario, mensagem).Enviar();
        }

        private static string ConfiguraBody(string corpo, PROJETOS projeto, ASSOCIADOS associado,
            string texto_inicial, string texto_principal, string info_final, string despedida, string prazoFinal, string etapaAtual)
        {
            // Padrão Email
            var newCorpo = corpo.Replace("[INFO_INICIAL]", texto_inicial);
            newCorpo = newCorpo.Replace("[TEXTO_PRINCIPAL]", texto_principal);
            newCorpo = newCorpo.Replace("[INFO_FINAL]", info_final);
            newCorpo = newCorpo.Replace("[CUMPRIMENTOS]", despedida);

            // Dados Pessoais
            newCorpo = newCorpo.Replace("[NOME]", associado.Nome);
            newCorpo = newCorpo.Replace("[CARGO]", associado.CARGOS.Cargo);
            newCorpo = newCorpo.Replace("[MENTOR]", associado.ASSOCIADOS2.Nome);
            newCorpo = newCorpo.Replace("[PROJETO]", projeto.Projeto);
            newCorpo = newCorpo.Replace("[GESTOR]", projeto.ASSOCIADOS1.Nome);
            newCorpo = newCorpo.Replace("[DATA_INICIO]", projeto.DataInicio.ToString("dd/MM/yyyy"));
            newCorpo = newCorpo.Replace("[DATA_FINAL]", projeto.DataFim?.ToString("dd/MM/yyyy"));
            newCorpo = newCorpo.Replace("[PRAZO_FINAL]", prazoFinal);
            newCorpo = newCorpo.Replace("[ETAPA_AVALIACAO]", etapaAtual);

            return newCorpo;
        }

    }
}

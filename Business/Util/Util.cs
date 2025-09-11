using Business.Services;
using System;
using System.Text.RegularExpressions;

namespace Business.Util
{
    public enum PerfilUsuariosEnum
    {
        Socio = 1,
        Gestor = 2,
        Mentor = 3,
        Consultor = 4,
        Consultor_Estagiario = 5,
        Consultor_Trainee = 6,
        Consultor_AnalistaJr = 7,
        Consultor_AnalistaPl = 8,
        Consultor_AnalistaSr = 9,
        Consultor_Consultor = 10,
        Consultor_ConsultorSr = 11
    }

    public enum ReturnTempo
    {
        datetime_TempoDePeers,
        datetime_TempoDeCargo,
        texto_TempoDePeers,
        texto_TempoDeCargo
    }

    public class Util
    {
        public Decimal TrataMoney(string valor)
        {
            var tam = valor.Length;
            int tamTotal = tam - 2;
            var retorno = valor.Substring(2, tamTotal);
            return Convert.ToDecimal(retorno);

        }

        public string FormataMoney(string field)
        {
            try
            {
                if (field.Length == 0)
                    return field;

                //Retira eventuais "," da string
                field = field.Replace(",", "");

                //Retira eventuais "." da string
                field = field.Replace(".", "");

                //Retira zeros à esquerda
                field = Convert.ToString(Convert.ToDouble(field));

                //Se for apenas decimais, acrecenta 0 à esquerda
                if (field.Length < 3)
                    field = field.PadLeft(3, Convert.ToChar("0"));

                //Separa Inteiros e Decimais
                string strValorInt = field.Substring(0, field.Length - 2);
                string strValorDec = field.Substring(field.Length - 2, 2);

                int pos = 3;
                for (int i = 1; i <= 8; i++)   //8 = quantidade de pontos a serem incluídos no número
                {
                    if (strValorInt.Length > pos)
                        strValorInt = strValorInt.Substring(0, strValorInt.Length - pos) + "." + strValorInt.Substring(strValorInt.Length - pos, pos);
                    else
                        break;
                    pos += 4;
                }
                field = strValorInt + "," + strValorDec;
                return "R$" + field;
            }
            catch
            {
                throw new Exception("Número inválido para conversão em valor monetário.");
            }
        }

        public bool ValidaCPF(string cpfCnpj)
        {
            return (IsCpf(cpfCnpj));
        }

        public bool validarData(string data)
        {
            int maiorAnoPermitido = 2200;
            int menorAnoPermitido = 1920;

            if (data.Length < 10)
                return false;

            string[] val = data.Split('/');

            int dia, mes, ano;

            if (int.TryParse(val[0], out dia) && int.TryParse(val[1], out mes) && int.TryParse(val[2], out ano))
            {
                if (ano >= menorAnoPermitido && ano <= maiorAnoPermitido)
                {
                    if (mes >= 1 && mes <= 12)
                    {

                        int maxDia = (mes == 2 ? (ano % 4 == 0 ? 29 : 28) : mes <= 7 ? (mes % 2 == 0 ? 30 : 31) : (mes % 2 == 0 ? 31 : 30));

                        if (dia >= 1 && dia <= maxDia)
                        {

                            //lblMsgData.Text = "Data Válida";
                            return true;
                        }
                        else
                        {
                            //lblMsgData.Text = "Dia inválido";
                            return false;
                        }
                    }
                    else
                    {
                        //lblMsgData.Text = "Mês inválido";
                        return false;
                    }
                }
                else
                {
                    //lblMsgData.Text = "Ano inválido";
                    return false;
                }
            }
            else
            {
                //lblMsgData.Text = "Data inválido";
                return false;
            }
        }

        private static bool IsCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            for (int j = 0; j < 10; j++)
                if (j.ToString().PadLeft(11, char.Parse(j.ToString())) == cpf)
                    return false;

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        public bool ValidarEmail(String email)
        {
            bool emailValido = false;

            email = email.ToLower();

            string emailRegex = string.Format("{0}{1}",
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))",
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");

            try
            {
                emailValido = Regex.IsMatch(
                    email,
                    emailRegex);
            }
            catch (RegexMatchTimeoutException)
            {
                emailValido = false;
            }

            return emailValido;
        }

        public string DataInicioAnoMesDia(string data)
        {
            var dia = data.Substring(0, 2);
            var mes = data.Substring(3, 2);
            var ano = data.Substring(6, 4);
            var dataInicio = ano + "-" + mes + "-" + dia + " 00:00";
            return dataInicio;
        }

        public string DataFimAnoMesDia(string data)
        {
            var dia = data.Substring(0, 2);
            var mes = data.Substring(3, 2);
            var ano = data.Substring(6, 4);
            var dataInicio = ano + "-" + mes + "-" + dia + " 23:59";
            return dataInicio;
        }

        // PROMOÇÕES
        public object TempoAssociado(int IdAssociado, ReturnTempo returnTempo)
        {
            object returnObect = null;
            try
            {
                var associadosService = new AssociadosService();
                var cargosService = new CargosService();

                var associado = associadosService.ObterAssociado(IdAssociado);
                var ultimaPromocao = cargosService.ObterUltimaPromocaoAssociado(associado.IdAssociado);

                string texto_TempoDePeers = "", texto_TempoDeCargo = "";

                if (ultimaPromocao != null)
                {
                    int promocaoAnos = DateTime.Now.Year - ((DateTime)ultimaPromocao.DataPromocao).Year;
                    int promocaoMesesTotal = (promocaoAnos * 12) + DateTime.Now.Month - ((DateTime)ultimaPromocao.DataPromocao).Month;

                    int anoHoje = DateTime.Now.Year, anoPromocao = ((DateTime)ultimaPromocao.DataPromocao).Year;
                    int mesHoje = DateTime.Now.Month, mesPromocao = ((DateTime)ultimaPromocao.DataPromocao).Month;
                    int diferencaMeses = ((anoHoje - anoPromocao) * 12) + mesHoje - mesPromocao;
                    int diferencaAnos = (int)Math.Floor(((decimal)diferencaMeses / 12));
                    diferencaMeses -= diferencaAnos * 12;

                    string anoTexto = diferencaAnos > 0 ? diferencaAnos.ToString() + " ano" : "";
                    string anoPlural = diferencaAnos > 1 ? "s" : "";
                    string preposicao = diferencaAnos > 0 && diferencaMeses > 0 ? " e " : "";
                    string mesesTexto = diferencaMeses > 0 ? diferencaMeses.ToString() + " m": "";
                    string mesesPlural = diferencaMeses > 1 ? "eses" : (diferencaMeses == 1 ? "ês" : "");
                    texto_TempoDeCargo = anoTexto + anoPlural + preposicao + mesesTexto + mesesPlural;
                }

                if (associado.DataAdmissao != null)
                {
                    int admissaoAnos = DateTime.Now.Year - ((DateTime)associado.DataAdmissao).Year;
                    int admissaoMesesTotal = (admissaoAnos * 12) + DateTime.Now.Month - ((DateTime)associado.DataAdmissao).Month;

                    int anoHoje = DateTime.Now.Year, anoAdmissao = ((DateTime)associado.DataAdmissao).Year;
                    int mesHoje = DateTime.Now.Month, mesAdmissao = ((DateTime)associado.DataAdmissao).Month;
                    int diferencaMeses = ((anoHoje - anoAdmissao) * 12) + mesHoje - mesAdmissao;
                    int diferencaAnos = (int)Math.Floor(((decimal)diferencaMeses / 12));
                    diferencaMeses -= diferencaAnos * 12;

                    string anoTexto = diferencaAnos > 0 ? diferencaAnos.ToString() + " ano" : "";
                    string anoPlural = diferencaAnos > 1 ? "s" : "";
                    string preposicao = diferencaAnos > 0 && diferencaMeses > 0 ? " e " : "";
                    string mesesTexto = diferencaMeses > 0 ? diferencaMeses.ToString() + " m" : "";
                    string mesesPlural = diferencaMeses > 1 ? "eses" : (diferencaMeses == 1 ? "ês" : "");
                    texto_TempoDePeers = anoTexto + anoPlural + preposicao + mesesTexto + mesesPlural;
                }

                switch (returnTempo)
                {
                    case ReturnTempo.texto_TempoDePeers:
                        {
                            returnObect = texto_TempoDePeers;
                        }break;
                    case ReturnTempo.texto_TempoDeCargo:
                        {
                            returnObect = texto_TempoDeCargo != "" ? texto_TempoDeCargo : texto_TempoDePeers;
                        }
                        break;
                }
            }
            catch
            {
                returnObect = null;
            }

            return returnObect;
        }
    }
}

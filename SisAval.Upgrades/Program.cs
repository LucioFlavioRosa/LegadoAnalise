using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SistemaAvaliacao;
using SisAval.Upgrades.Models;

namespace SisAval.Upgrades
{
    class Program
    {
        static void Main(string[] args)
        {
            SisAvalContext dtModel = new();
            string nome_inicio, nome_fim;
            List<string> nome_resto = new();

            foreach(Associado assoc in dtModel.Associados)
            {
                nome_inicio = assoc.Nome.Split(" ")[0].ToUpper();
                nome_resto = assoc.Nome.Split(" ").ToList();

                //nome_inicio = nome_inicio.Replace("Í", "I");

                List<string> formats = new() { ".jpg", ".jpeg", ".png" };

                foreach (string nome_meio in nome_resto)
                {
                    nome_fim = nome_meio.ToUpper();
                    //nome_fim = nome_meio.Replace("Í", "I").ToUpper();
                    foreach (string formato in formats)
                    {
                        if (File.Exists("Z:\\OneDrive - Peers Consulting\\Projetos\\SistemaAvaliacao\\" +
                            "UploadImagens\\OneDrive_2021-05-21\\Fotos Associados\\" +
                            nome_inicio + "_" + nome_fim + formato))
                        {
                            assoc.FotoNome = "Content/FotosAssociados/" + nome_inicio + "_" + nome_fim + formato;
                        }
                        else if (File.Exists("Z:\\OneDrive - Peers Consulting\\Projetos\\SistemaAvaliacao\\" +
                            "UploadImagens\\OneDrive_2021-05-21\\Fotos Associados\\" +
                            nome_inicio + " _" + nome_fim + formato))
                        {
                            assoc.FotoNome = "Content/FotosAssociados/" + nome_inicio + " _" + nome_fim + formato;
                        }
                        else if (File.Exists("Z:\\OneDrive - Peers Consulting\\Projetos\\SistemaAvaliacao\\" +
                            "UploadImagens\\OneDrive_2021-05-21\\Fotos Associados\\" +
                            nome_inicio + " _ " + nome_fim + formato))
                        {
                            assoc.FotoNome = "Content/FotosAssociados/" + nome_inicio + " _ " + nome_fim + formato;
                        }
                        else if (File.Exists("Z:\\OneDrive - Peers Consulting\\Projetos\\SistemaAvaliacao\\" +
                            "UploadImagens\\OneDrive_2021-05-21\\Fotos Associados\\" +
                            nome_inicio + " " + nome_fim + formato))
                        {
                            assoc.FotoNome = "Content/FotosAssociados/" + nome_inicio + " " + nome_fim + formato;
                        }
                    }
                }
                
            }
            dtModel.SaveChanges();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;

class Beneficiario
{
    public int SequencialLinha;
    public string Contrato;
    public string Nome;
    public string Familia;
    public string MatriculaFuncional;
    public string Titular;
    public string CPF;
    public string CPFTitular;
    public string Dependente;
    public string Status;
    public string DataAdesao;
    public string DataCancelamento; 
    public string Arquivo;
}

class Program
{
    static void Main()
    {
        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\20251105_Beneficiarios_Emps_1_7_11_12_13_14");
        var arquivos = Directory.GetFiles(downloadsPath, "*.txt");
        var beneficiarios = new List<Beneficiario>();

        Console.WriteLine($"Iniciando processamento de {arquivos.Length} arquivos...");
        foreach (var arquivo in arquivos)
        {
            var linhas = File.ReadAllLines(arquivo);
            string contrato = "", familia = "", matricula = "", titular = "", cpf = "", cpftitular = "", nome = "", dependente = "", status = "", dataCancelamento = "", dataAdesao = "";
            int sequencialLinha = 0;
            string nomeArquivo = Path.GetFileName(arquivo);

            for (int i = 0; i < linhas.Length; i++)
            {
                var linha = linhas[i];
                if (linha.Length < 2) continue;

                var bloco = linha.Substring(0, 2);

                switch (bloco)
                {
                    case "01":
                        // Se já havia dados, salva o beneficiário anterior
                        if (sequencialLinha > 0)
                        {
                            beneficiarios.Add(new Beneficiario
                            {
                                SequencialLinha = sequencialLinha,
                                Contrato = contrato,
                                Familia = familia,
                                MatriculaFuncional = matricula,
                                Titular = titular,
                                CPF = cpf,
                                CPFTitular = cpftitular,
                                Nome = nome,
                                Dependente = dependente,
                                Status = status,
                                DataAdesao = dataAdesao,
                                DataCancelamento = dataCancelamento,
                                Arquivo = nomeArquivo
                            });
                        }

                        // Inicia novo registro
                        sequencialLinha = i + 1;
                        contrato = linha.Length >= 12 ? linha.Substring(2, 10).Trim() : "";
                        titular = linha.Length >= 23 ? linha.Substring(22, 1).Trim() : "";
                        matricula = linha.Length >= 132 ? linha.Substring(102, 30).Trim() : "";
                        cpf = "";
                        cpftitular = "";
                        familia = "";
                        dependente = linha.Length >= 22 ? linha.Substring(12, 10).Trim() : "";
                        status = linha.Length >= 33 ? linha.Substring(24, 9).Trim() : "";
                        dataAdesao = linha.Length >= 168 ? linha.Substring(160, 8).Trim() : "";
                        dataCancelamento = linha.Length >= 41 ? linha.Substring(33, 8).Trim() : "";
                        break;

                    case "15":
                        cpf = linha.Length >= 171 ? linha.Substring(160, 11).Trim() : "";
                        nome = linha.Length >= 52 ? linha.Substring(2, 50).Trim() : "";
                        break;

                    case "20":
                        familia = linha.Length >= 12 ? linha.Substring(2, 10).Trim() : "";
                        cpftitular = linha.Length >= 43 ? linha.Substring(32, 11).Trim() : "";
                        break;

                    case "40":
                        // Finaliza o registro
                        if (sequencialLinha > 0)
                        {
                            beneficiarios.Add(new Beneficiario
                            {
                                SequencialLinha = sequencialLinha,
                                Contrato = contrato,
                                Familia = familia,
                                MatriculaFuncional = matricula,
                                Titular = titular,
                                CPF = cpf,
                                CPFTitular = cpftitular,
                                Nome = nome,
                                Dependente = dependente,  
                                Status = status,
                                DataAdesao = dataAdesao,
                                DataCancelamento = dataCancelamento,
                                Arquivo = nomeArquivo
                            });
                            sequencialLinha = 0;
                        }
                        break;
                }
            }

            // Se o último registro não foi finalizado com bloco 40
            if (sequencialLinha > 0)
            {
                beneficiarios.Add(new Beneficiario
                {
                    SequencialLinha = sequencialLinha,
                    Contrato = contrato,
                    Familia = familia,
                    MatriculaFuncional = matricula,
                    Titular = titular,
                    CPF = cpf,
                    CPFTitular = cpftitular,
                    Nome = nome,
                    Dependente = dependente,
                    Status = status,
                    DataAdesao = dataAdesao,
                    DataCancelamento = dataCancelamento,
                    Arquivo = nomeArquivo
                });
            }
            Console.WriteLine($"Processado arquivo: {nomeArquivo}");
        }

        // Exportar para CSV
        string csvPath = Path.Combine(downloadsPath, "beneficiarios.csv");
        using (var writer = new StreamWriter(csvPath))
        {
            writer.WriteLine("SEQUENCIAL_LINHA;NOME;CONTRATO;FAMILIA;SEQUENCIALDEP;MATRICULAFUNCIONAL;TITULAR;CPF;CPFTITULAR;STATUS;DATAADESAO;DATACANCELAMENTO;ARQUIVO");
            foreach (var b in beneficiarios)
            {
                writer.WriteLine($"{b.SequencialLinha};{b.Nome};{b.Contrato};{b.Familia};{b.Dependente};{b.MatriculaFuncional};{b.Titular};{b.CPF};{b.CPFTitular};{b.Status};{b.DataAdesao};{b.DataCancelamento};{b.Arquivo}");
            }
        }

        Console.WriteLine($"Arquivo CSV gerado com sucesso em: {csvPath}");
    }
}
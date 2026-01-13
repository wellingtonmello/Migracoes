using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

class Beneficiario
{
    public int SequencialLinha;
    public string Contrato;
    public string Nome;
    public string NomeMae;
    public string NomePai;
    public string Familia;
    public string MatriculaFuncional;
    public string Titular;
    public string CPF;
    public string CPFTitular;
    public string Dependente;
    public string Status;
    public string DataAdesao;
    public string DataCancelamento;
    public string DataFalecimento;
    public string Agencia;
    public string Banco;
    public string ContaCorrente;
    public string DVCC;
    public string CodigoAntigo;
    public string LogradouroResidencial;
    public string ComplementoResidencial;
    public string BairroResidencial;
    public string CepResidencial;
    public string LogradouroCorrespondencia;
    public string ComplementoCorrespondencia;   
    public string BairroCorrespondencia;
    public string CepCorrespondencia;
    public string Arquivo;
}

class Program
{
    static void Main()
    {
        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\20251130_benefs_empresas_1_7_11_12_13_14");
        var arquivos = Directory.GetFiles(downloadsPath, "*.txt");
        var beneficiarios = new List<Beneficiario>();
        int contadorArquivo = 0;
        Console.WriteLine($"Iniciando processamento de {arquivos.Length} arquivos...");
        foreach (var arquivo in arquivos.OrderBy(f =>
                                          {
                                              string nome = Path.GetFileNameWithoutExtension(f);
                                              string numeroStr = new string(nome.TakeWhile(char.IsDigit).ToArray());
                                              return int.TryParse(numeroStr, out int numero) ? numero : int.MaxValue;
                                          })
                                                    .ToList()
                                        )
        {
            var linhas = File.ReadAllLines(arquivo);
            string contrato = "", familia = "", matricula = "", titular = "", cpf = "", cpftitular = "", nome = "", dependente = "", status = "", dataCancelamento = "", dataAdesao = "", nomeMae = "", agencia = "", banco = "" ,codigoAntigo = "", contaCorrente = "", dvCC = "";
            string logradouroResidencial = "", complementoResidencial = "", bairroResidencial = "", cepResidencial = "", logradouroCorrespondencia = "", complementoCorrespondencia = "", bairroCorrespondencia = "", cepCorrespondencia = "", nomePai = "", dataFalecimento = "";

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
                                NomeMae = nomeMae,
                                NomePai = nomePai,
                                Dependente = dependente,
                                Status = status,
                                DataAdesao = dataAdesao,
                                DataCancelamento = dataCancelamento,
                                DataFalecimento = dataFalecimento,
                                Agencia = agencia,
                                Banco = banco,
                                ContaCorrente = contaCorrente,
                                DVCC = dvCC,
                                CodigoAntigo = codigoAntigo,
                                LogradouroCorrespondencia = logradouroCorrespondencia,
                                ComplementoCorrespondencia = complementoCorrespondencia,
                                BairroCorrespondencia = bairroCorrespondencia,
                                CepCorrespondencia = cepCorrespondencia,
                                LogradouroResidencial = logradouroResidencial,
                                ComplementoResidencial = complementoResidencial,
                                BairroResidencial = bairroResidencial,
                                CepResidencial = cepResidencial,
                                Arquivo = nomeArquivo
                                
                            });
                            contrato = "";
                            familia = "";
                            matricula = "";
                            titular = "";   
                            nomePai = "";
                            cpf = "";
                            cpftitular = "";
                            nome = "";
                            nomeMae = "";
                            dependente = "";
                            status = "";
                            dataAdesao = "";
                            dataCancelamento = "";
                            dataFalecimento = "";
                            agencia = "";
                            banco = "";
                            contaCorrente = "";
                            dvCC = "";
                            codigoAntigo = "";
                            logradouroCorrespondencia = "";
                            complementoCorrespondencia = "";
                            bairroCorrespondencia = "";
                            cepCorrespondencia = "";
                            logradouroResidencial = "";
                            complementoResidencial = "";
                            bairroResidencial = "";
                            cepResidencial = "";

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
                        codigoAntigo = linha.Length >= 152 ? linha.Substring(132, 20).Trim() : "";
                        break;

                    case "15":
                        cpf = linha.Length >= 171 ? linha.Substring(160, 11).Trim() : "";
                        nome = linha.Length >= 52 ? linha.Substring(2, 50).Trim() : "";
                        nomeMae = linha.Length >= 102 ? linha.Substring(52, 50).Trim() : "";
                        nomePai = linha.Length >= 152 ? linha.Substring(102, 50).Trim() : "";
                        dataFalecimento = linha.Length >= 230 ? linha.Substring(222, 8).Trim() : "";
                        break;

                    case "16":
                        logradouroResidencial = Regex.Replace(linha.Length >= 99 ? linha.Substring(49, 50).Trim() : "", @"[^a-zA-Z0-9 ]", "");
                        complementoResidencial = Regex.Replace(linha.Length >= 139 ? linha.Substring(99, 40).Trim() : "", @"[^a-zA-Z0-9 ]", "");
                        bairroResidencial = Regex.Replace(linha.Length >= 189 ? linha.Substring(149, 40).Trim() : "", @"[^a-zA-Z0-9 ]", "");
                        cepResidencial = linha.Length >= 49 ? linha.Substring(39, 10).Trim() : "";
                        break;
                     
                    case "18":
                        logradouroCorrespondencia = Regex.Replace(linha.Length >= 99 ? linha.Substring(49, 50).Trim() : "", @"[^a-zA-Z0-9 ]", "");
                        complementoCorrespondencia = Regex.Replace(linha.Length >= 139 ? linha.Substring(99, 40).Trim() : "", @"[^a-zA-Z0-9 ]", "");
                        bairroCorrespondencia = Regex.Replace(linha.Length >= 189 ? linha.Substring(149, 40).Trim() : "", @"[^a-zA-Z0-9 ]", "");
                        cepCorrespondencia = linha.Length >= 49 ? linha.Substring(39, 10).Trim() : "";
                        break;

                    case "20":
                        familia = linha.Length >= 12 ? linha.Substring(2, 10).Trim() : "";
                        cpftitular = linha.Length >= 43 ? linha.Substring(32, 11).Trim() : "";
                        break;

                    case "40":
                        banco = linha.Length >= 7 ? linha.Substring(4, 3).Trim() : "";
                        agencia = linha.Length >= 11 ? linha.Substring(7, 4).Trim() : "";
                        contaCorrente = linha.Length >= 24 ? linha.Substring(12, 12).Trim() : "";
                        dvCC = linha.Length >= 25 ? linha.Substring(24, 1).Trim() : "";
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
                                NomeMae = nomeMae,
                                NomePai = nomePai,
                                Dependente = dependente,  
                                Status = status,
                                DataAdesao = dataAdesao,
                                DataFalecimento = dataFalecimento,
                                DataCancelamento = dataCancelamento,
                                Agencia = agencia,
                                Banco = banco,
                                ContaCorrente = contaCorrente,
                                DVCC = dvCC,
                                CodigoAntigo = codigoAntigo,
                                LogradouroCorrespondencia = logradouroCorrespondencia,
                                ComplementoCorrespondencia = complementoCorrespondencia,
                                BairroCorrespondencia = bairroCorrespondencia,
                                CepCorrespondencia = cepCorrespondencia,
                                LogradouroResidencial = logradouroResidencial,
                                ComplementoResidencial = complementoResidencial,
                                BairroResidencial = bairroResidencial,
                                CepResidencial = cepResidencial,
                                Arquivo = nomeArquivo
                            });
                            sequencialLinha = 0;
                            contrato = "";
                            familia = "";
                            matricula = "";
                            titular = "";
                            cpf = "";
                            cpftitular = "";
                            nome = "";
                            nomeMae = "";
                            nomePai = "";
                            dependente = "";
                            status = "";
                            dataAdesao = "";
                            dataCancelamento = "";
                            dataFalecimento = "";
                            agencia = "";
                            banco = "";
                            contaCorrente = "";
                            dvCC = "";
                            codigoAntigo = "";
                            logradouroCorrespondencia = "";
                            complementoCorrespondencia = "";
                            bairroCorrespondencia = "";
                            cepCorrespondencia = "";
                            logradouroResidencial = "";
                            complementoResidencial = "";
                            bairroResidencial = "";
                            cepResidencial = "";

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
                    NomeMae = nomeMae,
                    NomePai = nomePai,
                    Dependente = dependente,
                    Status = status,
                    DataAdesao = dataAdesao,
                    DataFalecimento = dataFalecimento,
                    DataCancelamento = dataCancelamento,
                    Agencia = agencia,
                    Banco = banco,
                    ContaCorrente = contaCorrente,
                    DVCC = dvCC,
                    CodigoAntigo = codigoAntigo,
                    LogradouroCorrespondencia = logradouroCorrespondencia,
                    ComplementoCorrespondencia = complementoCorrespondencia,
                    BairroCorrespondencia = bairroCorrespondencia,
                    CepCorrespondencia = cepCorrespondencia,
                    LogradouroResidencial = logradouroResidencial,
                    ComplementoResidencial = complementoResidencial,
                    BairroResidencial = bairroResidencial,
                    CepResidencial = cepResidencial,
                    Arquivo = nomeArquivo
                });
            }
            Console.WriteLine($"Processado arquivo: {nomeArquivo}");
        }

        // Exportar para CSV
        string csvPath = Path.Combine(downloadsPath, "beneficiarios.csv");
        using (var writer = new StreamWriter(csvPath))
        {
            writer.WriteLine("SEQUENCIAL_LINHA;CONTADORARQUIVO;NOME;NOMEMAE;NOMEPAI;CONTRATO;FAMILIA;SEQUENCIALDEP;MATRICULAFUNCIONAL;TITULAR;CPF;CPFTITULAR;STATUS;DATAADESAO;DATAFALECIMENTO;DATACANCELAMENTO;BANCO;AGENCIA;CONTACORRENTE;DVCC;CODIGOANTIGO;LOGRADOUROCORR;COMPLEMENTOCORRESP;BAIRROCORRESP;CEPCORRESP;LOGRADOURORES;COMPLEMENTORES;BAIRRORES;CEPRES;ARQUIVO;");
            foreach (var b in beneficiarios)
            {
                contadorArquivo++;
                writer.WriteLine($"{b.SequencialLinha};{contadorArquivo};{b.Nome};{b.NomeMae};{b.NomePai};{b.Contrato};{b.Familia};{b.Dependente};{b.MatriculaFuncional};{b.Titular};{b.CPF};{b.CPFTitular};{b.Status};{b.DataAdesao};{b.DataFalecimento};{b.DataCancelamento};{b.Banco};{b.Agencia};{b.ContaCorrente};{b.DVCC};{b.CodigoAntigo};" +
                    $"{b.LogradouroCorrespondencia};{b.ComplementoCorrespondencia};{b.BairroCorrespondencia};{b.CepCorrespondencia};{b.LogradouroResidencial};{b.ComplementoResidencial};{b.BairroResidencial};{b.CepResidencial};{b.Arquivo};");
            }
        }

        Console.WriteLine($"Arquivo CSV gerado com sucesso em: {csvPath}");
    }
}
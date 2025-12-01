
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class ConversorCSV
{
    static void Main(string[] args)
    {
        // Diretório padrão
        string diretorio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ManipuladorTGE");

        // Caminhos dos arquivos
        string caminhoCsvEntrada = Path.Combine(diretorio, "Extracao_TGA_CBHPM_26112025.csv");
        string caminhoCodigos = Path.Combine(diretorio, "codigos.csv"); // Arquivo anexado convertido para CSV
        string caminhoCsvSaida = Path.Combine(diretorio, "saida.csv");

        // Verifica se diretório existe
        if (!Directory.Exists(diretorio))
        {
            Console.WriteLine($"Diretório não encontrado: {diretorio}");
            return;
        }

        // Tabela de máscaras fornecida
        var mascaras = new Dictionary<int, string>
        {
            {1, "9.99.99.999"},
            {105, "99999999"},
            {104, "99999"},
            {24, "99999999"},
            {106, "99999"},
            {107, "9999999"},
            {108, "9"},
            {109, "99999999"},
            {44, "99999999"},
            {49, "99999999"},
            {45, "99999999"},
            {47, "99999999"},
            {48, "99999999"},
            {84, "9999999999"},
            {85, "9999999999"},
            {86, "9999999999"},
            {87, "9999999999"},
            {88, "9999999999"},
            {89, "9999999999"},
            {90, "9999999999"},
            {91, "9999999999"},
            {92, "9999999999"},
            {93, "9999999999"},
            {94, "9999999999"},
            {95, "9999999999"},
            {64, "99999999"},
            {130, "99999999"},
            {110, "99999"},
            {111, "99999999"},
            {112, "99999999"},
            {113, "99999999"}
        };

        // Ler códigos do arquivo CSV anexado
        var codigosPlanilha = new HashSet<string>();
        var linhasCodigos = File.ReadAllLines(caminhoCodigos);
        for (int i = 1; i < linhasCodigos.Length; i++) // Ignora cabeçalho
        {
            var colunas = linhasCodigos[i].Split(';');
            if (colunas.Length > 0)
            {
                var valor = colunas[0].Trim();
                if (!string.IsNullOrEmpty(valor))
                {
                    codigosPlanilha.Add(valor);
                }
            }
        }

        // Processar arquivo de entrada
        var linhasSaida = new List<string>();
        var linhasEntrada = File.ReadAllLines(caminhoCsvEntrada);
        linhasSaida.Add(linhasEntrada[0]); // Cabeçalho

        for (int i = 1; i < linhasEntrada.Length; i++)
        {
            var colunas = linhasEntrada[i].Split(';');
            string estrutura = colunas[26].Trim();
            string estruturaNumerica = colunas[28].Trim();
            int mascaratge = int.Parse(colunas[60]);

            // Se estruturaNumerica está na planilha, força ID 130
            if (codigosPlanilha.Contains(estruturaNumerica))
            {
                mascaratge = 130;
            }

            // Aplica máscara
            if (mascaras.ContainsKey(mascaratge))
            {
                estrutura = AplicarMascara(estrutura, mascaras[mascaratge]);
            }

            colunas[26] = estrutura;
            colunas[60] = mascaratge.ToString();
            linhasSaida.Add(string.Join(";", colunas));
        }

        File.WriteAllLines(caminhoCsvSaida, linhasSaida);
        Console.WriteLine($"Conversão concluída. Arquivo gerado em: {caminhoCsvSaida}");
    }

    static string AplicarMascara(string valor, string mascara)
    {
        valor = new string(valor.Where(char.IsDigit).ToArray());
        int index = 0;
        var resultado = "";
        foreach (char c in mascara)
        {
            if (c == '9')
            {
                if (index < valor.Length)
                {
                    resultado += valor[index];
                    index++;
                }
            }
            else
            {
                resultado += c;
            }
        }
        return resultado;
    }
}

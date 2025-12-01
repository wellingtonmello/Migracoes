
using System;
using System.Text;

class Campo
{
    public string Nome { get; set; }
    public int Inicio { get; set; } // Posição inicial (0-based)
    public int Fim { get; set; }    // Posição final (inclusive)

    public string Extrair(string linha)
    {
        if (linha.Length < Fim + 1) return "";
        return linha.Substring(Inicio - 1, Fim - Inicio + 1).Trim();
    }
}

class Program
{
    static void Main()
    {
        const string tipoImportacao = "CaixasComunitarias";

        Console.Write("Informe o caminho completo do Arquivo de Localidades.csv: ");
        string caminhoTxt = Console.ReadLine().Trim();

        Console.Write("Informe o caminho completo de Saida.csv: ");
        string caminhoCsv = Console.ReadLine().Trim();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // register provider
        Encoding encoding = Encoding.GetEncoding(1252);


        var camposGu = new List<Campo>
        {
            new Campo { Nome = "ChaveLocalidade", Inicio = 12, Fim = 19 },
            new Campo { Nome = "Nome", Inicio = 20, Fim = 91 },
            new Campo { Nome = "CEP", Inicio = 92, Fim = 99 },
            new Campo { Nome = "ChaveSubordinacao", Inicio = 144, Fim = 151 },
            new Campo { Nome = "CodIbge", Inicio = 155, Fim = 161 }
            // Adicione mais campos conforme necessário
        };

        var camposCaixasComunitarias = new List<Campo>
        {
            new Campo { Nome = "ChaveLocalidade", Inicio = 10, Fim = 17 },
            new Campo { Nome = "Bairro", Inicio = 98, Fim = 169 },
            new Campo { Nome = "CEP", Inicio = 90, Fim = 97 },
            new Campo { Nome = "Logradouro", Inicio = 178, Fim = 249 }
            // Adicione mais campos conforme necessário
        };

        var campos = tipoImportacao == "Gu" ? camposGu : camposCaixasComunitarias;

        var linhasTxt = File.ReadAllLines(caminhoTxt, encoding);
        var linhasCsv = new List<string>();

        // Cabeçalho
        linhasCsv.Add(string.Join(";", campos.ConvertAll(c => c.Nome)));

        // Dados
        foreach (var linha in linhasTxt)
        {
            var valores = campos.ConvertAll(c => c.Extrair(linha));
            linhasCsv.Add(string.Join(";", valores));
        }

        File.WriteAllLines(caminhoCsv, linhasCsv, encoding);
        Console.WriteLine("Arquivo CSV gerado com sucesso!");
    }
}
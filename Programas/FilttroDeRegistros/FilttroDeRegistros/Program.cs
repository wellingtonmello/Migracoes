
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.Write("Informe o caminho completo do ARQUIVO_BD.csv: ");
        string bdPath = Console.ReadLine().Trim();

        Console.Write("Informe o caminho completo do ARQUIVO_ORIGEM.csv: ");
        string origemPath = Console.ReadLine().Trim();

        string outputPath = Path.Combine(Path.GetDirectoryName(origemPath), "ARQUIVO_FILTRADO.csv");

        Encoding ansiEncoding = Encoding.GetEncoding(1252); // Windows-1252 (ANSI)

        // Lê cabeçalhos
        string[] bdHeader;
        string[] origemHeader;
        using (var reader = new StreamReader(bdPath, ansiEncoding))
        {
            bdHeader = reader.ReadLine().Split(';');
        }
        using (var reader = new StreamReader(origemPath, ansiEncoding))
        {
            origemHeader = reader.ReadLine().Split(';');
        }

        Console.WriteLine("\nColunas disponíveis no ARQUIVO_ORIGEM:");
        Console.WriteLine(string.Join(", ", origemHeader));
        Console.Write("Informe as colunas de chave do ARQUIVO_ORIGEM (na mesma ordem das colunas do ARQUIVO_BD, separadas por vírgula): ");
        var origemKeys = Console.ReadLine().Split(',').Select(k => k.Trim()).ToArray();
        Console.WriteLine("\nDeseja filtrar os importados (I)?  ou os não Importados (N)?");
        var filtroTipo = Console.ReadLine().Trim().ToUpper();

        var bdIndices = Enumerable.Range(0, bdHeader.Length).ToArray();
        var origemIndices = origemKeys.Select(k => Array.IndexOf(origemHeader, k)).ToArray();

        if (origemIndices.Any(i => i == -1))
        {
            Console.WriteLine("\n❌ Erro: Uma ou mais colunas informadas não foram encontradas no ARQUIVO_ORIGEM.");
            return;
        }

        var bdChaves = new HashSet<string>();
        using (var reader = new StreamReader(bdPath, ansiEncoding))
        {
            reader.ReadLine(); // pula cabeçalho
            while (!reader.EndOfStream)
            {
                var partes = reader.ReadLine().Split(';');
                var chave = string.Join("|", bdIndices.Select(i => partes[i].Trim()));
                bdChaves.Add(chave);
            }
        }

        var linhasFiltradas = new List<string>();
        using (var reader = new StreamReader(origemPath, ansiEncoding))
        {
            string linhaCabecalho = reader.ReadLine();
            linhasFiltradas.Add(linhaCabecalho);

            while (!reader.EndOfStream)
            {
                var linha = reader.ReadLine();
                var partes = linha.Split(';');
                var chave = string.Join("|", origemIndices.Select(i => partes[i].Trim()));
                if (filtroTipo == "I" && bdChaves.Contains(chave))
                {
                    linhasFiltradas.Add(linha);
                }
                else if (filtroTipo == "N" && !bdChaves.Contains(chave))
                {
                    linhasFiltradas.Add(linha);
                }
                else if (filtroTipo != "I" && filtroTipo != "N" && filtroTipo != "")
                {
                    Console.WriteLine("\n❌ Erro: Tipo de filtro inválido. Use 'I' para importados ou 'N' para não importados.");
                    return;
                }
                else
                if (!bdChaves.Contains(chave))
                {
                    linhasFiltradas.Add(linha);
                }
            }
        }

        using (var writer = new StreamWriter(outputPath, false, ansiEncoding))
        {
            foreach (var linha in linhasFiltradas)
            {
                writer.WriteLine(linha);
            }
        }

        Console.WriteLine($"\n✅ Arquivo filtrado salvo como: {outputPath}");
        Console.WriteLine("\nPressione qualquer tecla para sair...");
        Console.ReadKey();
    }
}

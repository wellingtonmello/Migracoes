
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        int progressCounter = 0;
        Console.Write("Informe o caminho completo do ARQUIVO_BD.csv: ");
        string bdPath = Console.ReadLine().Trim();

        Console.Write("Informe o caminho completo do ARQUIVO_ORIGEM.csv: ");
        string origemPath = Console.ReadLine().Trim();

        string outputPath = Path.Combine(Path.GetDirectoryName(origemPath), "ARQUIVO_FILTRADO.csv");

        // Lê cabeçalhos
        var bdHeader = File.ReadLines(bdPath).First().Split(';');
        var origemHeader = File.ReadLines(origemPath).First().Split(';');

        Console.WriteLine("\nColunas disponíveis no ARQUIVO_ORIGEM:");
        Console.WriteLine(string.Join(", ", origemHeader));
        Console.Write("Informe as colunas de chave do ARQUIVO_ORIGEM (na mesma ordem das colunas do ARQUIVO_BD, separadas por vírgula): ");
        var origemKeys = Console.ReadLine().Split(',').Select(k => k.Trim()).ToArray();

        // Índices das colunas
        var bdIndices = Enumerable.Range(0, bdHeader.Length).ToArray(); // todas as colunas do BD são chave
        var origemIndices = origemKeys.Select(k => Array.IndexOf(origemHeader, k)).ToArray();

        var bdChaves = new HashSet<string>();

        // Lê e monta as chaves do ARQUIVO_BD
        foreach (var linha in File.ReadLines(bdPath).Skip(1))
        {
            var partes = linha.Split(';');
            var chave = string.Join("|", bdIndices.Select(i => partes[i].Trim()));
            bdChaves.Add(chave);
        }

        var linhasOrigem = File.ReadAllLines(origemPath);
        var linhasFiltradas = new List<string> { linhasOrigem[0] };

        // Filtra as linhas do ARQUIVO_ORIGEM
        foreach (var linha in linhasOrigem.Skip(1))
        {
            Console.WriteLine($"\n Total de linhas verificadas: {progressCounter}");
            var partes = linha.Split(';');
            var chave = string.Join("|", origemIndices.Select(i => partes[i].Trim()));
            if (!bdChaves.Contains(chave))
            {
                linhasFiltradas.Add(linha);
            }
            progressCounter++;
        }

        // Salva o resultado
        File.WriteAllLines(outputPath, linhasFiltradas);
        Console.WriteLine($"\n✅ Arquivo filtrado salvo como: {outputPath}");

        Console.WriteLine("\nPressione qualquer tecla para sair...");
        Console.ReadKey();
    }
}


using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        string caminhoOrigem = @"C:\Users\wellington.mello\Downloads\tge-adjusted191120251627.csv";
        string caminhoDestino = @"C:\Users\wellington.mello\Downloads\arquivo_utf8.txt";

        // Habilita suporte a encodings não padrão (como 1252)
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // Detecta e usa a codificação ANSI (geralmente Windows-1252)
        Encoding ansiEncoding = Encoding.GetEncoding(1252); // Windows-1252 para PT-BR

        // Lê todo o conteúdo do arquivo original
        string conteudo = File.ReadAllText(caminhoOrigem, ansiEncoding);

        // Grava o conteúdo em UTF-8 (sem BOM)
        File.WriteAllText(caminhoDestino, conteudo, new UTF8Encoding(false));

        Console.WriteLine("Conversão concluída com sucesso!");
    }
}

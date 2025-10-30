using System;
using System.IO;
using System.Globalization;
using System.Text;

static class Program
{
    static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var folder = "C:\\Users\\wellington.mello\\Downloads\\20251029_beneficiario_emp_1_7_11_12_13_14";

        var txtFiles = Directory.GetFiles(folder, "*.txt");
        int filesProcessed = 0;
        long totalType01 = 0;
        long totalAdesaoMaiorQueCancelamento = 0;
        long totalInvalidDates = 0;

        foreach (var file in txtFiles)
        {
            filesProcessed++;
            long fileType01 = 0;
            long fileAdesaoMaior = 0;
            long fileInvalidDates = 0;

            try
            {
                foreach (var line in File.ReadLines(file, Encoding.Default))
                {
                    if (string.IsNullOrEmpty(line) || line.Length < 168) // precisa ter pelo menos 168 caracteres (posição 168 inclusive)
                        continue;

                    if (line.Length >= 2 && line.Substring(0, 2) == "01")
                    {
                        fileType01++;
                        totalType01++;

                        // Posições 1-based informadas pelo usuário:
                        // - Data de cancelamento: posições 34-41 => índice 33, comprimento 8
                        // - Data de adesão: posições 161-168 => índice 160, comprimento 8
                        var cancelStr = line.Substring(33, 8);
                        var adesaoStr = line.Substring(160, 8);

                        if (DateTime.TryParseExact(adesaoStr, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var adesaoDate)
                            && DateTime.TryParseExact(cancelStr, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var cancelDate))
                        {
                            if (adesaoDate > cancelDate)
                            {
                                fileAdesaoMaior++;
                                totalAdesaoMaiorQueCancelamento++;
                            }
                        }
                        else
                        {
                            if cancel
                            fileInvalidDates++;
                            totalInvalidDates++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler arquivo {file}: {ex.Message}");
                continue;
            }

            Console.WriteLine($"Arquivo: {Path.GetFileName(file)} | Registros '01': {fileType01} | Adesão > Cancelamento: {fileAdesaoMaior} | Datas inválidas: {fileInvalidDates}");
        }

        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"Pasta: {folder}");
        Console.WriteLine($"Arquivos processados: {filesProcessed}");
        Console.WriteLine($"Total registros '01': {totalType01}");
        Console.WriteLine($"Total Adesão > Cancelamento: {totalAdesaoMaiorQueCancelamento}");
        Console.WriteLine($"Total datas inválidas: {totalInvalidDates}");
        return 0;
    }

    static string AskFolder()
    {
        Console.Write("Informe a pasta (ou Enter para usar o diretório atual): ");
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? Directory.GetCurrentDirectory() : input.Trim();
    }
}
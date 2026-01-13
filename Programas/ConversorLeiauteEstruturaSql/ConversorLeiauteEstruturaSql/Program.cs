
// Arquivo: Program.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO; // TextFieldParser

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.Write("Informe o caminho do arquivo CSV de leiaute: ");
        var csvPath = Console.ReadLine()?.Trim('"').Trim();
        if (string.IsNullOrWhiteSpace(csvPath) || !File.Exists(csvPath))
        {
            Console.WriteLine("CSV não encontrado.");
            return;
        }

        Console.Write("Informe o nome da tabela: ");
        var tableNameInput = Console.ReadLine();

        var tableName = SanitizeIdentifier(tableNameInput);
        var columns = ReadColumnsFromLayout(csvPath);

        if (columns.Count == 0)
        {
            Console.WriteLine("Nenhuma coluna válida encontrada (CAMPO vazio?).");
            return;
        }

        var sql = BuildCreateTableSql(tableName, columns);

        // Salva e imprime
        var outFile = Path.Combine(Path.GetDirectoryName(csvPath) ?? ".", $"create_table_{tableName}.sql");
        File.WriteAllText(outFile, sql, new UTF8Encoding(false));
        Console.WriteLine();
        Console.WriteLine(sql);
        Console.WriteLine();
        Console.WriteLine($"Arquivo gerado: {outFile}");
    }

    // Lê CSV com ';' e cabeçalho, mapeando colunas de interesse
    static List<(string ColumnName, string OracleType)> ReadColumnsFromLayout(string csvPath)
    {
        var result = new List<(string, string)>();

        using var parser = new TextFieldParser(csvPath,Encoding.UTF8)
        {
            TextFieldType = FieldType.Delimited
        };
        parser.SetDelimiters(";");

        // Cabeçalho
        if (parser.EndOfData) return result;
        var header = parser.ReadFields()?.Select(h => Normalize(h)).ToArray() ?? Array.Empty<string>();

        int idxCampo = Array.IndexOf(header, "CAMPO");
        int idxClasseDesc = Array.IndexOf(header, "CLASSE_DESCRICAO");
        int idxLargura = Array.IndexOf(header, "LARGURA");

        if (idxCampo < 0 || idxClasseDesc < 0) return result;

        while (!parser.EndOfData)
        {
            var fields = parser.ReadFields() ?? Array.Empty<string>();
            string campo = Get(fields, idxCampo);
            string classeDesc = Get(fields, idxClasseDesc);
            string largura = idxLargura >= 0 ? Get(fields, idxLargura) : null;

            if (string.IsNullOrWhiteSpace(campo)) continue;

            var colName = SanitizeIdentifier(campo);
            var oracleType = MapOracleType(classeDesc, largura);
            result.Add((colName, oracleType));
        }

        return result;
    }

    static string Get(string[] fields, int index)
        => (index >= 0 && index < fields.Length) ? fields[index] : null;

    // Remove acentos, mantém A-Z, 0-9, _ ; garante primeira letra; uppercase; limita 30 chars
    static string SanitizeIdentifier(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "COLUNA";
        var nfkd = name.Normalize(NormalizationForm.FormD);
        var noAccents = new string(nfkd.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray());
        var replaced = Regex.Replace(noAccents, @"[^A-Za-z0-9_]", "_");
        if (!Regex.IsMatch(replaced, @"^[A-Za-z]")) replaced = "C_" + replaced;
        replaced = replaced.ToUpperInvariant();
        return (replaced.Length <= 30) ? replaced : replaced.Substring(0, 30);
    }

    static string Normalize(string s) => (s ?? "").Trim().ToUpperInvariant();

    // De/para conforme especificação + ajustes de codificação (Numero/Número/N£mero)
    static string MapOracleType(string classeDescricao, string largura)
    {
        var d = (classeDescricao ?? "").Trim().ToLowerInvariant();

        // Normaliza variantes de 'número'
        if (d.Contains("n£mero") || d.Contains("número")) d = "numero";

        if (d == "data") return "DATE";
        if (d == "tabela") return "NUMBER(10)";
        if (d == "numero") return "NUMBER(15,4)"; // 4 casas decimais
        if (d == "string")
        {
            int len = 255;
            if (int.TryParse((largura ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) && n > 0)
                len = n;
            return $"VARCHAR2({len})";
        }
        if (d == "inteiro") return "NUMBER(10)";

        // Fallback
        return "VARCHAR2(255)";
    }

    static string BuildCreateTableSql(string tableName, List<(string ColumnName, string OracleType)> cols)
    {
        var sanitizedTable = SanitizeIdentifier(tableName);
        var lines = cols.Select(c => $"    {c.ColumnName} {c.OracleType}");
        return $"CREATE TABLE TMP_MIG_{sanitizedTable} (\n{string.Join(",\n", lines)}\n);";
    }
}
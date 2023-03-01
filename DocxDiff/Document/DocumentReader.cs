
// (c) 2023 Kazuki KOHZUKI

namespace DocxDiff.Document;

internal static class DocumentReader
{
    internal static string? ReadDocxFile(string path)
    {
        using var reader = new DocxReader(path);
        return reader.ReadWordDocument();
    } // internal static string? ReadDocxFile (string)

    internal static string ReadTextFile(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var encoding = bytes.GetEncoding();
        if (encoding == null) throw new IOException();
        return encoding.GetString(bytes);
    } // internal static string? ReadTextFile (string)
} // internal static class DocumentReader

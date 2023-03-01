
// (c) 2023 Kazuki KOHZUKI

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System.Text;

namespace DocxDiff.Document;

internal class DocxReader : IDisposable
{
    private bool disposed = false;
    private readonly WordprocessingDocument package;

    internal DocxReader(string filename)
    {
        package = WordprocessingDocument.Open(filename, true);
    } // ctor (string)

    internal string? ReadWordDocument()
    {
        var element = package?.MainDocumentPart.Document.Body;
        return element != null ? GetText(element) : null;
    } // internal string ReadWordDocument ()

    protected virtual string GetText(OpenXmlElement element)
    {
        var text = new StringBuilder();
        foreach (var section in element.Elements())
        {
            switch (section.LocalName)
            {
                case "t":                           // Text
                    text.Append(section.InnerText);
                    break;

                case "cr":                          // Carriage return
                case "br":                          // Page break
                    text.Append(Environment.NewLine);
                    break;

                case "tab":                         // Tab
                    text.Append('\t');
                    break;

                case "p":                           // Paragraph
                    text.Append(GetText(section));
                    text.AppendLine(Environment.NewLine);
                    break;

                default:
                    text.Append(GetText(section));
                    break;
            }
        }

        return text.ToString();
    } // protected virtual string GetText(OpenXmlElement element)

    #region IDisposable interface

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    } // public void Dispose ()

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing) package?.Dispose();

        disposed = true;
    } // protected virtual void Dispose (bool)

    #endregion IDisposable interface
} // internal class DocxReader : IDisposable

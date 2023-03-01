
// (c) 2023 Kazuki KOHZUKI

namespace DocxDiff.Diff;

internal sealed class DiffElement<T>
{
    required internal T Value { get; init; }
    required internal DiffType DiffType { get; set; }

    internal string Text => this.Value?.ToString() ?? string.Empty;

    internal DiffElement() { }

    override public string ToString()
        => this.DiffType switch
        {
            DiffType.Deletion => "-",
            DiffType.Addition => "+",
            _ => "",
        } + this.Text;

    public void Deconstruct(out T value, out DiffType type)
    {
        value = this.Value;
        type = this.DiffType;
    } // public void Deconstruct (out T, out DiffType)
} // internal sealed class DiffElement<T>

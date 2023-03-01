
// (c) 2023 Kazuki KOHZUKI

namespace DocxDiff.Diff;

internal sealed class DiffItem
{
    required internal int Start1 { get; init; }
    required internal int Start2 { get; init; }

    required internal int Deletion1 { get; init; }
    required internal int Addition2 { get; init; }

    internal DiffItem() { }

    private static bool ContainsPosition(int position, int start, int length)
    {
        if (length == 0) return false;
        if (position < start) return false;
        if (position >= start + length) return false;
        return true;
    } // private static bool ContainsPosition (int, int, int)

    internal bool IsDeleted(int position)
        => ContainsPosition(position, this.Start1, this.Deletion1);

    internal bool IsAdded(int position)
        => ContainsPosition(position, this.Start2, this.Addition2);
} // internal sealed class DiffItem


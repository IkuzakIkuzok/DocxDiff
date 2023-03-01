
// (c) 2023 Kazuki KOHZUKI

namespace DocxDiff.Diff;

internal sealed class DiffData<T>
{
    private readonly T[] data;

    internal int Length => this.data.Length;
    internal bool[] Modified { get; }

    internal T this[int index] => this.data[index];

    internal DiffData(T[] data)
    {
        this.data = data;
        this.Modified = new bool[this.Length + 2];
    } // ctor(T[] data)

    internal DiffData(IEnumerable<T> data) : this(data.ToArray()) { }
} // internal sealed class DiffData<T>
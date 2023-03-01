
// (c) 2023 Kazuki KOHZUKI

using System.Text;

namespace DocxDiff.Diff;

internal sealed class DiffInfo<T> where T : IEquatable<T>
{
    internal T[] Value1 { get; }
    internal T[] Value2 { get; }
    internal List<DiffItem> Diffs { get; }

    internal DiffInfo(T[] value1, T[] value2)
    {
        this.Value1 = value1;
        this.Value2 = value2;
        this.Diffs = CalcDiff(value1, value2);
    } // ctor (T[], T[])

    internal IEnumerable<DiffElement<T>> GetDiffs()
    {
        var rets = this.Value1.Select(v => new DiffElement<T>() { Value = v, DiffType = DiffType.None }).ToList();

        foreach (var diff in this.Diffs.Reverse<DiffItem>())
        {
            if (diff.Deletion1 > 0)
                rets.Skip(diff.Start1).Take(diff.Deletion1).ToList().ForEach(v => v.DiffType = DiffType.Deletion);
            if (diff.Addition2 > 0)
                rets.InsertRange(diff.Start1, this.Value2.Skip(diff.Start2).Take(diff.Addition2).Select(v => new DiffElement<T>() { Value = v, DiffType = DiffType.Addition }));
        }

        return rets;
    } // internal IEnumerable<DiffElement<T>> GetDiffs ()

    internal static IEnumerable<DiffElement<string>> OptimizeElements(IEnumerable<DiffElement<T>> elements)
    {
        if (!elements.Any()) yield break;

        var type = elements.First().DiffType;
        var buffer = new StringBuilder();
        foreach (var element in elements)
        {
            if (type == element.DiffType)
            {
                buffer.Append(element.Text);
            }
            else
            {
                yield return new() { Value = buffer.ToString(), DiffType = type };
                buffer.Clear();
                buffer.Append(element.Text);
                type = element.DiffType;
            }
        }

        if (buffer.Length > 0)
            yield return new() { Value = buffer.ToString(), DiffType = type };
    } // private static IEnumerable<DiffElement<T>> OptimizeElements (IEnumerable<DiffElement<T>>)

    #region calc diffs

    private static List<DiffItem> CalcDiff(T[] text1, T[] text2)
    {
        var data1 = new DiffData<T>(text1);
        var data2 = new DiffData<T>(text2);

        var max = data1.Length + data2.Length + 1;
        var dv = new int[(max + 1) << 1];
        var uv = new int[(max + 1) << 1];
        LCS(data1, 0, data1.Length, data2, 0, data2.Length, dv, uv);

        Optimize(data1);
        Optimize(data2);

        return CreateDiffs(data1, data2);
    } // private static void CalcDiff (T[], T[])

    /// <summary>
    /// Longest common-subsequence 
    /// </summary>
    private static void LCS(DiffData<T> data1, int lower1, int upper1, DiffData<T> data2, int lower2, int upper2, int[] down, int[] up)
    {
        while (lower1 < upper1 && lower2 < upper2 && data1[lower1].Equals(data2[lower2]))
        {
            ++lower1;
            ++lower2;
        }

        while (lower1 < upper1 && lower2 < upper2 && data1[upper1 - 1].Equals(data2[upper2 - 1]))
        {
            --upper1;
            --upper2;
        }

        if (lower1 == upper1)
        {
            while (lower2 < upper2) data2.Modified[lower2++] = true;
        }
        else if (lower2 == upper2)
        {
            while (lower1 < upper1) data1.Modified[lower1++] = true;
        }
        else
        {
            var smsrd = SMS(data1, lower1, upper1, data2, lower2, upper2, down, up);
            LCS(data1, lower1, smsrd.X, data2, lower2, smsrd.Y, down, up);
            LCS(data1, smsrd.X, upper1, data2, smsrd.Y, upper2, down, up);
        }
    } // private static void LCS (DiffData<T>, int, int, DiffData<T>, int, int, int[], int[])

    /// <summary>
    /// Shortest middle snake
    /// </summary>
    private static Point SMS(DiffData<T> data1, int lower1, int upper1, DiffData<T> data2, int lower2, int upper2, int[] downVector, int[] upVector)
    {
        var max = data1.Length + data2.Length + 1;

        var downK = lower1 - lower2;
        var upK = upper1 - upper2;

        var delta = (upper1 - lower1) - (upper2 - lower2);
        var oddDelta = (delta & 1) != 0;

        var downOffset = max - downK;
        var upOffset = max - upK;

        var maxD = ((upper1 - lower1 + upper2 - lower2) >> 1) + 1;

        downVector[downOffset + downK + 1] = lower1;
        upVector[upOffset + upK - 1] = upper1;

        for (var D = 0; D <= maxD; D++)
        {
            for (var k = downK - D; k <= downK + D; k += 2)
            {
                int x;
                if (k == downK - D)
                {
                    x = downVector[downOffset + k + 1];  // down
                }
                else
                {
                    x = downVector[downOffset + k - 1] + 1;  // right
                    if ((k < downK + D) && (downVector[downOffset + k + 1] >= x))
                        x = downVector[downOffset + k + 1];  // down
                }
                var y = x - k;

                while ((x < upper1) && (y < upper2) && (data1[x].Equals(data2[y])))
                {
                    ++x;
                    ++y;
                }
                downVector[downOffset + k] = x;

                if (oddDelta && (upK - D < k) && (k < upK + D))
                {
                    if (upVector[upOffset + k] <= downVector[downOffset + k])
                    {
                        return new()
                        {
                            X = downVector[downOffset + k],
                            Y = downVector[downOffset + k] - k,
                        };
                    }
                }
            } // for (k)

            for (var k = upK - D; k <= upK + D; k += 2)
            {
                int x;
                if (k == upK + D)
                {
                    x = upVector[upOffset + k - 1];  // up
                }
                else
                {
                    x = upVector[upOffset + k + 1] - 1;  // left
                    if ((k > upK - D) && (upVector[upOffset + k - 1] < x))
                        x = upVector[upOffset + k - 1];  // up
                }
                var y = x - k;

                while ((x > lower1) && (y > lower2) && (data1[x - 1].Equals(data2[y - 1])))
                {
                    --x;
                    --y;
                }
                upVector[upOffset + k] = x;

                if (!oddDelta && (downK - D <= k) && (k <= downK + D))
                {
                    if (upVector[upOffset + k] <= downVector[downOffset + k])
                    {
                        return new()
                        {
                            X = downVector[downOffset + k],
                            Y = downVector[downOffset + k] - k,
                        };
                    }
                }
            } // for (k)
        } // for (D)

        throw new Exception();
    } // private static Point SMS (DiffData<T>, int, int, DiffData<T>, int, int, int[], int[])

    private static void Optimize(DiffData<T> data)
    {
        var start = 0;
        while (start < data.Length)
        {
            while ((start < data.Length) && !data.Modified[start]) ++start;

            var end = start;
            while (end < data.Length && data.Modified[start]) ++end;

            if ((end < data.Length) && (data[start].Equals(data[end])))
            {
                data.Modified[start] = false;
                data.Modified[end] = true;
            }
            else
            {
                start = end;
            }
        }
    } // private static void Optimize (DiffData<T>)

    private static List<DiffItem> CreateDiffs(DiffData<T> data1, DiffData<T> data2)
    {
        var diffs = new List<DiffItem>();

        var pos1 = 0;
        var pos2 = 0;
        while ((pos1 < data1.Length) || (pos2 < data2.Length))
        {
            if ((pos1 < data1.Length) && (!data1.Modified[pos1]) && (pos2 < data2.Length) && (!data2.Modified[pos2]))
            {
                ++pos1;
                ++pos2;
            }
            else
            {
                var start1 = pos1;
                var start2 = pos2;

                while ((pos1 < data1.Length) && (pos2 >= data2.Length || data1.Modified[pos1])) ++pos1;
                while ((pos2 < data2.Length) && (pos1 >= data1.Length || data2.Modified[pos2])) ++pos2;

                if ((start1 < pos1) || (start2 < pos2))
                {
                    var item = new DiffItem()
                    {
                        Start1 = start1,
                        Start2 = start2,
                        Deletion1 = pos1 - start1,
                        Addition2 = pos2 - start2,
                    };
                    diffs.Add(item);
                }
            }
        }

        return diffs;
    } // private static List<DiffItem> CreateDiffs (DiffData<T>, DiffData<T>)

    #endregion calc diffs
} // internal sealed class DiffInfo<T> where T : IEquatable<T>

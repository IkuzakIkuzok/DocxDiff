
// (c) 2023 Kazuki KOHZUKI

namespace DocxDiff;

internal static class IterUtil
{
    internal static IEnumerable<(TKey key, TValue value)> Items<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> @this)
    {
        foreach (var pair in @this)
            yield return (pair.Key, pair.Value);
    } // internal static IEnumerable<(TKey, TValue)> Items<TKey, TValue> (this IEnumerable<KeyValuePair<TKey, TValue>>)

    internal static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> @this)
    {
        var index = 0;
        foreach (var item in @this)
            yield return (index++, item);
    } // internal static IEnumerable<(int, T)> Enumerate<T> (this IEnumerable<T>)
} // internal static class IterUtil

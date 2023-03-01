
// (c) 2023 Kazuki KOHZUKI

using Microsoft.Win32;

namespace DocxDiff.Divider;

internal static class TextDivider
{
    private const string WordBreakerKey = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\ContentIndex\Language\Japanese_Default";

    private const uint WBREAK_E_END_OF_TEXT = 0x80041780;

    private static readonly Type WordBreakerType;

    static TextDivider()
    {
        var guid = new Guid(Registry.GetValue(WordBreakerKey, @"WBreakerClass", string.Empty) as string ?? "{E1E8F15E-8BEC-45DF-83BF-50FF84D0CAB5}");
        WordBreakerType = Type.GetTypeFromCLSID(guid) ?? throw new Exception("");
    } // cctor ()

    internal static IEnumerable<string> Divide(this string text, DivideType divideType)
         => (divideType switch
         {
             DivideType.Whitespace => text.Split().ToList(),
             DivideType.Character  => text.ToCharArray().Select(c => c.ToString()).ToList(),
             _ => DivideByWord(text)
         }).RestoreSpecials(text).RemoveAddition(text);

    private static IWordBreaker GetWordBreaker()
        => Activator.CreateInstance(WordBreakerType) as IWordBreaker ?? throw new Exception("");

#pragma warning disable IDE1006
    private static uint pfnFillTextBuffer(ref TEXT_SOURCE pTextSource)
#pragma warning restore
        => WBREAK_E_END_OF_TEXT;

    private static List<string> DivideByWord(string text)
    {
        var buffer = new List<string>();

        var iwb = GetWordBreaker();
        iwb.Init(true, 4096, out var _);

        var cws = new CWordSink();
        var cps = new CPhraseSink();
        cws.OnWord += buffer.Add;
        var iws = (IWordSink)cws;
        var ips = (IPhraseSink)cps;

        var pTextSource = new TEXT_SOURCE()
        {
            pfnFillTextBuffer = new delFillTextBuffer(pfnFillTextBuffer),
            awcBuffer = text,
            iCur = 0,
            iEnd = text.Length,
        };
        iwb.BreakText(ref pTextSource, iws, ips);

        return buffer;
    } // private static List<string> DivideByWord (string)

    private static IEnumerable<string> RestoreSpecials(this List<string> words, string original)
    {
        var chars = words.SelectMany(w => w.ToCharArray()).ToList();
        foreach ((var i, var c) in original.Enumerate())
        {
            if (i >= chars.Count)
            {
                words.Add(original[i..]);
                break;
            }
            if (chars[i] == c) continue;

            chars.Insert(i, c);

            var w = 0;
            var l = i;
            while (l > 0) l -= words[w++].Length;
            words.Insert(w, c.ToString());
        }
        

        return words;
    } // private static IEnumerable<string> RestoreSpecials (this List<string>, string)

    private static IEnumerable<string> RemoveAddition(this IEnumerable<string> words, string original)
    {
        var l = 0;
        var len = original.Length;
        foreach (var word in words)
        {
            l += word.Length;
            if (l <= len)
                yield return word;
            else
                yield break;
        }
    } // private static IEnumerable<string> RemoveAddition (this IEnumerable<string>, string)
} // internal static class TextDivider

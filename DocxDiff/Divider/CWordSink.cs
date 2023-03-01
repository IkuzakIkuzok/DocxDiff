
// (c) 2023 Kazuki KOHZUKI

namespace DocxDiff.Divider;

delegate void WordHandler(string word);

public class CWordSink : IWordSink
{
    internal event WordHandler? OnWord;

    public void PutWord(int cwc, string pwcInBuf, int cwcSrcLen, int cwcSrcPos)
    {
        var word = pwcInBuf[..cwc];
        OnWord?.Invoke(word);
    } // public void PutWord (int, string, int, int)

    public void PutAltWord(int cwc, string pwcInBuf, int cwcSrcLen, int cwcSrcPos) { }

    public void StartAltPhrase() { }

    public void EndAltPhrase() { }

    public void PutBreak(WORDREP_BREAK_TYPE breakType) { }
} // public class CWordSink : IWordSink

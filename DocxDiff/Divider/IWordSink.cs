
// (c) 2023 Kazuki KOHZUKI

using System.Runtime.InteropServices;

namespace DocxDiff.Divider;

[ComImport]
[Guid("CC907054-C058-101A-B554-08002B33B0E6")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWordSink
{
    void PutWord(
        [MarshalAs(UnmanagedType.U4)] int cwc,
        [MarshalAs(UnmanagedType.LPWStr)] string pwcInBuf,
        [MarshalAs(UnmanagedType.U4)] int cwcSrcLen,
        [MarshalAs(UnmanagedType.U4)] int cwcSrcPos
    );

    void PutAltWord(
        [MarshalAs(UnmanagedType.U4)] int cwc,
        [MarshalAs(UnmanagedType.LPWStr)] string pwcInBuf,
        [MarshalAs(UnmanagedType.U4)] int cwcSrcLen,
        [MarshalAs(UnmanagedType.U4)] int cwcSrcPos
    );

    void StartAltPhrase();
    void EndAltPhrase();
    void PutBreak(WORDREP_BREAK_TYPE breakType);
} // public interface IWordSink


// (c) 2023 Kazuki KOHZUKI

using System.Runtime.InteropServices;

namespace DocxDiff.Divider;

[ComImport]
[Guid("CC906FF0-C058-101A-B554-08002B33B0E6")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IPhraseSink
{
    void PutSmallPhrase(
        [MarshalAs(UnmanagedType.LPWStr)] string pwcNoun,
        [MarshalAs(UnmanagedType.U4)] int cwcNoun,
        [MarshalAs(UnmanagedType.LPWStr)] string pwcModifier,
        [MarshalAs(UnmanagedType.U4)] int cwcModifier,
        [MarshalAs(UnmanagedType.U4)] int ulAttachmentType
    );

    void PutPhrase(
        [MarshalAs(UnmanagedType.LPWStr)] string pwcPhrase,
        [MarshalAs(UnmanagedType.U4)] int cwcPhrase
    );
} // public interface IPhraseSink

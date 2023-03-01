
// (c) 2023 Kazuki KOHZUKI

using System.Runtime.InteropServices;

namespace DocxDiff.Divider;

[ComImport]
[Guid("D53552C8-77E3-101A-B552-08002B33B0E6")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWordBreaker
{
    void Init(
        [MarshalAs(UnmanagedType.Bool)] bool fQuery,
        [MarshalAs(UnmanagedType.U4)] int maxTokenSize,
        [MarshalAs(UnmanagedType.Bool)] out bool pfLicense
    );

    void BreakText(
        [MarshalAs(UnmanagedType.Struct)] ref TEXT_SOURCE pTextSource,
        [MarshalAs(UnmanagedType.Interface)] IWordSink pWordSink,
        [MarshalAs(UnmanagedType.Interface)] IPhraseSink pPhraseSink
    );

    void GetLicenseToUse([MarshalAs(UnmanagedType.LPWStr)] out string ppwcsLicense);
} // public interface IWordBreaker
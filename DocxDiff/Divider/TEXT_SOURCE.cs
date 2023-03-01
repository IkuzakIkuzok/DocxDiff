
// (c) 2023 Kazuki KOHZUKI

using System.Runtime.InteropServices;

namespace DocxDiff.Divider;

// used to fill the buffer for TEXT_SOURCE
public delegate uint delFillTextBuffer([MarshalAs(UnmanagedType.Struct)] ref TEXT_SOURCE pTextSource);

[StructLayout(LayoutKind.Sequential)]
public struct TEXT_SOURCE
{
    [MarshalAs(UnmanagedType.FunctionPtr)] public delFillTextBuffer pfnFillTextBuffer;
    [MarshalAs(UnmanagedType.LPWStr)] public string awcBuffer;
    [MarshalAs(UnmanagedType.U4)] public int iEnd;
    [MarshalAs(UnmanagedType.U4)] public int iCur;
} // public struct TEXT_SOURCE

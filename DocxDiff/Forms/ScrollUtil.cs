
// (c) 2023 Kazuki KOHZUKI

using DocumentFormat.OpenXml.Spreadsheet;
using System.Runtime.InteropServices;

namespace DocxDiff.Forms;

internal static partial class ScrollUtil
{
    private const int SB_VERT         = 1;
    private const int WM_USER         = 0x0400;
    private const int EM_GETSCROLLPOS = WM_USER + 0x00dd;
    private const int EM_SETSCROLLPOS = WM_USER + 0x00de;

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

    [DllImport("user32.dll", EntryPoint = "SendMessageA")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, ref Point lParam);

    internal static Point GetScrollPosition(this RichTextBox control)
    {
        var p = Point.Empty;
        SendMessage(control.Handle, EM_GETSCROLLPOS, 0, ref p);
        return p;
    } // internal static Point GetScrollPosition (this Control)

    internal static int GetScrollY(this RichTextBox control)
    {
        (var min, var _) = control.GetScrollRange();
        var y = control.GetScrollPosition().Y + control.Height;
        return y - min;
    } // internal static int GetScrollY (this RichTextBox control)

    internal static void SetScrollY(this RichTextBox control, int y)
    {
        (var min, var max) = GetScrollRange(control);
        y += min - control.Height;
        if (y < min) y = min;
        else if (y > max) y = max;

        var p = control.GetScrollPosition();
        p.Y = y;
        SendMessage(control.Handle, EM_SETSCROLLPOS, 0, ref p);
    } // internal static void SetScrollY (this control, int)

    internal static (int min, int max) GetScrollRange(this RichTextBox control)
    {
        GetScrollRange(control.Handle, SB_VERT, out var min, out var max);
        return (min, max);
    } // internal static (int min, int max) GetScrollRange (this RichTextBox)
} // internal static class ScrollUtil

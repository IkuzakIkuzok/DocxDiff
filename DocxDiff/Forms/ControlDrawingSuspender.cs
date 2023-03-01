
// (c) 2023 Kazuki KOHZUKI

using System.Runtime.InteropServices;

namespace DocxDiff.Forms;

internal sealed class ControlDrawingSuspender : SuspenderBase
{
    [DllImport("user32", CharSet = CharSet.Auto)]
    private static extern int SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

    private const int WM_SETREDRAW = 0x000B;

    private readonly Control _control;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlDrawingSuspender"/> class.
    /// </summary>
    /// <param name="control">The control.</param>
    internal ControlDrawingSuspender(Control control)
    {
        this._control = control;
        BeginControlUpdate(control);
    } // ctor (Control)

    override protected void Terminate()
        => EndControlUpdate(this._control);

    /// <summary>
    /// Stops drawing to update the current control.
    /// </summary>
    /// <param name="control">The control.</param>
    internal static void BeginControlUpdate(Control control)
        => SendMessage(new HandleRef(control, control.Handle), WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

    /// <summary>
    /// Restarts drawing.
    /// </summary>
    /// <param name="control">The control.)</param>
    internal static void EndControlUpdate(Control control)
    {
        control.Invoke(() =>
        {
            SendMessage(new HandleRef(control, control.Handle), WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            control.Invalidate();
            control.Refresh();
        });
    } // internal static void EndControlUpdate (Control)
} // internal sealed class ControlDrawingSuspender : SuspenderBase

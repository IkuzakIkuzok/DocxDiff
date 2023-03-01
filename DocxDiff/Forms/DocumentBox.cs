
// (c) 2023 Kazuki KOHZUKI

using DocxDiff.Document;

namespace DocxDiff.Forms;

[DesignerCategory("code")]
internal class DocumentBox : RichTextBox
{
    private const int WM_PAINT = 0x000F;

    private int _scroll = 0;

    internal bool CheckScroll { get; set; } = true;

    internal int Scroll
    {
        get => this._scroll;
        set => this.SetScrollY(this._scroll = value);
    }

    internal event EventHandler? ScrollChanged;

    internal DocumentBox()
    {
        this.ScrollBars = RichTextBoxScrollBars.Both;
        this.AllowDrop = true;
    } // ctor ()

    private void CheckScrollChanged()
    {
        var s = this.GetScrollY();
        if (s != this._scroll)
        {
            this._scroll = s;
            OnScrollChanged(EventArgs.Empty);
        }
    } // private void CheckScrollChanged ()

    override protected void WndProc(ref Message m)
    {
        if (m.Msg == WM_PAINT && this.CheckScroll)
            CheckScrollChanged();

        base.WndProc(ref m);
    } // override protected void WndProc (ref Message)

    /*override protected void OnTextChanged(EventArgs e)
    {
        CheckScrollChanged();
        base.OnTextChanged(e);
    } // override protected void OnTextChanged (EventArgs)

    override protected void OnSizeChanged(EventArgs e)
    {
        CheckScrollChanged();
        base.OnSizeChanged(e);
    } // override protected void OnSizeChanged (EventArgs)*/

    override protected void OnDragEnter(DragEventArgs drgevent)
    {
        if (drgevent.Data?.GetDataPresent(DataFormats.FileDrop) ?? false)
            drgevent.Effect = DragDropEffects.Copy;
        base.OnDragEnter(drgevent);
    } // override protected void OnDragEnter (DragEventArgs)

    override protected void OnDragDrop(DragEventArgs drgevent)
    {
        if (drgevent.Data?.GetData(DataFormats.FileDrop, false) is string[] { Length: > 0 } files)
            OpenFile(files[0]);

        base.OnDragDrop(drgevent);
    } // override protected void OnDragDrop (DragEventArgs)

    internal virtual void OpenFile(string filename)
    {
        var ext = Path.GetExtension(filename).ToUpper();
        try
        {
            this.Text = ext switch
            {
                ".DOCX" => DocumentReader.ReadDocxFile(filename),
                _ => DocumentReader.ReadTextFile(filename),
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    } // internal virtual void OpenFile (string)

    protected virtual void OnScrollChanged(EventArgs e)
        => this.ScrollChanged?.Invoke(this, e);
} // internal class DocumentBox : RichTextBox

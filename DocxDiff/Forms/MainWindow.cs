
// (c) 2023 Kazuki KOHZUKI

using DocxDiff.Diff;
using DocxDiff.Divider;

namespace DocxDiff.Forms;

[DesignerCategory("code")]
internal sealed class MainWindow : Form
{
    private DivideType divideType = DivideType.Word;

    private readonly DocumentBox db_src1, db_src2;

    private readonly Graphics graphics;
    private readonly Font font;
    private readonly float space_width;

    private Color def_fore = Color.Black;
    private Color del_fore = Color.Black;
    private Color add_fore = Color.Black;
    private Color def_back = Color.White;
    private Color del_back = Color.FromArgb(102, 255, 129, 130);
    private Color add_back = Color.FromArgb(0, 171, 242, 188);

    internal MainWindow()
    {
        this.Size = new(600, 400);
        this.Text = "Docx Diff";

        var container = new SplitContainer()
        {
            SplitterWidth = 1,
            Dock = DockStyle.Fill,
            Parent = this,
        };

        this.db_src1 = new()
        {
            Dock = DockStyle.Fill,
            Parent = container.Panel1,
        };

        this.db_src2 = new()
        {
            Dock = DockStyle.Fill,
            Parent = container.Panel2,
        };

        Load += (sender, e) =>
        {
            container.SplitterDistance -= (this.db_src1.Width - this.db_src2.Width) >> 1;
        };

        static void SyncScrollPosition(DocumentBox box1, DocumentBox box2)
            => box1.ScrollChanged += (sender, e) => box2.Scroll = box1.Scroll;

        SyncScrollPosition(this.db_src1, this.db_src2);
        SyncScrollPosition(this.db_src2, this.db_src1);

        #region menu

        this.MainMenuStrip = new()
        {
            Dock = DockStyle.Top,
            Parent = this,
        };

        var file = new ToolStripMenuItem()
        {
            Text = "ファイル (&F)",
        };
        this.MainMenuStrip.Items.Add(file);

        var open1 = new ToolStripMenuItem()
        {
            Text = "基準ファイルを開く (&R)",
            ShortcutKeys = Keys.Control | Keys.Shift | Keys.O,
        };
        open1.Click += OpenFile(this.db_src1);
        file.DropDownItems.Add(open1);

        var open2 = new ToolStripMenuItem()
        {
            Text = "比較対象ファイルを開く (&O)",
            ShortcutKeys = Keys.Control | Keys.O,
        };
        open2.Click += OpenFile(this.db_src2);
        file.DropDownItems.Add(open2);

        file.DropDownItems.Add(new ToolStripSeparator());

        var exit = new ToolStripMenuItem()
        {
            Text = "終了 (&X)",
            ShortcutKeys = Keys.Alt | Keys.F4,
        };
        exit.Click += (sender, e) => Close();
        file.DropDownItems.Add(exit);

        var diff = new ToolStripMenuItem()
        {
            Text = "比較 (&D)",
        };
        this.MainMenuStrip.Items.Add(diff);

        var unit = new ToolStripMenuItem()
        {
            Text = "比較単位 (&U)",
        };
        diff.DropDownItems.Add(unit);

        #region diff unit menu

        void SelectUnit(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem u) return;
            foreach (var item in unit!.DropDownItems.OfType<ToolStripMenuItem>())
                item.Checked = false;
            u.Checked = true;
            this.divideType = (DivideType)u.Tag;
        }

        foreach ((var i, var u) in Enum.GetValues<DivideType>().Enumerate())
        {
            var text = u.ToString<UITextAttribute>();
            if (i < 10) text += $" (&{i})";
            var item = new ToolStripMenuItem()
            {
                Text = text,
                Checked = i == 0,
                Tag = u,
            };
            item.Click += SelectUnit;
            unit.DropDownItems.Add(item);
        }

        #endregion diff unit menu

        var run = new ToolStripMenuItem()
        {
            Text = "実行 (&R)",
            ShortcutKeys = Keys.Control | Keys.R,
        };
        run.Click += Run;
        diff.DropDownItems.Add(run);

        var view = new ToolStripMenuItem()
        {
            Text = "表示 (&V)",
        };
        this.MainMenuStrip.Items.Add(view);

        var syncScroll = new ToolStripMenuItem()
        {
            Text = "スクロールを同期する (&S)",
            Checked = true,
        };
        syncScroll.Click += (sender, e)
            => this.db_src1.CheckScroll = this.db_src2.CheckScroll = syncScroll.Checked = !syncScroll.Checked;
        view.DropDownItems.Add(syncScroll);

        var fore = new ToolStripMenuItem()
        {
            Text = "文字色 (&T)",
        };
        view.DropDownItems.Add(fore);

        var foreN = new ToolStripMenuItem()
        {
            Text = "通常 (&N)",
        };
        foreN.Click += (sender, e) => this.def_fore = SelectColor(this.def_fore);
        fore.DropDownItems.Add(foreN);

        var foreA = new ToolStripMenuItem()
        {
            Text = "追加 (&A)",
        };
        foreA.Click += (sender, e) => this.add_fore = SelectColor(this.add_fore);
        fore.DropDownItems.Add(foreA);

        var foreD = new ToolStripMenuItem()
        {
            Text = "削除 (&D)",
        };
        foreD.Click += (sender, e) => this.del_fore = SelectColor(this.del_fore);
        fore.DropDownItems.Add(foreD);

        var back = new ToolStripMenuItem()
        {
            Text = "背景色 (&B)",
        };
        view.DropDownItems.Add(back);

        var backN = new ToolStripMenuItem()
        {
            Text = "通常 (&N)",
        };
        backN.Click += (sender, e) => this.def_back = SelectColor(this.def_back);
        back.DropDownItems.Add(backN);

        var backA = new ToolStripMenuItem()
        {
            Text = "追加 (&A)",
        };
        backA.Click += (sender, e) => this.add_back = SelectColor(this.add_back);
        back.DropDownItems.Add(backA);

        var backD = new ToolStripMenuItem()
        {
            Text = "削除 (&D)",
        };
        backD.Click += (sender, e) => this.del_back = SelectColor(this.del_back);
        back.DropDownItems.Add(backD);

        #endregion menu

        this.graphics = CreateGraphics();
        this.font = this.db_src1.Font;
        this.space_width = this.graphics.MeasureString(" ", this.font).Width;
    } // ctor ()

    private static EventHandler OpenFile(DocumentBox box)
        => (sender, e) =>
        {
            using var ofd = new OpenFileDialog()
            {
                Filter = "Word 文書|*.docx|テキストファイル|*.txt|すべてのファイル|*.*",
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            box.OpenFile(ofd.FileName);
        };

    private static Color SelectColor(Color current)
    {
        using var cd = new ColorDialog()
        {
            Color = current,
            AllowFullOpen = true,
        };

        if (cd.ShowDialog() == DialogResult.OK) return cd.Color;
        return current;
    } // private static Color SelectColor (Color)

    private float GetWidth(string s)
        => this.graphics.MeasureString(s, this.font).Width;

    private void Run(object? sender, EventArgs e)
    {
        using var _ = new ControlDrawingSuspender(this);

        var words1 = this.db_src1.Text.Divide(this.divideType).ToArray();
        var words2 = this.db_src2.Text.Divide(this.divideType).ToArray();
        var diffs = DiffInfo<string>.OptimizeElements(new DiffInfo<string>(words1, words2).GetDiffs()).ToList();

        this.db_src1.Text = this.db_src2.Text = string.Empty;

        static void AddText(DocumentBox documentBox, string text, Color foreColor, Color backColor)
        {
            documentBox.SelectionStart = documentBox.TextLength;
            documentBox.SelectionLength = 0;
            documentBox.SelectionColor = foreColor;
            documentBox.SelectionBackColor = backColor;
            documentBox.SelectedText = text;
        } // static void AddText (DocumentBox, string, Color, Color)

        void AddNorm(DocumentBox box, string text)
            => AddText(box, text, this.def_fore, this.def_back);
        void AddAdd(DocumentBox box, string text)
            => AddText(box, text, this.add_fore, this.add_back);
        void AddDel(DocumentBox box, string text)
            => AddText(box, text, this.del_fore, this.del_back);
        void AddPadding(DocumentBox box, float width)
            => AddNorm(box, new string(' ', (int)(width / this.space_width)));

        var modify = false;
        float offset1 = 0;
        float offset2;
        DiffElement<string>? next;
        foreach ((var i, var diff) in diffs.Enumerate())
        {
            var t = diff.Text;

            if (diff.DiffType == DiffType.None)
            {
                AddNorm(this.db_src1, t);
                AddNorm(this.db_src2, t);
                modify = false;
                offset1 = offset2 = 0;
            }
            else if (diff.DiffType == DiffType.Addition)
            {
                if (diffs.Count > i+1 && (next = diffs[i + 1]).DiffType == DiffType.Deletion)
                {
                    modify = true;
                    var a = GetWidth(t);
                    var d = GetWidth(next.Text);
                    if (d > a)
                    {
                        offset1 = 0;
                        offset2 = d - a;
                    }
                    else if (a > d)
                    {
                        offset1 = a - d;
                        offset2 = 0;
                    }
                    else
                    {
                        offset1 = offset2 = 0;
                    }
                }
                else
                {
                    offset1 = 0;
                    offset2 = GetWidth(t);
                }

                AddAdd(this.db_src2, t);
            }
            else
            {
                offset1 = modify ? offset1 : GetWidth(t);
                offset2 = 0;
                modify = false;
                AddDel(this.db_src1, t);
            }

            AddPadding(this.db_src1, offset1);
            AddPadding(this.db_src2, offset2);
        }

        this.db_src1.SelectionLength = this.db_src1.SelectionStart =
        this.db_src2.SelectionLength = this.db_src2.SelectionStart = 0;
    } // private void Run (object?, EventArgs)
} // internal sealed class MainWindow : Form

namespace NgProductManager.Forms;
partial class UpdateNotesForm
{
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing) { if (disposing && components is not null) components.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        Text = "備考更新"; ClientSize = new Size(520, 300); MinimumSize = new Size(420, 240); StartPosition = FormStartPosition.CenterParent;
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 3 }; root.RowStyles.Add(new RowStyle(SizeType.AutoSize)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.Controls.Add(new Label { Text = "備考", AutoSize = true, Margin = new Padding(0, 0, 0, 4) }, 0, 0); textBoxNotes = new TextBox { Multiline = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical }; root.Controls.Add(textBoxNotes, 0, 1);
        var buttons = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, Margin = new Padding(0, 8, 0, 0) }; buttonCancel = new Button { Text = "キャンセル", Width = 100 }; buttonSave = new Button { Text = "保存", Width = 100 }; buttons.Controls.AddRange([buttonCancel, buttonSave]); root.Controls.Add(buttons, 0, 2); Controls.Add(root); buttonSave.Click += buttonSave_Click; buttonCancel.Click += (s, e) => Close();
    }
    private TextBox textBoxNotes = null!; private Button buttonSave = null!, buttonCancel = null!;
}

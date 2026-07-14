namespace NgProductManager.Forms;

partial class UpdateNotesForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.Text = "備考更新";
        this.ClientSize = new System.Drawing.Size(500, 260);
        this.StartPosition = FormStartPosition.CenterParent;

        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 2 };
        textBoxNotes = new TextBox { Multiline = true, Height = 120 };
        panel.Controls.Add(textBoxNotes);

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, Padding = new Padding(0, 10, 0, 0) };
        buttonSave = new Button { Text = "保存", Width = 100 };
        buttonPanel.Controls.Add(buttonSave);
        buttonCancel = new Button { Text = "キャンセル", Width = 100 };
        buttonPanel.Controls.Add(buttonCancel);
        panel.Controls.Add(buttonPanel);

        this.Controls.Add(panel);
        buttonSave.Click += buttonSave_Click;
        buttonCancel.Click += (s, e) => Close();
    }

    private TextBox textBoxNotes = null!;
    private Button buttonSave = null!;
    private Button buttonCancel = null!;
}

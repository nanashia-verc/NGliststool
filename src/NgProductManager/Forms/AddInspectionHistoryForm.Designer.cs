namespace NgProductManager.Forms;

partial class AddInspectionHistoryForm
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
        this.Text = "再検査登録";
        this.ClientSize = new System.Drawing.Size(600, 450);
        this.StartPosition = FormStartPosition.CenterParent;

        var table = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 8 };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

        AddLabel(table, 0, "検査日");
        dateTimePickerInspection = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy/MM/dd" };
        table.Controls.Add(dateTimePickerInspection, 1, 0);

        AddLabel(table, 1, "結果");
        var resultPanel = new FlowLayoutPanel { AutoSize = true };
        radioButtonNg = new RadioButton { Text = "NG", Checked = true, AutoSize = true };
        resultPanel.Controls.Add(radioButtonNg);
        radioButtonOk = new RadioButton { Text = "OK", AutoSize = true };
        resultPanel.Controls.Add(radioButtonOk);
        table.Controls.Add(resultPanel, 1, 1);

        AddLabel(table, 2, "NG理由");
        comboBoxDefectReason = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        table.Controls.Add(comboBoxDefectReason, 1, 2);

        AddLabel(table, 3, "NG詳細");
        textBoxDefectDetails = new TextBox { Multiline = true, Height = 60 };
        table.Controls.Add(textBoxDefectDetails, 1, 3);

        AddLabel(table, 4, "処置");
        comboBoxAction = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        table.Controls.Add(comboBoxAction, 1, 4);

        AddLabel(table, 5, "処置詳細");
        textBoxActionDetails = new TextBox { Multiline = true, Height = 60 };
        table.Controls.Add(textBoxActionDetails, 1, 5);

        AddLabel(table, 6, "担当者");
        textBoxInspectorName = new TextBox();
        table.Controls.Add(textBoxInspectorName, 1, 6);

        AddLabel(table, 7, "案件状態");
        var statusPanel = new FlowLayoutPanel { AutoSize = true };
        radioButtonPending = new RadioButton { Text = "再検査待ち", Checked = true, AutoSize = true };
        statusPanel.Controls.Add(radioButtonPending);
        radioButtonInProgress = new RadioButton { Text = "対応中", AutoSize = true };
        statusPanel.Controls.Add(radioButtonInProgress);
        table.Controls.Add(statusPanel, 1, 7);

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, Padding = new Padding(0, 10, 0, 0) };
        buttonSave = new Button { Text = "登録", Width = 100 };
        buttonPanel.Controls.Add(buttonSave);
        buttonCancel = new Button { Text = "キャンセル", Width = 100 };
        buttonPanel.Controls.Add(buttonCancel);
        this.Controls.Add(buttonPanel);
        this.Controls.Add(table);

        buttonSave.Click += buttonSave_Click;
        buttonCancel.Click += (s, e) => Close();
    }

    private static void AddLabel(TableLayoutPanel table, int row, string text)
    {
        table.Controls.Add(new Label { Text = text, AutoSize = true, Margin = new Padding(0, 5, 0, 0) }, 0, row);
    }

    private DateTimePicker dateTimePickerInspection = null!;
    private RadioButton radioButtonNg = null!;
    private RadioButton radioButtonOk = null!;
    private ComboBox comboBoxDefectReason = null!;
    private TextBox textBoxDefectDetails = null!;
    private ComboBox comboBoxAction = null!;
    private TextBox textBoxActionDetails = null!;
    private TextBox textBoxInspectorName = null!;
    private RadioButton radioButtonPending = null!;
    private RadioButton radioButtonInProgress = null!;
    private Button buttonSave = null!;
    private Button buttonCancel = null!;
}

namespace NgProductManager.Forms;

partial class NewNgCaseForm
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
        this.Text = "新規NG登録";
        this.ClientSize = new System.Drawing.Size(600, 520);
        this.StartPosition = FormStartPosition.CenterParent;

        var table = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 11 };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));

        AddLabel(table, 0, "ロット番号*");
        textBoxLotNumber = new TextBox();
        table.Controls.Add(textBoxLotNumber, 1, 0);

        AddLabel(table, 1, "型番*");
        comboBoxModel = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        table.Controls.Add(comboBoxModel, 1, 1);

        AddLabel(table, 2, "管理番号");
        textBoxSerialNumber = new TextBox();
        table.Controls.Add(textBoxSerialNumber, 1, 2);

        AddLabel(table, 3, "NG日時*");
        dateTimePickerInspection = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy/MM/dd HH:mm" };
        table.Controls.Add(dateTimePickerInspection, 1, 3);

        AddLabel(table, 4, "結果");
        var resultPanel = new FlowLayoutPanel { AutoSize = true };
        radioButtonNg = new RadioButton { Text = "NG", Checked = true, AutoSize = true };
        resultPanel.Controls.Add(radioButtonNg);
        radioButtonOk = new RadioButton { Text = "OK", AutoSize = true };
        resultPanel.Controls.Add(radioButtonOk);
        table.Controls.Add(resultPanel, 1, 4);

        AddLabel(table, 5, "NG理由*");
        comboBoxDefectReason = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        table.Controls.Add(comboBoxDefectReason, 1, 5);

        AddLabel(table, 6, "NG詳細");
        textBoxDefectDetail = new TextBox { Multiline = true, Height = 60 };
        table.Controls.Add(textBoxDefectDetail, 1, 6);

        AddLabel(table, 7, "処置");
        comboBoxAction = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        table.Controls.Add(comboBoxAction, 1, 7);

        AddLabel(table, 8, "処置詳細");
        textBoxActionDetail = new TextBox { Multiline = true, Height = 60 };
        table.Controls.Add(textBoxActionDetail, 1, 8);

        AddLabel(table, 9, "担当者");
        textBoxInspectorName = new TextBox();
        table.Controls.Add(textBoxInspectorName, 1, 9);

        AddLabel(table, 10, "案件備考");
        textBoxNotes = new TextBox { Multiline = true, Height = 60 };
        table.Controls.Add(textBoxNotes, 1, 10);

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

    private TextBox textBoxLotNumber = null!;
    private ComboBox comboBoxModel = null!;
    private TextBox textBoxSerialNumber = null!;
    private DateTimePicker dateTimePickerInspection = null!;
    private RadioButton radioButtonNg = null!;
    private RadioButton radioButtonOk = null!;
    private ComboBox comboBoxDefectReason = null!;
    private TextBox textBoxDefectDetail = null!;
    private ComboBox comboBoxAction = null!;
    private TextBox textBoxActionDetail = null!;
    private TextBox textBoxInspectorName = null!;
    private TextBox textBoxNotes = null!;
    private Button buttonSave = null!;
    private Button buttonCancel = null!;
}

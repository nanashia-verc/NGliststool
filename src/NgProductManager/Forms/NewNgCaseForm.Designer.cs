namespace NgProductManager.Forms;

partial class NewNgCaseForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        Text = "新規NG登録";
        ClientSize = new Size(620, 560);
        MinimumSize = new Size(560, 500);
        StartPosition = FormStartPosition.CenterParent;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 2, RowCount = 14 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var row = 0; row < 14; row++) root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        comboBoxLotNumber = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown, Dock = DockStyle.Top };
        AddControl(root, 0, "ロット番号*", comboBoxLotNumber);
        comboBoxModel = AddComboBox(root, 1, "型番*");
        comboBoxModel.DropDownStyle = ComboBoxStyle.DropDown;
        comboBoxProcess = AddComboBox(root, 2, "工程*"); comboBoxProcess.DropDownStyle = ComboBoxStyle.DropDown;
        dateTimePickerInspection = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy/MM/dd", Dock = DockStyle.Left, Width = 150 };
        AddControl(root, 3, "NG日*", dateTimePickerInspection);

        var resultPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 3, 0, 3) };
        radioButtonNg = new RadioButton { Text = "NG", Checked = true, AutoSize = true, Margin = new Padding(0, 3, 14, 3) };
        radioButtonOk = new RadioButton { Text = "OK", AutoSize = true, Margin = new Padding(0, 3, 0, 3) };
        resultPanel.Controls.AddRange([radioButtonNg, radioButtonOk]);
        AddControl(root, 4, "結果*", resultPanel);

        comboBoxDefectReason = AddComboBox(root, 5, "NG理由*");
        comboBoxDefectReason.DropDownStyle = ComboBoxStyle.DropDown;
        textBoxDefectDetail = AddMultilineTextBox(root, 6, "NG詳細", 58);
        comboBoxAction = AddComboBox(root, 7, "初回処置（任意）");
        comboBoxAction.DropDownStyle = ComboBoxStyle.DropDown;
        textBoxActionDetail = AddMultilineTextBox(root, 8, "処置詳細（任意）", 58);
        textBoxInspectorName = AddTextBox(root, 9, "担当者");
        textBoxNotes = AddMultilineTextBox(root, 10, "案件備考", 58);

        buttonAddAttachment = new Button { Text = "画像添付", Width = 100 }; labelAttachmentCount = new Label { Text = "添付画像: 0件", AutoSize = true, Margin = new Padding(8, 6, 0, 0) }; var attachmentPanel = new FlowLayoutPanel { AutoSize = true }; attachmentPanel.Controls.AddRange([buttonAddAttachment, labelAttachmentCount]); root.SetColumnSpan(attachmentPanel, 2); root.Controls.Add(attachmentPanel, 0, 11);
        var help = new Label { Text = "* は必須項目です。型番・NG理由・処置内容はマスター管理から登録できます。", AutoSize = true, ForeColor = SystemColors.GrayText, Margin = new Padding(0, 6, 0, 3) };
        root.SetColumnSpan(help, 2);
        root.Controls.Add(help, 0, 12);

        var buttons = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, Margin = new Padding(0, 8, 0, 0) };
        buttonCancel = new Button { Text = "キャンセル", Width = 100 };
        buttonSave = new Button { Text = "登録", Width = 100 };
        buttons.Controls.AddRange([buttonCancel, buttonSave]);
        root.SetColumnSpan(buttons, 2);
        root.Controls.Add(buttons, 0, 13);
        Controls.Add(root);

        buttonSave.Click += buttonSave_Click;
        buttonAddAttachment.Click += buttonAddAttachment_Click;
        buttonCancel.Click += (s, e) => Close();
    }

    private static TextBox AddTextBox(TableLayoutPanel table, int row, string label)
    {
        var textBox = new TextBox { Dock = DockStyle.Top };
        AddControl(table, row, label, textBox);
        return textBox;
    }

    private static ComboBox AddComboBox(TableLayoutPanel table, int row, string label)
    {
        var comboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Top };
        AddControl(table, row, label, comboBox);
        return comboBox;
    }

    private static TextBox AddMultilineTextBox(TableLayoutPanel table, int row, string label, int height)
    {
        var textBox = new TextBox { Multiline = true, Height = height, Dock = DockStyle.Top, ScrollBars = ScrollBars.Vertical };
        AddControl(table, row, label, textBox);
        return textBox;
    }

    private static void AddControl(TableLayoutPanel table, int row, string label, Control control)
    {
        table.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 8, 3) }, 0, row);
        control.Margin = new Padding(0, 3, 0, 3);
        table.Controls.Add(control, 1, row);
    }

    private ComboBox comboBoxLotNumber = null!;
    private ComboBox comboBoxModel = null!;
    private ComboBox comboBoxProcess = null!;
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
    private Button buttonAddAttachment = null!;
    private Label labelAttachmentCount = null!;
}

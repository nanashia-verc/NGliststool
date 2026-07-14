namespace NgProductManager.Forms;
partial class AddInspectionHistoryForm
{
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing) { if (disposing && components is not null) components.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        Text = "再検査登録"; ClientSize = new Size(560, 520); MinimumSize = new Size(500, 460); StartPosition = FormStartPosition.CenterParent;
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 2, RowCount = 9 }; root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110)); root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); for (var i = 0; i < 9; i++) root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        dateTimePickerInspection = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy/MM/dd", Width = 150 }; Add(root, 0, "検査日", dateTimePickerInspection);
        var result = new FlowLayoutPanel { AutoSize = true }; radioButtonNg = new RadioButton { Text = "NG", Checked = true, AutoSize = true }; radioButtonOk = new RadioButton { Text = "OK", AutoSize = true }; result.Controls.AddRange([radioButtonNg, radioButtonOk]); Add(root, 1, "結果", result);
        comboBoxDefectReason = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown, Dock = DockStyle.Top }; Add(root, 2, "NG理由", comboBoxDefectReason); textBoxDefectDetails = new TextBox { Multiline = true, Height = 58, Dock = DockStyle.Top }; Add(root, 3, "NG詳細", textBoxDefectDetails);
        comboBoxAction = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown, Dock = DockStyle.Top }; Add(root, 4, "処置", comboBoxAction); textBoxActionDetails = new TextBox { Multiline = true, Height = 58, Dock = DockStyle.Top }; Add(root, 5, "処置詳細", textBoxActionDetails); textBoxInspectorName = new TextBox { Dock = DockStyle.Top }; Add(root, 6, "担当者", textBoxInspectorName);
        var status = new FlowLayoutPanel { AutoSize = true }; radioButtonPending = new RadioButton { Text = "再検査待ち", Checked = true, AutoSize = true }; radioButtonInProgress = new RadioButton { Text = "対応中", AutoSize = true }; status.Controls.AddRange([radioButtonPending, radioButtonInProgress]); Add(root, 7, "案件状態", status);
        var buttons = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, Margin = new Padding(0, 8, 0, 0) }; buttonCancel = new Button { Text = "キャンセル", Width = 100 }; buttonSave = new Button { Text = "登録", Width = 100 }; buttons.Controls.AddRange([buttonCancel, buttonSave]); root.SetColumnSpan(buttons, 2); root.Controls.Add(buttons, 0, 8); Controls.Add(root); buttonSave.Click += buttonSave_Click; buttonCancel.Click += (s, e) => Close();
    }
    private static void Add(TableLayoutPanel root, int row, string label, Control control) { root.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 8, 3) }, 0, row); control.Margin = new Padding(0, 3, 0, 3); root.Controls.Add(control, 1, row); }
    private DateTimePicker dateTimePickerInspection = null!; private RadioButton radioButtonNg = null!, radioButtonOk = null!, radioButtonPending = null!, radioButtonInProgress = null!; private ComboBox comboBoxDefectReason = null!, comboBoxAction = null!; private TextBox textBoxDefectDetails = null!, textBoxActionDetails = null!, textBoxInspectorName = null!; private Button buttonSave = null!, buttonCancel = null!;
}

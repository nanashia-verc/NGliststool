namespace NgProductManager.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    protected override void Dispose(bool disposing) { if (disposing && components is not null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        Text = "NG品管理ツール";
        ClientSize = new Size(1100, 700);
        MinimumSize = new Size(1100, 700);
        MaximumSize = new Size(1100, 700);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 1, RowCount = 3 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true, Margin = new Padding(0, 0, 0, 6) };
        buttonNewCase = AddButton(actions, "新規NG登録", 120); buttonShowDetail = AddButton(actions, "詳細表示", 100); buttonReinspection = AddButton(actions, "再検査登録", 110); buttonCloseCase = AddButton(actions, "検査OK・クローズ", 130); buttonMaster = AddButton(actions, "マスター管理", 110); buttonCsv = AddButton(actions, "CSV出力", 90); buttonReload = AddButton(actions, "再読込", 90); buttonBackup = AddButton(actions, "バックアップ", 100);
        root.Controls.Add(actions, 0, 0);

        var search = new TableLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Margin = new Padding(0, 0, 0, 8) };
        var searchRow1 = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Dock = DockStyle.Fill };
        textBoxFreeText = AddText(searchRow1, "フリーワード", 130); textBoxLotNumber = AddText(searchRow1, "ロット番号", 120); textBoxProductModel = AddText(searchRow1, "型番", 120);
        searchRow1.Controls.Add(new Label { Text = "状態", AutoSize = true, Margin = new Padding(10, 6, 4, 0) }); comboBoxStatus = new ComboBox { Width = 115, DropDownStyle = ComboBoxStyle.DropDownList }; searchRow1.Controls.Add(comboBoxStatus);
        var searchRow2 = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Dock = DockStyle.Fill };
        searchRow2.Controls.Add(new Label { Text = "登録日", AutoSize = true, Margin = new Padding(10, 6, 4, 0) }); dateTimePickerFrom = new DateTimePicker { Width = 125, Format = DateTimePickerFormat.Short }; searchRow2.Controls.Add(dateTimePickerFrom); searchRow2.Controls.Add(new Label { Text = "～", AutoSize = true, Margin = new Padding(4, 6, 4, 0) }); dateTimePickerTo = new DateTimePicker { Width = 125, Format = DateTimePickerFormat.Short }; searchRow2.Controls.Add(dateTimePickerTo);
        checkBoxIncludeClosed = new CheckBox { Text = "クローズ済みを表示", AutoSize = true, Margin = new Padding(14, 5, 0, 0) }; searchRow2.Controls.Add(checkBoxIncludeClosed); buttonSearch = AddButton(searchRow2, "検索", 80); buttonClear = AddButton(searchRow2, "条件クリア", 90);
        search.Controls.Add(searchRow1, 0, 0); search.Controls.Add(searchRow2, 0, 1);
        root.Controls.Add(search, 0, 1);

        dataGridViewCases = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = SystemColors.Window, BorderStyle = BorderStyle.Fixed3D, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        root.Controls.Add(dataGridViewCases, 0, 2); Controls.Add(root);
        buttonSearch.Click += buttonSearch_Click; buttonClear.Click += buttonClear_Click; buttonNewCase.Click += buttonNewCase_Click; buttonShowDetail.Click += buttonShowDetail_Click; buttonReinspection.Click += buttonReinspection_Click; buttonCloseCase.Click += buttonCloseCase_Click; buttonMaster.Click += buttonMaster_Click; buttonCsv.Click += buttonCsv_Click; buttonReload.Click += buttonReload_Click; buttonBackup.Click += buttonBackup_Click; dataGridViewCases.DoubleClick += dataGridViewCases_DoubleClick;
    }
    private static Button AddButton(FlowLayoutPanel panel, string text, int width) { var button = new Button { Text = text, Width = width, Margin = new Padding(0, 0, 6, 0) }; panel.Controls.Add(button); return button; }
    private static TextBox AddText(FlowLayoutPanel panel, string label, int width) { panel.Controls.Add(new Label { Text = label, AutoSize = true, Margin = new Padding(10, 6, 4, 0) }); var box = new TextBox { Width = width }; panel.Controls.Add(box); return box; }
    private TextBox textBoxFreeText = null!, textBoxLotNumber = null!, textBoxProductModel = null!; private ComboBox comboBoxStatus = null!; private DateTimePicker dateTimePickerFrom = null!, dateTimePickerTo = null!; private CheckBox checkBoxIncludeClosed = null!; private Button buttonSearch = null!, buttonClear = null!, buttonNewCase = null!, buttonShowDetail = null!, buttonReinspection = null!, buttonCloseCase = null!, buttonMaster = null!, buttonCsv = null!, buttonReload = null!, buttonBackup = null!; private DataGridView dataGridViewCases = null!;
}

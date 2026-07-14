namespace NgProductManager.Forms;

partial class MainForm
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
        this.components = new System.ComponentModel.Container();
        this.Text = "NG品管理ツール";
        this.ClientSize = new System.Drawing.Size(1200, 700);
        this.MinimumSize = new System.Drawing.Size(1000, 600);

        var flowLayout = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
        flowLayout.Controls.Add(new Label { Text = "フリーワード", AutoSize = true, Margin = new Padding(0, 5, 5, 5) });
        textBoxFreeText = new TextBox { Width = 140 };
        flowLayout.Controls.Add(textBoxFreeText);
        flowLayout.Controls.Add(new Label { Text = "ロット番号", AutoSize = true, Margin = new Padding(10, 5, 5, 5) });
        textBoxLotNumber = new TextBox { Width = 120 };
        flowLayout.Controls.Add(textBoxLotNumber);
        flowLayout.Controls.Add(new Label { Text = "型番", AutoSize = true, Margin = new Padding(10, 5, 5, 5) });
        textBoxProductModel = new TextBox { Width = 120 };
        flowLayout.Controls.Add(textBoxProductModel);
        flowLayout.Controls.Add(new Label { Text = "状態", AutoSize = true, Margin = new Padding(10, 5, 5, 5) });
        comboBoxStatus = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
        flowLayout.Controls.Add(comboBoxStatus);
        flowLayout.Controls.Add(new Label { Text = "登録日From", AutoSize = true, Margin = new Padding(10, 5, 5, 5) });
        dateTimePickerFrom = new DateTimePicker { Width = 140, Format = DateTimePickerFormat.Short };
        flowLayout.Controls.Add(dateTimePickerFrom);
        flowLayout.Controls.Add(new Label { Text = "登録日To", AutoSize = true, Margin = new Padding(10, 5, 5, 5) });
        dateTimePickerTo = new DateTimePicker { Width = 140, Format = DateTimePickerFormat.Short };
        flowLayout.Controls.Add(dateTimePickerTo);
        checkBoxIncludeClosed = new CheckBox { Text = "クローズ済みを表示", AutoSize = true, Margin = new Padding(10, 8, 0, 0) };
        flowLayout.Controls.Add(checkBoxIncludeClosed);
        buttonSearch = new Button { Text = "検索", Width = 90, Margin = new Padding(10, 5, 0, 0) };
        flowLayout.Controls.Add(buttonSearch);
        buttonClear = new Button { Text = "条件クリア", Width = 100, Margin = new Padding(5, 5, 0, 0) };
        flowLayout.Controls.Add(buttonClear);

        this.Controls.Add(flowLayout);

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10), Margin = new Padding(0, 0, 0, 5) };
        buttonNewCase = new Button { Text = "新規NG登録", Width = 140, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonNewCase);
        buttonShowDetail = new Button { Text = "詳細表示", Width = 120, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonShowDetail);
        buttonReinspection = new Button { Text = "再検査登録", Width = 120, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonReinspection);
        buttonCloseCase = new Button { Text = "検査OK・クローズ", Width = 140, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonCloseCase);
        buttonMaster = new Button { Text = "マスター管理", Width = 130, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonMaster);
        buttonCsv = new Button { Text = "CSV出力", Width = 100, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonCsv);
        buttonReload = new Button { Text = "データ再読込", Width = 120, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonReload);
        buttonBackup = new Button { Text = "バックアップ", Width = 110, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonBackup);

        this.Controls.Add(buttonPanel);

        dataGridViewCases = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        this.Controls.Add(dataGridViewCases);

        buttonSearch.Click += buttonSearch_Click;
        buttonClear.Click += buttonClear_Click;
        buttonNewCase.Click += buttonNewCase_Click;
        buttonShowDetail.Click += buttonShowDetail_Click;
        buttonReinspection.Click += buttonReinspection_Click;
        buttonCloseCase.Click += buttonCloseCase_Click;
        buttonMaster.Click += buttonMaster_Click;
        buttonCsv.Click += buttonCsv_Click;
        buttonReload.Click += buttonReload_Click;
        buttonBackup.Click += buttonBackup_Click;
        dataGridViewCases.DoubleClick += dataGridViewCases_DoubleClick;
    }

    private TextBox textBoxFreeText = null!;
    private TextBox textBoxLotNumber = null!;
    private TextBox textBoxProductModel = null!;
    private ComboBox comboBoxStatus = null!;
    private DateTimePicker dateTimePickerFrom = null!;
    private DateTimePicker dateTimePickerTo = null!;
    private CheckBox checkBoxIncludeClosed = null!;
    private Button buttonSearch = null!;
    private Button buttonClear = null!;
    private Button buttonNewCase = null!;
    private Button buttonShowDetail = null!;
    private Button buttonReinspection = null!;
    private Button buttonCloseCase = null!;
    private Button buttonMaster = null!;
    private Button buttonCsv = null!;
    private Button buttonReload = null!;
    private Button buttonBackup = null!;
    private DataGridView dataGridViewCases = null!;
}

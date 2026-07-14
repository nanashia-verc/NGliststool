namespace NgProductManager.Forms;

partial class NgCaseDetailForm
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
        this.Text = "案件詳細";
        this.ClientSize = new System.Drawing.Size(1000, 700);
        this.StartPosition = FormStartPosition.CenterParent;

        var topPanel = new TableLayoutPanel { Dock = DockStyle.Top, Padding = new Padding(12), AutoSize = true, ColumnCount = 2, RowCount = 5 };
        topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));

        AddLabel(topPanel, 0, "ロット番号");
        labelLotNumber = new Label { AutoSize = true };
        topPanel.Controls.Add(labelLotNumber, 1, 0);

        AddLabel(topPanel, 1, "型番");
        labelProductModel = new Label { AutoSize = true };
        topPanel.Controls.Add(labelProductModel, 1, 1);

        AddLabel(topPanel, 2, "状態");
        labelStatus = new Label { AutoSize = true };
        topPanel.Controls.Add(labelStatus, 1, 2);

        AddLabel(topPanel, 3, "登録日時");
        labelRegisteredAt = new Label { AutoSize = true };
        topPanel.Controls.Add(labelRegisteredAt, 1, 3);

        AddLabel(topPanel, 4, "クローズ日時");
        labelClosedAt = new Label { AutoSize = true };
        topPanel.Controls.Add(labelClosedAt, 1, 4);

        this.Controls.Add(topPanel);

        var notesPanel = new GroupBox { Dock = DockStyle.Top, Text = "備考", Height = 120, Padding = new Padding(8) };
        labelNotes = new Label { AutoSize = true, MaximumSize = new Size(900, 80) };
        notesPanel.Controls.Add(labelNotes);
        this.Controls.Add(notesPanel);

        var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(10) };
        buttonAddReinspection = new Button { Text = "再検査結果追加", Width = 140, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonAddReinspection);
        buttonCloseCase = new Button { Text = "検査OK・クローズ", Width = 140, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonCloseCase);
        buttonUpdateNotes = new Button { Text = "備考更新", Width = 110, Margin = new Padding(0, 0, 5, 0) };
        buttonPanel.Controls.Add(buttonUpdateNotes);
        this.Controls.Add(buttonPanel);

        historyGrid = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false };
        historyGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "回数", DataPropertyName = "Sequence", Width = 60 });
        historyGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "検査日時", DataPropertyName = "InspectionDateTime", Width = 120 });
        historyGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "結果", DataPropertyName = "Result", Width = 60 });
        historyGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "NG理由", DataPropertyName = "DefectReasonName", Width = 120 });
        historyGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "詳細", DataPropertyName = "DefectDetails", Width = 180 });
        historyGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "処置", DataPropertyName = "ActionTypeName", Width = 120 });
        historyGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "処置詳細", DataPropertyName = "ActionDetails", Width = 180 });
        historyGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "担当者", DataPropertyName = "InspectorName", Width = 100 });
        this.Controls.Add(historyGrid);

        bindingSource = new BindingSource();
        historyGrid.DataSource = bindingSource;

        buttonAddReinspection.Click += buttonAddReinspection_Click;
        buttonCloseCase.Click += buttonCloseCase_Click;
        buttonUpdateNotes.Click += buttonUpdateNotes_Click;
    }

    private static void AddLabel(TableLayoutPanel table, int row, string text)
    {
        table.Controls.Add(new Label { Text = text, AutoSize = true, Margin = new Padding(0, 5, 0, 0) }, 0, row);
    }

    private Label labelLotNumber = null!;
    private Label labelProductModel = null!;
    private Label labelStatus = null!;
    private Label labelRegisteredAt = null!;
    private Label labelClosedAt = null!;
    private Label labelNotes = null!;
    private Button buttonAddReinspection = null!;
    private Button buttonCloseCase = null!;
    private Button buttonUpdateNotes = null!;
    private DataGridView historyGrid = null!;
    private BindingSource bindingSource = null!;
}

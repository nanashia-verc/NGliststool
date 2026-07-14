namespace NgProductManager.Forms;

partial class MasterManagementForm
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
        this.Text = "マスター管理";
        this.ClientSize = new System.Drawing.Size(900, 560);
        this.StartPosition = FormStartPosition.CenterParent;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 2, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(new Label { Text = "登録したマスターは新規NG登録画面ですぐに使えます。", AutoSize = true, Margin = new Padding(0, 0, 0, 6) }, 0, 0);

        var tab = new TabControl { Dock = DockStyle.Fill };
        root.Controls.Add(tab, 0, 1);
        this.Controls.Add(root);
        var pageModels = new TabPage("型番");
        var pageReasons = new TabPage("NG理由");
        var pageActions = new TabPage("処置内容");

        var modelPanel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 2, RowCount = 2 };
        listBoxModels = new ListBox { Dock = DockStyle.Fill };
        modelPanel.Controls.Add(listBoxModels, 0, 0);

        var modelInput = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        textBoxModelCode = new TextBox();
        textBoxModelName = new TextBox();
        buttonAddModel = new Button { Text = "追加", Width = 100 };
        modelInput.Controls.Add(new Label { Text = "型番コード" }, 0, 0);
        modelInput.Controls.Add(textBoxModelCode, 0, 1);
        modelInput.Controls.Add(new Label { Text = "表示名" }, 0, 2);
        modelInput.Controls.Add(textBoxModelName, 0, 3);
        modelInput.Controls.Add(buttonAddModel, 0, 4);
        modelPanel.Controls.Add(modelInput, 1, 0);
        pageModels.Controls.Add(modelPanel);
        tab.TabPages.Add(pageModels);

        var reasonPanel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 2, RowCount = 2 };
        listBoxDefectReasons = new ListBox { Dock = DockStyle.Fill };
        reasonPanel.Controls.Add(listBoxDefectReasons, 0, 0);
        var reasonInput = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        textBoxReasonName = new TextBox();
        buttonAddReason = new Button { Text = "追加", Width = 100 };
        reasonInput.Controls.Add(new Label { Text = "NG理由名" }, 0, 0);
        reasonInput.Controls.Add(textBoxReasonName, 0, 1);
        reasonInput.Controls.Add(buttonAddReason, 0, 2);
        reasonPanel.Controls.Add(reasonInput, 1, 0);
        pageReasons.Controls.Add(reasonPanel);
        tab.TabPages.Add(pageReasons);

        var actionPanel = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 2, RowCount = 2 };
        listBoxActionTypes = new ListBox { Dock = DockStyle.Fill };
        actionPanel.Controls.Add(listBoxActionTypes, 0, 0);
        var actionInput = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        textBoxActionName = new TextBox();
        buttonAddAction = new Button { Text = "追加", Width = 100 };
        actionInput.Controls.Add(new Label { Text = "処置内容名" }, 0, 0);
        actionInput.Controls.Add(textBoxActionName, 0, 1);
        actionInput.Controls.Add(buttonAddAction, 0, 2);
        actionPanel.Controls.Add(actionInput, 1, 0);
        pageActions.Controls.Add(actionPanel);
        tab.TabPages.Add(pageActions);

        buttonAddModel.Click += buttonAddModel_Click;
        buttonAddReason.Click += buttonAddReason_Click;
        buttonAddAction.Click += buttonAddAction_Click;
    }

    private ListBox listBoxModels = null!;
    private ListBox listBoxDefectReasons = null!;
    private ListBox listBoxActionTypes = null!;
    private TextBox textBoxModelCode = null!;
    private TextBox textBoxModelName = null!;
    private TextBox textBoxReasonName = null!;
    private TextBox textBoxActionName = null!;
    private Button buttonAddModel = null!;
    private Button buttonAddReason = null!;
    private Button buttonAddAction = null!;
}

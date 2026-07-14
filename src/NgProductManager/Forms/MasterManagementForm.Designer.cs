namespace NgProductManager.Forms;

partial class MasterManagementForm
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
        Text = "マスター管理";
        ClientSize = new Size(680, 420);
        MinimumSize = new Size(620, 380);
        StartPosition = FormStartPosition.CenterParent;

        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 2 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(new Label
        {
            Text = "型番・NG理由・処置内容を登録すると、新規NG登録で選択できます。",
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8)
        }, 0, 0);

        var tab = new TabControl { Dock = DockStyle.Fill };
        tab.TabPages.Add(CreateModelPage());
        tab.TabPages.Add(CreateSingleNamePage("工程", "工程名", out listBoxProcesses, out textBoxProcessName, out buttonAddProcess));
        tab.TabPages.Add(CreateSingleNamePage("NG理由", "NG理由名", out listBoxDefectReasons, out textBoxReasonName, out buttonAddReason));
        tab.TabPages.Add(CreateSingleNamePage("処置内容", "処置内容名", out listBoxActionTypes, out textBoxActionName, out buttonAddAction));
        root.Controls.Add(tab, 0, 1);
        Controls.Add(root);

        buttonAddModel.Click += buttonAddModel_Click;
        buttonAddProcess.Click += buttonAddProcess_Click;
        buttonAddReason.Click += buttonAddReason_Click;
        buttonAddAction.Click += buttonAddAction_Click;
    }

    private TabPage CreateModelPage()
    {
        var page = new TabPage("型番");
        var layout = CreatePageLayout(out listBoxModels, out var inputPanel);
        textBoxModelCode = AddTextField(inputPanel, "型番コード");
        textBoxModelName = AddTextField(inputPanel, "表示名");
        buttonAddModel = AddButton(inputPanel, "型番を登録");
        page.Controls.Add(layout);
        return page;
    }

    private TabPage CreateSingleNamePage(string title, string label, out ListBox listBox, out TextBox textBox, out Button button)
    {
        var page = new TabPage(title);
        var layout = CreatePageLayout(out listBox, out var inputPanel);
        textBox = AddTextField(inputPanel, label);
        button = AddButton(inputPanel, $"{title}を登録");
        page.Controls.Add(layout);
        return page;
    }

    private static TableLayoutPanel CreatePageLayout(out ListBox listBox, out TableLayoutPanel inputPanel)
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));

        listBox = new ListBox { Dock = DockStyle.Fill, IntegralHeight = false };
        layout.Controls.Add(listBox, 0, 0);

        inputPanel = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 1, AutoSize = true, Padding = new Padding(12, 0, 0, 0) };
        layout.Controls.Add(inputPanel, 1, 0);
        return layout;
    }

    private static TextBox AddTextField(TableLayoutPanel panel, string label)
    {
        panel.Controls.Add(new Label { Text = label, AutoSize = true, Margin = new Padding(0, 0, 0, 3) });
        var textBox = new TextBox { Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 12) };
        panel.Controls.Add(textBox);
        return textBox;
    }

    private static Button AddButton(TableLayoutPanel panel, string text)
    {
        var button = new Button { Text = text, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 0, 0, 8) };
        panel.Controls.Add(button);
        return button;
    }

    private ListBox listBoxModels = null!;
    private ListBox listBoxProcesses = null!;
    private ListBox listBoxDefectReasons = null!;
    private ListBox listBoxActionTypes = null!;
    private TextBox textBoxModelCode = null!;
    private TextBox textBoxModelName = null!;
    private TextBox textBoxProcessName = null!;
    private TextBox textBoxReasonName = null!;
    private TextBox textBoxActionName = null!;
    private Button buttonAddModel = null!;
    private Button buttonAddProcess = null!;
    private Button buttonAddReason = null!;
    private Button buttonAddAction = null!;
}

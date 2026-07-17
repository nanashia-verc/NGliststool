using NgProductManager.Models;
using NgProductManager.Services;

namespace NgProductManager.Forms;

public partial class MasterManagementForm : Form
{
    private readonly NgCaseService _service;

    public MasterManagementForm(NgCaseService service)
    {
        _service = service;
        InitializeComponent();
        ConfigureMasterMenus();
        LoadMasters();
    }

    private void LoadMasters()
    {
        listBoxModels.Items.Clear();
        foreach (var model in _service.GetActiveProductModels())
        {
            listBoxModels.Items.Add(new MasterListItem<ProductModelMaster>(model, $"{model.DisplayName} ({model.ModelCode})"));
        }

        listBoxProcesses.Items.Clear();
        foreach (var process in _service.GetActiveProcesses())
        {
            listBoxProcesses.Items.Add(new MasterListItem<ProcessMaster>(process, process.Name));
        }

        listBoxDefectReasons.Items.Clear();
        foreach (var reason in _service.GetActiveDefectReasons())
        {
            listBoxDefectReasons.Items.Add(new MasterListItem<DefectReasonMaster>(reason, reason.Name));
        }

        listBoxActionTypes.Items.Clear();
        foreach (var action in _service.GetActiveActionTypes())
        {
            listBoxActionTypes.Items.Add(new MasterListItem<ActionTypeMaster>(action, action.Name));
        }
    }

    private void buttonAddModel_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBoxModelCode.Text) || string.IsNullOrWhiteSpace(textBoxModelName.Text))
        {
            MessageBox.Show(this, "型番コードと名称を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _service.CreateProductModel(textBoxModelCode.Text, textBoxModelName.Text);
            textBoxModelCode.Clear();
            textBoxModelName.Clear();
            LoadMasters();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buttonAddReason_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBoxReasonName.Text))
        {
            MessageBox.Show(this, "NG理由名を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _service.CreateDefectReason(textBoxReasonName.Text);
            textBoxReasonName.Clear();
            LoadMasters();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buttonAddProcess_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBoxProcessName.Text))
        {
            MessageBox.Show(this, "工程名を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _service.CreateProcess(textBoxProcessName.Text);
            textBoxProcessName.Clear();
            LoadMasters();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buttonAddAction_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(textBoxActionName.Text))
        {
            MessageBox.Show(this, "処置内容名を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _service.CreateActionType(textBoxActionName.Text);
            textBoxActionName.Clear();
            LoadMasters();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ConfigureMasterMenus()
    {
        AddMasterMenu(listBoxModels, EditModel, id => _service.DeleteProductModel(id));
        AddMasterMenu(listBoxProcesses, EditProcess, id => _service.DeleteProcess(id));
        AddMasterMenu(listBoxDefectReasons, EditReason, id => _service.DeleteDefectReason(id));
        AddMasterMenu(listBoxActionTypes, EditAction, id => _service.DeleteActionType(id));
    }

    private void buttonEditModel_Click(object? sender, EventArgs e) => EditSelected(listBoxModels, EditModel);
    private void buttonEditProcess_Click(object? sender, EventArgs e) => EditSelected(listBoxProcesses, EditProcess);
    private void buttonEditReason_Click(object? sender, EventArgs e) => EditSelected(listBoxDefectReasons, EditReason);
    private void buttonEditAction_Click(object? sender, EventArgs e) => EditSelected(listBoxActionTypes, EditAction);
    private void buttonDeleteModel_Click(object? sender, EventArgs e) => DeleteSelected(listBoxModels, id => _service.DeleteProductModel(id));
    private void buttonDeleteProcess_Click(object? sender, EventArgs e) => DeleteSelected(listBoxProcesses, id => _service.DeleteProcess(id));
    private void buttonDeleteReason_Click(object? sender, EventArgs e) => DeleteSelected(listBoxDefectReasons, id => _service.DeleteDefectReason(id));
    private void buttonDeleteAction_Click(object? sender, EventArgs e) => DeleteSelected(listBoxActionTypes, id => _service.DeleteActionType(id));

    private void EditSelected(ListBox listBox, Action<object> edit)
    {
        if (listBox.SelectedItem is not IMasterListItem item) { MessageBox.Show(this, "修正する項目を選択してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        try { edit(item.Value); LoadMasters(); } catch (Exception ex) { MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void DeleteSelected(ListBox listBox, Action<int> delete)
    {
        if (listBox.SelectedItem is not IMasterListItem item) { MessageBox.Show(this, "削除する項目を選択してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (MessageBox.Show(this, "このマスターを削除しますか？\n既存案件の表示は維持され、新規入力では選べなくなります。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        delete(item.Id); LoadMasters();
    }

    private void AddMasterMenu(ListBox listBox, Action<object> edit, Action<int> delete)
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("修正", null, (_, _) => { if (listBox.SelectedItem is IMasterListItem item) { try { edit(item.Value); LoadMasters(); } catch (Exception ex) { MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error); } } });
        menu.Items.Add("削除", null, (_, _) => { if (listBox.SelectedItem is IMasterListItem item && MessageBox.Show(this, "このマスターを削除しますか？\n既存案件の表示は維持され、新規入力では選べなくなります。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) { delete(item.Id); LoadMasters(); } });
        listBox.ContextMenuStrip = menu;
    }

    private void EditModel(object value)
    {
        var item = (ProductModelMaster)value;
        if (ShowEditDialog("型番を修正", new[] { ("型番コード", item.ModelCode), ("表示名", item.DisplayName) }, out var values)) _service.UpdateProductModel(item.Id, values[0], values[1]);
    }
    private void EditProcess(object value) { var item = (ProcessMaster)value; if (ShowEditDialog("工程を修正", new[] { ("工程名", item.Name) }, out var values)) _service.UpdateProcess(item.Id, values[0]); }
    private void EditReason(object value) { var item = (DefectReasonMaster)value; if (ShowEditDialog("NG理由を修正", new[] { ("NG理由名", item.Name) }, out var values)) _service.UpdateDefectReason(item.Id, values[0]); }
    private void EditAction(object value) { var item = (ActionTypeMaster)value; if (ShowEditDialog("処置内容を修正", new[] { ("処置内容名", item.Name) }, out var values)) _service.UpdateActionType(item.Id, values[0]); }

    private static bool ShowEditDialog(string title, (string Label, string Value)[] fields, out string[] values)
    {
        using var dialog = new Form { Text = title, ClientSize = new Size(360, 70 + fields.Length * 55), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 2, RowCount = fields.Length + 1 }; layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var boxes = new List<TextBox>();
        for (var i = 0; i < fields.Length; i++) { layout.Controls.Add(new Label { Text = fields[i].Label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, i); var box = new TextBox { Text = fields[i].Value, Dock = DockStyle.Fill }; boxes.Add(box); layout.Controls.Add(box, 1, i); }
        var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill }; var ok = new Button { Text = "保存", DialogResult = DialogResult.OK }; buttons.Controls.Add(ok); buttons.Controls.Add(new Button { Text = "キャンセル", DialogResult = DialogResult.Cancel }); layout.SetColumnSpan(buttons, 2); layout.Controls.Add(buttons, 0, fields.Length); dialog.AcceptButton = ok; dialog.Controls.Add(layout);
        var result = dialog.ShowDialog() == DialogResult.OK; values = boxes.Select(box => box.Text).ToArray(); return result;
    }

    private interface IMasterListItem { int Id { get; } object Value { get; } }
    private sealed class MasterListItem<T>(T value, string text) : IMasterListItem where T : class
    {
        public int Id => Value switch { ProductModelMaster x => x.Id, ProcessMaster x => x.Id, DefectReasonMaster x => x.Id, ActionTypeMaster x => x.Id, _ => 0 };
        public object Value => value;
        public override string ToString() => text;
    }
}

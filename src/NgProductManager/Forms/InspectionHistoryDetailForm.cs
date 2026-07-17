using NgProductManager.Models;
using NgProductManager.Services;

namespace NgProductManager.Forms;

public sealed class InspectionHistoryDetailForm : Form
{
    private readonly NgCaseService _service;
    private readonly int _caseId;
    private readonly int _historyId;
    private readonly bool _isInitial;

    public InspectionHistoryDetailForm(NgCaseService service, int caseId, int historyId, bool isInitial)
    {
        _service = service; _caseId = caseId; _historyId = historyId; _isInitial = isInitial;
        Text = "検査履歴詳細"; ClientSize = new Size(600, 500); MinimumSize = new Size(500, 420); StartPosition = FormStartPosition.CenterParent;
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 2, AutoScroll = true };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var history = _service.GetCase(caseId)?.InspectionHistories.FirstOrDefault(item => item.Id == historyId);
        if (history is null) { Close(); return; }
        Add(root, "検査日", history.InspectionDateTime.ToString("yyyy/MM/dd")); Add(root, "結果", history.Result == InspectionResult.Ng ? "NG" : "OK"); Add(root, "NG理由", history.DefectReasonName); Add(root, "NG詳細", history.DefectDetails); Add(root, "処置", history.ActionTypeName); Add(root, "処置詳細", history.ActionDetails); Add(root, "担当者", history.InspectorName);
        var buttons = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 0) }; var close = new Button { Text = "閉じる", DialogResult = DialogResult.Cancel, Width = 90 }; var edit = new Button { Text = "内容修正", Width = 90 }; edit.Click += (_, _) => EditHistory(); buttons.Controls.Add(close); buttons.Controls.Add(edit); root.SetColumnSpan(buttons, 2); root.Controls.Add(buttons, 0, root.RowCount++); Controls.Add(root);
    }

    private void EditHistory()
    {
        if (_isInitial) { using var dialog = new NewNgCaseForm(_service, _caseId); if (dialog.ShowDialog(this) == DialogResult.OK) DialogResult = DialogResult.OK; }
        else { using var dialog = new AddInspectionHistoryForm(_service, _caseId, _historyId); if (dialog.ShowDialog(this) == DialogResult.OK) DialogResult = DialogResult.OK; }
    }

    private static void Add(TableLayoutPanel panel, string label, string? value)
    {
        var row = panel.RowCount++; panel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); panel.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 6, 8, 6) }, 0, row); panel.Controls.Add(new TextBox { Text = value ?? string.Empty, ReadOnly = true, Multiline = true, AutoSize = true, MinimumSize = new Size(0, 30), Dock = DockStyle.Top, BorderStyle = BorderStyle.FixedSingle, BackColor = SystemColors.Window }, 1, row);
    }
}

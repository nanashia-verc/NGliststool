using NgProductManager.Models;
using NgProductManager.Services;

namespace NgProductManager.Forms;

public partial class NgCaseDetailForm : Form
{
    private readonly NgCaseService _service;
    private readonly int _caseId;

    public NgCaseDetailForm(NgCaseService service, int caseId)
    {
        _service = service;
        _caseId = caseId;
        InitializeComponent();
        LoadCase();
    }

    private void LoadCase()
    {
        var detail = _service.GetCase(_caseId);
        if (detail is null)
        {
            return;
        }

        labelLotNumber.Text = detail.LotNumber;
        labelProductModel.Text = detail.ProductModelName;
        labelProcess.Text = detail.ProcessName ?? string.Empty;
        labelStatus.Text = detail.Status switch
        {
            NgCaseStatus.InProgress => "対応中",
            NgCaseStatus.PendingReinspection => "再検査待ち",
            NgCaseStatus.Closed => "クローズ済み",
            _ => string.Empty
        };
        labelRegisteredAt.Text = detail.RegisteredAt.ToString("yyyy/MM/dd");
        labelClosedAt.Text = detail.ClosedAt?.ToString("yyyy/MM/dd") ?? string.Empty;
        labelNotes.Text = detail.Notes ?? string.Empty;

        var initialHistoryId = detail.InspectionHistories
            .OrderBy(history => history.InspectionDateTime)
            .ThenBy(history => history.Id)
            .Select(history => (int?)history.Id)
            .FirstOrDefault();

        var rows = detail.InspectionHistories
            .OrderByDescending(history => history.InspectionDateTime)
            .ThenByDescending(history => history.Id)
            .Select((history, index) => new InspectionRowViewModel(index == 0, history.Id == initialHistoryId, history))
            .ToList();
        bindingSource.DataSource = rows;
        historyGrid.ClearSelection();
    }

    private void buttonAddReinspection_Click(object sender, EventArgs e)
    {
        using var dialog = new AddInspectionHistoryForm(_service, _caseId);
        dialog.ShowDialog(this);
        LoadCase();
    }

    private void buttonCloseCase_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show(this, $"案件ID {_caseId} をクローズしますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _service.CloseCase(_caseId);
            LoadCase();
            MessageBox.Show(this, "案件をクローズしました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buttonUpdateNotes_Click(object sender, EventArgs e)
    {
        if (historyGrid.SelectedRows.Count > 0 && historyGrid.CurrentRow?.DataBoundItem is InspectionRowViewModel row && !row.IsInitial)
        {
            using var historyDialog = new AddInspectionHistoryForm(_service, _caseId, row.Id);
            if (historyDialog.ShowDialog(this) == DialogResult.OK) LoadCase();
            return;
        }

        using var dialog = new NewNgCaseForm(_service, _caseId);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadCase();
        }
    }

    private void buttonAddAttachment_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Filter = "画像ファイル|*.png;*.jpg;*.jpeg;*.bmp" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            _service.AddAttachment(_caseId, dialog.FileName);
            MessageBox.Show(this, "画像を添付しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buttonShowAttachments_Click(object? sender, EventArgs e)
    {
        var attachments = _service.GetAttachments(_caseId);
        if (attachments.Count == 0)
        {
            MessageBox.Show(this, "添付画像はありません。", "画像", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var viewer = new Form { Text = "添付画像", ClientSize = new Size(760, 560), StartPosition = FormStartPosition.CenterParent };
        var list = new ListBox { Dock = DockStyle.Left, Width = 220 };
        list.Items.AddRange(attachments.Select(x => (object)x.FileName).ToArray());
        var picture = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.White };
        void ShowImage(int index)
        {
            picture.Image?.Dispose();
            using var stream = new MemoryStream(attachments[index].Content);
            using var image = Image.FromStream(stream);
            picture.Image = new Bitmap(image);
        }
        list.SelectedIndexChanged += (_, _) => { if (list.SelectedIndex >= 0) ShowImage(list.SelectedIndex); };
        viewer.Controls.Add(picture); viewer.Controls.Add(list); list.SelectedIndex = 0; viewer.ShowDialog(this); picture.Image?.Dispose();
    }

    private void buttonDeleteCase_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show(this, "案件と履歴・添付画像を削除します。元に戻せません。", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        _service.DeleteCase(_caseId);
        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed class InspectionRowViewModel
    {
        public InspectionRowViewModel(bool isCurrent, bool isInitial, InspectionHistoryDetail history)
        {
            Id = history.Id;
            IsInitial = isInitial;
            CurrentState = isCurrent ? "● 現在" : string.Empty;
            InspectionDateTime = history.InspectionDateTime.ToString("yyyy/MM/dd");
            Result = history.Result == InspectionResult.Ng ? "NG" : "OK";
            DefectReasonName = history.DefectReasonName ?? string.Empty;
            DefectDetails = history.DefectDetails ?? string.Empty;
            ActionTypeName = history.ActionTypeName ?? string.Empty;
            ActionDetails = history.ActionDetails ?? string.Empty;
            InspectorName = history.InspectorName ?? string.Empty;
        }

        public int Id { get; }
        public bool IsInitial { get; }

        public string CurrentState { get; }
        public string InspectionDateTime { get; }
        public string Result { get; }
        public string DefectReasonName { get; }
        public string DefectDetails { get; }
        public string ActionTypeName { get; }
        public string ActionDetails { get; }
        public string InspectorName { get; }
    }
}

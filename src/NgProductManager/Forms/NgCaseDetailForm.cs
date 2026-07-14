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

        var rows = detail.InspectionHistories.Select((history, index) => new InspectionRowViewModel(index + 1, history)).ToList();
        bindingSource.DataSource = rows;
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
        using var dialog = new UpdateNotesForm(_service, _caseId);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadCase();
        }
    }

    private sealed class InspectionRowViewModel
    {
        public InspectionRowViewModel(int sequence, InspectionHistoryDetail history)
        {
            Sequence = sequence;
            InspectionDateTime = history.InspectionDateTime.ToString("yyyy/MM/dd");
            Result = history.Result == InspectionResult.Ng ? "NG" : "OK";
            DefectReasonName = history.DefectReasonName ?? string.Empty;
            DefectDetails = history.DefectDetails ?? string.Empty;
            ActionTypeName = history.ActionTypeName ?? string.Empty;
            ActionDetails = history.ActionDetails ?? string.Empty;
            InspectorName = history.InspectorName ?? string.Empty;
        }

        public int Sequence { get; }
        public string InspectionDateTime { get; }
        public string Result { get; }
        public string DefectReasonName { get; }
        public string DefectDetails { get; }
        public string ActionTypeName { get; }
        public string ActionDetails { get; }
        public string InspectorName { get; }
    }
}

using NgProductManager.Models;
using NgProductManager.Services;
using NgProductManager.Utilities;

namespace NgProductManager.Forms;

public partial class AddInspectionHistoryForm : Form
{
    private readonly NgCaseService _service;
    private readonly int _caseId;
    private readonly int? _historyId;

    public AddInspectionHistoryForm(NgCaseService service, int caseId, int? historyId = null)
    {
        _service = service;
        _caseId = caseId;
        _historyId = historyId;
        InitializeComponent();
        LoadMasters();
        if (_historyId.HasValue) LoadHistoryForEdit(_historyId.Value);
    }

    private void LoadMasters()
    {
        comboBoxDefectReason.Items.Clear();
        foreach (var reason in _service.GetActiveDefectReasons())
        {
            comboBoxDefectReason.Items.Add(new ComboBoxItem<int>(reason.Name, reason.Id));
        }

        comboBoxAction.Items.Clear();
        foreach (var action in _service.GetActiveActionTypes())
        {
            comboBoxAction.Items.Add(new ComboBoxItem<int>(action.Name, action.Id));
        }
    }

    private void buttonSave_Click(object sender, EventArgs e)
    {
        if (radioButtonNg.Checked && string.IsNullOrWhiteSpace(comboBoxDefectReason.Text))
        {
            MessageBox.Show(this, "NG理由を選択してください。マスター管理から登録してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var input = new InspectionHistoryInput
            {
                InspectionDateTime = dateTimePickerInspection.Value.Date,
                Result = radioButtonNg.Checked ? InspectionResult.Ng : InspectionResult.Ok,
                DefectReasonId = radioButtonNg.Checked ? ResolveReason() : null,
                DefectDetails = textBoxDefectDetails.Text,
                ActionTypeId = radioButtonNg.Checked ? ResolveAction() : null,
                ActionDetails = textBoxActionDetails.Text,
                InspectorName = textBoxInspectorName.Text,
                NextStatus = radioButtonPending.Checked ? NgCaseStatus.PendingReinspection : NgCaseStatus.InProgress
            };

            if (_historyId.HasValue)
            {
                _service.UpdateInspectionHistory(_caseId, _historyId.Value, input);
            }
            else
            {
                _service.AddInspectionHistory(_caseId, input);
            }
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(this, ex.Message, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("再検査結果の保存に失敗しました。", ex);
            MessageBox.Show(this, "保存に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private int ResolveReason() => comboBoxDefectReason.SelectedItem is ComboBoxItem<int> item ? item.Value : _service.CreateDefectReason(comboBoxDefectReason.Text.Trim());
    private int ResolveAction() => comboBoxAction.SelectedItem is ComboBoxItem<int> item ? item.Value : _service.CreateActionType(comboBoxAction.Text.Trim());

    private void LoadHistoryForEdit(int historyId)
    {
        var history = _service.GetCase(_caseId)?.InspectionHistories.FirstOrDefault(x => x.Id == historyId);
        if (history is null)
        {
            MessageBox.Show(this, "対象の検査履歴が見つかりません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
        }

        Text = "再検査内容修正";
        buttonSave.Text = "保存";
        dateTimePickerInspection.Value = history.InspectionDateTime;
        radioButtonNg.Checked = history.Result == InspectionResult.Ng;
        radioButtonOk.Checked = history.Result == InspectionResult.Ok;
        SelectItemByText(comboBoxDefectReason, history.DefectReasonName);
        textBoxDefectDetails.Text = history.DefectDetails ?? string.Empty;
        SelectItemByText(comboBoxAction, history.ActionTypeName);
        textBoxActionDetails.Text = history.ActionDetails ?? string.Empty;
        textBoxInspectorName.Text = history.InspectorName ?? string.Empty;

        var status = _service.GetCase(_caseId)?.Status;
        radioButtonPending.Checked = status == NgCaseStatus.PendingReinspection;
        radioButtonInProgress.Checked = status != NgCaseStatus.PendingReinspection;
    }

    private static void SelectItemByText(ComboBox comboBox, string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        for (var index = 0; index < comboBox.Items.Count; index++)
        {
            if (string.Equals(comboBox.Items[index]?.ToString(), text, StringComparison.OrdinalIgnoreCase))
            {
                comboBox.SelectedIndex = index;
                return;
            }
        }
        comboBox.Text = text;
    }

    private sealed class ComboBoxItem<T>
    {
        public ComboBoxItem(string displayText, T value)
        {
            DisplayText = displayText;
            Value = value;
        }

        public string DisplayText { get; }
        public T Value { get; }

        public override string ToString() => DisplayText;
    }
}

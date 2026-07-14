using NgProductManager.Models;
using NgProductManager.Services;
using NgProductManager.Utilities;

namespace NgProductManager.Forms;

public partial class AddInspectionHistoryForm : Form
{
    private readonly NgCaseService _service;
    private readonly int _caseId;

    public AddInspectionHistoryForm(NgCaseService service, int caseId)
    {
        _service = service;
        _caseId = caseId;
        InitializeComponent();
        LoadMasters();
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
        if (radioButtonNg.Checked && comboBoxDefectReason.SelectedItem is null)
        {
            MessageBox.Show(this, "NG理由を選択してください。マスター管理から登録してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (radioButtonNg.Checked && comboBoxAction.SelectedItem is null)
        {
            MessageBox.Show(this, "処置内容を選択してください。マスター管理から登録してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var input = new InspectionHistoryInput
            {
                InspectionDateTime = dateTimePickerInspection.Value.Date,
                Result = radioButtonNg.Checked ? InspectionResult.Ng : InspectionResult.Ok,
                DefectReasonId = radioButtonNg.Checked ? ((ComboBoxItem<int>)comboBoxDefectReason.SelectedItem!).Value : null,
                DefectDetails = textBoxDefectDetails.Text,
                ActionTypeId = radioButtonNg.Checked ? ((ComboBoxItem<int>)comboBoxAction.SelectedItem!).Value : null,
                ActionDetails = textBoxActionDetails.Text,
                InspectorName = textBoxInspectorName.Text,
                NextStatus = radioButtonPending.Checked ? NgCaseStatus.PendingReinspection : NgCaseStatus.InProgress
            };

            _service.AddInspectionHistory(_caseId, input);
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

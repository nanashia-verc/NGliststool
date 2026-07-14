using NgProductManager.Models;
using NgProductManager.Services;

namespace NgProductManager.Forms;

public partial class NewNgCaseForm : Form
{
    private readonly NgCaseService _service;

    public NewNgCaseForm(NgCaseService service)
    {
        _service = service;
        InitializeComponent();
        LoadMasters();
    }

    private void LoadMasters()
    {
        comboBoxModel.Items.Clear();
        foreach (var model in _service.GetActiveProductModels())
        {
            comboBoxModel.Items.Add(new ComboBoxItem<int>(model.DisplayName, model.Id));
        }

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
        if (comboBoxModel.SelectedItem is null)
        {
            MessageBox.Show(this, "型番を選択してください。マスター管理から登録してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

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
            var inspectionDate = dateTimePickerInspection.Value.Date;
            var request = new CreateCaseRequest
            {
                LotNumber = textBoxLotNumber.Text,
                ProductModelId = ((ComboBoxItem<int>)comboBoxModel.SelectedItem).Value,
                RegisteredAt = inspectionDate,
                Notes = textBoxNotes.Text,
                InspectionDateTime = inspectionDate,
                Result = radioButtonNg.Checked ? InspectionResult.Ng : InspectionResult.Ok,
                DefectReasonId = radioButtonNg.Checked ? ((ComboBoxItem<int>)comboBoxDefectReason.SelectedItem!).Value : null,
                DefectDetails = textBoxDefectDetail.Text,
                ActionTypeId = radioButtonNg.Checked ? ((ComboBoxItem<int>)comboBoxAction.SelectedItem!).Value : null,
                ActionDetails = textBoxActionDetail.Text,
                InspectorName = textBoxInspectorName.Text
            };

            _service.CreateCaseWithInitialInspection(request);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(this, ex.Message, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception)
        {
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

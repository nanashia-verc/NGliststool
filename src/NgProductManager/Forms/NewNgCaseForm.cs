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
        try
        {
            var request = new CreateCaseRequest
            {
                LotNumber = textBoxLotNumber.Text,
                ProductModelId = ((ComboBoxItem<int>)comboBoxModel.SelectedItem!).Value,
                SerialNumber = textBoxSerialNumber.Text,
                RegisteredAt = dateTimePickerInspection.Value,
                Notes = textBoxNotes.Text,
                InspectionDateTime = dateTimePickerInspection.Value,
                Result = radioButtonNg.Checked ? InspectionResult.Ng : InspectionResult.Ok,
                DefectReasonId = ((ComboBoxItem<int>)comboBoxDefectReason.SelectedItem!).Value,
                DefectDetails = textBoxDefectDetail.Text,
                ActionTypeId = ((ComboBoxItem<int>)comboBoxAction.SelectedItem!).Value,
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
        catch (Exception ex)
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

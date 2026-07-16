using NgProductManager.Models;
using NgProductManager.Services;
using NgProductManager.Utilities;

namespace NgProductManager.Forms;

public partial class NewNgCaseForm : Form
{
    private readonly NgCaseService _service;
    private readonly int? _caseId;
    private readonly List<string> _attachmentPaths = [];

    public NewNgCaseForm(NgCaseService service, int? caseId = null)
    {
        _service = service;
        _caseId = caseId;
        InitializeComponent();
        LoadMasters();
        if (_caseId.HasValue)
        {
            LoadCaseForEdit(_caseId.Value);
        }
    }

    private void LoadMasters()
    {
        comboBoxLotNumber.Items.Clear();
        comboBoxLotNumber.Items.AddRange(_service.GetLotNumbers().Cast<object>().ToArray());
        comboBoxModel.Items.Clear();
        foreach (var model in _service.GetActiveProductModels())
        {
            comboBoxModel.Items.Add(new ComboBoxItem<int>(model.DisplayName, model.Id));
        }

        comboBoxProcess.Items.Clear();
        foreach (var process in _service.GetActiveProcesses())
        {
            comboBoxProcess.Items.Add(new ComboBoxItem<int>(process.Name, process.Id));
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
        var inspectionResult = radioButtonOk.Checked ? InspectionResult.Ok : InspectionResult.Ng;
        if (string.IsNullOrWhiteSpace(comboBoxLotNumber.Text))
        {
            MessageBox.Show(this, "ロット番号を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(comboBoxModel.Text))
        {
            MessageBox.Show(this, "型番を選択してください。マスター管理から登録してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (inspectionResult == InspectionResult.Ng && string.IsNullOrWhiteSpace(comboBoxDefectReason.Text))
        {
            MessageBox.Show(this, "NG理由を選択してください。マスター管理から登録してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(comboBoxProcess.Text))
        {
            MessageBox.Show(this, "工程を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var inspectionDate = dateTimePickerInspection.Value.Date;
            var modelId = ResolveModel();
            var processId = ResolveProcess();
            var reasonId = ResolveOptionalReason();
            var actionId = ResolveAction();
            var request = new CreateCaseRequest
            {
                LotNumber = comboBoxLotNumber.Text,
                ProductModelId = modelId,
                ProcessId = processId,
                RegisteredAt = inspectionDate,
                Notes = textBoxNotes.Text,
                InspectionDateTime = inspectionDate,
                Result = inspectionResult,
                DefectReasonId = reasonId,
                DefectDetails = textBoxDefectDetail.Text,
                ActionTypeId = actionId,
                ActionDetails = textBoxActionDetail.Text,
                InspectorName = textBoxInspectorName.Text
            };

            var caseId = _caseId ?? _service.CreateCaseWithInitialInspection(request);
            if (_caseId.HasValue)
            {
                _service.UpdateInitialInspection(caseId, request);
            }
            foreach (var path in _attachmentPaths) _service.AddAttachment(caseId, path);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(this, ex.Message, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("新規NG案件の保存に失敗しました。", ex);
            MessageBox.Show(this, "保存に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private int ResolveModel() => comboBoxModel.SelectedItem is ComboBoxItem<int> item ? item.Value : _service.CreateProductModel(comboBoxModel.Text.Trim(), comboBoxModel.Text.Trim());
    private int ResolveProcess() => comboBoxProcess.SelectedItem is ComboBoxItem<int> item ? item.Value : _service.CreateProcess(comboBoxProcess.Text.Trim());
    private int? ResolveOptionalReason() => string.IsNullOrWhiteSpace(comboBoxDefectReason.Text) ? null : comboBoxDefectReason.SelectedItem is ComboBoxItem<int> item ? item.Value : _service.CreateDefectReason(comboBoxDefectReason.Text.Trim());
    private int? ResolveAction() => string.IsNullOrWhiteSpace(comboBoxAction.Text) ? null : comboBoxAction.SelectedItem is ComboBoxItem<int> item ? item.Value : _service.CreateActionType(comboBoxAction.Text.Trim());

    private void LoadCaseForEdit(int caseId)
    {
        var detail = _service.GetCase(caseId);
        if (detail is null)
        {
            MessageBox.Show(this, "対象案件が見つかりません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
        }

        Text = "内容修正";
        buttonSave.Text = "保存";
        comboBoxLotNumber.Text = detail.LotNumber;
        SelectItem(comboBoxModel, detail.ProductModelId);
        SelectItem(comboBoxProcess, detail.ProcessId);
        textBoxNotes.Text = detail.Notes ?? string.Empty;

        var history = detail.InspectionHistories.OrderBy(x => x.InspectionDateTime).ThenBy(x => x.Id).FirstOrDefault();
        if (history is null)
        {
            return;
        }

        dateTimePickerInspection.Value = history.InspectionDateTime;
        radioButtonNg.Checked = history.Result == InspectionResult.Ng;
        radioButtonOk.Checked = history.Result == InspectionResult.Ok;
        SelectItemByText(comboBoxDefectReason, history.DefectReasonName);
        textBoxDefectDetail.Text = history.DefectDetails ?? string.Empty;
        SelectItemByText(comboBoxAction, history.ActionTypeName);
        textBoxActionDetail.Text = history.ActionDetails ?? string.Empty;
        textBoxInspectorName.Text = history.InspectorName ?? string.Empty;
        labelAttachmentCount.Text = $"添付画像: {_service.GetAttachments(caseId).Count}件（追加可）";
    }

    private static void SelectItem(ComboBox comboBox, int? value)
    {
        if (!value.HasValue) return;
        for (var index = 0; index < comboBox.Items.Count; index++)
        {
            if (comboBox.Items[index] is ComboBoxItem<int> item && item.Value == value.Value)
            {
                comboBox.SelectedIndex = index;
                return;
            }
        }
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

    private void buttonAddAttachment_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Filter = "画像ファイル|*.png;*.jpg;*.jpeg;*.bmp", Multiselect = true };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        _attachmentPaths.AddRange(dialog.FileNames.Where(x => !_attachmentPaths.Contains(x, StringComparer.OrdinalIgnoreCase)));
        labelAttachmentCount.Text = $"添付画像: {_attachmentPaths.Count}件";
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

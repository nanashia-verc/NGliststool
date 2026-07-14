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
        LoadMasters();
    }

    private void LoadMasters()
    {
        listBoxModels.Items.Clear();
        foreach (var model in _service.GetActiveProductModels())
        {
            listBoxModels.Items.Add($"{model.DisplayName} ({model.ModelCode})");
        }

        listBoxDefectReasons.Items.Clear();
        foreach (var reason in _service.GetActiveDefectReasons())
        {
            listBoxDefectReasons.Items.Add(reason.Name);
        }

        listBoxActionTypes.Items.Clear();
        foreach (var action in _service.GetActiveActionTypes())
        {
            listBoxActionTypes.Items.Add(action.Name);
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
}

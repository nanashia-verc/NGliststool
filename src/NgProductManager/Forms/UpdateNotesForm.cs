using NgProductManager.Services;

namespace NgProductManager.Forms;

public partial class UpdateNotesForm : Form
{
    private readonly NgCaseService _service;
    private readonly int _caseId;

    public UpdateNotesForm(NgCaseService service, int caseId)
    {
        _service = service;
        _caseId = caseId;
        InitializeComponent();
        LoadCase();
    }

    private void LoadCase()
    {
        var caseDetail = _service.GetCase(_caseId);
        textBoxNotes.Text = caseDetail?.Notes ?? string.Empty;
    }

    private void buttonSave_Click(object sender, EventArgs e)
    {
        try
        {
            _service.UpdateCaseNotes(_caseId, textBoxNotes.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

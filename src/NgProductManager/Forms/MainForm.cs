using NgProductManager.Models;
using NgProductManager.Services;
using NgProductManager.Utilities;

namespace NgProductManager.Forms;

public partial class MainForm : Form
{
    private readonly NgCaseService _service;
    private readonly BindingSource _bindingSource = new();

    public MainForm()
    {
        InitializeComponent();
        _service = new NgCaseService();
        InitializeUi();
        LoadCases();
    }

    private void InitializeUi()
    {
        Text = "NG品管理ツール";
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleMode = AutoScaleMode.Dpi;

        _bindingSource.DataSource = typeof(List<NgCaseListItem>);
        dataGridViewCases.AutoGenerateColumns = false;
        dataGridViewCases.ReadOnly = true;
        dataGridViewCases.AllowUserToAddRows = false;
        dataGridViewCases.AllowUserToDeleteRows = false;
        dataGridViewCases.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewCases.MultiSelect = false;
        dataGridViewCases.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        dataGridViewCases.Columns.AddRange(
            new DataGridViewTextBoxColumn { DataPropertyName = "StatusText", HeaderText = "状態", Width = 90 },
            new DataGridViewTextBoxColumn { DataPropertyName = "LotNumber", HeaderText = "ロット番号", Width = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = "ProductModelName", HeaderText = "型番", Width = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = "ProcessName", HeaderText = "工程", Width = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = "RegisteredAtText", HeaderText = "初回NG日", Width = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = "LatestInspectionDateTimeText", HeaderText = "最新検査日", Width = 100 },
            new DataGridViewTextBoxColumn { DataPropertyName = "LatestDefectReasonName", HeaderText = "最新NG理由", Width = 120 },
            new DataGridViewTextBoxColumn { DataPropertyName = "LatestActionTypeName", HeaderText = "最新処置", Width = 120 },
            new DataGridViewTextBoxColumn { DataPropertyName = "InspectionHistoryCount", HeaderText = "NG回数", Width = 60 },
            new DataGridViewTextBoxColumn { DataPropertyName = "UpdatedAtText", HeaderText = "更新日時", Width = 120 });

        _bindingSource.DataSource = new List<NgCaseListItem>();
        dataGridViewCases.DataSource = _bindingSource;

    }

    private void LoadCases()
    {
        try
        {
            var criteria = new NgCaseSearchCriteria
            {
                FreeText = textBoxFreeText.Text.Trim(),
                LotNumber = textBoxLotNumber.Text.Trim(),
                ProductModel = textBoxProductModel.Text.Trim(),
                Statuses = GetSelectedStatuses()
            };

            var cases = _service.SearchCases(criteria, includeClosed: true);
            var rows = cases.Select(item => new NgCaseRowViewModel(item)).ToList();
            _bindingSource.DataSource = rows;
            dataGridViewCases.Refresh();
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("一覧の読み込みに失敗しました。", ex);
            MessageBox.Show(this, "一覧の読み込みに失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private IReadOnlyCollection<NgCaseStatus>? GetSelectedStatuses()
    {
        var statuses = new List<NgCaseStatus>();
        if (checkBoxInProgressOnly.Checked)
        {
            statuses.Add(NgCaseStatus.InProgress);
        }

        if (checkBoxPendingOnly.Checked)
        {
            statuses.Add(NgCaseStatus.PendingReinspection);
        }

        return statuses.Count == 0 ? null : statuses;
    }

    private void buttonSearch_Click(object sender, EventArgs e) => LoadCases();

    private void buttonClear_Click(object sender, EventArgs e)
    {
        textBoxFreeText.Clear();
        textBoxLotNumber.Clear();
        textBoxProductModel.Clear();
        checkBoxInProgressOnly.Checked = false;
        checkBoxPendingOnly.Checked = false;
        LoadCases();
    }

    private void buttonNewCase_Click(object sender, EventArgs e)
    {
        using var dialog = new NewNgCaseForm(_service);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadCases();
        }
    }

    private void buttonShowDetail_Click(object sender, EventArgs e)
    {
        if (dataGridViewCases.CurrentRow?.DataBoundItem is NgCaseRowViewModel row)
        {
            using var dialog = new NgCaseDetailForm(_service, row.Id);
            dialog.ShowDialog(this);
            LoadCases();
        }
        else
        {
            MessageBox.Show(this, "案件を選択してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void buttonReinspection_Click(object sender, EventArgs e)
    {
        if (dataGridViewCases.CurrentRow?.DataBoundItem is NgCaseRowViewModel row)
        {
            using var dialog = new AddInspectionHistoryForm(_service, row.Id);
            dialog.ShowDialog(this);
            LoadCases();
        }
        else
        {
            MessageBox.Show(this, "案件を選択してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void buttonCloseCase_Click(object sender, EventArgs e)
    {
        if (dataGridViewCases.CurrentRow?.DataBoundItem is not NgCaseRowViewModel row)
        {
            MessageBox.Show(this, "案件を選択してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = MessageBox.Show(this, $"案件ID {row.Id} をクローズしますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _service.CloseCase(row.Id);
            LoadCases();
            MessageBox.Show(this, "案件をクローズしました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("案件のクローズに失敗しました。", ex);
            MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buttonMaster_Click(object sender, EventArgs e)
    {
        using var dialog = new MasterManagementForm(_service);
        dialog.ShowDialog(this);
    }

    private void buttonCsv_Click(object sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog { Filter = "CSVファイル (*.csv)|*.csv", FileName = "ng-cases.csv" };
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            var criteria = new NgCaseSearchCriteria
            {
                FreeText = textBoxFreeText.Text.Trim(),
                LotNumber = textBoxLotNumber.Text.Trim(),
                ProductModel = textBoxProductModel.Text.Trim(),
                Statuses = GetSelectedStatuses()
            };
            _service.ExportCsv(criteria, dialog.FileName, includeClosed: true);
            MessageBox.Show(this, "CSVを出力しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("CSV出力に失敗しました。", ex);
            MessageBox.Show(this, "CSV出力に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buttonReload_Click(object sender, EventArgs e) => LoadCases();

    private void buttonBackup_Click(object sender, EventArgs e)
    {
        try
        {
            var path = _service.CreateBackup();
            MessageBox.Show(this, $"バックアップを作成しました。{Environment.NewLine}{path}", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("バックアップに失敗しました。", ex);
            MessageBox.Show(this, "バックアップに失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buttonRestore_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "バックアップファイル (*.db)|*.db|すべてのファイル (*.*)|*.*",
            InitialDirectory = Directory.Exists(ApplicationPaths.BackupDirectory) ? ApplicationPaths.BackupDirectory : null
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        if (MessageBox.Show(this, "現在のデータは選択したバックアップで置き換えられます。\n復元前に現在のデータをバックアップすることをおすすめします。\n復元しますか？", "復元確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

        try
        {
            _service.RestoreBackup(dialog.FileName);
            LoadCases();
            MessageBox.Show(this, "バックアップを復元しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("バックアップの復元に失敗しました。", ex);
            MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void dataGridViewCases_DoubleClick(object sender, EventArgs e)
    {
        if (dataGridViewCases.CurrentRow?.DataBoundItem is NgCaseRowViewModel row)
        {
            using var dialog = new NgCaseDetailForm(_service, row.Id);
            dialog.ShowDialog(this);
            LoadCases();
        }
    }

    private sealed class NgCaseRowViewModel
    {
        public NgCaseRowViewModel(NgCaseListItem item)
        {
            Id = item.Id;
            StatusText = item.Status switch
            {
                NgCaseStatus.InProgress => "対応中",
                NgCaseStatus.PendingReinspection => "再検査待ち",
                NgCaseStatus.Closed => "クローズ済み",
                _ => string.Empty
            };
            LotNumber = item.LotNumber;
            ProductModelName = item.ProductModelName;
            ProcessName = item.ProcessName;
            RegisteredAtText = item.RegisteredAt.ToString("yyyy/MM/dd");
            LatestInspectionDateTimeText = item.LatestInspectionDateTime?.ToString("yyyy/MM/dd") ?? string.Empty;
            LatestDefectReasonName = item.LatestDefectReasonName;
            LatestActionTypeName = item.LatestActionTypeName;
            InspectionHistoryCount = item.InspectionHistoryCount;
            UpdatedAtText = item.UpdatedAt.ToString("yyyy/MM/dd HH:mm");
        }

        public int Id { get; }
        public string StatusText { get; }
        public string LotNumber { get; }
        public string ProductModelName { get; }
        public string? ProcessName { get; }
        public string RegisteredAtText { get; }
        public string LatestInspectionDateTimeText { get; }
        public string? LatestDefectReasonName { get; }
        public string? LatestActionTypeName { get; }
        public int InspectionHistoryCount { get; }
        public string UpdatedAtText { get; }
    }
}

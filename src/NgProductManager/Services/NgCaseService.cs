using Microsoft.Data.Sqlite;
using NgProductManager.Data;
using NgProductManager.Models;
using NgProductManager.Utilities;

namespace NgProductManager.Services;

public sealed class NgCaseService
{
    private readonly DatabaseManager _databaseManager;
    private readonly NgCaseRepository _repository;
    private readonly string _databasePath;

    public NgCaseService(string? databasePath = null)
    {
        _databasePath = databasePath ?? ApplicationPaths.DatabasePath;
        _databaseManager = new DatabaseManager(_databasePath);
        _repository = new NgCaseRepository(_databaseManager);
        EnsureWritableDatabasePath();
        _databaseManager.EnsureDatabaseCreated();
        InitializeSeedData();
    }

    public string DatabasePath => _databasePath;

    public int CreateProductModel(string modelCode, string displayName)
    {
        if (string.IsNullOrWhiteSpace(modelCode) || string.IsNullOrWhiteSpace(displayName))
        {
            throw new InvalidOperationException("型番コードと表示名は必須です。");
        }

        try
        {
            if (_repository.ExistsProductModelCode(modelCode.Trim()))
            {
                throw new InvalidOperationException("既に登録済みの型番コードです。");
            }

            var now = DateTime.Now;
            return _repository.InsertProductModel(new ProductModelMaster
            {
                ModelCode = modelCode.Trim(),
                DisplayName = displayName.Trim(),
                IsActive = 1,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            AppLogger.WriteError("型番マスター登録に失敗しました。", ex);
            throw new InvalidOperationException("型番マスター登録に失敗しました。", ex);
        }
    }

    public void SetProductModelActive(int id, bool isActive)
    {
        _repository.SetProductModelActive(id, isActive);
    }

    public List<ProductModelMaster> GetActiveProductModels() => _repository.GetProductModels(activeOnly: true);
    public List<ProductModelMaster> GetProductModels() => _repository.GetProductModels();
    public void UpdateProductModel(int id, string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("型番コードと名称を入力してください。");
        if (_repository.ExistsProductModelCode(code.Trim(), id)) throw new InvalidOperationException("既に登録済みの型番コードです。");
        _repository.UpdateProductModel(id, code.Trim(), name.Trim());
    }
    public void DeleteProductModel(int id) => _repository.SetProductModelActive(id, false);

    public int CreateProcess(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("工程名は必須です。");
        using var connection = _databaseManager.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT OR IGNORE INTO Processes (Name, IsActive, SortOrder, CreatedAt, UpdatedAt) VALUES (@Name, 1, 0, @Now, @Now); SELECT Id FROM Processes WHERE Name = @Name;";
        command.Parameters.AddWithValue("@Name", name.Trim()); command.Parameters.AddWithValue("@Now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public List<ProcessMaster> GetActiveProcesses()
    {
        using var connection = _databaseManager.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, IsActive, SortOrder, CreatedAt, UpdatedAt FROM Processes WHERE IsActive = 1 ORDER BY SortOrder, Name";
        using var reader = command.ExecuteReader();
        var items = new List<ProcessMaster>();
        while (reader.Read()) items.Add(new ProcessMaster { Id = reader.GetInt32(0), Name = reader.GetString(1), IsActive = reader.GetInt32(2), SortOrder = reader.GetInt32(3), CreatedAt = DateTime.Parse(reader.GetString(4)), UpdatedAt = DateTime.Parse(reader.GetString(5)) });
        return items;
    }

    public void UpdateProcess(int id, string name) { if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("工程名を入力してください。"); _repository.UpdateProcess(id, name.Trim()); }
    public void DeleteProcess(int id) => _repository.SetProcessActive(id, false);

    public List<string> GetLotNumbers() => _repository.GetLotNumbers();

    public int CreateDefectReason(string name, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("NG理由名は必須です。");
        }

        var normalizedName = name.Trim();
        if (_repository.ExistsDefectReason(normalizedName))
        {
            return _repository.GetDefectReasons().First(x => x.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase)).Id;
        }

        var now = DateTime.Now;
        return _repository.InsertDefectReason(new DefectReasonMaster
        {
            Name = normalizedName,
            IsActive = 1,
            SortOrder = sortOrder,
            CreatedAt = now,
            UpdatedAt = now
        });
    }

    public int CreateActionType(string name, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("処置内容名は必須です。");
        }

        var normalizedName = name.Trim();
        if (_repository.ExistsActionType(normalizedName))
        {
            return _repository.GetActionTypes().First(x => x.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase)).Id;
        }

        var now = DateTime.Now;
        return _repository.InsertActionType(new ActionTypeMaster
        {
            Name = normalizedName,
            IsActive = 1,
            SortOrder = sortOrder,
            CreatedAt = now,
            UpdatedAt = now
        });
    }

    public int CreateCaseWithInitialInspection(CreateCaseRequest request)
    {
        ValidateCreateCaseRequest(request);

        try
        {
            using var connection = _databaseManager.OpenConnection();
            using var transaction = connection.BeginTransaction();
            var now = DateTime.Now;
            var ngCase = new NgCase
            {
                LotNumber = request.LotNumber.Trim(),
                ProductModelId = request.ProductModelId,
                ProcessId = request.ProcessId,
                SerialNumber = string.IsNullOrWhiteSpace(request.SerialNumber) ? null : request.SerialNumber.Trim(),
                Status = (int)NgCaseStatus.InProgress,
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
                RegisteredAt = request.RegisteredAt,
                CreatedAt = now,
                UpdatedAt = now
            };

            var caseId = _repository.InsertNgCase(ngCase, connection, transaction);
            var history = new InspectionHistory
            {
                NgCaseId = caseId,
                InspectionDateTime = request.InspectionDateTime,
                Result = (int)request.Result,
                DefectReasonId = request.DefectReasonId,
                DefectDetails = string.IsNullOrWhiteSpace(request.DefectDetails) ? null : request.DefectDetails.Trim(),
                ActionTypeId = request.ActionTypeId,
                ActionDetails = string.IsNullOrWhiteSpace(request.ActionDetails) ? null : request.ActionDetails.Trim(),
                InspectorName = string.IsNullOrWhiteSpace(request.InspectorName) ? null : request.InspectorName.Trim(),
                CreatedAt = now
            };
            _repository.InsertInspectionHistory(history, connection, transaction);

            if (request.Result == InspectionResult.Ok)
            {
                _repository.UpdateNgCaseStatus(caseId, NgCaseStatus.Closed, now, ngCase.Notes, updatedAt: now, connection: connection, transaction: transaction);
            }

            transaction.Commit();
            return caseId;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            AppLogger.WriteError("初回NG案件登録に失敗しました。", ex);
            throw new InvalidOperationException("初回NG案件登録に失敗しました。", ex);
        }
    }

    public void AddInspectionHistory(int ngCaseId, InspectionHistoryInput input)
    {
        if (!_repository.CaseExists(ngCaseId))
        {
            throw new InvalidOperationException("対象案件が見つかりません。", new InvalidOperationException());
        }

        ValidateInspectionHistoryInput(input);

        try
        {
            using var connection = _databaseManager.OpenConnection();
            using var transaction = connection.BeginTransaction();
            var now = DateTime.Now;
            var history = new InspectionHistory
            {
                NgCaseId = ngCaseId,
                InspectionDateTime = input.InspectionDateTime,
                Result = (int)input.Result,
                DefectReasonId = input.DefectReasonId,
                DefectDetails = string.IsNullOrWhiteSpace(input.DefectDetails) ? null : input.DefectDetails.Trim(),
                ActionTypeId = input.ActionTypeId,
                ActionDetails = string.IsNullOrWhiteSpace(input.ActionDetails) ? null : input.ActionDetails.Trim(),
                InspectorName = string.IsNullOrWhiteSpace(input.InspectorName) ? null : input.InspectorName.Trim(),
                CreatedAt = now
            };
            _repository.InsertInspectionHistory(history, connection, transaction);

            var nextStatus = input.Result == InspectionResult.Ok ? NgCaseStatus.Closed : input.NextStatus;
            var closedAt = input.Result == InspectionResult.Ok ? now : (DateTime?)null;
            var existingCase = _repository.GetNgCase(ngCaseId, connection, transaction);
            _repository.UpdateNgCaseStatus(ngCaseId, nextStatus, closedAt, existingCase?.Notes, updatedAt: now, connection: connection, transaction: transaction);
            transaction.Commit();
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            AppLogger.WriteError("再検査履歴の追加に失敗しました。", ex);
            throw new InvalidOperationException("再検査履歴の追加に失敗しました。", ex);
        }
    }

    public void CloseCase(int caseId)
    {
        if (!_repository.CaseExists(caseId))
        {
            throw new InvalidOperationException("対象案件が見つかりません。", new InvalidOperationException());
        }

        try
        {
            using var connection = _databaseManager.OpenConnection();
            using var transaction = connection.BeginTransaction();
            var existingCase = _repository.GetNgCase(caseId, connection, transaction);
            if (existingCase is null)
            {
                throw new InvalidOperationException("対象案件が見つかりません。", new InvalidOperationException());
            }

            if (existingCase.Status == (int)NgCaseStatus.Closed)
            {
                throw new InvalidOperationException("この案件は既にクローズ済みです。", new InvalidOperationException());
            }

            var now = DateTime.Now;
            _repository.UpdateNgCaseStatus(caseId, NgCaseStatus.Closed, now, existingCase.Notes, updatedAt: now, connection: connection, transaction: transaction);
            transaction.Commit();
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            AppLogger.WriteError("案件のクローズに失敗しました。", ex);
            throw new InvalidOperationException("案件のクローズに失敗しました。", ex);
        }
    }

    public NgCaseDetail? GetCase(int id)
    {
        var ngCase = _repository.GetNgCase(id);
        if (ngCase is null)
        {
            return null;
        }

        var productModel = _repository.GetProductModels().FirstOrDefault(x => x.Id == ngCase.ProductModelId);
        var process = ngCase.ProcessId.HasValue ? GetActiveProcesses().FirstOrDefault(x => x.Id == ngCase.ProcessId.Value) : null;
        var histories = _repository.GetInspectionHistories(id).Select(history => new InspectionHistoryDetail
        {
            Id = history.Id,
            InspectionDateTime = history.InspectionDateTime,
            Result = (InspectionResult)history.Result,
            DefectReasonName = history.DefectReasonId.HasValue ? _repository.GetDefectReasons().FirstOrDefault(x => x.Id == history.DefectReasonId)?.Name : null,
            DefectDetails = history.DefectDetails,
            ActionTypeName = history.ActionTypeId.HasValue ? _repository.GetActionTypes().FirstOrDefault(x => x.Id == history.ActionTypeId)?.Name : null,
            ActionDetails = history.ActionDetails,
            InspectorName = history.InspectorName,
            CreatedAt = history.CreatedAt
        }).ToList();

        return new NgCaseDetail
        {
            Id = ngCase.Id,
            LotNumber = ngCase.LotNumber,
            ProductModelId = ngCase.ProductModelId,
            ProcessId = ngCase.ProcessId,
            ProductModelName = productModel?.DisplayName ?? string.Empty,
            ProcessName = process?.Name,
            SerialNumber = ngCase.SerialNumber,
            Status = (NgCaseStatus)ngCase.Status,
            Notes = ngCase.Notes,
            RegisteredAt = ngCase.RegisteredAt,
            ClosedAt = ngCase.ClosedAt,
            CreatedAt = ngCase.CreatedAt,
            UpdatedAt = ngCase.UpdatedAt,
            InspectionHistories = histories
        };
    }

    public List<NgCaseListItem> SearchCases(NgCaseSearchCriteria criteria, bool includeClosed = false)
    {
        return _repository.SearchCases(criteria, includeClosed);
    }

    public void UpdateDefectReason(int id, string name) { if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("NG理由名を入力してください。"); if (_repository.ExistsDefectReason(name.Trim(), id)) throw new InvalidOperationException("既に登録済みのNG理由名です。"); _repository.UpdateDefectReason(id, name.Trim()); }
    public void UpdateActionType(int id, string name) { if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("処置内容名を入力してください。"); if (_repository.ExistsActionType(name.Trim(), id)) throw new InvalidOperationException("既に登録済みの処置内容名です。"); _repository.UpdateActionType(id, name.Trim()); }
    public void DeleteDefectReason(int id) => _repository.SetDefectReasonActive(id, false);
    public void DeleteActionType(int id) => _repository.SetActionTypeActive(id, false);

    public void UpdateInspectionHistory(int ngCaseId, int historyId, InspectionHistoryInput input)
    {
        if (!_repository.CaseExists(ngCaseId)) throw new InvalidOperationException("対象案件が見つかりません。");
        ValidateInspectionHistoryInput(input);

        var history = _repository.GetInspectionHistories(ngCaseId).FirstOrDefault(x => x.Id == historyId);
        if (history is null) throw new InvalidOperationException("対象の検査履歴が見つかりません。");

        history.InspectionDateTime = input.InspectionDateTime;
        history.Result = (int)input.Result;
        history.DefectReasonId = input.DefectReasonId;
        history.DefectDetails = string.IsNullOrWhiteSpace(input.DefectDetails) ? null : input.DefectDetails.Trim();
        history.ActionTypeId = input.ActionTypeId;
        history.ActionDetails = string.IsNullOrWhiteSpace(input.ActionDetails) ? null : input.ActionDetails.Trim();
        history.InspectorName = string.IsNullOrWhiteSpace(input.InspectorName) ? null : input.InspectorName.Trim();
        _repository.UpdateInspectionHistory(historyId, history);
    }

    public void DeleteCase(int caseId)
    {
        if (!_repository.CaseExists(caseId)) throw new InvalidOperationException("対象案件が見つかりません。");
        using var connection = _databaseManager.OpenConnection(); using var transaction = connection.BeginTransaction();
        _repository.DeleteCase(caseId, connection, transaction);
        transaction.Commit();
    }

    public int AddAttachment(int caseId, string filePath)
    {
        if (!_repository.CaseExists(caseId)) throw new InvalidOperationException("対象案件が見つかりません。");
        if (!File.Exists(filePath)) throw new InvalidOperationException("添付ファイルが見つかりません。");
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (extension is not ".png" and not ".jpg" and not ".jpeg" and not ".bmp") throw new InvalidOperationException("画像ファイルを選択してください。");
        return _repository.InsertAttachment(new CaseAttachment { NgCaseId = caseId, FileName = Path.GetFileName(filePath), ContentType = extension, Content = File.ReadAllBytes(filePath), CreatedAt = DateTime.Now });
    }

    public List<CaseAttachment> GetAttachments(int caseId) => _repository.GetAttachments(caseId);

    public void UpdateCaseNotes(int caseId, string notes)
    {
        var ngCase = _repository.GetNgCase(caseId);
        if (ngCase is null)
        {
            throw new InvalidOperationException("対象案件が見つかりません。", new InvalidOperationException());
        }

        _repository.UpdateNgCaseStatus(caseId, (NgCaseStatus)ngCase.Status, ngCase.ClosedAt, notes);
    }

    public void UpdateCaseBasics(int caseId, string lotNumber, int productModelId, int? processId, string? notes)
    {
        if (string.IsNullOrWhiteSpace(lotNumber)) throw new InvalidOperationException("ロット番号は必須です。");
        if (!_repository.CaseExists(caseId)) throw new InvalidOperationException("対象案件が見つかりません。");
        _repository.UpdateCaseBasics(caseId, lotNumber.Trim(), productModelId, processId, notes);
    }

    public void UpdateInspectionHistoryText(int historyId, string? defectDetails, string? actionDetails, string? inspectorName) => _repository.UpdateInspectionHistoryText(historyId, defectDetails, actionDetails, inspectorName);

    public void UpdateInitialInspection(int caseId, CreateCaseRequest request)
    {
        ValidateCreateCaseRequest(request);

        if (!_repository.CaseExists(caseId))
        {
            throw new InvalidOperationException("対象案件が見つかりません。");
        }

        var initialHistory = _repository.GetInspectionHistories(caseId).FirstOrDefault();
        if (initialHistory is null)
        {
            throw new InvalidOperationException("初回検査履歴が見つかりません。");
        }

        _repository.UpdateCaseBasics(caseId, request.LotNumber.Trim(), request.ProductModelId, request.ProcessId, request.Notes, request.RegisteredAt);
        initialHistory.InspectionDateTime = request.InspectionDateTime;
        initialHistory.Result = (int)request.Result;
        initialHistory.DefectReasonId = request.DefectReasonId;
        initialHistory.DefectDetails = string.IsNullOrWhiteSpace(request.DefectDetails) ? null : request.DefectDetails.Trim();
        initialHistory.ActionTypeId = request.ActionTypeId;
        initialHistory.ActionDetails = string.IsNullOrWhiteSpace(request.ActionDetails) ? null : request.ActionDetails.Trim();
        initialHistory.InspectorName = string.IsNullOrWhiteSpace(request.InspectorName) ? null : request.InspectorName.Trim();
        _repository.UpdateInspectionHistory(initialHistory.Id, initialHistory);
    }

    public List<DefectReasonMaster> GetActiveDefectReasons() => _repository.GetDefectReasons(activeOnly: true);

    public List<ActionTypeMaster> GetActiveActionTypes() => _repository.GetActionTypes(activeOnly: true);

    public string CreateBackup(string? outputPath = null)
    {
        var backupDirectory = ApplicationPaths.EnsureDirectory(ApplicationPaths.BackupDirectory);
        var fileName = $"ng-manager_{DateTime.Now:yyyyMMdd_HHmmss}.db";
        var destinationPath = outputPath ?? Path.Combine(backupDirectory, fileName);
        try
        {
            using var source = new SqliteConnection($"Data Source={_databasePath}");
            source.Open();
            using var destination = new SqliteConnection($"Data Source={destinationPath}");
            destination.Open();
            source.BackupDatabase(destination);
            return destinationPath;
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("バックアップに失敗しました。", ex);
            throw new InvalidOperationException("バックアップに失敗しました。", ex);
        }
    }

    public void RestoreBackup(string backupPath)
    {
        if (string.IsNullOrWhiteSpace(backupPath) || !File.Exists(backupPath))
        {
            throw new InvalidOperationException("復元するバックアップファイルが見つかりません。");
        }

        if (string.Equals(Path.GetFullPath(backupPath), Path.GetFullPath(_databasePath), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("現在使用中のデータベースは復元元に選択できません。");
        }

        try
        {
            using var source = new SqliteConnection($"Data Source={backupPath}");
            source.Open();
            using var validation = source.CreateCommand();
            validation.CommandText = "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = 'NgCases';";
            if (validation.ExecuteScalar() is null)
            {
                throw new InvalidOperationException("NG品管理ツールのバックアップファイルではありません。");
            }

            SqliteConnection.ClearAllPools();
            using var destination = new SqliteConnection($"Data Source={_databasePath}");
            destination.Open();
            source.BackupDatabase(destination);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            AppLogger.WriteError("バックアップの復元に失敗しました。", ex);
            throw new InvalidOperationException("バックアップの復元に失敗しました。", ex);
        }
    }

    public void ExportCsv(NgCaseSearchCriteria criteria, string outputPath, bool includeClosed = false)
    {
        try
        {
            var cases = SearchCases(criteria, includeClosed);
            using var writer = new StreamWriter(outputPath, false, new System.Text.UTF8Encoding(true));
            writer.WriteLine("案件ID,状態,ロット番号,型番,工程,初回NG日,最新検査日,最新NG理由,最新処置,NG回数,登録日,クローズ日");
            foreach (var item in cases)
            {
                var ngCase = GetCase(item.Id);
                var initialHistory = ngCase?.InspectionHistories.FirstOrDefault();
                var latestHistory = ngCase?.InspectionHistories.LastOrDefault();
                writer.WriteLine(string.Join(",",
                    EscapeCsv(item.Id.ToString()),
                    EscapeCsv(item.Status.ToString()),
                    EscapeCsv(item.LotNumber),
                    EscapeCsv(item.ProductModelName),
                    EscapeCsv(item.ProcessName ?? string.Empty),
                    EscapeCsv(FormatCsvDate(initialHistory?.InspectionDateTime)),
                    EscapeCsv(FormatCsvDate(latestHistory?.InspectionDateTime)),
                    EscapeCsv(latestHistory?.DefectReasonName ?? string.Empty),
                    EscapeCsv(latestHistory?.ActionTypeName ?? string.Empty),
                    EscapeCsv(item.InspectionHistoryCount.ToString()),
                    EscapeCsv(FormatCsvDate(item.RegisteredAt)),
                    EscapeCsv(FormatCsvDate(item.ClosedAt))));
            }
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("CSV出力に失敗しました。", ex);
            throw new InvalidOperationException("CSV出力に失敗しました。", ex);
        }
    }

    private static string EscapeCsv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";

    // Excel が日付を数値として扱い、列幅不足で #### と表示するのを防ぐため、日付列は文字列として出力する。
    private static string FormatCsvDate(DateTime? value) => value.HasValue ? $"\u200B{value.Value:yyyy/MM/dd}" : string.Empty;

    private void EnsureWritableDatabasePath()
    {
        try
        {
            var directory = Path.GetDirectoryName(_databasePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                ApplicationPaths.EnsureDirectory(directory);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("データ保存先のフォルダを作成できません。アクセス権を確認してください。", ex);
        }
    }

    private void InitializeSeedData()
    {
        try
        {
            if (_repository.GetDefectReasons().Count == 0)
            {
                foreach (var name in new[] { "寸法不良", "傷", "変形", "外観不良", "動作不良", "その他" })
                {
                    CreateDefectReason(name);
                }
            }

            if (_repository.GetActionTypes().Count == 0)
            {
                foreach (var name in new[] { "再加工", "修正", "部品交換", "再検査", "廃棄", "保留", "その他" })
                {
                    CreateActionType(name);
                }
            }
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("初期マスターデータ登録に失敗しました。", ex);
        }
    }

    private void ValidateCreateCaseRequest(CreateCaseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LotNumber))
        {
            throw new InvalidOperationException("ロット番号は必須です。", new InvalidOperationException());
        }

        if (request.ProductModelId <= 0)
        {
            throw new InvalidOperationException("型番を選択してください。マスター管理から登録してください。", new InvalidOperationException());
        }

        if (!_repository.GetProductModels(activeOnly: true).Any(x => x.Id == request.ProductModelId))
        {
            throw new InvalidOperationException("型番マスターが見つかりません。マスター管理から登録してください。", new InvalidOperationException());
        }

        if (request.InspectionDateTime == default)
        {
            throw new InvalidOperationException("NG日は必須です。", new InvalidOperationException());
        }

        if (request.Result == InspectionResult.Ng)
        {
            if (request.DefectReasonId is null || !_repository.GetDefectReasons(activeOnly: true).Any(x => x.Id == request.DefectReasonId.Value))
            {
                throw new InvalidOperationException("NG理由を選択してください。マスター管理から登録してください。", new InvalidOperationException());
            }

        }
    }

    private void ValidateInspectionHistoryInput(InspectionHistoryInput input)
    {
        if (input.Result == InspectionResult.Ng)
        {
            if (input.DefectReasonId is null || !_repository.GetDefectReasons(activeOnly: true).Any(x => x.Id == input.DefectReasonId.Value))
            {
                throw new InvalidOperationException("NG理由を選択してください。マスター管理から登録してください。", new InvalidOperationException());
            }

        }
    }
}

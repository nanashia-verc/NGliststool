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
            ProductModelName = productModel?.DisplayName ?? string.Empty,
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

    public void UpdateCaseNotes(int caseId, string notes)
    {
        var ngCase = _repository.GetNgCase(caseId);
        if (ngCase is null)
        {
            throw new InvalidOperationException("対象案件が見つかりません。", new InvalidOperationException());
        }

        _repository.UpdateNgCaseStatus(caseId, (NgCaseStatus)ngCase.Status, ngCase.ClosedAt, notes);
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

    public void ExportCsv(NgCaseSearchCriteria criteria, string outputPath, bool includeClosed = false)
    {
        try
        {
            var cases = SearchCases(criteria, includeClosed);
            using var writer = new StreamWriter(outputPath, false, new System.Text.UTF8Encoding(true));
            writer.WriteLine("案件ID,状態,ロット番号,型番,初回NG日,最新検査日,最新NG理由,最新処置,NG回数,登録日,クローズ日");
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
                    EscapeCsv(initialHistory?.InspectionDateTime.ToString("yyyy-MM-dd") ?? string.Empty),
                    EscapeCsv(latestHistory?.InspectionDateTime.ToString("yyyy-MM-dd") ?? string.Empty),
                    EscapeCsv(latestHistory?.DefectReasonName ?? string.Empty),
                    EscapeCsv(latestHistory?.ActionTypeName ?? string.Empty),
                    EscapeCsv(item.InspectionHistoryCount.ToString()),
                    EscapeCsv(item.RegisteredAt.ToString("yyyy-MM-dd")),
                    EscapeCsv(item.ClosedAt?.ToString("yyyy-MM-dd") ?? string.Empty)));
            }
        }
        catch (Exception ex)
        {
            AppLogger.WriteError("CSV出力に失敗しました。", ex);
            throw new InvalidOperationException("CSV出力に失敗しました。", ex);
        }
    }

    private static string EscapeCsv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";

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

            if (request.ActionTypeId is null || !_repository.GetActionTypes(activeOnly: true).Any(x => x.Id == request.ActionTypeId.Value))
            {
                throw new InvalidOperationException("処置内容を選択してください。マスター管理から登録してください。", new InvalidOperationException());
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

            if (input.ActionTypeId is null || !_repository.GetActionTypes(activeOnly: true).Any(x => x.Id == input.ActionTypeId.Value))
            {
                throw new InvalidOperationException("処置内容を選択してください。マスター管理から登録してください。", new InvalidOperationException());
            }
        }
    }
}

using Microsoft.Data.Sqlite;
using NgProductManager.Models;
using NgProductManager.Utilities;

namespace NgProductManager.Data;

public sealed class NgCaseRepository
{
    private readonly DatabaseManager _databaseManager;

    public NgCaseRepository(DatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public int InsertProductModel(ProductModelMaster model, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            command.CommandText = @"
INSERT INTO ProductModels (ModelCode, DisplayName, IsActive, CreatedAt, UpdatedAt)
VALUES (@ModelCode, @DisplayName, @IsActive, @CreatedAt, @UpdatedAt);
SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@ModelCode", model.ModelCode);
            command.Parameters.AddWithValue("@DisplayName", model.DisplayName);
            command.Parameters.AddWithValue("@IsActive", model.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", model.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@UpdatedAt", model.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            var scalar = command.ExecuteScalar();
            return Convert.ToInt32(scalar);
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public bool ExistsProductModelCode(string modelCode, int? excludeId = null, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            var sql = "SELECT 1 FROM ProductModels WHERE ModelCode = @ModelCode";
            if (excludeId.HasValue)
            {
                sql += " AND Id <> @ExcludeId";
            }

            command.CommandText = sql;
            command.Parameters.AddWithValue("@ModelCode", modelCode);
            if (excludeId.HasValue)
            {
                command.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
            }

            return command.ExecuteScalar() is not null;
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public int InsertNgCase(NgCase ngCase, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            command.CommandText = @"
INSERT INTO NgCases (LotNumber, ProductModelId, ProcessId, SerialNumber, Status, Notes, RegisteredAt, ClosedAt, CreatedAt, UpdatedAt)
VALUES (@LotNumber, @ProductModelId, @ProcessId, @SerialNumber, @Status, @Notes, @RegisteredAt, @ClosedAt, @CreatedAt, @UpdatedAt);
SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@LotNumber", ngCase.LotNumber);
            command.Parameters.AddWithValue("@ProductModelId", ngCase.ProductModelId);
            command.Parameters.AddWithValue("@ProcessId", (object?)ngCase.ProcessId ?? DBNull.Value);
            command.Parameters.AddWithValue("@SerialNumber", (object?)ngCase.SerialNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", ngCase.Status);
            command.Parameters.AddWithValue("@Notes", (object?)ngCase.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@RegisteredAt", ngCase.RegisteredAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@ClosedAt", (object?)ngCase.ClosedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", ngCase.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@UpdatedAt", ngCase.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            var id = Convert.ToInt32(command.ExecuteScalar());
            return id;
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public int InsertInspectionHistory(InspectionHistory history, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            command.CommandText = @"
INSERT INTO InspectionHistories (NgCaseId, InspectionDateTime, Result, DefectReasonId, DefectDetails, ActionTypeId, ActionDetails, InspectorName, CreatedAt)
VALUES (@NgCaseId, @InspectionDateTime, @Result, @DefectReasonId, @DefectDetails, @ActionTypeId, @ActionDetails, @InspectorName, @CreatedAt);
SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@NgCaseId", history.NgCaseId);
            command.Parameters.AddWithValue("@InspectionDateTime", history.InspectionDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@Result", history.Result);
            command.Parameters.AddWithValue("@DefectReasonId", (object?)history.DefectReasonId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefectDetails", (object?)history.DefectDetails ?? DBNull.Value);
            command.Parameters.AddWithValue("@ActionTypeId", (object?)history.ActionTypeId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ActionDetails", (object?)history.ActionDetails ?? DBNull.Value);
            command.Parameters.AddWithValue("@InspectorName", (object?)history.InspectorName ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", history.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            return Convert.ToInt32(command.ExecuteScalar());
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public void UpdateNgCaseStatus(int caseId, NgCaseStatus status, DateTime? closedAt, string? notes, DateTime? updatedAt = null, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            command.CommandText = @"
UPDATE NgCases
SET Status = @Status,
    ClosedAt = @ClosedAt,
    Notes = @Notes,
    UpdatedAt = @UpdatedAt
WHERE Id = @Id;";
            command.Parameters.AddWithValue("@Status", (int)status);
            command.Parameters.AddWithValue("@ClosedAt", (object?)closedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (updatedAt ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@Id", caseId);
            command.ExecuteNonQuery();
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public NgCase? GetNgCase(int caseId, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            command.CommandText = "SELECT Id, LotNumber, ProductModelId, ProcessId, SerialNumber, Status, Notes, RegisteredAt, ClosedAt, CreatedAt, UpdatedAt FROM NgCases WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", caseId);
            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new NgCase
            {
                Id = reader.GetInt32(0),
                LotNumber = reader.GetString(1),
                ProductModelId = reader.GetInt32(2),
                ProcessId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                SerialNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                Status = reader.GetInt32(5),
                Notes = reader.IsDBNull(6) ? null : reader.GetString(6),
                RegisteredAt = DateTime.Parse(reader.GetString(7)),
                ClosedAt = reader.IsDBNull(8) ? null : DateTime.Parse(reader.GetString(8)),
                CreatedAt = DateTime.Parse(reader.GetString(9)),
                UpdatedAt = DateTime.Parse(reader.GetString(10))
            };
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public List<InspectionHistory> GetInspectionHistories(int ngCaseId, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            command.CommandText = @"
SELECT Id, NgCaseId, InspectionDateTime, Result, DefectReasonId, DefectDetails, ActionTypeId, ActionDetails, InspectorName, CreatedAt
FROM InspectionHistories
WHERE NgCaseId = @NgCaseId
ORDER BY InspectionDateTime ASC, Id ASC;";
            command.Parameters.AddWithValue("@NgCaseId", ngCaseId);
            using var reader = command.ExecuteReader();
            var histories = new List<InspectionHistory>();
            while (reader.Read())
            {
                histories.Add(new InspectionHistory
                {
                    Id = reader.GetInt32(0),
                    NgCaseId = reader.GetInt32(1),
                    InspectionDateTime = DateTime.Parse(reader.GetString(2)),
                    Result = reader.GetInt32(3),
                    DefectReasonId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    DefectDetails = reader.IsDBNull(5) ? null : reader.GetString(5),
                    ActionTypeId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                    ActionDetails = reader.IsDBNull(7) ? null : reader.GetString(7),
                    InspectorName = reader.IsDBNull(8) ? null : reader.GetString(8),
                    CreatedAt = DateTime.Parse(reader.GetString(9))
                });
            }

            return histories;
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public int InsertDefectReason(DefectReasonMaster reason, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            command.CommandText = @"
INSERT INTO DefectReasons (Name, IsActive, SortOrder, CreatedAt, UpdatedAt)
VALUES (@Name, @IsActive, @SortOrder, @CreatedAt, @UpdatedAt);
SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@Name", reason.Name);
            command.Parameters.AddWithValue("@IsActive", reason.IsActive);
            command.Parameters.AddWithValue("@SortOrder", reason.SortOrder);
            command.Parameters.AddWithValue("@CreatedAt", reason.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@UpdatedAt", reason.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            return Convert.ToInt32(command.ExecuteScalar());
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public int InsertActionType(ActionTypeMaster actionType, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            command.CommandText = @"
INSERT INTO ActionTypes (Name, IsActive, SortOrder, CreatedAt, UpdatedAt)
VALUES (@Name, @IsActive, @SortOrder, @CreatedAt, @UpdatedAt);
SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@Name", actionType.Name);
            command.Parameters.AddWithValue("@IsActive", actionType.IsActive);
            command.Parameters.AddWithValue("@SortOrder", actionType.SortOrder);
            command.Parameters.AddWithValue("@CreatedAt", actionType.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@UpdatedAt", actionType.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            return Convert.ToInt32(command.ExecuteScalar());
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public List<ProductModelMaster> GetProductModels(bool activeOnly = false, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            var sql = "SELECT Id, ModelCode, DisplayName, IsActive, CreatedAt, UpdatedAt FROM ProductModels";
            if (activeOnly)
            {
                sql += " WHERE IsActive = 1";
            }

            sql += " ORDER BY DisplayName ASC";
            command.CommandText = sql;
            using var reader = command.ExecuteReader();
            var items = new List<ProductModelMaster>();
            while (reader.Read())
            {
                items.Add(new ProductModelMaster
                {
                    Id = reader.GetInt32(0),
                    ModelCode = reader.GetString(1),
                    DisplayName = reader.GetString(2),
                    IsActive = reader.GetInt32(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4)),
                    UpdatedAt = DateTime.Parse(reader.GetString(5))
                });
            }

            return items;
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public void UpdateInspectionHistoryText(int historyId, string? defectDetails, string? actionDetails, string? inspectorName)
    {
        using var connection = _databaseManager.OpenConnection(); using var command = connection.CreateCommand();
        command.CommandText = "UPDATE InspectionHistories SET DefectDetails=@DefectDetails, ActionDetails=@ActionDetails, InspectorName=@InspectorName WHERE Id=@Id";
        command.Parameters.AddWithValue("@DefectDetails", (object?)defectDetails ?? DBNull.Value); command.Parameters.AddWithValue("@ActionDetails", (object?)actionDetails ?? DBNull.Value); command.Parameters.AddWithValue("@InspectorName", (object?)inspectorName ?? DBNull.Value); command.Parameters.AddWithValue("@Id", historyId); command.ExecuteNonQuery();
    }

    public void UpdateInspectionHistory(int historyId, InspectionHistory history)
    {
        using var connection = _databaseManager.OpenConnection(); using var command = connection.CreateCommand();
        command.CommandText = "UPDATE InspectionHistories SET InspectionDateTime=@Date, Result=@Result, DefectReasonId=@Reason, DefectDetails=@Details, ActionTypeId=@Action, ActionDetails=@ActionDetails, InspectorName=@Inspector WHERE Id=@Id";
        command.Parameters.AddWithValue("@Date", history.InspectionDateTime.ToString("yyyy-MM-dd HH:mm:ss")); command.Parameters.AddWithValue("@Result", history.Result); command.Parameters.AddWithValue("@Reason", (object?)history.DefectReasonId ?? DBNull.Value); command.Parameters.AddWithValue("@Details", (object?)history.DefectDetails ?? DBNull.Value); command.Parameters.AddWithValue("@Action", (object?)history.ActionTypeId ?? DBNull.Value); command.Parameters.AddWithValue("@ActionDetails", (object?)history.ActionDetails ?? DBNull.Value); command.Parameters.AddWithValue("@Inspector", (object?)history.InspectorName ?? DBNull.Value); command.Parameters.AddWithValue("@Id", historyId); command.ExecuteNonQuery();
    }

    public void UpdateCaseBasics(int caseId, string lotNumber, int productModelId, int? processId, string? notes, DateTime? registeredAt = null)
    {
        using var connection = _databaseManager.OpenConnection(); using var command = connection.CreateCommand();
        command.CommandText = "UPDATE NgCases SET LotNumber=@LotNumber, ProductModelId=@ModelId, ProcessId=@ProcessId, RegisteredAt=COALESCE(@RegisteredAt, RegisteredAt), Notes=@Notes, UpdatedAt=@UpdatedAt WHERE Id=@Id";
        command.Parameters.AddWithValue("@LotNumber", lotNumber); command.Parameters.AddWithValue("@ModelId", productModelId); command.Parameters.AddWithValue("@ProcessId", (object?)processId ?? DBNull.Value); command.Parameters.AddWithValue("@RegisteredAt", registeredAt.HasValue ? registeredAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value); command.Parameters.AddWithValue("@Notes", (object?)notes ?? DBNull.Value); command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); command.Parameters.AddWithValue("@Id", caseId); command.ExecuteNonQuery();
    }

    public int InsertAttachment(CaseAttachment attachment, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        try
        {
            using var command = activeConnection.CreateCommand(); command.Transaction = transaction;
            command.CommandText = "INSERT INTO CaseAttachments (NgCaseId, FileName, ContentType, Content, CreatedAt) VALUES (@CaseId, @FileName, @ContentType, @Content, @CreatedAt); SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@CaseId", attachment.NgCaseId); command.Parameters.AddWithValue("@FileName", attachment.FileName); command.Parameters.AddWithValue("@ContentType", (object?)attachment.ContentType ?? DBNull.Value); command.Parameters.AddWithValue("@Content", attachment.Content); command.Parameters.AddWithValue("@CreatedAt", attachment.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            return Convert.ToInt32(command.ExecuteScalar());
        }
        finally { if (connection is null) activeConnection.Dispose(); }
    }

    public List<CaseAttachment> GetAttachments(int caseId)
    {
        using var connection = _databaseManager.OpenConnection(); using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, NgCaseId, FileName, ContentType, Content, CreatedAt FROM CaseAttachments WHERE NgCaseId = @CaseId ORDER BY Id"; command.Parameters.AddWithValue("@CaseId", caseId);
        using var reader = command.ExecuteReader(); var items = new List<CaseAttachment>();
        while (reader.Read()) items.Add(new CaseAttachment { Id = reader.GetInt32(0), NgCaseId = reader.GetInt32(1), FileName = reader.GetString(2), ContentType = reader.IsDBNull(3) ? null : reader.GetString(3), Content = (byte[])reader[4], CreatedAt = DateTime.Parse(reader.GetString(5)) });
        return items;
    }

    public void DeleteCase(int caseId, SqliteConnection connection, SqliteTransaction transaction)
    {
        foreach (var sql in new[] { "DELETE FROM CaseAttachments WHERE NgCaseId = @Id", "DELETE FROM InspectionHistories WHERE NgCaseId = @Id", "DELETE FROM NgCases WHERE Id = @Id" })
        {
            using var command = connection.CreateCommand(); command.Transaction = transaction; command.CommandText = sql; command.Parameters.AddWithValue("@Id", caseId); command.ExecuteNonQuery();
        }
    }

    public List<string> GetLotNumbers()
    {
        using var connection = _databaseManager.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT DISTINCT LotNumber FROM NgCases WHERE TRIM(LotNumber) <> '' ORDER BY LotNumber";
        using var reader = command.ExecuteReader();
        var lots = new List<string>();
        while (reader.Read())
        {
            lots.Add(reader.GetString(0));
        }

        return lots;
    }

    public List<DefectReasonMaster> GetDefectReasons(bool activeOnly = false, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            var sql = "SELECT Id, Name, IsActive, SortOrder, CreatedAt, UpdatedAt FROM DefectReasons";
            if (activeOnly)
            {
                sql += " WHERE IsActive = 1";
            }

            sql += " ORDER BY SortOrder ASC, Name ASC";
            command.CommandText = sql;
            using var reader = command.ExecuteReader();
            var items = new List<DefectReasonMaster>();
            while (reader.Read())
            {
                items.Add(new DefectReasonMaster
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    IsActive = reader.GetInt32(2),
                    SortOrder = reader.GetInt32(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4)),
                    UpdatedAt = DateTime.Parse(reader.GetString(5))
                });
            }

            return items;
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public List<ActionTypeMaster> GetActionTypes(bool activeOnly = false, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            var sql = "SELECT Id, Name, IsActive, SortOrder, CreatedAt, UpdatedAt FROM ActionTypes";
            if (activeOnly)
            {
                sql += " WHERE IsActive = 1";
            }

            sql += " ORDER BY SortOrder ASC, Name ASC";
            command.CommandText = sql;
            using var reader = command.ExecuteReader();
            var items = new List<ActionTypeMaster>();
            while (reader.Read())
            {
                items.Add(new ActionTypeMaster
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    IsActive = reader.GetInt32(2),
                    SortOrder = reader.GetInt32(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4)),
                    UpdatedAt = DateTime.Parse(reader.GetString(5))
                });
            }

            return items;
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public void SetProductModelActive(int id, bool isActive, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            command.CommandText = "UPDATE ProductModels SET IsActive = @IsActive, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            command.Parameters.AddWithValue("@IsActive", isActive ? 1 : 0);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public List<NgCaseListItem> SearchCases(NgCaseSearchCriteria criteria, bool includeClosed = false, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            var sql = @"
SELECT ng.Id, ng.Status, ng.LotNumber, pm.DisplayName, p.Name, ng.SerialNumber, ng.RegisteredAt,
       latest.InspectionDateTime, latestDefect.Name, latestAction.Name, latest.HistoryCount,
       ng.UpdatedAt, ng.ClosedAt
FROM NgCases ng
LEFT JOIN ProductModels pm ON pm.Id = ng.ProductModelId
LEFT JOIN Processes p ON p.Id = ng.ProcessId
LEFT JOIN (
    SELECT h.NgCaseId, h.Id, h.InspectionDateTime,
           (SELECT COUNT(*) FROM InspectionHistories c WHERE c.NgCaseId = h.NgCaseId) AS HistoryCount
    FROM InspectionHistories h
    WHERE h.Id = (SELECT h2.Id FROM InspectionHistories h2 WHERE h2.NgCaseId = h.NgCaseId ORDER BY h2.InspectionDateTime DESC, h2.Id DESC LIMIT 1)
) latest ON latest.NgCaseId = ng.Id
LEFT JOIN InspectionHistories latestHistory ON latestHistory.Id = latest.Id
LEFT JOIN DefectReasons latestDefect ON latestDefect.Id = latestHistory.DefectReasonId
LEFT JOIN ActionTypes latestAction ON latestAction.Id = latestHistory.ActionTypeId
WHERE 1 = 1";

            if (!includeClosed)
            {
                sql += " AND ng.Status <> @ClosedStatus";
                command.Parameters.AddWithValue("@ClosedStatus", (int)NgCaseStatus.Closed);
            }

            if (!string.IsNullOrWhiteSpace(criteria.FreeText))
            {
                sql += " AND (ng.LotNumber LIKE @FreeText OR pm.DisplayName LIKE @FreeText OR ng.Notes LIKE @FreeText)";
                command.Parameters.AddWithValue("@FreeText", $"%{criteria.FreeText}%");
            }

            if (!string.IsNullOrWhiteSpace(criteria.LotNumber))
            {
                sql += " AND ng.LotNumber LIKE @LotNumber";
                command.Parameters.AddWithValue("@LotNumber", $"%{criteria.LotNumber}%");
            }

            if (!string.IsNullOrWhiteSpace(criteria.ProductModel))
            {
                sql += " AND pm.DisplayName LIKE @ProductModel";
                command.Parameters.AddWithValue("@ProductModel", $"%{criteria.ProductModel}%");
            }

            if (criteria.Status.HasValue)
            {
                sql += " AND ng.Status = @Status";
                command.Parameters.AddWithValue("@Status", (int)criteria.Status.Value);
            }

            if (criteria.RegisteredFrom.HasValue)
            {
                sql += " AND ng.RegisteredAt >= @RegisteredFrom";
                command.Parameters.AddWithValue("@RegisteredFrom", criteria.RegisteredFrom.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            if (criteria.RegisteredTo.HasValue)
            {
                sql += " AND ng.RegisteredAt <= @RegisteredTo";
                command.Parameters.AddWithValue("@RegisteredTo", criteria.RegisteredTo.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            sql += " ORDER BY ng.UpdatedAt DESC, ng.Id DESC";
            command.CommandText = sql;

            using var reader = command.ExecuteReader();
            var items = new List<NgCaseListItem>();
            while (reader.Read())
            {
                items.Add(new NgCaseListItem
                {
                    Id = reader.GetInt32(0),
                    Status = (NgCaseStatus)reader.GetInt32(1),
                    LotNumber = reader.GetString(2),
                    ProductModelName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    ProcessName = reader.IsDBNull(4) ? null : reader.GetString(4),
                    SerialNumber = reader.IsDBNull(5) ? null : reader.GetString(5),
                    RegisteredAt = DateTime.Parse(reader.GetString(6)),
                    LatestInspectionDateTime = reader.IsDBNull(7) ? null : DateTime.Parse(reader.GetString(7)),
                    LatestDefectReasonName = reader.IsDBNull(8) ? null : reader.GetString(8),
                    LatestActionTypeName = reader.IsDBNull(9) ? null : reader.GetString(9),
                    InspectionHistoryCount = reader.GetInt32(10),
                    UpdatedAt = DateTime.Parse(reader.GetString(11)),
                    ClosedAt = reader.IsDBNull(12) ? null : DateTime.Parse(reader.GetString(12))
                });
            }

            return items;
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public bool CaseExists(int caseId, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            command.CommandText = "SELECT 1 FROM NgCases WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", caseId);
            return command.ExecuteScalar() is not null;
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public bool ExistsDefectReason(string name, int? excludeId = null, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            var sql = "SELECT 1 FROM DefectReasons WHERE Name = @Name";
            if (excludeId.HasValue)
            {
                sql += " AND Id <> @ExcludeId";
            }

            command.CommandText = sql;
            command.Parameters.AddWithValue("@Name", name);
            if (excludeId.HasValue)
            {
                command.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
            }

            return command.ExecuteScalar() is not null;
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }

    public bool ExistsActionType(string name, int? excludeId = null, SqliteConnection? connection = null, SqliteTransaction? transaction = null)
    {
        var activeConnection = connection ?? _databaseManager.OpenConnection();
        var activeTransaction = transaction;
        try
        {
            using var command = activeConnection.CreateCommand();
            command.Transaction = activeTransaction;
            var sql = "SELECT 1 FROM ActionTypes WHERE Name = @Name";
            if (excludeId.HasValue)
            {
                sql += " AND Id <> @ExcludeId";
            }

            command.CommandText = sql;
            command.Parameters.AddWithValue("@Name", name);
            if (excludeId.HasValue)
            {
                command.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
            }

            return command.ExecuteScalar() is not null;
        }
        finally
        {
            if (connection is null)
            {
                activeConnection.Dispose();
            }
        }
    }
}

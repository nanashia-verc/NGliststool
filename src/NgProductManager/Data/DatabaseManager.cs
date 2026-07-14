using System.Data;
using Microsoft.Data.Sqlite;

namespace NgProductManager.Data;

public sealed class DatabaseManager
{
    private readonly string _databasePath;

    public DatabaseManager(string databasePath)
    {
        _databasePath = databasePath;
    }

    public string DatabasePath => _databasePath;

    public void EnsureDatabaseCreated()
    {
        if (File.Exists(_databasePath))
        {
            return;
        }

        var directory = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var connection = OpenConnection();
        connection.Open();

        var createSql = @"
CREATE TABLE IF NOT EXISTS NgCases (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    LotNumber TEXT NOT NULL,
    ProductModelId INTEGER NOT NULL,
    SerialNumber TEXT,
    Status INTEGER NOT NULL,
    Notes TEXT,
    RegisteredAt TEXT NOT NULL,
    ClosedAt TEXT,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    FOREIGN KEY(ProductModelId) REFERENCES ProductModels(Id)
);

CREATE TABLE IF NOT EXISTS InspectionHistories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    NgCaseId INTEGER NOT NULL,
    InspectionDateTime TEXT NOT NULL,
    Result INTEGER NOT NULL,
    DefectReasonId INTEGER,
    DefectDetails TEXT,
    ActionTypeId INTEGER,
    ActionDetails TEXT,
    InspectorName TEXT,
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY(NgCaseId) REFERENCES NgCases(Id),
    FOREIGN KEY(DefectReasonId) REFERENCES DefectReasons(Id),
    FOREIGN KEY(ActionTypeId) REFERENCES ActionTypes(Id)
);

CREATE TABLE IF NOT EXISTS ProductModels (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ModelCode TEXT NOT NULL UNIQUE,
    DisplayName TEXT NOT NULL,
    IsActive INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS DefectReasons (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    IsActive INTEGER NOT NULL,
    SortOrder INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS ActionTypes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    IsActive INTEGER NOT NULL,
    SortOrder INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS IDX_NgCases_LotNumber ON NgCases(LotNumber);
CREATE INDEX IF NOT EXISTS IDX_NgCases_ProductModelId ON NgCases(ProductModelId);
CREATE INDEX IF NOT EXISTS IDX_NgCases_Status ON NgCases(Status);
CREATE INDEX IF NOT EXISTS IDX_NgCases_RegisteredAt ON NgCases(RegisteredAt);
CREATE INDEX IF NOT EXISTS IDX_InspectionHistories_InspectionDateTime ON InspectionHistories(InspectionDateTime);
";

        using var command = connection.CreateCommand();
        command.CommandText = createSql;
        command.ExecuteNonQuery();
    }

    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection($"Data Source={_databasePath}");
        connection.Open();
        return connection;
    }

    public IDbTransaction BeginTransaction()
    {
        var connection = OpenConnection();
        return connection.BeginTransaction();
    }
}

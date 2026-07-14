namespace NgProductManager.Utilities;

public static class ApplicationPaths
{
    public static string BaseDirectory => AppContext.BaseDirectory;

    public static string DataDirectory => Path.Combine(BaseDirectory, "data");

    public static string DatabasePath => Path.Combine(DataDirectory, "ng-manager.db");

    public static string BackupDirectory => Path.Combine(BaseDirectory, "backup");

    public static string LogDirectory => Path.Combine(BaseDirectory, "logs");

    public static string EnsureDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("保存先パスが指定されていません。");
        }

        Directory.CreateDirectory(path);
        return path;
    }
}

namespace NgProductManager.Utilities;

public static class AppLogger
{
    public static void WriteError(string message, Exception? exception = null)
    {
        WriteLog("ERROR", message, exception);
    }

    public static void WriteInfo(string message)
    {
        WriteLog("INFO", message, null);
    }

    private static void WriteLog(string level, string message, Exception? exception)
    {
        try
        {
            var directory = ApplicationPaths.EnsureDirectory(ApplicationPaths.LogDirectory);
            var filePath = Path.Combine(directory, $"app-{DateTime.Now:yyyyMMdd}.log");
            var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            if (exception is not null)
            {
                text += $"{Environment.NewLine}Exception: {exception.GetType().Name}{Environment.NewLine}Message: {exception.Message}{Environment.NewLine}StackTrace: {exception.StackTrace}";
            }

            File.AppendAllText(filePath, text + Environment.NewLine);
        }
        catch
        {
            // ログ出力に失敗してもアプリが落ちないようにする
        }
    }
}

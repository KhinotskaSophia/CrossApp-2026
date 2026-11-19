namespace Core.Configuration;

public sealed class AppOptions
{
    public string EnvironmentName { get; init; } = "Development";
    public LoggingOptions Logging { get; init; } = new();
    public StorageOptions Storage { get; init; } = new();
    public LibraryOptions Library { get; init; } = new();
}

public sealed class LoggingOptions
{
    public string MinimumLevel { get; init; } = "Information";
}

public sealed class LibraryOptions
{
    public int LoanDays { get; init; } = 14;
}

public sealed class StorageOptions
{
    public string DataPath { get; init; } = "data";
    public string FileName { get; init; } = "items.json";

    public string ResolveFullPath(string baseDir)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDir);
        ArgumentException.ThrowIfNullOrWhiteSpace(DataPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(FileName);

        var root = Path.IsPathRooted(DataPath)
            ? DataPath
            : Path.Combine(baseDir, DataPath);

        Directory.CreateDirectory(root);
        return Path.GetFullPath(Path.Combine(root, FileName));
    }
}
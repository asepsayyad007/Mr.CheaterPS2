namespace MrCheater.Core.Common;

public static class StorageHelper
{
    private static string? _baseDirectory;

    public static string GetBaseDirectory()
    {
        if (_baseDirectory != null)
            return _baseDirectory;

        // Check if portable mode is enabled (portable.dat file exists next to exe)
        var appExeDir = AppDomain.CurrentDomain.BaseDirectory;
        if (File.Exists(Path.Combine(appExeDir, "portable.dat")))
        {
            _baseDirectory = Path.Combine(appExeDir, "Data");
        }
        else
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _baseDirectory = Path.Combine(appData, "UniversalCheatManager");
        }

        Directory.CreateDirectory(_baseDirectory);
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "profiles"));
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "emulators"));
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "logs"));

        return _baseDirectory;
    }

    public static string GetProfilesDirectory() => Path.Combine(GetBaseDirectory(), "profiles");
    public static string GetEmulatorsDirectory() => Path.Combine(GetBaseDirectory(), "emulators");
    public static string GetLogsDirectory() => Path.Combine(GetBaseDirectory(), "logs");
    public static string GetSettingsFilePath() => Path.Combine(GetBaseDirectory(), "settings.json");

    public static void SetCustomBaseDirectory(string path)
    {
        _baseDirectory = path;
        Directory.CreateDirectory(_baseDirectory);
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "profiles"));
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "emulators"));
        Directory.CreateDirectory(Path.Combine(_baseDirectory, "logs"));
    }
}

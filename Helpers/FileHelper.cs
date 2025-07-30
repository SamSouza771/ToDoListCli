namespace ToDoListCli.Helpers;

public static class FileHelper
{
    public static string EnsureStoragePath()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "ToDoCli"
        );

        Directory.CreateDirectory(dir);

        var file = Path.Combine(dir, "Tasks.json");

        if (!File.Exists(file))
            File.WriteAllText(file, "[]");

        return file;
    }
}

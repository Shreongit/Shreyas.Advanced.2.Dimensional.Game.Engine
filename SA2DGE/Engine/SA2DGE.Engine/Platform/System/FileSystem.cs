namespace SA2DGE.Engine.Platform.System;

public static class FileSystem
{
    public static bool Exists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return File.Exists(path) || Directory.Exists(path);
    }

    public static bool FileExists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return File.Exists(path);
    }

    public static bool DirectoryExists(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return Directory.Exists(path);
    }

    public static void CreateDirectory(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Directory.CreateDirectory(path);
    }

    public static void DeleteFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void DeleteDirectory(
        string path,
        bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive);
        }
    }

    public static string[] GetFiles(
        string directory,
        string searchPattern = "*",
        bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        ArgumentException.ThrowIfNullOrWhiteSpace(searchPattern);

        SearchOption option = recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        return Directory.GetFiles(directory, searchPattern, option);
    }

    public static string[] GetDirectories(
        string directory,
        bool recursive = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);

        SearchOption option = recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        return Directory.GetDirectories(directory, "*", option);
    }

    public static string ReadText(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return File.ReadAllText(path);
    }

    public static void WriteText(
        string path,
        string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        File.WriteAllText(path, content);
    }

    public static byte[] ReadBytes(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return File.ReadAllBytes(path);
    }

    public static void WriteBytes(
        string path,
        byte[] data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(data);

        File.WriteAllBytes(path, data);
    }

    public static string GetFullPath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return Path.GetFullPath(path);
    }

    public static string GetFileName(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return Path.GetFileName(path);
    }

    public static string GetExtension(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return Path.GetExtension(path);
    }

    public static string Combine(params string[] paths)
    {
        ArgumentNullException.ThrowIfNull(paths);

        return Path.Combine(paths);
    }

    public static string GetCurrentDirectory()
    {
        return Directory.GetCurrentDirectory();
    }
}
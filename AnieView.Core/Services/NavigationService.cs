using AnieView.Core.Interfaces;

namespace AnieView.Core.Services;

public class NavigationService : INavigationService
{
    private static readonly string[] SupportedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".bmp" };

    public string? GetNextFile(string currentFilePath)
    {
        return GetAdjacentFile(currentFilePath, 1);
    }

    public string? GetPreviousFile(string currentFilePath)
    {
        return GetAdjacentFile(currentFilePath, -1);
    }

    private string? GetAdjacentFile(string currentFilePath, int offset)
    {
        var directory = Path.GetDirectoryName(currentFilePath);
        if (string.IsNullOrEmpty(directory)) return null;

        var files = Directory.GetFiles(directory)
            .Where(f => SupportedExtensions.Contains(Path.GetExtension(f).ToLower()))
            .OrderBy(f => f)
            .ToList();

        if (files.Count <= 1) return null;

        int currentIndex = files.IndexOf(currentFilePath);
        if (currentIndex == -1) return null;

        int nextIndex = (currentIndex + offset + files.Count) % files.Count;
        return files[nextIndex];
    }
}

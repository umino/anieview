using System.IO;
using AnieView.Core.Interfaces;
using AnieView.Core.Models;

namespace AnieView.Infrastructure.Services;

public class NavigationService : INavigationService
{
    private static readonly string[] SupportedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".bmp" };

    public SortOrder SortOrder { get; set; } = SortOrder.FileName;

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

        var filteredFiles = Directory.GetFiles(directory)
            .Where(f => SupportedExtensions.Contains(Path.GetExtension(f).ToLower()));

        var files = SortOrder switch
        {
            SortOrder.LastModified => filteredFiles.OrderBy(f => File.GetLastWriteTime(f)).ToList(),
            _ => filteredFiles.OrderBy(f => f).ToList(),
        };

        if (files.Count <= 1) return null;

        int currentIndex = files.IndexOf(currentFilePath);
        if (currentIndex == -1) return null;

        int nextIndex = (currentIndex + offset + files.Count) % files.Count;
        return files[nextIndex];
    }
}

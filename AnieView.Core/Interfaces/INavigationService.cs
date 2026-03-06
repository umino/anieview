using AnieView.Core.Models;

namespace AnieView.Core.Interfaces;

public interface INavigationService
{
    SortOrder SortOrder { get; set; }
    string? GetNextFile(string currentFilePath);
    string? GetPreviousFile(string currentFilePath);
}

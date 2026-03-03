namespace AnieView.Core.Interfaces;

public interface INavigationService
{
    string? GetNextFile(string currentFilePath);
    string? GetPreviousFile(string currentFilePath);
}

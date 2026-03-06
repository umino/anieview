using AnieView.Core.Interfaces;
using AnieView.Core.Models;

namespace AnieView.Tests.Stubs;

public class StubNavigationService : INavigationService
{
    public SortOrder SortOrder { get; set; } = SortOrder.FileName;
    public string? NextFileResult { get; set; }
    public string? PreviousFileResult { get; set; }

    public string? GetNextFile(string currentFilePath)
    {
        return NextFileResult;
    }

    public string? GetPreviousFile(string currentFilePath)
    {
        return PreviousFileResult;
    }
}

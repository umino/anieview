using AnieView.Core.Interfaces;
using AnieView.Core.Models;

namespace AnieView.Tests.Stubs;

public class StubSettingsService : ISettingsService
{
    public SortOrder SortOrder { get; set; } = SortOrder.FileName;
    public int SaveCallCount { get; private set; }
    public int LoadCallCount { get; private set; }

    public void Save() => SaveCallCount++;
    public void Load() => LoadCallCount++;
}

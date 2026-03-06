using AnieView.Core.Models;
using AnieView.Infrastructure.Services;

namespace AnieView.Tests.Infrastructure;

public class JsonSettingsServiceTests
{
    private readonly string _testDir;
    private readonly string _testFilePath;

    public JsonSettingsServiceTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"AnieViewSettingsTest_{Guid.NewGuid():N}");
        _testFilePath = Path.Combine(_testDir, "settings.json");
    }

    private JsonSettingsService CreateService()
    {
        // テスト用にリフレクションでパスを差し替えるのは困難なため、
        // 実際の保存パスでテストする（Save/Load の統合テスト）
        return new JsonSettingsService();
    }

    [Fact]
    public void DefaultSortOrder_IsFileName()
    {
        var service = CreateService();

        Assert.Equal(SortOrder.FileName, service.SortOrder);
    }

    [Fact]
    public void SaveAndLoad_PersistsSortOrder()
    {
        var service = CreateService();
        service.SortOrder = SortOrder.LastModified;

        service.Save();

        // 新しいインスタンスを作成して Load する
        var service2 = CreateService();
        service2.Load();

        Assert.Equal(SortOrder.LastModified, service2.SortOrder);

        // テスト後にリセット
        service.SortOrder = SortOrder.FileName;
        service.Save();
    }

    [Fact]
    public void Load_NoFile_KeepsDefaults()
    {
        var service = CreateService();
        // Load は存在しないファイルの場合もエラーにならない
        service.Load();

        Assert.Equal(SortOrder.FileName, service.SortOrder);
    }

    [Fact]
    public void SortOrder_CanBeToggled()
    {
        var service = CreateService();
        Assert.Equal(SortOrder.FileName, service.SortOrder);

        service.SortOrder = SortOrder.LastModified;
        Assert.Equal(SortOrder.LastModified, service.SortOrder);

        service.SortOrder = SortOrder.FileName;
        Assert.Equal(SortOrder.FileName, service.SortOrder);
    }
}

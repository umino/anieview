using AnieView.Core.Models;
using AnieView.Infrastructure.Services;

namespace AnieView.Tests.Infrastructure;

public class NavigationServiceTests
{
    private readonly string _testDir;

    public NavigationServiceTests()
    {
        // テスト用の一時ディレクトリを作成
        _testDir = Path.Combine(Path.GetTempPath(), $"AnieViewTest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    private string CreateTestFile(string fileName, DateTime? lastWriteTime = null)
    {
        var filePath = Path.Combine(_testDir, fileName);
        File.WriteAllText(filePath, "test");
        if (lastWriteTime.HasValue)
        {
            File.SetLastWriteTime(filePath, lastWriteTime.Value);
        }
        return filePath;
    }

    [Fact]
    public void GetNextFile_ByFileName_ReturnsAlphabeticallyNext()
    {
        var fileA = CreateTestFile("a.jpg");
        var fileB = CreateTestFile("b.jpg");
        var fileC = CreateTestFile("c.jpg");

        var service = new NavigationService { SortOrder = SortOrder.FileName };

        var result = service.GetNextFile(fileA);

        Assert.Equal(fileB, result);
    }

    [Fact]
    public void GetPreviousFile_ByFileName_ReturnsAlphabeticallyPrevious()
    {
        var fileA = CreateTestFile("a.jpg");
        var fileB = CreateTestFile("b.jpg");
        var fileC = CreateTestFile("c.jpg");

        var service = new NavigationService { SortOrder = SortOrder.FileName };

        var result = service.GetPreviousFile(fileB);

        Assert.Equal(fileA, result);
    }

    [Fact]
    public void GetNextFile_ByLastModified_ReturnsChronologicallyNext()
    {
        var fileOld = CreateTestFile("z_old.jpg", new DateTime(2020, 1, 1));
        var fileMid = CreateTestFile("a_mid.jpg", new DateTime(2022, 1, 1));
        var fileNew = CreateTestFile("m_new.jpg", new DateTime(2024, 1, 1));

        var service = new NavigationService { SortOrder = SortOrder.LastModified };

        // 更新日時順で oldest → middle → newest
        var result = service.GetNextFile(fileOld);

        Assert.Equal(fileMid, result);
    }

    [Fact]
    public void GetNextFile_SingleFile_ReturnsNull()
    {
        CreateTestFile("only.jpg");
        var filePath = Path.Combine(_testDir, "only.jpg");

        var service = new NavigationService();

        var result = service.GetNextFile(filePath);

        Assert.Null(result);
    }

    [Fact]
    public void GetNextFile_WrapsAround()
    {
        var fileA = CreateTestFile("a.jpg");
        var fileB = CreateTestFile("b.jpg");
        var fileC = CreateTestFile("c.jpg");

        var service = new NavigationService { SortOrder = SortOrder.FileName };

        // 最後のファイルの「次」は最初のファイル
        var result = service.GetNextFile(fileC);

        Assert.Equal(fileA, result);
    }

    [Fact]
    public void GetNextFile_IgnoresNonImageFiles()
    {
        var fileA = CreateTestFile("a.jpg");
        CreateTestFile("b.txt"); // 非画像ファイル
        var fileC = CreateTestFile("c.jpg");

        var service = new NavigationService { SortOrder = SortOrder.FileName };

        var result = service.GetNextFile(fileA);

        Assert.Equal(fileC, result);
    }

    // テスト後にディレクトリをクリーンアップ
    ~NavigationServiceTests()
    {
        try { Directory.Delete(_testDir, true); } catch { }
    }
}

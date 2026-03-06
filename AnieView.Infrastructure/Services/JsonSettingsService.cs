using System.IO;
using System.Text.Json;
using AnieView.Core.Interfaces;
using AnieView.Core.Models;

namespace AnieView.Infrastructure.Services;

/// <summary>
/// JSON ファイルを使用してアプリケーション設定を永続化するサービス
/// </summary>
public class JsonSettingsService : ISettingsService
{
    private static readonly string SettingsDirectory = 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AnieView");
    private static readonly string SettingsFilePath = 
        Path.Combine(SettingsDirectory, "settings.json");

    public SortOrder SortOrder { get; set; } = SortOrder.FileName;

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(SettingsDirectory);
            var data = new SettingsData { SortOrder = SortOrder.ToString() };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }
        catch (Exception)
        {
            // 書き込み失敗時は無視（次回起動時にデフォルト値が使われる）
        }
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(SettingsFilePath)) return;

            var json = File.ReadAllText(SettingsFilePath);
            var data = JsonSerializer.Deserialize<SettingsData>(json);
            if (data != null && Enum.TryParse<SortOrder>(data.SortOrder, out var sortOrder))
            {
                SortOrder = sortOrder;
            }
        }
        catch (Exception)
        {
            // 読み込み失敗時はデフォルト値を使用
        }
    }

    /// <summary>
    /// JSON シリアライズ用の内部データクラス
    /// </summary>
    private class SettingsData
    {
        public string SortOrder { get; set; } = "FileName";
    }
}

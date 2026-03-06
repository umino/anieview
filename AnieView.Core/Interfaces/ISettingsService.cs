using AnieView.Core.Models;

namespace AnieView.Core.Interfaces;

/// <summary>
/// アプリケーション設定の永続化を担うインターフェース
/// </summary>
public interface ISettingsService
{
    /// <summary>画像遷移時のソート順</summary>
    SortOrder SortOrder { get; set; }

    /// <summary>設定をファイルに保存する</summary>
    void Save();

    /// <summary>設定をファイルから読み込む</summary>
    void Load();
}

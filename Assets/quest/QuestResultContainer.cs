// QuestResultDataContainer.cs
using UnityEngine;

/// <summary>
/// クエスト結果のJSON文字列を、シーンをまたいで保持するためのシングルトンコンテナ。
/// </summary>
public class QuestResultContainer : MonoBehaviour
{
    // 💡 シングルトンインスタンスへの静的なアクセスポイント
    public static QuestResultContainer Instance { get; private set; }

    // 保持する生のJSON文字列データ
    private string rawResultJson;

    void Awake()
    {
        // シングルトンの初期化ロジック
        if (Instance == null)
        {
            Instance = this;
            // 💡 シーンを切り替えてもこのGameObjectが破棄されないようにする
            DontDestroyOnLoad(gameObject);
        }
        else // 既にインスタンスが存在する場合
        {
            // 新しく作成されたGameObjectを破棄する
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// APIから取得した結果JSON文字列を保存します。
    /// QuestManager の OnGetResultSuccess メソッド内で呼び出されます。
    /// </summary>
    /// <param name="json">/quest/result APIから取得した生のJSON文字列</param>
    public void SetRawResultJson(string json)
    {
        rawResultJson = json;
        Debug.Log("コンテナにクエスト結果JSONデータが保存されました。");
    }

    /// <summary>
    /// 保存されている結果JSON文字列を取得します。
    /// QuestResultManager の Start メソッド内で呼び出されます。
    /// </summary>
    /// <returns>保存された生のJSON文字列</returns>
    public string GetRawResultJson()
    {
        return rawResultJson;
    }

    /// <summary>
    /// データをリセットします。次のクエストに備えて、シーン遷移時にDestroyされる前のオブジェクトから呼び出すことを推奨します。
    /// </summary>
    public void ResetData()
    {
        rawResultJson = null;
        Debug.Log("QuestResultDataContainerのデータをリセットしました。");
    }
}
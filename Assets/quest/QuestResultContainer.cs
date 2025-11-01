// QuestResultContainer.cs
using UnityEngine;

public class QuestResultContainer : MonoBehaviour
{
    // シングルトンインスタンス
    public static QuestResultContainer Instance { get; private set; }

    // 結果データを保持する変数
    private string rawResultJson;

    // --- 初期化と永続化 ---
    private void Awake()
    {
        if (Instance == null)
        {
            // 最初にロードされたインスタンスを保持
            Instance = this;

            // 💡 永続化の最重要設定 💡
            // このオブジェクトをシーンが切り替わっても破棄されないようにする
            DontDestroyOnLoad(gameObject);

            Debug.Log("QuestResultContainer: 永続化インスタンスが設定されました。");
        }
        else
        {
            // 2つ目以降のインスタンスは破棄
            Destroy(gameObject);
            Debug.LogWarning("QuestResultContainer: 重複インスタンスを破棄しました。");
        }
    }

    // --- データ保存/取得 ---

    public void SetRawResultJson(string json)
    {
        this.rawResultJson = json;
    }

    public string GetRawResultJson()
    {
        return rawResultJson;
    }

    // --- シーン遷移後のクリーンアップ（オプション） ---

    // 結果画面でデータを使用した後、破棄するためのヘルパーメソッド
    // QuestResultManager.cs の ReturnToHome() メソッドから呼ばれることを想定
    public static void DestroyInstance()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null; // インスタンス参照をクリア
            Debug.Log("QuestResultContainer: インスタンスを破棄しました。");
        }
    }
}
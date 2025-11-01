// QuestResultManager.cs (クエストバトル結果表示用)
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System; // Exceptionのために必要

public class QuestResultManager : MonoBehaviour
{
    // --- UIコンポーネント (Inspectorで設定) ---
    public TextMeshProUGUI resultTitleText;     // "クエストクリア" or "クエスト失敗"
    public TextMeshProUGUI bossNameText;        // ボス名
    public TextMeshProUGUI finalHpText;         // 最終HP情報（ボス残りHPやユーザー残りHP）
    public TextMeshProUGUI levelText;           // ユーザーの最終レベル
    public TextMeshProUGUI expText;             // ユーザーの総経験値 (攻撃力などを添える)
    public TextMeshProUGUI gachaPointText;      // 獲得したガチャポイントなど
    public Button returnToHomeButton;
    public string homeSceneName = "Home";       // 戻る先のシーン名

    void Start()
    {
        // UIボタンにリスナーを設定
        if (returnToHomeButton != null)
        {
            returnToHomeButton.onClick.AddListener(ReturnToHome);
        }

        // 前のシーンから渡されたデータを確認
        string resultJson = GetResultData();

        if (!string.IsNullOrEmpty(resultJson))
        {
            DisplayResult(resultJson);
        }
        else
        {
            // データがない場合はエラー表示
            DisplayError("クエストデータが見つかりませんでした。");
        }
    }

    /// <summary>
    /// QuestResultDataContainerから結果JSONを取得します。
    /// </summary>
    private string GetResultData()
    {
        // シングルトンインスタンスからJSONを取得
        if (QuestResultDataContainer.Instance != null)
        {
            return QuestResultDataContainer.Instance.GetRawResultJson();
        }
        else
        {
            Debug.LogWarning("QuestResultDataContainerのインスタンスが見つかりません。");
            return null;
        }
    }

    /// <summary>
    /// 結果JSONをパースし、UIに表示します。
    /// </summary>
    private void DisplayResult(string json)
    {
        try
        {
            // JSONをレスポンス構造体に変換 (QuestData.csにある前提)
            QuestResultResponse response = JsonUtility.FromJson<QuestResultResponse>(json);

            // 1. クエスト結果の表示
            bool isCompleted = response.questStatus == "completed";
            if (resultTitleText != null)
            {
                resultTitleText.text = isCompleted ? "🎉 クエストクリア！ 🎉" : "😭 クエスト失敗... 😭";
                resultTitleText.color = isCompleted ? Color.green : Color.red;
            }

            // 2. ボスとユーザーHPの表示
            if (bossNameText != null)
            {
                bossNameText.text = $"対戦ボス: {response.bossName}";
            }
            if (finalHpText != null)
            {
                string statusMsg = isCompleted
                    ? $"ボスを撃破しました！ (最終HP: {response.finalUserHp})"
                    : $"ユーザー敗北... (ボス残りHP: {response.finalBossHp})";
                finalHpText.text = statusMsg;
            }

            // 3. ユーザーの最終ステータスとポイントの表示
            var status = response.finalUserCharacterStatus;

            if (levelText != null)
            {
                levelText.text = $"最終レベル: {status.currentLevel}";
            }
            if (expText != null)
            {
                expText.text = $"総経験値: {status.currentExperience} | 攻撃力:{status.attackPower}";
            }
            // ガチャポイントはAPIレスポンスに含まれていませんが、クリア報酬として50ポイントを仮定
            if (gachaPointText != null)
            {
                // サーバー側のanswer.tsでクリア時に50ポイント付与しているため、その情報を表示
                gachaPointText.text = isCompleted ? "獲得ガチャポイント: 50 pt" : "獲得ガチャポイント: 0 pt";
            }

            Debug.Log("クエスト結果の表示に成功しました。");
        }
        catch (System.Exception e)
        {
            DisplayError($"結果データのパースに失敗しました: {e.Message}");
        }
    }

    /// <summary>
    /// エラーメッセージをコンソールとUIに表示します。
    /// </summary>
    private void DisplayError(string message)
    {
        Debug.LogError(message);
        if (resultTitleText != null)
        {
            resultTitleText.text = "エラーが発生しました。";
            resultTitleText.color = Color.yellow;
        }
        // 他のUIも初期化
        if (bossNameText != null) bossNameText.text = "";
        if (finalHpText != null) finalHpText.text = message;
    }

    /// <summary>
    /// ホーム画面に戻り、データコンテナを破棄します。
    /// </summary>
    public void ReturnToHome()
    {
        // データコンテナを破棄
        if (QuestResultDataContainer.Instance != null)
        {
            // シングルトンのgameObjectを破棄することで、DontDestroyOnLoadを解除
            Destroy(QuestResultDataContainer.Instance.gameObject);
        }

        SceneManager.LoadScene(homeSceneName);
    }
}
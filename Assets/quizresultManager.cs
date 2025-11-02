using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 💡 修正点 1: SceneManagerのために追加

public class ResultSceneManager : MonoBehaviour
{
    // UIコンポーネント (インスペクターから設定)
    public TextMeshProUGUI resultTitleText;        // 例: "クイズ結果！"
    public TextMeshProUGUI correctCountText;       // 正解数
    public TextMeshProUGUI expGainedText;          // 獲得した経験値
    public TextMeshProUGUI totalExpText;           // 総経験値
    public TextMeshProUGUI levelStatusText;        // 現在のレベル
    public TextMeshProUGUI levelUpMessageText;     // レベルアップメッセージ
    public Button homeButton;
    public string homeSceneName = "home";

    void Start()
    {
        DisplayResults();
        if (homeButton != null)
        {
            homeButton.onClick.AddListener(GoToHomeScene);
        }
        else
        {
            Debug.LogError("Home Buttonがインスペクターで設定されていません！");
        }
    }

    private void DisplayResults()
    {
        // コンテナのインスタンスを取得
        // 💡 修正点 2: 重複していた行を削除
        var container = QuizResultDataContainer.Instance;

        if (container == null || !container.IsQuizFinished || container.FinalExperienceResult == null)
        {
            // エラーまたはデータがない場合
            resultTitleText.text = "エラー：結果データがありません。";
            return;
        }

        // 必要なデータをコンテナから取得
        int correctCount = container.CorrectAnswerCount;
        var expResult = container.FinalExperienceResult;

        correctCountText.text = $"正解数: {correctCount} 問";
        expGainedText.text = $"今回獲得した経験値: {expResult.totalExperienceGained}";
        totalExpText.text = $"現在の総経験値: {expResult.experience}";
        levelStatusText.text = $"現在のレベル: {expResult.level}";

        // 5. レベルアップ判定
        if (expResult.leveledUp)
        {
            levelUpMessageText.gameObject.SetActive(true);
            levelUpMessageText.text = $"🎉 レベルアップ！ Lv.{expResult.level} になりました！ 🎉";
            levelUpMessageText.color = Color.yellow;
        }
        else
        {
            levelUpMessageText.gameObject.SetActive(false);
        }

        resultTitleText.text = "クイズ結果発表！";

        // 💡 データを使い終わったらリセット (QuizResultDataContainerにResetData()がある場合)
        // container.ResetData();
    }
    // 💡 修正点 3: DisplayResults の閉じ括弧 (ここで閉じる)

    // 💡 修正点 4: GoToHomeSceneメソッドを DisplayResults の外に移動
    public void GoToHomeScene()
    {
        if (string.IsNullOrEmpty(homeSceneName))
        {
            Debug.LogError("ホームシーン名が設定されていません。インスペクターで設定してください！");
            return; // 💡 修正点 5: return を追加
        }

        // 💡 修正点 6: データコンテナの破棄ロジックを修正
        // このスクリプトは 'QuizResultDataContainer' を参照しているため、それを破棄
        if (QuizResultDataContainer.Instance != null)
        {
            Destroy(QuizResultDataContainer.Instance.gameObject);
        }

        Debug.Log($"シーン遷移: {homeSceneName} へ");
        SceneManager.LoadScene(homeSceneName); // 💡 修正点 7: シーンロード処理を追加
    }
} // 💡 修正点 8: クラスの閉じ括弧
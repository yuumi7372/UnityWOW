using UnityEngine;
using TMPro;

public class ResultSceneManager : MonoBehaviour
{
    // UIコンポーネント (インスペクターから設定)
    public TextMeshProUGUI resultTitleText;         // 例: "クイズ結果！"
    public TextMeshProUGUI correctCountText;        // 正解数
    public TextMeshProUGUI expGainedText;           // 獲得した経験値
    public TextMeshProUGUI totalExpText;            // 総経験値
    public TextMeshProUGUI levelStatusText;         // 現在のレベル
    public TextMeshProUGUI levelUpMessageText;      // レベルアップメッセージ

    void Start()
    {
        DisplayResults();
    }

    private void DisplayResults()
    {
        // コンテナのインスタンスを取得
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

        // UIへの表示

        // 1. 正解数
        correctCountText.text = $"正解数: {correctCount} 問";

        // 2. 獲得した経験値
        expGainedText.text = $"今回獲得した経験値: {expResult.totalExperienceGained}";

        // 3. 総経験値
        totalExpText.text = $"現在の総経験値: {expResult.experience}";

        // 4. 現在のレベル
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

        // データ使用後、このコンテナは次のクイズのためにリセットしても良い（要件による）
        // container.ResetData();
    }
}
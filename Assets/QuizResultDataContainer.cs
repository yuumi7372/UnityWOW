using UnityEngine;

// QuizApiClient.cs で定義した ExperienceUpdateResult クラスが必要になります。
// 必要に応じて、ExperienceUpdateResultの定義も同じファイルにコピーするか、
// 別途作成したファイルからusingディレクティブで参照できるようにしてください。

public class QuizResultDataContainer : MonoBehaviour
{
    // シングルトンのインスタンス
    public static QuizResultDataContainer Instance { get; private set; }

    // 1. 正解数
    public int CorrectAnswerCount { get; private set; } = 0;

    // 2. APIからの経験値/レベル情報（獲得経験値、総経験値、レベル、レベルアップ判定を含む）
    public ExperienceUpdateResult FinalExperienceResult { get; private set; } = null;

    // クイズが最後までプレイされてデータが格納されたかを示すフラグ
    public bool IsQuizFinished { get; private set; } = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // シーンを跨いでも破棄されないように設定
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 既にインスタンスが存在する場合は、新しいオブジェクトを破棄する
            Destroy(gameObject);
        }
    }

    /**
     * Quiz.cs の PostFinalScore() で呼び出され、結果を格納する
     * @param correctCount 最終的な正解数
     * @param expResult 経験値更新APIからの応答データ
     */
    public void SetFinalResult(int correctCount, ExperienceUpdateResult expResult)
    {
        CorrectAnswerCount = correctCount;
        FinalExperienceResult = expResult;
        IsQuizFinished = true;
        Debug.Log($"コンテナに結果を保存しました。正解数: {correctCount}, 獲得EXP: {expResult?.totalExperienceGained}");
    }

    /**
     * 結果表示後にデータをリセットし、次のクイズに備える
     */
    public void ResetData()
    {
        CorrectAnswerCount = 0;
        FinalExperienceResult = null;
        IsQuizFinished = false;
        Debug.Log("QuizResultDataContainerのデータをリセットしました。");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using TMPro;
using System.Text;
using UnityEngine.SceneManagement;

// QuizApiClientで使用する構造体との互換性のため、ここにも定義を残します

// APIから返ってくるクイズデータの単体構造を定義
[Serializable]
public class QuizData
{
    // JSONのwordIdが数値型であるため、intに変更
    public int wordId;
    public string question;
    public List<string> options;
    public string correctAnswer;
    public int difficultyLevel;
}

// JSON配列をデシリアライズするために一時的に使用するラッパークラス
[Serializable]
public class QuizListWrapper
{
    public QuizData[] quizzes;
}

public class Quiz : MonoBehaviour
{
    // 取得したクイズデータを格納するリスト
    private List<QuizData> fetchedQuizzes;
    private int correctAnswerCount = 0;

    // ユーザーIDはログイン時に取得し、ここで保持していると想定 (仮の値)
    private string CurrentUserId
    {
        get
        {
            return PlayerPrefs.GetString("userId", ""); // PlayerPrefsから取得する例
        }
    }
    // 正解した問題の難易度を一時的に保存するリスト
    // 経験値計算のために使用
    private List<int> correctDifficultyLevels = new List<int>();

    // UIコンポーネント (インスペクターから設定が必要です！)
    public TextMeshProUGUI questionText;
    public Button[] optionButtons; // 4つの選択肢ボタン
    public TextMeshProUGUI[] optionTexts; // 各ボタンのテキスト
    public TextMeshProUGUI resultText;

    // 画面遷移先のシーン名
    public string resultSceneName = "quiz_result";

    private int currentQuizIndex = 0;

    void Start()
    {
        // 実際にはログイン成功後にFetchQuizzes()を呼び出すべきですが、テストのためStartに残します。
        if (resultText != null)
        {
            resultText.text = "がんばれ！";
        }
        FetchQuizzes();
    }

    // クイズデータを取得する公開メソッド
    public void FetchQuizzes()
    {
        StartCoroutine(GetQuizData());
        Debug.Log("クイズデータの取得を開始します。");
    }

    private IEnumerator GetQuizData()
    {
        // ApiClientがJWTトークンを自動でヘッダーに追加することを前提とします。
        // ★注意: ApiClient.CreateGet("question") の実装は、このファイルには含まれていません。
        using (UnityWebRequest webRequest = ApiClient.CreateGet("question"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 文字化け対策（UTF-8デコード）を維持
                byte[] data = webRequest.downloadHandler.data;
                string jsonString = System.Text.Encoding.UTF8.GetString(data);
                Debug.Log("クイズデータの取得に成功しました！");
                Debug.Log("取得したJSONデータ: " + jsonString);

                try
                {
                    string jsonWrapper = "{ \"quizzes\": " + jsonString + "}";
                    QuizListWrapper wrapper = JsonUtility.FromJson<QuizListWrapper>(jsonWrapper);

                    if (wrapper != null && wrapper.quizzes != null)
                    {
                        fetchedQuizzes = new List<QuizData>(wrapper.quizzes);
                        Debug.Log("クイズデータのパースに成功しました。取得件数: " + fetchedQuizzes.Count);

                        if (fetchedQuizzes.Count > 0)
                        {
                            DisplayQuiz();
                        }
                    }
                    else
                    {
                        Debug.LogError("JSONデータのパースに失敗しました。Wrapperまたはquizzesがnullです。");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("JSONデータのパース中に例外が発生しました: " + e.Message);
                }
            }
            else if (webRequest.responseCode == 401)
            {
                Debug.LogError("クイズデータの取得に失敗しました (401 Unauthorized): トークンが無効または期限切れです。");
                // TODO: ここでログイン画面に戻る、または再ログインを促す処理を追加
            }
            else
            {
                Debug.LogError("クイズデータの取得に失敗しました: " + webRequest.error);
            }
        }
    }

    private void DisplayQuiz()
    {
        // NullReferenceException対策: UIコンポーネントが設定されているかチェック
        if (questionText == null || optionButtons == null || optionTexts == null)
        {
            Debug.LogError("UIコンポーネント(QuestionText/Buttons/Texts)がインスペクターで設定されていません。");
            return;
        }

        if (fetchedQuizzes == null || fetchedQuizzes.Count <= currentQuizIndex)
        {
            Debug.LogError("表示するクイズがありません。");
            return;
        }

        QuizData currentQuiz = fetchedQuizzes[currentQuizIndex];

        // APIデータの選択肢数とUIボタンの数が一致するかチェック
        if (currentQuiz.options.Count != optionButtons.Length)
        {
            Debug.LogError($"エラー: APIデータの選択肢数 ({currentQuiz.options.Count}) がUIのボタン数 ({optionButtons.Length}) と一致しません。");
            return;
        }

        questionText.text = currentQuiz.question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;

            // NullReferenceException対策: 配列要素が設定されているかチェック
            if (optionButtons[index] == null)
            {
                Debug.LogError($"ボタン配列の {index} 番目がインスペクターで設定されていません。");
                return;
            }
            if (optionTexts[index] == null)
            {
                Debug.LogError($"テキスト配列の {index} 番目がインスペクターで設定されていません。");
                return;
            }

            optionTexts[index].text = currentQuiz.options[index];

            // リスナーの再登録（二重登録防止のため、一旦すべて削除）
            optionButtons[index].onClick.RemoveAllListeners();
            optionButtons[index].onClick.AddListener(() => CheckAnswer(index));

            // ボタンを活性化
            optionButtons[index].interactable = true;
        }
    }

    private void CheckAnswer(int selectedOptionIndex)
    {
        // NullReferenceException対策: resultTextが設定されているかチェック
        if (resultText == null)
        {
            Debug.LogError("ResultTextがインスペクターで設定されていません。結果表示をスキップします。");
        }

        // currentQuizIndex の範囲チェック
        if (fetchedQuizzes == null || currentQuizIndex < 0 || currentQuizIndex >= fetchedQuizzes.Count)
        {
            Debug.LogError("エラー: currentQuizIndex が範囲外です。クイズデータが不正です。Index: " + currentQuizIndex);
            return;
        }

        QuizData currentQuiz = fetchedQuizzes[currentQuizIndex];

        // selectedOptionIndex の範囲チェック
        if (selectedOptionIndex < 0 || selectedOptionIndex >= currentQuiz.options.Count)
        {
            Debug.LogError("エラー: 選択肢インデックスが範囲外です。ボタン設定またはAPIデータに問題があります。Index: " + selectedOptionIndex + ", Options Count: " + currentQuiz.options.Count);
            return;
        }

        // 回答時にボタンを無効化（二重押し防止）
        foreach (Button btn in optionButtons)
        {
            if (btn != null) btn.interactable = false;
        }

        string selectedAnswer = currentQuiz.options[selectedOptionIndex];
        string correctAnswer = currentQuiz.correctAnswer;
        bool isCorrect = (selectedAnswer == correctAnswer); // 正解フラグを定義

        // 1. 解答履歴を記録するためにQuizApiClientを呼び出し
        StartCoroutine(QuizApiClient.RegisterAnswer(currentQuiz.wordId, isCorrect));

        if (isCorrect)
        {
            Debug.Log("正解！おめでとうございます！");
            if (resultText != null)
            {
                resultText.text = "正解！";
            }
            // 正解の場合、経験値計算のために難易度をリストに追加
            correctDifficultyLevels.Add(currentQuiz.difficultyLevel);
            correctAnswerCount++;
        }
        else
        {
            Debug.Log("残念、不正解です。");
            if (resultText != null)
            {
                resultText.text = "残念、不正解";
            }
        }

        currentQuizIndex++;
        StartCoroutine(NextQuizAfterDelay(1.5f));
    }

    private IEnumerator NextQuizAfterDelay(float delay)
    {
        // メッセージ表示のため、一定時間待機
        yield return new WaitForSeconds(delay);

        // UIをリセット
        if (resultText != null) resultText.text = "";

        // index が次のクイズへ進める状態かチェック
        if (currentQuizIndex < fetchedQuizzes.Count)
        {
            DisplayQuiz(); // 次のクイズを表示
        }
        else
        {
            Debug.Log("すべてのクイズが終了しました！Resultページへ遷移します。");

            // クイズ終了時に経験値計算と結果画面遷移を行う
            yield return StartCoroutine(PostFinalScore());

            // 画面遷移の実行
            SceneManager.LoadScene(resultSceneName);
        }
    }

    // ★経験値計算と結果画面遷移のロジック (NextQuizAfterDelayから切り出し)★
    private IEnumerator PostFinalScore()
    {
        Debug.Log($"UserId: {CurrentUserId}");
        Debug.Log($"Difficulties Count: {correctDifficultyLevels.Count}");
        // 1. UIを更新
        if (questionText != null) questionText.text = "集計中...";
        if (resultText != null) resultText.text = "経験値計算中...";

        // 2. 経験値更新APIを呼び出し、結果を待機
        ExperienceUpdateResult finalResult = null;
        yield return StartCoroutine(
            QuizApiClient.UpdateExperienceStatus(
                CurrentUserId,
                correctDifficultyLevels,
                (result) => finalResult = result
            )
        );

        // 3. 結果の表示（またはResultシーンに渡す処理）
        if (finalResult != null)
        {
            
            Debug.Log($"最終獲得レベル: {finalResult.level}, レベルアップ: {finalResult.leveledUp}");
            // finalResult のデータを保存し、Resultシーンで表示できるようにする
            if (resultText != null) resultText.text = finalResult.leveledUp ? "レベルアップ！結果発表へ！" : "結果発表へ！";
            if (QuizResultDataContainer.Instance != null)
            {
                QuizResultDataContainer.Instance.SetFinalResult(correctAnswerCount, finalResult);
            }
        }
        else
        {
            Debug.LogError("経験値計算に失敗しました。");
            if (resultText != null) resultText.text = "エラー発生。結果発表へ！";
        }

        // ボタンを無効化
        if (optionButtons != null)
        {
            foreach (Button btn in optionButtons)
            {
                if (btn != null)
                {
                    btn.interactable = false;
                }
            }
        }
    }

    public List<QuizData> GetQuizzes()
    {
        return fetchedQuizzes;
    }
}
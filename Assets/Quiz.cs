using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using TMPro;
using System.Text;
using UnityEngine.SceneManagement;

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
        // ★NullReferenceException対策: UIコンポーネントが設定されているかチェック
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

            // ★NullReferenceException対策: 配列要素が設定されているかチェック
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

            // 139行目付近
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
        // ★NullReferenceException対策: resultTextが設定されているかチェック
        // (169行目付近でresultTextを使用している可能性があるため)
        if (resultText == null)
        {
            Debug.LogError("ResultTextがインスペクターで設定されていません。結果表示をスキップします。");
            // エラーログを出して処理は継続
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

        if (selectedAnswer == correctAnswer)
        {
            Debug.Log("正解！おめでとうございます！");
            if (resultText != null) // 169行目付近
            {
                resultText.text = "正解！";
            }
            currentQuizIndex++;

        }
        else
        {
            Debug.Log("残念、不正解です。");
            if (resultText != null) // 169行目付近
            {
                resultText.text = "残念、不正解";
            }
            currentQuizIndex++;
        }
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

            // 1. 終了メッセージを表示
            if (questionText != null) questionText.text = "クイズ終了！";
            if (resultText != null) resultText.text = "結果発表へ！";

            // 2. ボタンを無効化（誤操作防止）
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

            // 3. 画面遷移の実行
            SceneManager.LoadScene(resultSceneName);
        }
    }

    public List<QuizData> GetQuizzes()
    {
        return fetchedQuizzes;
    }
}
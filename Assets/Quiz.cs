using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using TMPro;
// using System.Runtime.InteropServices; // JWT認証に切り替えたため、このインポートは不要

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

    public TextMeshProUGUI questionText;
    public Button[] optionButtons; // 4つの選択肢ボタン
    public TextMeshProUGUI[] optionTexts; // 各ボタンのテキスト

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
        if (fetchedQuizzes == null || fetchedQuizzes.Count <= currentQuizIndex)
        {
            Debug.LogError("表示するクイズがありません。");
            return;
        }

        QuizData currentQuiz = fetchedQuizzes[currentQuizIndex];

        // ★修正1: optionsの数がUIボタンの数と一致するかチェック★
        if (currentQuiz.options.Count != optionButtons.Length)
        {
            Debug.LogError($"エラー: APIデータの選択肢数 ({currentQuiz.options.Count}) がUIのボタン数 ({optionButtons.Length}) と一致しません。");
            // 致命的なエラーなので、ここでクイズを終了させても良い
            return;
        }

        questionText.text = currentQuiz.question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;

            // optionsの数はチェック済みなので、ここでは安全
            optionTexts[index].text = currentQuiz.options[index];

            // 修正箇所：リスナーの登録方法
            optionButtons[index].onClick.RemoveAllListeners();
            // CheckAnswerメソッドを引数付きで呼び出すための修正
            optionButtons[index].onClick.AddListener(() => CheckAnswer(index));
        }
    }

    // 修正箇所：CheckAnswerメソッドの引数を変更
    private void CheckAnswer(int selectedOptionIndex)
    {
        // ★修正2: currentQuizIndex の範囲チェック★
        if (fetchedQuizzes == null || currentQuizIndex < 0 || currentQuizIndex >= fetchedQuizzes.Count)
        {
            Debug.LogError("エラー: currentQuizIndex が範囲外です。クイズデータが不正です。Index: " + currentQuizIndex);
            return;
        }

        QuizData currentQuiz = fetchedQuizzes[currentQuizIndex];

        // ★修正3: selectedOptionIndex の範囲チェック★
        if (selectedOptionIndex < 0 || selectedOptionIndex >= currentQuiz.options.Count)
        {
            Debug.LogError("エラー: 選択肢インデックスが範囲外です。ボタン設定またはAPIデータに問題があります。Index: " + selectedOptionIndex + ", Options Count: " + currentQuiz.options.Count);
            return;
        }

        // ここ（122行目付近）が安全になります
        string selectedAnswer = currentQuiz.options[selectedOptionIndex];
        string correctAnswer = currentQuiz.correctAnswer;

        if (selectedAnswer == correctAnswer)
        {
            Debug.Log("正解！おめでとうございます！");
            currentQuizIndex++;
            if (currentQuizIndex < fetchedQuizzes.Count)
            {
                DisplayQuiz();
            }
            else
            {
                Debug.Log("すべてのクイズが終了しました！");
            }
        }
        else
        {
            Debug.Log("残念、不正解です。");
        }
    }

    public List<QuizData> GetQuizzes()
    {
        return fetchedQuizzes;
    }
}
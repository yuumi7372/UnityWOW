using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using TMPro;

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

    // クイズデータを取得する公開メソッド
    public void FetchQuizzes()
    {
        StartCoroutine(GetQuizData());
        Debug.Log("クイズデータの取得を開始します。");
    }

    private IEnumerator GetQuizData()
    {
        using (UnityWebRequest webRequest = ApiClient.CreateGet("unity_question"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonString = webRequest.downloadHandler.text;
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
        questionText.text = currentQuiz.question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
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
        string selectedAnswer = fetchedQuizzes[currentQuizIndex].options[selectedOptionIndex];
        string correctAnswer = fetchedQuizzes[currentQuizIndex].correctAnswer;

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
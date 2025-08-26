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
    // BigIntはJSONの仕様上stringで受け取る
    public string wordId;
    public string question;
    public List<string> options;
    public string correctAnswer;
    public int difficultyLevel;
}

// JSON配列をデシリアライズするために一時的に使用するラッパークラス
[Serializable]
class QuizListWrapper
{
    public QuizData[] quizzes;
}

public class Quiz : MonoBehaviour
{
    // APIのエンドポイントURL
    // ここにあなたのバックエンドAPIのURLを設定してください
    private const string API_URL = "http://localhost:3000/api/unity_question";

    // 取得したクイズデータを格納するリスト
    private List<QuizData> fetchedQuizzes;

    public TextMeshProUGUI questionText;
    public Button[] optionButtons; // 4つの選択肢ボタン
    public TextMeshProUGUI[] optionTexts; // 各ボタンのテキスト

    
    private int currentQuizIndex = 0;


    // クイズデータを取得する公開メソッド
    // このメソッドをUIボタンのクリックイベントなどから呼び出します
    public void FetchQuizzes()
    {
        StartCoroutine(GetQuizData());
        Debug.Log("成功");
    }

    // 実際のAPIリクエストを行うコルーチン
    private IEnumerator GetQuizData()
{
    using (UnityWebRequest webRequest = UnityWebRequest.Get(API_URL))
    {
        // PlayerPrefsから保存しておいたトークンを取得
        string token = PlayerPrefs.GetString("jwt_token");

        // トークンがあればAuthorizationヘッダーに追加
        if (!string.IsNullOrEmpty(token))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
        }

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // 成功したときの処理...
            Debug.Log("クイズデータの取得に成功しました！");
        }
        else
        {
            // 失敗したときの処理...
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
            // ボタンに選択肢テキストを設定
            optionTexts[i].text = currentQuiz.options[i];

            // ボタンがクリックされたときの処理を設定
            // 既存のリスナーをクリアしてから追加する
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => CheckAnswer(optionTexts[i].text));
        }
    }

    private void CheckAnswer(string selectedAnswer)
    {
        string correctAnswer = fetchedQuizzes[currentQuizIndex].correctAnswer;
        if (selectedAnswer == correctAnswer)
        {
            Debug.Log("正解！おめでとうございます！");
            // 正解時の処理（スコア加算、次の問題へ進むなど）
            currentQuizIndex++;
            DisplayQuiz();
        }
        else
        {
            Debug.Log("残念、不正解です。");
            // 不正解時の処理
        }
    }

    // 他のスクリプトからクイズデータにアクセスするためのメソッド
    public List<QuizData> GetQuizzes()
    {
        return fetchedQuizzes;
    }
}

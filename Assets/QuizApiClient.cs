using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

// =================================================================
// 通信用データ構造 (Express APIの入出力に合わせています)
// =================================================================

// --- 解答履歴記録用 (POST /api/quiz-register) ---
[System.Serializable]
public class AnswerRegistrationData
{
    public int wordId;
    public bool isCorrect;
}

// --- 経験値更新用 (POST /api/exp-status) ---
[System.Serializable]
public class ExperienceUpdateData
{
    // Express側がbodyからuserIdを取得するため、文字列で含める
    public string userId;
    public List<int> correctDifficulties;
}

// --- 経験値更新のレスポンス構造 ---
[System.Serializable]
public class ExperienceUpdateResult
{
    public int userId;
    public int level;
    public int experience;
    public bool leveledUp;
    public string newCharacterImage;
    public int totalExperienceGained;
}

// --- クイズ取得用 (GET /api/question) ---
// Quiz.cs に既に定義されている QuizData と QuizListWrapper を使用することを前提とします。


// =================================================================
// APIクライアント本体
// =================================================================

public static class QuizApiClient
{
    // ★ ExpressサーバーのBase URLを設定してください ★
    private const string BaseUrl = "http://localhost:3000/api/";


    private static void SetCommonHeaders(UnityWebRequest webRequest, bool isJson = false)
    {
        string token = PlayerPrefs.GetString("token", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
        }
        if (isJson)
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }
    }

    // --- 1. 解答履歴の記録 (POST /api/quiz-register) ---
    public static IEnumerator RegisterAnswer(
        int wordId,
        bool isCorrect,
        Action<bool> onComplete = null)
    {
        string url = BaseUrl + "quiz-register";
        AnswerRegistrationData data = new AnswerRegistrationData { wordId = wordId, isCorrect = isCorrect };
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            SetCommonHeaders(webRequest, true);

            yield return webRequest.SendWebRequest();

            bool success = webRequest.result == UnityWebRequest.Result.Success;

            if (!success)
            {
                Debug.LogError($"解答履歴の記録に失敗しました: {webRequest.error} - {webRequest.downloadHandler.text}");
            }

            onComplete?.Invoke(success);
        }
    }

    // --- 2. 経験値の更新とレベルアップ (POST /api/exp-status) ---
    public static IEnumerator UpdateExperienceStatus(
        string userId,
        List<int> correctDifficulties,
        Action<ExperienceUpdateResult> onComplete = null)
    {
        string url = BaseUrl + "exp-status";
        ExperienceUpdateData data = new ExperienceUpdateData { userId = userId, correctDifficulties = correctDifficulties };
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            SetCommonHeaders(webRequest, true);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                ExperienceUpdateResult result = JsonUtility.FromJson<ExperienceUpdateResult>(jsonResponse);
                onComplete?.Invoke(result);
            }
            else
            {
                Debug.LogError($"経験値の更新に失敗しました: {webRequest.error} - {webRequest.downloadHandler.text}");
                onComplete?.Invoke(null);
            }
        }
    }

    // --- 3. クイズデータ取得 (GET /api/question) ---
    // ※ この処理は、Quiz.csのGetQuizData()内で直接UnityWebRequestを使っているため、
    // 静的メソッドとしてQuizApiClientに切り出すことも可能ですが、ここではQuiz.csの元の実装を維持します。
}
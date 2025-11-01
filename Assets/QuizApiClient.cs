using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

[System.Serializable]
public class AnswerRegistrationData
{
    public int wordId;
    public bool isCorrect;
}

[System.Serializable]
public class ExperienceUpdateData
{
    public string userId;
    public List<int> correctDifficulties;
}

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


public static class QuizApiClient
{
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
                Debug.LogError($"Answer registration failed: {webRequest.error} - {webRequest.downloadHandler.text}");
            }

            onComplete?.Invoke(success);
        }
    }

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
                Debug.LogError($"Experience update failed: {webRequest.error} - {webRequest.downloadHandler.text}");
                onComplete?.Invoke(null);
            }
        }
    }

}
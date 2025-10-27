using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

// =================================================================
// �ʐM�p�f�[�^�\�� (Express API�̓��o�͂ɍ��킹�Ă��܂�)
// =================================================================

// --- �𓚗����L�^�p (POST /api/quiz-register) ---
[System.Serializable]
public class AnswerRegistrationData
{
    public int wordId;
    public bool isCorrect;
}

// --- �o���l�X�V�p (POST /api/exp-status) ---
[System.Serializable]
public class ExperienceUpdateData
{
    // Express����body����userId���擾���邽�߁A������Ŋ܂߂�
    public string userId;
    public List<int> correctDifficulties;
}

// --- �o���l�X�V�̃��X�|���X�\�� ---
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

// --- �N�C�Y�擾�p (GET /api/question) ---
// Quiz.cs �Ɋ��ɒ�`����Ă��� QuizData �� QuizListWrapper ���g�p���邱�Ƃ�O��Ƃ��܂��B


// =================================================================
// API�N���C�A���g�{��
// =================================================================

public static class QuizApiClient
{
    // �� Express�T�[�o�[��Base URL��ݒ肵�Ă������� ��
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

    // --- 1. �𓚗����̋L�^ (POST /api/quiz-register) ---
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
                Debug.LogError($"�𓚗����̋L�^�Ɏ��s���܂���: {webRequest.error} - {webRequest.downloadHandler.text}");
            }

            onComplete?.Invoke(success);
        }
    }

    // --- 2. �o���l�̍X�V�ƃ��x���A�b�v (POST /api/exp-status) ---
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
                Debug.LogError($"�o���l�̍X�V�Ɏ��s���܂���: {webRequest.error} - {webRequest.downloadHandler.text}");
                onComplete?.Invoke(null);
            }
        }
    }

    // --- 3. �N�C�Y�f�[�^�擾 (GET /api/question) ---
    // �� ���̏����́AQuiz.cs��GetQuizData()���Œ���UnityWebRequest���g���Ă��邽�߁A
    // �ÓI���\�b�h�Ƃ���QuizApiClient�ɐ؂�o�����Ƃ��\�ł����A�����ł�Quiz.cs�̌��̎������ێ����܂��B
}
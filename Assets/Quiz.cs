using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using TMPro;

// API����Ԃ��Ă���N�C�Y�f�[�^�̒P�̍\�����`
[Serializable]
public class QuizData
{
    // JSON��wordId�����l�^�ł��邽�߁Aint�ɕύX
    public int wordId;
    public string question;
    public List<string> options;
    public string correctAnswer;
    public int difficultyLevel;
}

// JSON�z����f�V���A���C�Y���邽�߂Ɉꎞ�I�Ɏg�p���郉�b�p�[�N���X
[Serializable]
public class QuizListWrapper
{
    public QuizData[] quizzes;
}

public class Quiz : MonoBehaviour
{
    // �擾�����N�C�Y�f�[�^���i�[���郊�X�g
    private List<QuizData> fetchedQuizzes;

    public TextMeshProUGUI questionText;
    public Button[] optionButtons; // 4�̑I�����{�^��
    public TextMeshProUGUI[] optionTexts; // �e�{�^���̃e�L�X�g

    private int currentQuizIndex = 0;

    // �N�C�Y�f�[�^���擾������J���\�b�h
    public void FetchQuizzes()
    {
        StartCoroutine(GetQuizData());
        Debug.Log("�N�C�Y�f�[�^�̎擾���J�n���܂��B");
    }

    private IEnumerator GetQuizData()
    {
        using (UnityWebRequest webRequest = ApiClient.CreateGet("unity_question"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonString = webRequest.downloadHandler.text;
                Debug.Log("�N�C�Y�f�[�^�̎擾�ɐ������܂����I");
                Debug.Log("�擾����JSON�f�[�^: " + jsonString);

                try
                {
                    string jsonWrapper = "{ \"quizzes\": " + jsonString + "}";
                    QuizListWrapper wrapper = JsonUtility.FromJson<QuizListWrapper>(jsonWrapper);

                    if (wrapper != null && wrapper.quizzes != null)
                    {
                        fetchedQuizzes = new List<QuizData>(wrapper.quizzes);
                        Debug.Log("�N�C�Y�f�[�^�̃p�[�X�ɐ������܂����B�擾����: " + fetchedQuizzes.Count);

                        if (fetchedQuizzes.Count > 0)
                        {
                            DisplayQuiz();
                        }
                    }
                    else
                    {
                        Debug.LogError("JSON�f�[�^�̃p�[�X�Ɏ��s���܂����BWrapper�܂���quizzes��null�ł��B");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("JSON�f�[�^�̃p�[�X���ɗ�O���������܂���: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("�N�C�Y�f�[�^�̎擾�Ɏ��s���܂���: " + webRequest.error);
            }
        }
    }

    private void DisplayQuiz()
    {
        if (fetchedQuizzes == null || fetchedQuizzes.Count <= currentQuizIndex)
        {
            Debug.LogError("�\������N�C�Y������܂���B");
            return;
        }

        QuizData currentQuiz = fetchedQuizzes[currentQuizIndex];
        questionText.text = currentQuiz.question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionTexts[index].text = currentQuiz.options[index];

            // �C���ӏ��F���X�i�[�̓o�^���@
            optionButtons[index].onClick.RemoveAllListeners();
            // CheckAnswer���\�b�h�������t���ŌĂяo�����߂̏C��
            optionButtons[index].onClick.AddListener(() => CheckAnswer(index));
        }
    }

    // �C���ӏ��FCheckAnswer���\�b�h�̈�����ύX
    private void CheckAnswer(int selectedOptionIndex)
    {
        string selectedAnswer = fetchedQuizzes[currentQuizIndex].options[selectedOptionIndex];
        string correctAnswer = fetchedQuizzes[currentQuizIndex].correctAnswer;

        if (selectedAnswer == correctAnswer)
        {
            Debug.Log("�����I���߂łƂ��������܂��I");
            currentQuizIndex++;
            if (currentQuizIndex < fetchedQuizzes.Count)
            {
                DisplayQuiz();
            }
            else
            {
                Debug.Log("���ׂẴN�C�Y���I�����܂����I");
            }
        }
        else
        {
            Debug.Log("�c�O�A�s�����ł��B");
        }
    }

    public List<QuizData> GetQuizzes()
    {
        return fetchedQuizzes;
    }
}
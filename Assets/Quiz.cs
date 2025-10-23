using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using TMPro;
// using System.Runtime.InteropServices; // JWT�F�؂ɐ؂�ւ������߁A���̃C���|�[�g�͕s�v

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

    void Start()
    {
        // ���ۂɂ̓��O�C���������FetchQuizzes()���Ăяo���ׂ��ł����A�e�X�g�̂���Start�Ɏc���܂��B
        FetchQuizzes();
    }

    // �N�C�Y�f�[�^���擾������J���\�b�h
    public void FetchQuizzes()
    {
        StartCoroutine(GetQuizData());
        Debug.Log("�N�C�Y�f�[�^�̎擾���J�n���܂��B");
    }

    private IEnumerator GetQuizData()
    {
        // ApiClient��JWT�g�[�N���������Ńw�b�_�[�ɒǉ����邱�Ƃ�O��Ƃ��܂��B
        using (UnityWebRequest webRequest = ApiClient.CreateGet("question"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // ���������΍�iUTF-8�f�R�[�h�j���ێ�
                byte[] data = webRequest.downloadHandler.data;
                string jsonString = System.Text.Encoding.UTF8.GetString(data);
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
            else if (webRequest.responseCode == 401)
            {
                Debug.LogError("�N�C�Y�f�[�^�̎擾�Ɏ��s���܂��� (401 Unauthorized): �g�[�N���������܂��͊����؂�ł��B");
                // TODO: �����Ń��O�C����ʂɖ߂�A�܂��͍ă��O�C���𑣂�������ǉ�
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

        // ���C��1: options�̐���UI�{�^���̐��ƈ�v���邩�`�F�b�N��
        if (currentQuiz.options.Count != optionButtons.Length)
        {
            Debug.LogError($"�G���[: API�f�[�^�̑I������ ({currentQuiz.options.Count}) ��UI�̃{�^���� ({optionButtons.Length}) �ƈ�v���܂���B");
            // �v���I�ȃG���[�Ȃ̂ŁA�����ŃN�C�Y���I�������Ă��ǂ�
            return;
        }

        questionText.text = currentQuiz.question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;

            // options�̐��̓`�F�b�N�ς݂Ȃ̂ŁA�����ł͈��S
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
        // ���C��2: currentQuizIndex �͈̔̓`�F�b�N��
        if (fetchedQuizzes == null || currentQuizIndex < 0 || currentQuizIndex >= fetchedQuizzes.Count)
        {
            Debug.LogError("�G���[: currentQuizIndex ���͈͊O�ł��B�N�C�Y�f�[�^���s���ł��BIndex: " + currentQuizIndex);
            return;
        }

        QuizData currentQuiz = fetchedQuizzes[currentQuizIndex];

        // ���C��3: selectedOptionIndex �͈̔̓`�F�b�N��
        if (selectedOptionIndex < 0 || selectedOptionIndex >= currentQuiz.options.Count)
        {
            Debug.LogError("�G���[: �I�����C���f�b�N�X���͈͊O�ł��B�{�^���ݒ�܂���API�f�[�^�ɖ�肪����܂��BIndex: " + selectedOptionIndex + ", Options Count: " + currentQuiz.options.Count);
            return;
        }

        // �����i122�s�ڕt�߁j�����S�ɂȂ�܂�
        string selectedAnswer = currentQuiz.options[selectedOptionIndex];
        string correctAnswer = currentQuiz.correctAnswer;

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
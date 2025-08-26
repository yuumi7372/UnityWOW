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
    // BigInt��JSON�̎d�l��string�Ŏ󂯎��
    public string wordId;
    public string question;
    public List<string> options;
    public string correctAnswer;
    public int difficultyLevel;
}

// JSON�z����f�V���A���C�Y���邽�߂Ɉꎞ�I�Ɏg�p���郉�b�p�[�N���X
[Serializable]
class QuizListWrapper
{
    public QuizData[] quizzes;
}

public class Quiz : MonoBehaviour
{
    // API�̃G���h�|�C���gURL
    // �����ɂ��Ȃ��̃o�b�N�G���hAPI��URL��ݒ肵�Ă�������
    private const string API_URL = "http://localhost:3000/api/unity_question";

    // �擾�����N�C�Y�f�[�^���i�[���郊�X�g
    private List<QuizData> fetchedQuizzes;

    public TextMeshProUGUI questionText;
    public Button[] optionButtons; // 4�̑I�����{�^��
    public TextMeshProUGUI[] optionTexts; // �e�{�^���̃e�L�X�g

    
    private int currentQuizIndex = 0;


    // �N�C�Y�f�[�^���擾������J���\�b�h
    // ���̃��\�b�h��UI�{�^���̃N���b�N�C�x���g�Ȃǂ���Ăяo���܂�
    public void FetchQuizzes()
    {
        StartCoroutine(GetQuizData());
        Debug.Log("����");
    }

    // ���ۂ�API���N�G�X�g���s���R���[�`��
    private IEnumerator GetQuizData()
{
    using (UnityWebRequest webRequest = UnityWebRequest.Get(API_URL))
    {
        // PlayerPrefs����ۑ����Ă������g�[�N�����擾
        string token = PlayerPrefs.GetString("jwt_token");

        // �g�[�N���������Authorization�w�b�_�[�ɒǉ�
        if (!string.IsNullOrEmpty(token))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
        }

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // ���������Ƃ��̏���...
            Debug.Log("�N�C�Y�f�[�^�̎擾�ɐ������܂����I");
        }
        else
        {
            // ���s�����Ƃ��̏���...
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
            // �{�^���ɑI�����e�L�X�g��ݒ�
            optionTexts[i].text = currentQuiz.options[i];

            // �{�^�����N���b�N���ꂽ�Ƃ��̏�����ݒ�
            // �����̃��X�i�[���N���A���Ă���ǉ�����
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => CheckAnswer(optionTexts[i].text));
        }
    }

    private void CheckAnswer(string selectedAnswer)
    {
        string correctAnswer = fetchedQuizzes[currentQuizIndex].correctAnswer;
        if (selectedAnswer == correctAnswer)
        {
            Debug.Log("�����I���߂łƂ��������܂��I");
            // �������̏����i�X�R�A���Z�A���̖��֐i�ނȂǁj
            currentQuizIndex++;
            DisplayQuiz();
        }
        else
        {
            Debug.Log("�c�O�A�s�����ł��B");
            // �s�������̏���
        }
    }

    // ���̃X�N���v�g����N�C�Y�f�[�^�ɃA�N�Z�X���邽�߂̃��\�b�h
    public List<QuizData> GetQuizzes()
    {
        return fetchedQuizzes;
    }
}

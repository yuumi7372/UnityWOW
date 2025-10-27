using UnityEngine;

// QuizApiClient.cs �Œ�`���� ExperienceUpdateResult �N���X���K�v�ɂȂ�܂��B
// �K�v�ɉ����āAExperienceUpdateResult�̒�`�������t�@�C���ɃR�s�[���邩�A
// �ʓr�쐬�����t�@�C������using�f�B���N�e�B�u�ŎQ�Ƃł���悤�ɂ��Ă��������B

public class QuizResultDataContainer : MonoBehaviour
{
    // �V���O���g���̃C���X�^���X
    public static QuizResultDataContainer Instance { get; private set; }

    // 1. ����
    public int CorrectAnswerCount { get; private set; } = 0;

    // 2. API����̌o���l/���x�����i�l���o���l�A���o���l�A���x���A���x���A�b�v������܂ށj
    public ExperienceUpdateResult FinalExperienceResult { get; private set; } = null;

    // �N�C�Y���Ō�܂Ńv���C����ăf�[�^���i�[���ꂽ���������t���O
    public bool IsQuizFinished { get; private set; } = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // �V�[�����ׂ��ł��j������Ȃ��悤�ɐݒ�
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ���ɃC���X�^���X�����݂���ꍇ�́A�V�����I�u�W�F�N�g��j������
            Destroy(gameObject);
        }
    }

    /**
     * Quiz.cs �� PostFinalScore() �ŌĂяo����A���ʂ��i�[����
     * @param correctCount �ŏI�I�Ȑ���
     * @param expResult �o���l�X�VAPI����̉����f�[�^
     */
    public void SetFinalResult(int correctCount, ExperienceUpdateResult expResult)
    {
        CorrectAnswerCount = correctCount;
        FinalExperienceResult = expResult;
        IsQuizFinished = true;
        Debug.Log($"�R���e�i�Ɍ��ʂ�ۑ����܂����B����: {correctCount}, �l��EXP: {expResult?.totalExperienceGained}");
    }

    /**
     * ���ʕ\����Ƀf�[�^�����Z�b�g���A���̃N�C�Y�ɔ�����
     */
    public void ResetData()
    {
        CorrectAnswerCount = 0;
        FinalExperienceResult = null;
        IsQuizFinished = false;
        Debug.Log("QuizResultDataContainer�̃f�[�^�����Z�b�g���܂����B");
    }
}
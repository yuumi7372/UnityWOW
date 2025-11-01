using UnityEngine;


public class QuizResultDataContainer : MonoBehaviour
{
    public static QuizResultDataContainer Instance { get; private set; }

    public int CorrectAnswerCount { get; private set; } = 0;

    public ExperienceUpdateResult FinalExperienceResult { get; private set; } = null;

    public bool IsQuizFinished { get; private set; } = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

     public void SetFinalResult(int correctCount, ExperienceUpdateResult expResult)
    {
        CorrectAnswerCount = correctCount;
        FinalExperienceResult = expResult;
        IsQuizFinished = true;

        }

     public void ResetData()
    {
        CorrectAnswerCount = 0;
        FinalExperienceResult = null;
        IsQuizFinished = false;
        
    }
}
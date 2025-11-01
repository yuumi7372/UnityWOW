using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PagesButton : MonoBehaviour
{

    public void OnClickQuestbutton()
    {
        SceneManager.LoadScene("quest");
    }

    public void OnClickAdventurebutton()
    {
        SceneManager.LoadScene("adventure_home");
    }

    public void OnClickProfilebutton()
    {
        SceneManager.LoadScene("profile");
    }

    public void OnClickAddWordsbutton()
    {
        SceneManager.LoadScene("word");
    }
    public void onClickGotohomebutton()
    {
        SceneManager.LoadScene("home");
    }
    public void onClickGotoQuizbutton(){
        SceneManager.LoadScene("gotoQuiz");
    }
    public void onClickranking()
    {
        SceneManager.LoadScene("ranking");
    }
    public void onClickCamerabutton()
    {
        SceneManager.LoadScene("camera");
    }

}
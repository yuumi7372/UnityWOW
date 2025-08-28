using UnityEngine;
using UnityEngine.SceneManagement;

public class PagesButton : MonoBehaviour
{
    public GameObject startButton;
    public GameObject loginPanel;


    public void OnClickStartbutton()
    {
        // ログイン済みかチェック
        int isLoggedIn = PlayerPrefs.GetInt("isLoggedIn", 0);

        if (isLoggedIn == 1)
        {
            // すでにログイン済み → ゲームのホームページへ
            SceneManager.LoadScene("home");
        }
        else
        {
            // 未ログイン → ログインUIを表示
            startButton.SetActive(false);   // スタートボタンを非表示
            loginPanel.SetActive(true);     // ログインパネルを表示
        }
    }

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

   
}
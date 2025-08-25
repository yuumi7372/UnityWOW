using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject startButton;
    public GameObject loginPanel;

    public void Logout()
    {
        // トークン削除
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.SetInt("isLoggedIn", 0); // ログアウト状態に

        // デバッグ表示
        Debug.Log("ログアウト完了にょ💔");

        // タイトルシーンに戻す
        SceneManager.LoadScene("title");
    }
}

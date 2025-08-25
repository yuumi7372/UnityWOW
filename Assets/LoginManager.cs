using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public PagesButton pagesButton;

    [System.Serializable]
    public class LoginResponse
    {
        public string token;
        public string username;
        public int level;
    }
    void Start()
    {
        loginButton.onClick.AddListener(() => StartCoroutine(Login()));
    }
    
    public IEnumerator Login()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        var form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/api/unity_login", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("レスポンスにょ💛: " + json);

                // JSONをクラスに変換
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(json);

                // デバッグ確認
                Debug.Log("ログイン成功: " + response.username + " (Lv. " + response.level + ")");

                // PlayerPrefsに保存
                PlayerPrefs.SetString("token", response.token);
                PlayerPrefs.SetString("username", response.username);
                PlayerPrefs.SetInt("level", response.level);

                // ログイン済みフラグを保存
                PlayerPrefs.SetInt("isLoggedIn", 1);

                // ログインパネルを閉じてスタートボタンを再表示
                pagesButton.startButton.SetActive(true);
                pagesButton.loginPanel.SetActive(false);
            }
            else
            {
                string errorMessage = www.downloadHandler.text;
                Debug.Log("ログイン失敗にょ💔: " + errorMessage);
            }
        }
    }
}

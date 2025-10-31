using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text; // ★追加: エンコーディング用★

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
        public string userId;
    }

    // ★追加: JSON送信用のデータ構造★
    [System.Serializable]
    public class LoginData
    {
        public string email;
        public string password;
    }
    // ------------------------------------

    void Start()
    {
        loginButton.onClick.AddListener(() => StartCoroutine(Login()));
    }

    public IEnumerator Login()
    {
        // 1. 送信用データをオブジェクトに格納
        LoginData data = new LoginData
        {
            email = emailInput.text,
            password = passwordInput.text
        };

        // 2. オブジェクトをJSON文字列に変換
        string jsonPayload = JsonUtility.ToJson(data);

        // 3. UnityWebRequestを作成 (Postメソッドを指定)
        // URLは Express の設定に合わせて "/api/login" にしておきます
        using (UnityWebRequest www = new UnityWebRequest("http://localhost:3000/api/login", "POST"))
        {
            // 4. JSONデータをバイト配列に変換してアップロードハンドラーに設定
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);

            // 5. ダウンロードハンドラーも設定
            www.downloadHandler = new DownloadHandlerBuffer();

            // 6. ヘッダーを "application/json" に設定 (★最重要★)
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("レスポンスにょ💛: " + json);

                // JSONをクラスに変換
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(json);
                Debug.Log("取得したユーザーID: " + response.userId + " / トークン: " + response.token);

                // デバッグ確認
                Debug.Log("ログイン成功: " + response.username + " (Lv. " + response.level + ")");

                // PlayerPrefsに保存 (クイズAPIで必要)
                PlayerPrefs.SetString("token", response.token);
                PlayerPrefs.SetString("username", response.username);
                PlayerPrefs.SetInt("level", response.level);
                PlayerPrefs.SetString("userId", response.userId);

                PlayerPrefs.SetInt("isLoggedIn", 1);

                // ログインパネルを閉じてスタートボタンを再表示
                pagesButton.startButton.SetActive(true);
                pagesButton.loginPanel.SetActive(false);
            }
            else
            {
                string errorMessage = www.downloadHandler.text;
                Debug.Log("ログイン失敗にょ💔: " + www.responseCode + ": " + errorMessage);
            }
        }
    }
}
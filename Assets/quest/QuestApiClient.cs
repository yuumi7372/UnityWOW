// QuestApiClient.cs
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class QuestApiClient : MonoBehaviour
{
    // サーバーのベースURL (例: http://localhost:3000)
    private const string API_BASE_URL = "http://localhost:3000"; 
    
    // ログインなどで取得したJWTトークン
    public string jwtToken
    {
        get
        {
            // PlayerPrefs に保存された "token" の値を取得する
            return PlayerPrefs.GetString("token", "");
        }
        set
        {
            // Setは使用しませんが、プロパティとして定義しておきます
            Debug.LogWarning("QuestApiClient.jwtToken は PlayerPrefs から自動取得されます。設定はLoginManagerで行ってください。");
        }
    }
    // エラーハンドリングのためのデリゲート
    public delegate void OnRequestFailed(string errorMessage);

    // ===============================================
    // 汎用リクエスト送信コルーチン (GET)
    // ===============================================
    private IEnumerator SendGetRequest<T>(string endpoint, System.Action<T> onSuccess, OnRequestFailed onFailure)
    {
        string url = $"{API_BASE_URL}{endpoint}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // JWT認証トークンをAuthorizationヘッダーに設定 (Bearerスキーマ)
            webRequest.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // JSONデータをデシリアライズ
                T responseData = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                onSuccess?.Invoke(responseData);
            }
            else
            {
                string error = $"API Request Failed: {webRequest.error}. Response: {webRequest.downloadHandler.text}";
                Debug.LogError(error);
                onFailure?.Invoke(error);
            }
        }
    }

    // ===============================================
    // 汎用リクエスト送信コルーチン (POST - JSON)
    // ===============================================
    private IEnumerator SendPostRequest<TResponse, TRequest>(string endpoint, TRequest requestBody, System.Action<TResponse> onSuccess, OnRequestFailed onFailure)
    {
        string url = $"{API_BASE_URL}{endpoint}";
        string json = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            // UnityWebRequest.Post(url, json) は application/x-www-form-urlencoded としてエンコードしようとするため、
            // JSONを送信するには UploadHandlerRaw を使って手動で設定するのが確実です。
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            
            // JWT認証トークンをAuthorizationヘッダーに設定
            webRequest.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            webRequest.SetRequestHeader("Content-Type", "application/json"); // JSONであることを明示
            webRequest.SetRequestHeader("Accept", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                TResponse responseData = JsonUtility.FromJson<TResponse>(webRequest.downloadHandler.text);
                onSuccess?.Invoke(responseData);
            }
            else
            {
                string error = $"API Request Failed: {webRequest.error}. Response: {webRequest.downloadHandler.text}";
                Debug.LogError(error);
                onFailure?.Invoke(error);
            }
        }
    }

    // ===============================================
    // 1. /quest/start の呼び出し
    // ===============================================
    public void StartQuest(System.Action<StartQuestResponse> onSuccess, OnRequestFailed onFailure)
    {
        StartCoroutine(SendGetRequest("/quest/start", onSuccess, onFailure));
    }

    // ===============================================
    // 2. /quest/answer の呼び出し
    // ===============================================
    public void AnswerQuest(int sessionId, int wordId, string userAnswer, bool isCorrect, System.Action<AnswerQuestResponse> onSuccess, OnRequestFailed onFailure)
    {
        var requestBody = new AnswerQuestRequest
        {
            questSessionId = sessionId,
            wordId = wordId,
            userAnswer = userAnswer,
            isCorrect = isCorrect,
            // userCurrentHp はサーバー側で管理するため、ここでは省略（必要に応じて追加）
        };
        
        StartCoroutine(SendPostRequest("/quest/answer", requestBody, onSuccess, onFailure));
    }
    
    // ===============================================
    // 3. /quest/result の呼び出し
    // ===============================================
    public void GetQuestResult(int sessionId, System.Action<string> onSuccess, OnRequestFailed onFailure)
    {
        // クエリパラメータを使用
        StartCoroutine(SendGetRequest($"/quest/result?questSessionId={sessionId}", onSuccess, onFailure));
    }
    
    // Note: GetQuestResultのレスポンス構造体は、長くなるためここでは省略し、stringとして受け取る例としています。
    // 必要に応じて AnswerQuestResponse のように専用のクラスを作成してください。
}
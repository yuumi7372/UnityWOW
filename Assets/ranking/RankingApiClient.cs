// RankingApiClient.cs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

// Note: QuestApiClient との混同を避けるため、別ファイルまたは QuestApiClient にメソッドを追加しても良いですが、
// ここでは独立したクライアントとして作成します。

public class RankingApiClient : MonoBehaviour
{
    private const string API_BASE_URL = "http://localhost:3000";

    public string jwtToken
    {
        get { return PlayerPrefs.GetString("token", ""); }
    }

    public delegate void OnRequestFailed(string errorMessage);

    /// <summary>
    /// ランキングデータを取得し、デシリアライズします。
    /// </summary>
    public void FetchRanking(Action<RankingItem[]> onSuccess, OnRequestFailed onFailure)
    {
        // 認証が不要なAPIである場合は、authenticateToken ミドルウェアを外してください。
        // ここでは、ランキング取得には認証が不要であると仮定し、トークンチェックはスキップします。

        StartCoroutine(SendGetRankingRequest(onSuccess, onFailure));
    }

    private IEnumerator SendGetRankingRequest(Action<RankingItem[]> onSuccess, OnRequestFailed onFailure)
    {
        // Expressで /ranking ルートに設定したことを想定
        string url = $"{API_BASE_URL}/ranking";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // ?? ランキングAPIに認証が必要な場合は、以下の行をコメント解除してください
            // if (!string.IsNullOrEmpty(jwtToken)) {
            //     webRequest.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            // }

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonString = webRequest.downloadHandler.text;
                Debug.Log("ランキングJSON取得成功: " + jsonString);

                try
                {
                    // JSONUtilityで配列をデシリアライズするため、ラッパーを使用します
                    string wrappedJson = "{ \"ranking\": " + jsonString + "}";
                    RankingListWrapper wrapper = JsonUtility.FromJson<RankingListWrapper>(wrappedJson);

                    if (wrapper != null && wrapper.ranking != null)
                    {
                        onSuccess?.Invoke(wrapper.ranking);
                    }
                    else
                    {
                        throw new Exception("ランキングデータのパースに失敗しました。");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("ランキングパースエラー: " + e.Message);
                    onFailure?.Invoke("ランキングデータの形式エラー: " + e.Message);
                }
            }
            else
            {
                string error = $"API Request Failed: HTTP/{webRequest.responseCode} {webRequest.error}. Response: {webRequest.downloadHandler.text}";
                Debug.LogError(error);
                onFailure?.Invoke(error);
            }
        }
    }
}
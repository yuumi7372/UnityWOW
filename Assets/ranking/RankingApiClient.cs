// RankingApiClient.cs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;


public class RankingApiClient : MonoBehaviour
{
    private const string API_BASE_URL = "http://localhost:3000";

    public string jwtToken
    {
        get { return PlayerPrefs.GetString("token", ""); }
    }

    public delegate void OnRequestFailed(string errorMessage);

    /// <summary>
    /// </summary>
    public void FetchRanking(Action<RankingItem[]> onSuccess, OnRequestFailed onFailure)
    {

        StartCoroutine(SendGetRankingRequest(onSuccess, onFailure));
    }

    private IEnumerator SendGetRankingRequest(Action<RankingItem[]> onSuccess, OnRequestFailed onFailure)
    {
        string url = $"{API_BASE_URL}/ranking";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // if (!string.IsNullOrEmpty(jwtToken)) {
            //     webRequest.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            // }

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonString = webRequest.downloadHandler.text;
                Debug.Log("ランキングJSONを取得しました。" + jsonString);

                try
                {
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
                    Debug.LogError("ランキングパースエラー" + e.Message);
                    onFailure?.Invoke("ランキングデータの形式エラー" + e.Message);
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
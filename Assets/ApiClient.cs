using UnityEngine.Networking;
using UnityEngine;

public static class ApiClient
{
    // APIのベースURL（本番・開発で切り替えたい場合は設定ファイル化）
    private const string BASE_URL = "http://localhost:3000/api/";

    // POSTリクエストを作成（認証あり）
    public static UnityWebRequest CreatePost(string endpoint, WWWForm form)
    {
        var url = BASE_URL + endpoint;
        var www = UnityWebRequest.Post(url, form);

        // トークンを付与
        string token = PlayerPrefs.GetString("token", "");
        if (!string.IsNullOrEmpty(token))
        {
            www.SetRequestHeader("Authorization", "Bearer " + token);
        }

        return www;
    }

    // GETリクエストを作成（認証あり）
    public static UnityWebRequest CreateGet(string endpoint)
    {
        var url = BASE_URL + endpoint;
        var www = UnityWebRequest.Get(url);

        // トークンを付与
        string token = PlayerPrefs.GetString("token", "");
        if (!string.IsNullOrEmpty(token))
        {
            www.SetRequestHeader("Authorization", "Bearer " + token);
        }

        return www;
    }

    // Delete 用
    public static UnityWebRequest CreateDelete(string path)
    {
        UnityWebRequest www = UnityWebRequest.Delete(BASE_URL + path);
        string token = PlayerPrefs.GetString("token", "");
        if (!string.IsNullOrEmpty(token))
            www.SetRequestHeader("Authorization", "Bearer " + token);
        www.downloadHandler = new DownloadHandlerBuffer(); // 忘れずに
        return www;
    }
}

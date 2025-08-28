using UnityEngine.Networking;

public static class ApiResponseHandler
{
    // 成功判定
    public static bool IsSuccess(UnityWebRequest www)
    {
        return www.result == UnityWebRequest.Result.Success && www.responseCode >= 200 && www.responseCode < 300;
    }

    // エラーメッセージ取得
    public static string GetError(UnityWebRequest www)
    {
        if (www.result == UnityWebRequest.Result.Success)
        {
            return null;
        }

        // サーバーからのメッセージを返す（JSONならJSON文字列がそのまま出る）
        if (!string.IsNullOrEmpty(www.downloadHandler.text))
        {
            return www.downloadHandler.text;
        }

        // UnityWebRequestのエラーメッセージ
        return www.error;
    }
}

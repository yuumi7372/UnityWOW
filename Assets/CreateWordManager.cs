using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;

public class CreateWordManager : MonoBehaviour
{
    public void CreateWord(string word, string meaning, Action<bool, string, string> callback)
    {
        StartCoroutine(CreateWordCoroutine(word, meaning, callback));
    }
    
    [System.Serializable]
    public class WordPayload
    {
        public string word;
        public string meaning;
        public int difficultyLevel = 3;
    }

    [System.Serializable]
    public class CreateWordResponse
    {
        public string id;
        public string status; // APIが返すなら
    }

    private IEnumerator CreateWordCoroutine(string word, string meaning, Action<bool, string, string> callback)
    {
        // JSON用クラス作成
        WordPayload payload = new WordPayload { word = word, meaning = meaning };
        string json = JsonUtility.ToJson(payload);

        // UnityWebRequest 作成
        UnityWebRequest www = new UnityWebRequest("http://localhost:3000/api/unity_create", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();

        // ヘッダー設定
        www.SetRequestHeader("Content-Type", "application/json");
        string token = PlayerPrefs.GetString("token", "");
        if (!string.IsNullOrEmpty(token))
        {
            www.SetRequestHeader("Authorization", "Bearer " + token);
        }

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success || www.responseCode == 201)
        {
            string responseJson = www.downloadHandler.text;
            Debug.Log("CreateWord Response: " + responseJson);

            // JSONをパースしてIDを取得
            CreateWordResponse res = JsonUtility.FromJson<CreateWordResponse>(responseJson);
            callback?.Invoke(true, res.id, null);
        }
        else
        {
            Debug.Log("単語追加失敗API: " + www.downloadHandler.text);
            string errorMessage = "サーバーエラーが発生しました。";
            if (www.responseCode == 409) // 重複エラー
            {
                errorMessage = "この単語は既に登録されています。";
            }
            callback?.Invoke(false, null, errorMessage);
        }
    }
}

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;

public class CreateWordManager : MonoBehaviour
{
    public void CreateWord(string word, string meaning, Action<bool> callback)
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

    private IEnumerator CreateWordCoroutine(string word, string meaning, Action<bool> callback)
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
            callback?.Invoke(true);
        }
        else
        {
            Debug.Log("単語追加失敗API: " + www.downloadHandler.text);
            callback?.Invoke(false);
        }
    }
}

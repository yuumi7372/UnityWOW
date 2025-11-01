//ワードリスト部分の処理（登録している単語を取得→表示、テキストの表示、各ボタン処理）
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI; 

[System.Serializable]
public class WordData
{
    public string id;
    public string word;
    public string meaning;
}

[System.Serializable]
public class WordDataArray
{
    public WordData[] words;
}

public class WordListManager : MonoBehaviour
{
    public GameObject wordItemPrefab; // 単語アイテムのPrefab
    public Transform content; // Scroll ViewのContent
    public ExplainPopupManager popupManager;//単語詳細のポップアップ
    public Button plusButton;
    public AddWordPopupManager addWordPopupManager;//単語追加のポップアップ

    private HashSet<string> existingWords = new HashSet<string>();  //既存単語の管理


    void Start()
    {
        StartCoroutine(LoadWordsFromServer());

        // ＋ボタンにリスナー追加

        plusButton.onClick.AddListener(addWordPopupManager.ShowPopup);
    }

    IEnumerator LoadWordsFromServer()
    {
        UnityWebRequest www = ApiClient.CreateGet("getwords");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // JsonUtilityは配列を直接デシリアライズできないのでラッパークラスを使う
            WordData[] words = JsonHelper.FromJson<WordData>(www.downloadHandler.text);
            foreach (var w in words)
            {
                AddWordToList(w.id, w.word, w.meaning);
            }

        }
        else
        {
            Debug.Log("単語取得失敗: " + www.downloadHandler.text);
        }
    }

    /// <summary>
    /// 単語リストに追加（重複チェックあり）
    /// </summary>
    public void AddWordToList(string id, string word, string meaning)
    {
        string normalizedWord = word.Trim().ToLower();
        // 重複チェック
        if (existingWords.Contains(normalizedWord))
        {
            Debug.LogWarning($"重複: {word} はすでに登録されています。");
            return;
        }
        existingWords.Add(normalizedWord); // HashSetに追加

        GameObject newItem = Instantiate(wordItemPrefab, content);
        DetailWordButton detailButton = newItem.GetComponent<DetailWordButton>();
        detailButton.Setup(id, word, meaning);

        // TextMeshProUGUIを探して設定
        TextMeshProUGUI[] texts = newItem.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 2)
        {
            texts[0].text = word;    // 1つ目のTextに単語
            texts[1].text = meaning; // 2つ目のTextに意味
        }
        Debug.Log($"AddWordToList: id={id}, word={word}, meaning={meaning}");
    }
    
    /// <summary>
    /// 指定した単語が既に登録されているか確認
    /// </summary>
    public bool IsWordDuplicate(string word)
    {
        return existingWords.Contains(word.Trim().ToLower());
    }
}

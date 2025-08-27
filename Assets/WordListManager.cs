using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordListManager : MonoBehaviour
{
    public GameObject wordItemPrefab; // 単語アイテムのPrefab
    public Transform content; // Scroll ViewのContent

    public void AddWordToList(string word, string meaning)
    {
        GameObject newItem = Instantiate(wordItemPrefab, content);
        // TextMeshProUGUIを探して設定
        TextMeshProUGUI[] texts = newItem.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 2)
        {
            texts[0].text = word;    // 1つ目のTextに単語
            texts[1].text = meaning; // 2つ目のTextに意味
        }
    }
}

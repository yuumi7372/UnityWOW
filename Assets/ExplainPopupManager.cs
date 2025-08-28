//単語詳細ポップアップ内処理
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

public class ExplainPopupManager : MonoBehaviour
{
    public GameObject popupPanel;      
    public Button closeButton;  //closeボタン 
    public Button editButton;      //editボタン
    public TextMeshProUGUI titleText; 

    // ポップアップに渡された単語情報
    private string currentWordId;
    private string currentWord;
    private string currentMeaning;

    public void ShowPopup(string wordId, string word, string meaning)
    {
        currentWordId = wordId;
        currentWord = word;
        currentMeaning = meaning;

        if (popupPanel == null)
        {
            Debug.LogError("popupPanelが設定されていません");
            return;
        }

        popupPanel.SetActive(true);

        if (titleText != null)
            titleText.text = word;
        else
            Debug.LogError("titleTextが設定されていません");


        Debug.Log($"Popup表示:{wordId}- {word} - {meaning}");

        if (closeButton != null)
            closeButton.onClick.AddListener(HidePopup);
        //  Edit ボタンに登録
        if (editButton != null)
        {
            editButton.onClick.AddListener(OnClickEdit);
        }
    }

    public void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void OnClickEdit()
    {
        Debug.Log($"[Popup Edit] wordId={currentWordId}, word={currentWord}, meaning={currentMeaning}");
        EditWordData.wordId = currentWordId;
        EditWordData.word = currentWord;
        EditWordData.meaning = currentMeaning;

        SceneManager.LoadScene("word_edit");
    }

    void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(HidePopup);
    }

}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddWordPopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_InputField wordInput;
    public TMP_InputField meaningInput;
    public Button addButton;//ポップアップ内addボタン
    public Button cancelButton;//ポップアップ内cancelボタン
    public CreateWordManager createWordManager; // 既存のスクリプト参照
    public WordListManager wordListManager;

    private string pendingWord;
    private string pendingMeaning;

    void Start()
    {
        popupPanel.SetActive(false); // 初期は非表示

        addButton.onClick.AddListener(OnAddClicked);
        cancelButton.onClick.AddListener(HidePopup);
    }

    //+ボタンから呼ぶ
    public void ShowPopup()
    {
        popupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
        wordInput.text = "";
        meaningInput.text = "";
    }

    void OnAddClicked()
    {
        string word = wordInput.text.Trim();
        string meaning = meaningInput.text.Trim();

        if (string.IsNullOrEmpty(word) || string.IsNullOrEmpty(meaning))
        {
            Debug.Log("単語と意味を入力してください。");
            return;
        }

        pendingWord = word;
        pendingMeaning = meaning;


        createWordManager.CreateWord(word, meaning, OnWordAdded);
    }

    void OnWordAdded(bool success, string newId)
    {
        if (success)
        {
            // WordListManagerに反映
            wordListManager.AddWordToList(newId, pendingWord, pendingMeaning);
            HidePopup(); // 成功したら閉じる
        }
        else
        {
            Debug.Log("単語追加に失敗しました。");
        }
    }
}

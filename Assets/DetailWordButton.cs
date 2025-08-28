//リストの単語ボタンを押すと単語情報を持ったままポップアップを出す処理
using UnityEngine;
using UnityEngine.UI;


public class DetailWordButton : MonoBehaviour
{
    public string wordId;
    public string word;
    public string meaning;

    private ExplainPopupManager popupManager;

    public void Setup(string id, string w, string m)
    {
        wordId = id;
        word = w;
        meaning = m;
    }

    

    void Start()
    {
        // シーン上のExplainPopupManagerを探す（複数あるならFindAnyObjectByType推奨）
        popupManager = FindFirstObjectByType<ExplainPopupManager>();

        if (popupManager == null)
        {
            Debug.LogError("ExplainPopupManagerがシーンに見つかりません");
        }

        // Buttonコンポーネントを取得して、クリックイベントをコード側で登録する
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickWord); // 単語説明ポップアップ表示の関数へ
    }

    void OnClickWord()
    {
        Debug.Log($"[DetailWordButton] wordId={wordId}, word={word}, meaning={meaning}");
        if (popupManager != null)
        {
            popupManager.ShowPopup(wordId, word, meaning);
        }
    }

}

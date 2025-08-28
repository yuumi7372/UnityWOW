using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // ← シーン遷移に必要
using System.Collections;
using UnityEngine.Networking;

public class EditWordManager : MonoBehaviour
{
    public Button deleteButton;
    public string wordId; // 削除対象の単語ID


    void Start()
    {
        wordId = EditWordData.wordId;
        Debug.Log("削除対象の単語ID: " + wordId);
        deleteButton.onClick.AddListener(OnDeleteButtonClicked);
    }

    void OnDeleteButtonClicked()
    {
        StartCoroutine(DeleteWordCoroutine());
    }

    IEnumerator DeleteWordCoroutine()
    {
        // API に DELETE リクエスト
        UnityWebRequest www = ApiClient.CreateDelete($"delete_word/{wordId}");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("単語削除成功");

            // 1秒くらい待ってから一覧に戻る（UIが消えるのを確認できる）
            yield return new WaitForSeconds(0.5f);

            // WordList シーンに戻る
            SceneManager.LoadScene("word"); 
        }
        else
        {
            Debug.LogError("単語削除失敗: " + www.downloadHandler.text);
        }
    }
}

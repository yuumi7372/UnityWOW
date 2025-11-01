using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public GameObject blackPanel;    // 黒パネル
    public RectTransform rocket;     // 横に流れるロケット
    public TMP_Text loadingText;     // 後ろに表示する「LOADING…」
    public float loadDuration = 1.5f; // ロードアニメ時間
    public float startX = -500f;      // ロケット開始位置
    public float endX = 500f;         // ロケット終了位置
    public float charInterval = 0.1f; // 1文字表示の間隔

    private string fullText = "LOADING...";

    void Start()
    {
        // 黒パネル表示
        if (blackPanel != null)
            blackPanel.SetActive(true);

        // ロケット位置リセット
        if (rocket != null)
            rocket.anchoredPosition = new Vector2(startX, rocket.anchoredPosition.y);

        // テキストは最初非表示
        if (loadingText != null)
            loadingText.text = "";

        StartCoroutine(PlayLoading());
    }

    private IEnumerator PlayLoading()
    {
        float elapsed = 0f;

        while (elapsed < loadDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / loadDuration);

            // ロケット移動
            if (rocket != null)
                rocket.anchoredPosition = new Vector2(Mathf.Lerp(startX, endX, t), rocket.anchoredPosition.y);

            // 文字表示
            if (loadingText != null)
            {
                float startDelay = 1.5f;
                int charCount = Mathf.FloorToInt((elapsed - startDelay)/ charInterval);
                charCount = Mathf.Clamp(charCount, 0, fullText.Length);
                loadingText.text = fullText.Substring(0, charCount);
            }

            yield return null;
        }

        // 終わったら全表示して少し待つ
        if (loadingText != null)
            loadingText.text = fullText;

        yield return new WaitForSeconds(0.3f);

        // 非表示
        if (blackPanel != null)
            blackPanel.SetActive(false);
        if (rocket != null)
            rocket.gameObject.SetActive(false);
        if (loadingText != null)
            loadingText.gameObject.SetActive(false);
    }
}

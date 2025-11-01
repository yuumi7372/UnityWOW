// RankingManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    // --- 外部コンポーネント (Inspectorで設定) ---
    public RankingApiClient apiClient;
    public GameObject rankingItemPrefab; // ランキングの行のテンプレートUI (Prefab)
    public Transform contentParent;       // ランキングの行を生成する親オブジェクト (Scroll View Content)
    public TextMeshProUGUI statusText;   // 状態表示用テキスト

    void Start()
    {
        if (apiClient == null)
        {
            apiClient = FindObjectOfType<RankingApiClient>();
            if (apiClient == null)
            {
                Debug.LogError("RankingApiClientが見つかりません。Hierarchyに配置してください。");
                return;
            }
        }

        FetchAndDisplayRanking();
    }

    public void FetchAndDisplayRanking()
    {
        if (statusText != null) statusText.text = "ランキングをロード中...";

        // 既存のランキングアイテムをクリア
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        apiClient.FetchRanking(OnRankingSuccess, OnRankingFailure);
    }

    private void OnRankingSuccess(RankingItem[] rankingItems)
    {
        if (statusText != null) statusText.text = $"ランキング取得成功！ (全 {rankingItems.Length} 位)";

        for (int i = 0; i < rankingItems.Length; i++)
        {
            CreateRankingItem(i + 1, rankingItems[i]);
        }
    }

    private void OnRankingFailure(string errorMessage)
    {
        if (statusText != null) statusText.text = $"ランキング取得失敗: {errorMessage}";
    }

    private void CreateRankingItem(int rank, RankingItem item)
    {
        if (rankingItemPrefab == null || contentParent == null) return;

        // プレハブをContent内に生成
        GameObject newItem = Instantiate(rankingItemPrefab, contentParent);

        // プレハブ内のUI要素を取得し、データを設定
        // "RankText"
        Transform rankTextTransform = newItem.transform.Find("RankText");
        if (rankTextTransform != null)
        {
            TextMeshProUGUI rankText = rankTextTransform.GetComponent<TextMeshProUGUI>();
            if (rankText != null)
            {
                rankText.text = rank.ToString();
            }
        }

        // "NameText"
        Transform nameTextTransform = newItem.transform.Find("NameText");
        if (nameTextTransform != null)
        {
            TextMeshProUGUI nameText = nameTextTransform.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = item.username;
            }
        }

        // "LevelText"
        Transform levelTextTransform = newItem.transform.Find("LevelText");
        if (levelTextTransform != null)
        {
            TextMeshProUGUI levelText = levelTextTransform.GetComponent<TextMeshProUGUI>();
            if (levelText != null)
            {
                levelText.text = $"Lv.{item.level}";
            }
        }

        // Note: キャラクター画像の表示には、URLからの画像ロード処理（別途実装が必要な処理）が必要です。
    }
}
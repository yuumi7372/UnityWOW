// RankingManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    public RankingApiClient apiClient;
    public GameObject rankingItemPrefab; 
    public Transform contentParent;       
    public TextMeshProUGUI statusText;   

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
        if (statusText != null) statusText.text = "ロード中...";

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        apiClient.FetchRanking(OnRankingSuccess, OnRankingFailure);
    }

    private void OnRankingSuccess(RankingItem[] rankingItems)
    {
        if (statusText != null) statusText.text = $"ランキング";

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

        GameObject newItem = Instantiate(rankingItemPrefab, contentParent);

        // "RankText"
        Transform rankAreaTransform = newItem.transform.Find("RankArea");
        Transform rankTextTransform = rankAreaTransform.Find("RankText");
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

    }
}
// RankingManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    // --- �O���R���|�[�l���g (Inspector�Őݒ�) ---
    public RankingApiClient apiClient;
    public GameObject rankingItemPrefab; // �����L���O�̍s�̃e���v���[�gUI (Prefab)
    public Transform contentParent;       // �����L���O�̍s�𐶐�����e�I�u�W�F�N�g (Scroll View Content)
    public TextMeshProUGUI statusText;   // ��ԕ\���p�e�L�X�g

    void Start()
    {
        if (apiClient == null)
        {
            apiClient = FindObjectOfType<RankingApiClient>();
            if (apiClient == null)
            {
                Debug.LogError("RankingApiClient��������܂���BHierarchy�ɔz�u���Ă��������B");
                return;
            }
        }

        FetchAndDisplayRanking();
    }

    public void FetchAndDisplayRanking()
    {
        if (statusText != null) statusText.text = "�����L���O�����[�h��...";

        // �����̃����L���O�A�C�e�����N���A
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        apiClient.FetchRanking(OnRankingSuccess, OnRankingFailure);
    }

    private void OnRankingSuccess(RankingItem[] rankingItems)
    {
        if (statusText != null) statusText.text = $"�����L���O�擾�����I (�S {rankingItems.Length} ��)";

        for (int i = 0; i < rankingItems.Length; i++)
        {
            CreateRankingItem(i + 1, rankingItems[i]);
        }
    }

    private void OnRankingFailure(string errorMessage)
    {
        if (statusText != null) statusText.text = $"�����L���O�擾���s: {errorMessage}";
    }

    private void CreateRankingItem(int rank, RankingItem item)
    {
        if (rankingItemPrefab == null || contentParent == null) return;

        // �v���n�u��Content���ɐ���
        GameObject newItem = Instantiate(rankingItemPrefab, contentParent);

        // �v���n�u����UI�v�f���擾���A�f�[�^��ݒ�
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

        // Note: �L�����N�^�[�摜�̕\���ɂ́AURL����̉摜���[�h�����i�ʓr�������K�v�ȏ����j���K�v�ł��B
    }
}
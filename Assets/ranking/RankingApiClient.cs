// RankingApiClient.cs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

// Note: QuestApiClient �Ƃ̍���������邽�߁A�ʃt�@�C���܂��� QuestApiClient �Ƀ��\�b�h��ǉ����Ă��ǂ��ł����A
// �����ł͓Ɨ������N���C�A���g�Ƃ��č쐬���܂��B

public class RankingApiClient : MonoBehaviour
{
    private const string API_BASE_URL = "http://localhost:3000";

    public string jwtToken
    {
        get { return PlayerPrefs.GetString("token", ""); }
    }

    public delegate void OnRequestFailed(string errorMessage);

    /// <summary>
    /// �����L���O�f�[�^���擾���A�f�V���A���C�Y���܂��B
    /// </summary>
    public void FetchRanking(Action<RankingItem[]> onSuccess, OnRequestFailed onFailure)
    {
        // �F�؂��s�v��API�ł���ꍇ�́AauthenticateToken �~�h���E�F�A���O���Ă��������B
        // �����ł́A�����L���O�擾�ɂ͔F�؂��s�v�ł���Ɖ��肵�A�g�[�N���`�F�b�N�̓X�L�b�v���܂��B

        StartCoroutine(SendGetRankingRequest(onSuccess, onFailure));
    }

    private IEnumerator SendGetRankingRequest(Action<RankingItem[]> onSuccess, OnRequestFailed onFailure)
    {
        // Express�� /ranking ���[�g�ɐݒ肵�����Ƃ�z��
        string url = $"{API_BASE_URL}/ranking";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // ?? �����L���OAPI�ɔF�؂��K�v�ȏꍇ�́A�ȉ��̍s���R�����g�������Ă�������
            // if (!string.IsNullOrEmpty(jwtToken)) {
            //     webRequest.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            // }

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonString = webRequest.downloadHandler.text;
                Debug.Log("�����L���OJSON�擾����: " + jsonString);

                try
                {
                    // JSONUtility�Ŕz����f�V���A���C�Y���邽�߁A���b�p�[���g�p���܂�
                    string wrappedJson = "{ \"ranking\": " + jsonString + "}";
                    RankingListWrapper wrapper = JsonUtility.FromJson<RankingListWrapper>(wrappedJson);

                    if (wrapper != null && wrapper.ranking != null)
                    {
                        onSuccess?.Invoke(wrapper.ranking);
                    }
                    else
                    {
                        throw new Exception("�����L���O�f�[�^�̃p�[�X�Ɏ��s���܂����B");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("�����L���O�p�[�X�G���[: " + e.Message);
                    onFailure?.Invoke("�����L���O�f�[�^�̌`���G���[: " + e.Message);
                }
            }
            else
            {
                string error = $"API Request Failed: HTTP/{webRequest.responseCode} {webRequest.error}. Response: {webRequest.downloadHandler.text}";
                Debug.LogError(error);
                onFailure?.Invoke(error);
            }
        }
    }
}
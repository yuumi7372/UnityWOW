// OcrApiClient.cs
using System;
using System.Collections;
using System.Text; // UTF8Encoding �̂��߂ɕK�v
using UnityEngine;
using UnityEngine.Networking; // UnityWebRequest �̂��߂ɕK�v

public class OcrApiClient : MonoBehaviour
{
    // �T�[�o�[��URL (Express�T�[�o�[�����삵�Ă���ꏊ�ɍ��킹�ĕύX���Ă�������)
    private const string API_BASE_URL = "http://localhost:3000";

    // ���̃X�N���v�g����Ăяo�����߂̃f���Q�[�g (�R�[���o�b�N)
    public delegate void OnOcrSuccess(TranslatedLabel[] labels);
    public delegate void OnOcrFailure(string errorMessage);

    /// <summary>
    /// �摜�f�[�^���T�[�o�[�ɑ��M���ĕ��͂��˗����܂��B
    /// </summary>
    /// <param name="imageBytes">�摜�̃o�C�g�z�� (��: Texture2D.EncodeToPNG())</param>
    /// <param name="onSuccess">�������̃R�[���o�b�N</param>
    /// <param name="onFailure">���s���̃R�[���o�b�N</param>
    public void AnalyzeImage(byte[] imageBytes, OnOcrSuccess onSuccess, OnOcrFailure onFailure)
    {
        // 1. �o�C�g�z���Base64������ɕϊ�
        string base64String = System.Convert.ToBase64String(imageBytes);

        // 2. �R���[�`�����J�n
        StartCoroutine(SendOcrRequest(base64String, onSuccess, onFailure));
    }

    private IEnumerator SendOcrRequest(string base64String, OnOcrSuccess onSuccess, OnOcrFailure onFailure)
    {
        string url = $"{API_BASE_URL}/api/ocr";

        // 1. ���M����JSON���N�G�X�g�{�f�B���쐬
        OcrRequest requestBody = new OcrRequest
        {
            imageBase64 = base64String
        };
        string jsonPayload = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        // 2. UnityWebRequest (POST) ���쐬
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            // 3. �f�[�^��ݒ�
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            // 4. �w�b�_�[��ݒ� (Express���� express.json() ���g�����߂ɕK�{)
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");

            // 5. ���N�G�X�g�𑗐M
            yield return www.SendWebRequest();

            // 6. ���ʂ�����
            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("OCR���X�|���X: " + jsonResponse);

                try
                {
                    // 7. JSON��C#�N���X�Ƀf�V���A���C�Y
                    OcrResponse response = JsonUtility.FromJson<OcrResponse>(jsonResponse);
                    onSuccess?.Invoke(response.translatedLabels);
                }
                catch (Exception e)
                {
                    Debug.LogError("OCR JSON�̃p�[�X�Ɏ��s: " + e.Message);
                    onFailure?.Invoke("���X�|���X�f�[�^�̌`��������������܂���B");
                }
            }
            else
            {
                // 4xx �� 5xx �G���[
                string error = $"API Request Failed: HTTP/{www.responseCode} {www.error}. Response: {www.downloadHandler.text}";
                Debug.LogError(error);
                onFailure?.Invoke(error);
            }
        }
    }
}
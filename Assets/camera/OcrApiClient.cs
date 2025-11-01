// OcrApiClient.cs
using System;
using System.Collections;
using System.Text; // UTF8Encoding のために必要
using UnityEngine;
using UnityEngine.Networking; // UnityWebRequest のために必要

public class OcrApiClient : MonoBehaviour
{
    // サーバーのURL (Expressサーバーが動作している場所に合わせて変更してください)
    private const string API_BASE_URL = "http://localhost:3000";

    // 他のスクリプトから呼び出すためのデリゲート (コールバック)
    public delegate void OnOcrSuccess(TranslatedLabel[] labels);
    public delegate void OnOcrFailure(string errorMessage);

    /// <summary>
    /// 画像データをサーバーに送信して分析を依頼します。
    /// </summary>
    /// <param name="imageBytes">画像のバイト配列 (例: Texture2D.EncodeToPNG())</param>
    /// <param name="onSuccess">成功時のコールバック</param>
    /// <param name="onFailure">失敗時のコールバック</param>
    public void AnalyzeImage(byte[] imageBytes, OnOcrSuccess onSuccess, OnOcrFailure onFailure)
    {
        // 1. バイト配列をBase64文字列に変換
        string base64String = System.Convert.ToBase64String(imageBytes);

        // 2. コルーチンを開始
        StartCoroutine(SendOcrRequest(base64String, onSuccess, onFailure));
    }

    private IEnumerator SendOcrRequest(string base64String, OnOcrSuccess onSuccess, OnOcrFailure onFailure)
    {
        string url = $"{API_BASE_URL}/api/ocr";

        // 1. 送信するJSONリクエストボディを作成
        OcrRequest requestBody = new OcrRequest
        {
            imageBase64 = base64String
        };
        string jsonPayload = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        // 2. UnityWebRequest (POST) を作成
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            // 3. データを設定
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            // 4. ヘッダーを設定 (Express側が express.json() を使うために必須)
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");

            // 5. リクエストを送信
            yield return www.SendWebRequest();

            // 6. 結果を処理
            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("OCRレスポンス: " + jsonResponse);

                try
                {
                    // 7. JSONをC#クラスにデシリアライズ
                    OcrResponse response = JsonUtility.FromJson<OcrResponse>(jsonResponse);
                    onSuccess?.Invoke(response.translatedLabels);
                }
                catch (Exception e)
                {
                    Debug.LogError("OCR JSONのパースに失敗: " + e.Message);
                    onFailure?.Invoke("レスポンスデータの形式が正しくありません。");
                }
            }
            else
            {
                // 4xx や 5xx エラー
                string error = $"API Request Failed: HTTP/{www.responseCode} {www.error}. Response: {www.downloadHandler.text}";
                Debug.LogError(error);
                onFailure?.Invoke(error);
            }
        }
    }
}
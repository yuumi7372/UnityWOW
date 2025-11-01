// OcrDemoManager.cs
using UnityEngine;
using UnityEngine.UI; // Button, RawImage のために必要
using TMPro; // TextMeshPro のために必要
using System.Collections; // コルーチンのために必要
// 💡 StringBuilder を使わなくなったため、System.Text は不要になりました

public class OcrDemoManager : MonoBehaviour
{
    // --- Inspectorで設定 ---
    public OcrApiClient apiClient; // シーン内の OcrApiClient を接続
    public Button analyzeButton;     // 実行ボタン
    public RawImage cameraDisplay;    // カメラ映像を映すUI
    public TextMeshProUGUI resultText;  // 結果表示用のテキスト
    // -------------------------

    private WebCamTexture webCamTexture; // カメラデバイスを制御

    void Start()
    {
        if (apiClient == null)
        {
            apiClient = FindObjectOfType<OcrApiClient>();
        }

        if (analyzeButton != null)
        {
            analyzeButton.onClick.AddListener(OnAnalyzeButtonPressed);
        }

        // カメラの初期化と起動
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("カメラが見つかりません。");
            resultText.text = "カメラがありません。";
            return;
        }

        webCamTexture = new WebCamTexture(devices[0].name);

        if (cameraDisplay != null)
        {
            cameraDisplay.texture = webCamTexture;
        }

        webCamTexture.Play();
        Debug.Log("カメラを起動しました。");
    }

    private void OnAnalyzeButtonPressed()
    {
        if (webCamTexture == null || !webCamTexture.isPlaying)
        {
            Debug.LogError("カメラが動作していません。");
            resultText.text = "カメラが動作していません。";
            return;
        }

        resultText.text = "分析中...";
        analyzeButton.interactable = false;

        byte[] imageBytes = CaptureWebCamFrame();

        if (imageBytes != null)
        {
            apiClient.AnalyzeImage(imageBytes, OnOcrSuccess, OnOcrFailure);
        }
        else
        {
            OnOcrFailure("カメラフレームのキャプチャに失敗しました。");
        }
    }

    private byte[] CaptureWebCamFrame()
    {
        Texture2D texture = new Texture2D(webCamTexture.width, webCamTexture.height);
        texture.SetPixels(webCamTexture.GetPixels());
        texture.Apply();
        return texture.EncodeToPNG();
    }

    private void OnOcrSuccess(TranslatedLabel[] labels)
    {
        analyzeButton.interactable = true;

        // 1. ラベルが null または 0件 でないか確認
        if (labels == null || labels.Length == 0)
        {
            resultText.text = "ラベルが見つかりませんでした。";
            return;
        }

        // 2. 一番上の結果を取得 (信頼度が最も高いもの)
        TranslatedLabel topLabel = labels[0];
        string japaneseWord = topLabel.word_ja;
        string englishWord = topLabel.word_en;

        // 3. 英語の単語の最初の文字を大文字にする (例: "bottle" -> "Bottle")
        if (!string.IsNullOrEmpty(englishWord))
        {
            englishWord = char.ToUpper(englishWord[0]) + englishWord.Substring(1);
        }

        // 4. ユーザーの希望する形式でテキストを設定
        resultText.text = $"これは **{japaneseWord}** だよ！\n英語では **{englishWord}** っていうんだ！";
    }

    // 失敗コールバック
    private void OnOcrFailure(string errorMessage)
    {
        analyzeButton.interactable = true;
        resultText.text = $"エラーが発生しました:\n{errorMessage}";
    }
}
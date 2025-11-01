// OcrData.cs (ApiModels.cs などのファイルにまとめてもOKです)
using System;
using System.Collections.Generic;

/// <summary>
/// POST /api/ocr に送信するリクエストボディ
/// </summary>
[Serializable]
public class OcrRequest
{
    // imageBase64 は "data:image/png;base64,..." を含まない、純粋なBase64文字列
    public string imageBase64;
}

/// <summary>
/// APIから返ってくる翻訳済みラベルの単体
/// </summary>
[Serializable]
public class TranslatedLabel
{
    public string word_en;
    public string word_ja;
}

/// <summary>
/// APIから返ってくるレスポンス全体
/// (JsonUtilityがルート配列を直接パースできないため、ラッパークラスを使用)
/// </summary>
[Serializable]
public class OcrResponse
{
    // サーバー側の "translatedLabels" と名前を一致させる
    public TranslatedLabel[] translatedLabels;
}
// OcrData.cs (ApiModels.cs �Ȃǂ̃t�@�C���ɂ܂Ƃ߂Ă�OK�ł�)
using System;
using System.Collections.Generic;

/// <summary>
/// POST /api/ocr �ɑ��M���郊�N�G�X�g�{�f�B
/// </summary>
[Serializable]
public class OcrRequest
{
    // imageBase64 �� "data:image/png;base64,..." ���܂܂Ȃ��A������Base64������
    public string imageBase64;
}

/// <summary>
/// API����Ԃ��Ă���|��ς݃��x���̒P��
/// </summary>
[Serializable]
public class TranslatedLabel
{
    public string word_en;
    public string word_ja;
}

/// <summary>
/// API����Ԃ��Ă��郌�X�|���X�S��
/// (JsonUtility�����[�g�z��𒼐ڃp�[�X�ł��Ȃ����߁A���b�p�[�N���X���g�p)
/// </summary>
[Serializable]
public class OcrResponse
{
    // �T�[�o�[���� "translatedLabels" �Ɩ��O����v������
    public TranslatedLabel[] translatedLabels;
}
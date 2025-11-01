// RankingData.cs
using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RankingItem
{
    // API����Ԃ����t�B�[���h�ɑΉ�
    public int userId;
    public string username;
    public int level;
    public string characterImage; // �摜URL
}

// �z����p�[�X���邽�߂̃��b�p�[�N���X
// Unity��JsonUtility�͔z��̃��[�g�𒼐ڃf�V���A���C�Y�ł��Ȃ����ߕK�v
[System.Serializable]
public class RankingListWrapper
{
    public RankingItem[] ranking;
}
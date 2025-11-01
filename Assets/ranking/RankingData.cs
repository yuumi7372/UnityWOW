// RankingData.cs
using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RankingItem
{
    // APIから返されるフィールドに対応
    public int userId;
    public string username;
    public int level;
    public string characterImage; // 画像URL
}

// 配列をパースするためのラッパークラス
// UnityのJsonUtilityは配列のルートを直接デシリアライズできないため必要
[System.Serializable]
public class RankingListWrapper
{
    public RankingItem[] ranking;
}
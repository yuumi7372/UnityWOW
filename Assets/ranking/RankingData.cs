// RankingData.cs
using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RankingItem
{
    public int userId;
    public string username;
    public int level;
    public string characterImage; 
}

[System.Serializable]
public class RankingListWrapper
{
    public RankingItem[] ranking;
}
// QuestData.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossData
{
    public string id; // BigIntが文字列化されるため
    public string name;
    public int initialHp;
    public int currentHp;
    public int attack;
    public int defense;
    public string imageUrl;
}

[System.Serializable]
public class UserStatusData
{
    public int userId;
    public int currentHp;
    public int maxHp;
    public int currentLevel;
    public int attackPower;
    public int defensePower;
    public string characterImage;
}

[System.Serializable]
public class ProblemData
{
    public string wordId; // BigIntが文字列化されるため
    public string question;
    public string[] options;
    public string correctAnswer;
    public int difficultyLevel;
}

// =======================
// /quest/start のレスポンス
// =======================
[System.Serializable]
public class StartQuestResponse
{
    public string questSessionId; // BigIntが文字列化されるため
    public BossData boss;
    public UserStatusData userStatus;
    public ProblemData currentProblem;
}


// =======================
// /quest/answer のリクエストボディ
// =======================
[System.Serializable]
public class AnswerQuestRequest
{
    public int questSessionId; // C#側でIntに変換してから送信するか、Stringとして扱う
    public int wordId;         // C#側でIntに変換してから送信するか、Stringとして扱う
    public string userAnswer;
    public bool isCorrect;
    // Note: ユーザーのHPはサーバー側でQuestSessionから取得する方が安全ですが、
    // 既存のTSコードに合わせてクライアント側から送信する場合はここに追加します。
    // public int userCurrentHp; 
}

// =======================
// /quest/answer のレスポンス
// =======================
[System.Serializable]
public class AnswerQuestResponse
{
    public string damageDealtToBoss;
    public string damageTakenByUser;
    public int newBossHp;
    public int newUserHp;
    public string questStatus; // "ongoing", "completed", "failed"
    public ProblemData nextProblem;
    // ... その他、更新されたQuestSessionの情報など
}

// ユーザーの最終キャラクター状態
[System.Serializable]
public class FinalUserCharacterStatus
{
    public int userId;
    public int currentLevel;
    public int currentExperience;
    public string characterImage;
    public int attackPower;
    public int defensePower;
    public string skillUnlocked;
    public int hp;
}

// =======================
// /quest/result のレスポンス
// =======================
[System.Serializable]
public class QuestResultResponse
{
    public string questSessionId;
    public string questStatus; // "completed" or "failed"
    public int finalBossHp;
    public int finalUserHp;
    public string bossName;
    public FinalUserCharacterStatus finalUserCharacterStatus;
    // 必要に応じて、報酬アイテムなどの情報も追加
}

// =======================
// シーン間で結果データを渡すためのシングルトン
// =======================
public class QuestResultDataContainer : MonoBehaviour
{
    public static QuestResultDataContainer Instance;
    private string rawResultJson;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでも破棄されない
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetRawResultJson(string json)
    {
        rawResultJson = json;
    }

    public string GetRawResultJson()
    {
        return rawResultJson;
    }
}
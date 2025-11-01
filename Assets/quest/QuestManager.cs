// QuestManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    // --- 外部コンポーネントとUI ---
    // Note: Inspectorで手動設定するか、FindObjectOfTypeで取得が必要です。
    public QuestApiClient apiClient;
    public string resultSceneName = "quest_result_scene"; // クエスト結果画面のシーン名

    // UIコンポーネント (TMProを使用)
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public TextMeshProUGUI[] optionTexts;
    public TextMeshProUGUI resultMessageText; // "正解！", "不正解！" などのメッセージ
    public Slider userHpSlider;
    public TextMeshProUGUI userHpText;
    public Slider bossHpSlider;
    public TextMeshProUGUI bossHpText;

    // --- クエスト状態のデータ ---
    private int currentQuestSessionId = -1;
    private int userMaxHp = 1;
    private int bossMaxHp = 1;
    private ProblemData currentProblem; // 現在出題中の問題データ

    void Start()
    {
        // NullチェックとApiClientの自動取得
        if (apiClient == null)
        {
            apiClient = FindObjectOfType<QuestApiClient>();
            if (apiClient == null)
            {
                Debug.LogError("QuestApiClientが見つかりません。Hierarchyに配置し、アタッチされているか確認してください。");
                return;
            }
        }

        if (resultMessageText != null) resultMessageText.text = "クエスト開始準備中...";

        // ログインが完了し、トークンが設定されるのを待ってからクエストを開始する
        StartCoroutine(WaitForLoginAndStartQuest());
    }

    private IEnumerator WaitForLoginAndStartQuest()
    {
        // ログインが完了し、PlayerPrefsにトークンが設定されるのを待機
        float startTime = Time.time;
        float timeout = 10f; // 10秒でタイムアウト

        while (string.IsNullOrEmpty(apiClient.jwtToken) && Time.time < startTime + timeout)
        {
            if (resultMessageText != null)
                resultMessageText.text = "ログイン情報を待機中...";

            yield return null; // 1フレーム待機
        }

        // 待機を終えた後、トークンが存在するか最終チェック
        if (string.IsNullOrEmpty(apiClient.jwtToken))
        {
            Debug.LogError("JWTトークンが設定されていません。ログイン処理が失敗したか、タイムアウトしました。");
            if (resultMessageText != null) resultMessageText.text = "ログインエラー。";
            SetButtonsInteractable(false);
            yield break;
        }

        if (resultMessageText != null) resultMessageText.text = "認証成功！クエスト開始中...";
        StartQuest();
    }

    public void StartQuest()
    {
        apiClient.StartQuest(OnStartQuestSuccess, OnApiRequestFailed);
        Debug.Log("クエスト開始データの取得を開始します。");
    }

    private void OnStartQuestSuccess(StartQuestResponse response)
    {
        Debug.Log($"クエスト開始成功！ボス: {response.boss.name}, ユーザーHP: {response.userStatus.currentHp}");

        // クエストデータの初期設定 (BigIntの文字列をintに変換)
        currentQuestSessionId = int.Parse(response.questSessionId);
        userMaxHp = response.userStatus.maxHp;
        bossMaxHp = response.boss.initialHp;
        currentProblem = response.currentProblem;

        // HPバーの初期設定
        SetHpBar(userHpSlider, userHpText, response.userStatus.currentHp, userMaxHp);
        SetHpBar(bossHpSlider, bossHpText, response.boss.currentHp, bossMaxHp);

        DisplayProblem(); // 最初の問題を表示
        if (resultMessageText != null) resultMessageText.text = "バトル開始！";
    }

    private void DisplayProblem()
    {
        if (currentProblem == null || questionText == null || optionButtons == null || optionTexts == null)
        {
            Debug.LogError("UIコンポーネントまたは出題データが設定されていません。");
            return;
        }

        questionText.text = currentProblem.question;

        ColorBlock initialColors = ColorBlock.defaultColorBlock;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;

            if (index < currentProblem.options.Length)
            {
                optionTexts[index].text = currentProblem.options[index];
                optionButtons[index].gameObject.SetActive(true);

                optionButtons[index].onClick.RemoveAllListeners();
                optionButtons[index].onClick.AddListener(() => CheckAnswer(index));
                optionButtons[index].interactable = true;
                optionButtons[index].colors = initialColors;
            }
            else
            {
                optionButtons[index].gameObject.SetActive(false);
            }
        }
    }

    private void CheckAnswer(int selectedOptionIndex)
    {
        if (currentProblem == null || currentQuestSessionId == -1)
        {
            Debug.LogError("クエストが初期化されていません。");
            return;
        }

        SetButtonsInteractable(false);

        string selectedAnswer = currentProblem.options[selectedOptionIndex];
        bool isCorrect = (selectedAnswer == currentProblem.correctAnswer);

        HighlightAnswer(selectedOptionIndex, isCorrect);
        if (resultMessageText != null)
        {
            resultMessageText.text = isCorrect ? "正解！🎯 ボスに攻撃！" : "不正解...🤕 ダメージを受けた！";
        }

        // wordId(string) → int型に変換して渡す
        int wordIdInt;
        if (!int.TryParse(currentProblem.wordId, out wordIdInt))
        {
            Debug.LogError($"wordIdの変換に失敗しました: {currentProblem.wordId}");
            OnApiRequestFailed("wordId変換エラー");
            return;
        }

        // クイズ解答APIを呼び出し、サーバーでダメージ計算を実行
        apiClient.AnswerQuest(
            currentQuestSessionId,
            wordIdInt,
            selectedAnswer,
            isCorrect,
            OnAnswerQuestSuccess,
            OnApiRequestFailed
        );
    }

    private void OnAnswerQuestSuccess(AnswerQuestResponse response)
    {
        Debug.Log($"解答成功。新HP: ユーザー={response.newUserHp}, ボス={response.newBossHp}, ステータス={response.questStatus}");

        // HPバーの更新
        SetHpBar(userHpSlider, userHpText, response.newUserHp, userMaxHp);
        SetHpBar(bossHpSlider, bossHpText, response.newBossHp, bossMaxHp);

        if (response.questStatus == "ongoing")
        {
            currentProblem = response.nextProblem;
            if (currentProblem != null)
            {
                StartCoroutine(NextProblemAfterDelay(1.5f));
            }
            else
            {
                Debug.LogError("次の問題が返却されませんでした。");
                SetButtonsInteractable(false);
            }
        }
        else // "completed" または "failed"
        {
            HandleQuestEnd(response.questStatus);
        }
    }

    private void HandleQuestEnd(string finalStatus)
    {
        SetButtonsInteractable(false);
        if (resultMessageText != null)
        {
            resultMessageText.text = finalStatus == "completed" ? "🎉 クエストクリア！" : "😭 クエスト失敗...";
        }

        // 結果取得APIを呼び出し、コールバックでデータを保存し遷移
        apiClient.GetQuestResult(
            currentQuestSessionId,
            OnGetResultSuccess_SaveAndTransition,
            OnApiRequestFailed
        );

    }

    // データ保存と遷移を担当するメソッド (OnGetResultSuccessの曖昧さ回避のためリネーム)
    private void OnGetResultSuccess_SaveAndTransition(QuestResultResponse response)
    {
        Debug.Log("クエスト結果取得完了。結果画面へ遷移します。");
        string resultJson = JsonUtility.ToJson(response);

        if (QuestResultContainer.Instance != null)
        {
            // JSON文字列が空でないか確認
            if (!string.IsNullOrEmpty(resultJson) && resultJson != "{}") // {} も空とみなす
            {
                QuestResultContainer.Instance.SetRawResultJson(resultJson);
                Debug.Log("結果JSONデータをコンテナに保存しました。");
            }
            else
            {
                Debug.LogError("保存失敗: APIからの結果JSONが空またはnullです。サーバー側の GetQuestResultHandler を確認してください。");
            }
        }
        else
        {
            Debug.LogError("保存失敗: QuestResultContainerの永続化インスタンスが見つかりません。");
        }
        // 画面遷移
        SceneManager.LoadScene(resultSceneName);
    }

    private void OnApiRequestFailed(string errorMessage)
    {
        Debug.LogError($"クエストAPIエラー: {errorMessage}");
        if (resultMessageText != null)
        {
            resultMessageText.text = "通信エラーが発生しました。";
        }
        SetButtonsInteractable(false);
    }

    private IEnumerator NextProblemAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (resultMessageText != null) resultMessageText.text = "";

        ResetButtonColors();
        DisplayProblem();
    }

    // --- ヘルパー関数 ---

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Button btn in optionButtons)
        {
            if (btn != null) btn.interactable = interactable;
        }
    }

    private void SetHpBar(Slider slider, TextMeshProUGUI text, int currentHp, int maxHp)
    {
        if (slider != null)
        {
            slider.maxValue = maxHp;
            slider.value = currentHp;
        }
        if (text != null)
        {
            text.text = $"{Mathf.Max(0, currentHp)} / {maxHp}";
        }
    }

    private void HighlightAnswer(int selectedOptionIndex, bool isCorrect)
    {
        ColorBlock correctBlock = ColorBlock.defaultColorBlock;
        correctBlock.normalColor = Color.green;
        correctBlock.disabledColor = Color.green;

        ColorBlock incorrectBlock = ColorBlock.defaultColorBlock;
        incorrectBlock.normalColor = Color.red;
        incorrectBlock.disabledColor = Color.red;

        SetButtonsInteractable(false);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (optionButtons[i] != null)
            {
                bool isThisButtonCorrect = (currentProblem.options[i] == currentProblem.correctAnswer);

                if (i == selectedOptionIndex)
                {
                    optionButtons[i].colors = isCorrect ? correctBlock : incorrectBlock;
                }
                else if (isThisButtonCorrect)
                {
                    optionButtons[i].colors = correctBlock;
                }
                else
                {
                    optionButtons[i].colors = ColorBlock.defaultColorBlock;
                }
            }
        }
    }

    private void ResetButtonColors()
    {
        ColorBlock initialColors = ColorBlock.defaultColorBlock;
        if (optionButtons != null)
        {
            foreach (Button btn in optionButtons)
            {
                if (btn != null)
                {
                    btn.colors = initialColors;
                }
            }
        }
    }
}
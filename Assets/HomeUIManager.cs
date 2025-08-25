using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI levelText;
    //public Image userIcon; // 後で差し替え予定

    void Start()
    {
        string username = PlayerPrefs.GetString("username", "Guest");
        int level = PlayerPrefs.GetInt("level", 1);

        usernameText.text = username;
        levelText.text = "Lv. " + level;
    }

}

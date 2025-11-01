using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TapToLogin : MonoBehaviour
{
    public GameObject loginPanel;

    void Update()
    {
        // パネルがすでに出ていたら反応しない
        if (loginPanel.activeSelf) return;

        bool tapped = false;

        // PCクリック
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            tapped = true;

        // スマホタップ（押した瞬間だけ反応）
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            tapped = true;

        if (tapped)
        {
            HandleStart();
        }
    }

    private void HandleStart()
    {
        int isLoggedIn = PlayerPrefs.GetInt("isLoggedIn", 0);

        if (isLoggedIn == 1)
        {
            // ログイン済み → ホーム画面へ
            SceneManager.LoadScene("home");
        }
        else
        {
            // 未ログイン → ログインパネル表示
            if (!loginPanel.activeSelf) // すでに開いていなければ
                loginPanel.SetActive(true);
        }
    }
}
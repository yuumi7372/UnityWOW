using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class TapToLogin : MonoBehaviour
{
    public GameObject loginPanel;
    public RocketMoving rocket;
    public GameObject tapToStartUI; // ★追加★ TapToStartのUI

    void Update()
    {
        // パネルが出ていたら反応しない
        if (loginPanel.activeSelf) return;

        bool tapped = false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            tapped = true;

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
            // ★TapToStartを消す★
            if (tapToStartUI != null)
                tapToStartUI.SetActive(false);

            // ロケットを上に飛ばすフラグをセット
            if (rocket != null)
            {
                rocket.goToHome = true;
                StartCoroutine(GoHomeAfterDelay(1f)); // 1秒後にホームに遷移
            }
            else
            {
                SceneManager.LoadScene("home");
            }
        }
        else
        {
            if (!loginPanel.activeSelf)
                loginPanel.SetActive(true);
        }
    }

    private IEnumerator GoHomeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("home");
    }
}

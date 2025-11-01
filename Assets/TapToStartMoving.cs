using UnityEngine;
using UnityEngine.UI;

public class TapToStartMoving : MonoBehaviour
{
    public GameObject tapToStart; // Inspectorで自分自身をセットしてもOK
    public GameObject loginPanel; // ログインパネルをセット
    public RocketMoving rocket;   // ロケットスクリプト
    public float amplitude = 10f; // 上下に動く幅
    public float frequency = 1f;  // 速さ
    public float zoomDuration = 0.5f; // ズームインの時間
    public float appearOffset = 0.5f; // 中央手前で出すためのオフセット
    private Vector3 startPos;
    private bool started = false;
    private float zoomElapsed = 0f;

    void Start()
    {
        startPos = transform.localPosition;
        transform.localScale = Vector3.zero; // 最初は見えない
    }

    void Update()
    {
        // ログインパネルが出てたら非表示
        if (loginPanel != null && loginPanel.activeSelf)
        {
            if (tapToStart != null && tapToStart.activeSelf)
                tapToStart.SetActive(false);
            return; // パネル表示中は動かさない
        }

        if (!started)
        {
            // ロケットが targetY - appearOffset に到達したらスタート
            if (rocket.transform.position.y >= rocket.targetY - appearOffset)
            {
                started = true;
            }
        }
        else
        {
            // ズームイン
            if (zoomElapsed < zoomDuration)
            {
                zoomElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(zoomElapsed / zoomDuration);
                transform.localScale = Vector3.one * t;
            }

            // Sin波で上下に揺らす
            float yOffset = Mathf.Sin(Time.time * frequency * 2 * Mathf.PI) * amplitude;
            transform.localPosition = startPos + new Vector3(0, yOffset, 0);
        }
    }
}

using UnityEngine;

public class RocketMoving : MonoBehaviour
{
    public float floatAmplitude = 0.2f; // ふわふわ幅
    public float floatFrequency = 2f;   // ふわふわ速さ
    public float targetY = 0f;          // 中央の高さ
    public float easeTime = 2f;         // 中央に到達するまでの時間
    public bool ReachedTarget => reachedTarget;
    public bool goToHome = false; // ホームに向かうフラグ
    public float flySpeed = 5f;   // 上に飛ぶ速度


    private float startY;
    private float elapsed = 0f;
    private bool reachedTarget = false;
    private float floatStartTime;       // ふわふわ開始用タイム

    void Start()
    {
        float screenBottom = Camera.main.transform.position.y - Camera.main.orthographicSize;
        startY = screenBottom - 1f;
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
    }

    void Update()
    {
        if (!reachedTarget)
        {
            // 中央まで上昇（ふわふわ前）
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / easeTime);
            float newY = startY + (targetY - startY) * (1f - (1f - t) * (1f - t));
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            if (t >= 1f)
            {
                reachedTarget = true;
                floatStartTime = Time.time;
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
            }
        }
        else if (goToHome)
        {
            // ホームへ飛んでいく動き（上方向）
            transform.position += Vector3.up * flySpeed * Time.deltaTime;
        }
        else
        {
            // 中央でふわふわ
            float newY = targetY + Mathf.Sin((Time.time - floatStartTime) * floatFrequency) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

}
